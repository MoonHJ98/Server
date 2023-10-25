using AccountServer;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using Server.Game;
using Server.Game.Object;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
	public partial class ClientSession : PacketSession
	{
		public string AccountId { get; private set; }
		public List<LobbyPlayerInfo> LobbyPlayers { get; set; } = new List<LobbyPlayerInfo>();
		public void HandleLogin(C_Login packet)
		{

			if (ServerState != PlayerServerState.ServerStateLogin)
				return;
			LobbyPlayers.Clear();

			// 더미 클라이언트가 아니다
			if(string.IsNullOrEmpty(packet.AccountName) == false)
			{
				AccountId = packet.AccountName;
			}
			else // 더미 클라이언트라면
			{
				AccountId = packet.UniqueId;

				// AccountDB에 계정 넣어주고
				CreateAccount createAccount = new CreateAccount()
				{
					accountName = AccountId,
					password = "1"
				};

				AccountDB.Request(createAccount);
			}

			// 계정내의 플레이어 가져오기
			GetPlayerByAccount playerRequest = new GetPlayerByAccount()
			{
				accountName = AccountId
			};
			GameDB.Request(playerRequest).ContinueWith(args =>
			{
				var res = args.Result as GetPlayerByAccountResult;
				S_Login logInOk = new S_Login();
				logInOk.LoginOk = 1;
				logInOk.ServerInfo = ServerConnecInfo.Dungeon;

				foreach (var info in res.playerList)
				{
					// 메모리에서 들고있기
					LobbyPlayers.Add(info);
					// 패킷 보내기 용도
					logInOk.Players.Add(info);
				}


				Send(logInOk);
				// 로비로 이동
				ServerState = PlayerServerState.ServerStateLobby;
			});
		}
		public void HandleCreatePlayer(C_CreatePlayer packet)
		{
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			DataManager.StatDict.TryGetValue("Player", out var stat);
			if (stat == null)
				return;

			CreatePlayer request = new CreatePlayer()
			{
				accountName = AccountId,
				playerName = packet.Name,
				level = stat.level,
				hp = stat.hp,
				maxHp = stat.maxHp,
				damage = stat.damage,
				defense = stat.defense,
				exp = stat.exp,
				totalExp = stat.totalExp
			};

			GameDB.Request(request).ContinueWith(args =>
			{
				var res = args.Result as CreatePlayerResult;

				if (res.result == CreatePlayerResult.DBResult.Already_Exists)
					Send(new S_CreatePlayer());
				else if (res.result == CreatePlayerResult.DBResult.Success)
				{
					LobbyPlayers.Add(res.playerInfo);
					S_CreatePlayer newPlayer = new S_CreatePlayer()
					{
						Player = new LobbyPlayerInfo(),
						ServerInfo = ServerConnecInfo.Dungeon
					};
					newPlayer.Player.MergeFrom(res.playerInfo);

					Send(newPlayer);
				}
			});

		}
		public void HandleEnterGame(C_EnterGame packet)
		{
			if (ServerState != PlayerServerState.ServerStateLobby)
				return;

			var playerInfo = LobbyPlayers.Find(args => args.Name == packet.Name);
			if (playerInfo == null)
				return;

			// TODO : 로비에서 캐릭터 선택
			MyPlayer = PlayerManager.Instance.Add();

			MyPlayer.ObjectInfo.Name = playerInfo.Name;
			MyPlayer.ObjectInfo.PosDirInfo.State = CreatureState.Idle;
			MyPlayer.ObjectInfo.PosDirInfo.LookDirX = 0f;
			MyPlayer.ObjectInfo.PosDirInfo.LookDirZ = -1f;

			MyPlayer.ObjectInfo.PosDirInfo.PosX = 0f;
			MyPlayer.ObjectInfo.PosDirInfo.PosY = 0.5f;
			MyPlayer.ObjectInfo.PosDirInfo.PosZ = 0f;
			MyPlayer.ObjectInfo.SpawnInfo = SpawnInfo.Player;


			MyPlayer.StatInfo.MergeFrom(playerInfo.StatInfo);
			MyPlayer.Session = this;

			// 게임에 아직 입장하기 전이니까 여기서 db접근해도 문제 없음
			// 아이템 로드
			GetItems request = new GetItems()
			{
				playerName = playerInfo.Name
			};
			GameDB.Request(request).ContinueWith( (args) =>
			{
				var res = args.Result as GetItemsResult;

				if(res.result == GetItemsResult.DBResult.Success)
				{
					S_ItemList itemPacket = new S_ItemList();

					foreach (var itemInfo in res.items)
					{
						// 내 인벤토리에 추가
						var item = Item.MakeItem(itemInfo);
						if(item != null)
						{
							// 서버상 인벤토리에 추가
							MyPlayer.Inventory.Add(item);

							// 클라에 전송
							ItemInfo info = new ItemInfo();
							// Send하는 타이밍이 언제일지 모르기 때문에 원본을 add하는게 아니라 복사해서 add
							info.MergeFrom(itemInfo);
							itemPacket.Items.Add(info);
						}
					}

					Send(itemPacket);

					GameLogic.Instance.Push(() =>
					{
						GameRoom room = GameLogic.Instance.Find(1);
						room.Push(room.EnterGame, MyPlayer);
					});

					ServerState = PlayerServerState.ServerStateGame;
				}
			});

		}
	}
}
