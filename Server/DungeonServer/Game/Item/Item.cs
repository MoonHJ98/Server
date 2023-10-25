using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Item
	{
		public ItemInfo Info { get; } = new ItemInfo();

		public int ItemId
		{
			get { return Info.ItemId; }
			set { Info.ItemId = value; }
		}
		public int TemplateId
		{
			get { return Info.TemplateId; }
			set { Info.TemplateId = value; }
		}
		public int Count
		{
			get { return Info.Count; }
			set { Info.Count = value; }
		}
		public int Slot
		{
			get { return Info.Slot; }
			set { Info.Slot = value; }
		}
		public bool Equipped
		{
			get
			{
				return Info.Equipped;
			}
			set
			{
				if (Info.Equipped.Equals(value))
					return;
				Info.Equipped = value;
			}
		}

		public ItemType ItemType { get; private set; }
		public bool Stackable { get; protected set; }

		public Item(ItemType itemType)
		{
			ItemType = itemType;
		}
		public static Item MakeItem(ItemInfo info)
		{
			Item item = null;

			DataManager.ItemDict.TryGetValue(info.TemplateId, out var itemData);
			if (itemData == null)
				return null;

			switch (itemData.itemType)
			{
				case ItemType.None:
					break;
				case ItemType.Weapon:
					item = new Weapon(info.TemplateId);
					break;
				case ItemType.Armour:
					item = new Armour(info.TemplateId);
					break;
				case ItemType.Consumable:
					item = new Consumable(info.TemplateId);
					break;
				default:
					break;
			}

			if(item != null)
			{
				item.ItemId = info.ItemId;
				item.Count = info.Count;
				item.Slot = info.Slot;
				item.Equipped = info.Equipped;
			}

			return item;
		}
	}

	public class Weapon : Item
	{
		public WeaponType WeaponType { get; private set; }
		public int Damage { get; private set; }
		public Weapon(int templateId) : base(ItemType.Weapon)
		{
			Init(templateId);
		}
		void Init(int templateId)
		{
			DataManager.ItemDict.TryGetValue(templateId, out var itemData);
			if (itemData.itemType != ItemType.Weapon)
				return;

			WeaponData data = itemData as WeaponData;
			TemplateId = data.id;
			Count = 1;
			WeaponType = data.weaponType;
			Damage = data.damage;
			Stackable = false;
		}
	}
	public class Armour : Item
	{
		public ArmourType ArmourType { get; private set; }
		public int Defence { get; private set; }
		public Armour(int templateId) : base(ItemType.Armour)
		{
			Init(templateId);
		}
		void Init(int templateId)
		{
			DataManager.ItemDict.TryGetValue(templateId, out var itemData);
			if (itemData.itemType != ItemType.Armour)
				return;

			ArmourData data = itemData as ArmourData;
			TemplateId = data.id;
			Count = 1;
			ArmourType = data.armourType;
			Defence = data.defence;
			Stackable = false;
		}
	}
	public class Consumable : Item
	{
		public ConsumableType ConsumableType { get; private set; }
		public int MaxCount { get; set; }
		public Consumable(int templateId) : base(ItemType.Consumable)
		{
			Init(templateId);
		}
		void Init(int templateId)
		{
			DataManager.ItemDict.TryGetValue(templateId, out var itemData);
			if (itemData.itemType != ItemType.Consumable)
				return;

			ConsumableData data = itemData as ConsumableData;
			TemplateId = data.id;
			Count = 1;
			ConsumableType = data.consumableType;
			MaxCount = data.maxCount;
			Stackable = (data.maxCount > 1);
		}
	}

}
