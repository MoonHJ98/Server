using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_SelectServerPopup : UI_Popup
{
	// ������ UI ������. ���� ������ ������ InventoryManager��
	public List<UI_SelectServerPopup_Item> Items { get; } = new List<UI_SelectServerPopup_Item>();
	public override void Init()
	{
		base.Init();


	}
	public void SetServers(List<ServerInfo> servers)
	{
		Items.Clear();
		GameObject grid = GetComponentInChildren<GridLayoutGroup>().gameObject;

		// Ȥ�ó� ����(�����ͻ� �׽�Ʈ��) ������ â ����
		foreach (Transform child in grid.transform)
			Destroy(child.gameObject);

		for (int i = 0; i < servers.Count; ++i)
		{
			//grid.transform�� �θ�� �α�
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
