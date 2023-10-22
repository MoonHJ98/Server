using Google.Protobuf;
using Google.Protobuf.Protocol;
using Org.BouncyCastle.Bcpg;
using Server.Data;
using Server.Game;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
	public class DbThread : JobSerializer
	{
		public static DbThread Instance { get; } = new DbThread();

		public static void SavePlayerStatus(Player player, GameRoom room)
		{
			if (player == null || room == null)
				return;

			SetStat request = new SetStat()
			{
				playerName = player.ObjectInfo.Name,
				statInfo = new StatInfo()
			};
			request.statInfo.MergeFrom(player.StatInfo);

			// db 스레드에서 저장
			Instance.Push(() =>
			{
				GameDB.Request(request).ContinueWith(args =>
				{
					var res = args.Result as SetStatResult;

					// 저장 후 game room 스레드에서 후속 처리
					room.Push(() => Console.WriteLine($"Stat saved({player.StatInfo.Hp})"));
				});
			});

		}
		public static void RewardPlayer(Player player, RewardData rewardData, GameRoom room)
		{
			if (player == null || room == null || rewardData == null)
				return;

			// 몬스터 두마리를 동시에 잡는다면?
			// 같은 슬롯을 바라볼 수 있음
			// 메모리에 선 적용 후 디비가 아니라 이런 문제 발생
			// 근데 디비에 저장함으로서 itemId가 나오는 상황
			// Inventory에 resolvedSlots 추가해서 해결
			int? slot = player.Inventory.GetEmptySlot();
			if (slot == null)
				return;


			AddItem request = new AddItem()
			{
				itemTemplateId = rewardData.itemTemplateId,
				count = rewardData.count,
				slot = slot.Value,
				owner = player.ObjectInfo.Name
			};

			// db 스레드에서 저장
			Instance.Push(() =>
			{
				GameDB.Request(request).ContinueWith(args =>
				{
					var res = args.Result as AddItemResult;

					if (res.result == AddItemResult.DBResult.Success)
					{
						// DB작업 후 ReservedSlots에서 제거
						player.Inventory.RemoveReservedSlot(res.slot);

						ItemInfo info = new ItemInfo()
						{
							ItemId = res.itemId,
							TemplateId = res.itemTemplateId,
							Count = res.count,
							Slot = res.slot
						};

						// 저장 후 game room 스레드에서 후속 처리
						room.Push(() =>
						{
							Item newItem = Item.MakeItem(info);
							player.Inventory.Add(newItem);

							S_AddItem addItem = new S_AddItem();
							ItemInfo itemInfo = new ItemInfo();
							itemInfo.MergeFrom(newItem.Info);
							addItem.Items.Add(itemInfo);

							player.Session.Send(addItem);
						});
					}
				});
			});
		}
		public static void ChangeItemEquipState(Player player, Item item, GameRoom room)
		{
			if (player == null || room == null)
				return;

			ChangeItemEquipState request = new ChangeItemEquipState()
			{
				itemId = item.ItemId,
				equipped = item.Equipped
			};

			// db 스레드에서 저장
			Instance.Push(() =>
			{
				GameDB.Request(request).ContinueWith(args =>
				{
					var res = args.Result as ChangeItemEquipStateResult;

					if (res.result == ChangeItemEquipStateResult.DBResult.Success)
					{
						S_EquipItem equipItem = new S_EquipItem()
						{
							ItemId = res.itemId,
							Equipped = res.equipped,
							PlayerId = player.ObjectInfo.ObjectId,
							ItemTemplateId = item.TemplateId,
							ItemType = item.ItemType
						};

						// 성공이라면 클라에 통보
						room.Push(room.Broadcast, player.ObjectInfo.PosDirInfo, equipItem);
						//room.Push(() => player.Session.Send(equipItem));
					}
					// 실패라면 GameRoom스레드에서 메모리 데이터 복구
					else
					{
						room.Push(() => player.Inventory.Get(res.itemId).Equipped = !res.equipped);
					}

				});
			});
		}
	}
}
