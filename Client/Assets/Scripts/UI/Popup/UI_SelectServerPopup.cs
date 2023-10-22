using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectServerPopup : UI_Popup
{
	// 아이템 UI 정보들. 실제 아이템 정보는 InventoryManager에
	public List<UI_SelectServerPopup_Item> Items { get; } = new List<UI_SelectServerPopup_Item>();
	public override void Init()
	{
		base.Init();


	}
	public void SetServers(List<ServerInfo> servers)
	{
		Items.Clear();
		GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;

		// 혹시나 있을(에디터상 테스트용) 아이템 창 삭제
		foreach (Transform child in grid.transform)
			Destroy(child.gameObject);

		for (int i = 0; i < servers.Count; ++i)
		{
			//grid.transform를 부모로 두기
			GameObject gameObject = Managers.Resource.Instantiate("UI/Popup/UI_SelectServerPopup_Item", grid.transform);
			UI_SelectServerPopup_Item item = gameObject.GetOrAddComponent<UI_SelectServerPopup_Item>();
			item.info = servers[i];
			Items.Add(item);
		}

		RefreshUI();
	}

	public void RefreshUI()
	{
		if (Items.Count == 0)
			return;

		foreach (var item in Items)
		{
			item.RefreshUI();
		}
	}
}
