using Google.Protobuf.Protocol;
using Org.BouncyCastle.Asn1.X509;
using Server.DB;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server.Game.Object
{
	public partial class Player : GameObject
	{
		public ClientSession Session { get; set; }
		public Inventory Inventory { get; private set; } = new Inventory();

		public int WeaponDamage { get; private set; }
		public int ArmourDefense { get; private set; }

		public override int TotalAttack { get { return StatInfo.Damage + WeaponDamage; } }
		public override int TotalDefense { get { return StatInfo.Defense + ArmourDefense; } }

		public Player()
		{
			ObjectType = GameObjectType.Player;
		}
		// TODO : 서버가 다운되면 저장되지 않는 정보가 날라감
		public void OnLeaveGame()
		{
			DbThread.SavePlayerStatus(this, Room);
		}

		public void HandleEquipItem(C_EquipItem equipPacket)
		{


			Item item = Inventory.Get(equipPacket.ItemId);
			if (item == null)
				return;

			if (item.ItemType == ItemType.Consumable)
				return;

			GameRoom room = Room;
			if (room == null)
				return;
			// 겹치는 부위가 있다면 해제
			if (equipPacket.Equipped == true)
			{
				Item unequipItem = null;
				if (item.ItemType == ItemType.Weapon)
				{
					unequipItem = Inventory.Find(args => args.Equipped == true && args.ItemType == ItemType.Weapon);
				}
				else if (item.ItemType == ItemType.Armour)
				{
					ArmourType armourType = ((Armour)item).ArmourType;
					unequipItem = Inventory.Find(args => args.Equipped == true && args.ItemType == ItemType.Armour && ((Armour)args).ArmourType == armourType);

				}
				if (unequipItem != null)
				{
					// 메모리에 선 적용. 착용중이였던 장비 해제
					unequipItem.Equipped = false;

					DbThread.ChangeItemEquipState(this, unequipItem, room);
				}
			}

			// 메모리에 선 적용
			item.Equipped = equipPacket.Equipped;
			if ((ItemType)equipPacket.ItemType == ItemType.Weapon)
			{
				ObjectInfo.EquippedWeaponTemplateId = equipPacket.ItemTemplateId;
				if (item.Equipped == false)
					ObjectInfo.EquippedWeaponTemplateId = -1;
			}

			DbThread.ChangeItemEquipState(this, item, room);

			RefreshAdditionStat();
		}

		public void RefreshAdditionStat()
		{
			WeaponDamage = 0;
			ArmourDefense = 0;

			foreach (var item in Inventory.Items.Values)
			{
				if (item.Equipped == false)
					continue;

				switch (item.ItemType)
				{
					case ItemType.Weapon:
						WeaponDamage += ((Weapon)item).Damage;
						break;
					case ItemType.Armour:
						WeaponDamage += ((Armour)item).Defence;
						break;

				}
			}
		}
	}
}
