using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Inventory : UI_Base
{
	// 아이템 UI 정보들. 실제 아이템 정보는 InventoryManager에
	public List<UI_Inventory_Item> Items { get; } = new List<UI_Inventory_Item>();
	public override void Init()
	{
		Items.Clear();

		GameObject grid = transform.Find("ItemGrid").gameObject;

		// 혹시나 있을(에디터상 테스트용) 아이템 창 삭제
		foreach (Transform child in grid.transform)
			Destroy(child.gameObject);

		for(int i = 0; i < 20; ++i)
		{
			//grid.transform를 부모로 두기
			GameObject gameObject = Managers.Resource.Instantiate("UI/Scene/UI_Inventory_Item", grid.transform);
			UI_Inventory_Item item = gameObject.GetComponent<UI_Inventory_Item>();
			Items.Add(item);
		}

		RefreshUI();
	}

	public void RefreshUI()
	{
		if (Items.Count == 0)
			return;

		List<Item> items = Managers.Inventory.Items.Values.ToList();
		//items.Sort((left, right) => { return left.Slot - right.Slot; });

		foreach (Item item in items)
		{
			if (item.Slot < 0 || item.Slot > 20)
				continue;

			Items[item.Slot].SetItem(item);
		}
	}
}
