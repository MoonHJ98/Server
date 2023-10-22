using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stat : UI_Base
{
	enum Images
	{
		Slot_Helmet,
		Slot_Armour,
		Slot_Boots,
		Slot_Weapon,
		Slot_Gloves
	}
	enum Texts
	{
		NameText,
		AttackValueText,
		DefenseValueText
	}

	bool init = false;
	public override void Init()
	{
		Bind<Image>(typeof(Images));
		Bind<Text>(typeof(Texts));

		init = true;
		RefreshUI();
	}
	public void RefreshUI()
	{
		if (init == false)
			return;

		// 모든 이미지 다 끈 후
		Get<Image>((int)Images.Slot_Helmet).enabled = false;
		Get<Image>((int)Images.Slot_Armour).enabled = false;
		Get<Image>((int)Images.Slot_Boots).enabled = false;
		Get<Image>((int)Images.Slot_Weapon).enabled = false;
		Get<Image>((int)Images.Slot_Gloves).enabled = false;

		// 채워준다
		foreach(Item item in Managers.Inventory.Items.Values)
		{
			if (item.Equipped == false)
				continue;

			Managers.Data.ItemDict.TryGetValue(item.TemplateId, out var itemData);
			Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);

			if (item.ItemType == ItemType.Weapon)
			{
				Get<Image>((int)Images.Slot_Weapon).enabled = true;
				Get<Image>((int)Images.Slot_Weapon).sprite = icon;
			}
			else if (item.ItemType == ItemType.Armour)
			{
				Armour armour = item as Armour;

				switch (armour.ArmourType)
				{
					case ArmourType.Helmet:
						Get<Image>((int)Images.Slot_Helmet).enabled = true;
						Get<Image>((int)Images.Slot_Helmet).sprite = icon;
						break;
					case ArmourType.Armour:
						Get<Image>((int)Images.Slot_Armour).enabled = true;
						Get<Image>((int)Images.Slot_Armour).sprite = icon;
						break;
					case ArmourType.Boots:
						Get<Image>((int)Images.Slot_Boots).enabled = true;
						Get<Image>((int)Images.Slot_Boots).sprite = icon;
						break;
					case ArmourType.Gloves:
						Get<Image>((int)Images.Slot_Gloves).enabled = true;
						Get<Image>((int)Images.Slot_Gloves).sprite = icon;
						break;
					default:
						break;
				}
			}
		}

		// Text
		MyPlayerController player = Managers.Objects.MyPlayer;
		player.RefreshAdditionStat();

		Get<Text>((int)Texts.NameText).text = player.name;
		Get<Text>((int)Texts.AttackValueText).text = $"{player.Stat.Damage + player.WeaponDamage}";
		Get<Text>((int)Texts.DefenseValueText).text = $"{player.Stat.Defense + player.ArmourDefense}";
	}
}
