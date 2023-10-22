using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory_Item : UI_Base
{
	public Image _icon;

	public Image _frame;

	public int ItemId { get; set; }
	public int TemplateId { get; set; }
	public int Count { get; set; }
	public bool Equipped { get; set; }
	public override void Init()
	{
		_icon.gameObject.BindEvent((e) =>
		{
			if (e.button != PointerEventData.InputButton.Right)
				return;

			Managers.Data.ItemDict.TryGetValue(TemplateId, out var itemData);
			if (itemData == null || itemData.itemType == ItemType.Consumable)
				return;

			if (TemplateId == 1 && Equipped == false)
			{
				GameObject go = Managers.Resource.Instantiate($"Creature/sword-1");
				Transform[] allChildren = Managers.Objects.MyPlayer.GetComponentsInChildren<Transform>();
				foreach (Transform child in allChildren)
				{
					if (child.name == "SwordSlot")
					{
						go.transform.SetParent(child, true);
						go.transform.localPosition = Vector3.zero;
						go.transform.localRotation = Quaternion.identity;
						Managers.Objects.MyPlayer.weapon = go;
					}
				}
			}
			if(TemplateId == 1 && Equipped == true)
			{
				Destroy(Managers.Objects.MyPlayer.weapon);
				Managers.Objects.MyPlayer.weapon = null;
			}

			C_EquipItem equipItem = new C_EquipItem()
			{
				ItemId = ItemId,
				Equipped = !Equipped,
				ItemType = (int)itemData.itemType,
				ItemTemplateId = TemplateId
			};
			Managers.Network.Send(equipItem);
		});
	}

	public void SetItem(Item item)
	{
		if (item == null)
		{
			ItemId = -1;
			TemplateId = -1;
			Count = -1;
			Equipped = false;

			_icon.gameObject.SetActive(false);
			_frame.gameObject.SetActive(false);
		}
		else
		{
			ItemId = item.ItemId;
			TemplateId = item.TemplateId;
			Count = item.Count;
			Equipped = item.Equipped;
			Managers.Data.ItemDict.TryGetValue(TemplateId, out var itemData);

			Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
			_icon.sprite = icon;

			_frame.gameObject.SetActive(item.Equipped);
		}
	}
}
