using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_InvenStat : UI_Base
{
	enum Buttons
	{
		InvenButton,
		StatButton
	}
	public override void Init()
	{
		Bind<Button>(typeof(Buttons));

		GetButton((int)Buttons.InvenButton).gameObject.BindEvent(OnClickInvenButton);
		GetButton((int)Buttons.StatButton).gameObject.BindEvent(OnClickStatButton);


	}
	public void OnClickInvenButton(PointerEventData evt)
	{
		var gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		UI_Inventory invenUI = gameSceneUI.InvenUI;
		if (invenUI.gameObject.activeSelf)
		{
			invenUI.gameObject.SetActive(false);
		}
		else
		{
			invenUI.gameObject.SetActive(true);
			invenUI.RefreshUI();
		}

	}
	public void OnClickStatButton(PointerEventData evt)
	{
		var gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		UI_Stat statUI = gameSceneUI.StatUI;

		if (statUI.gameObject.activeSelf)
		{
			statUI.gameObject.SetActive(false);
		}
		else
		{
			statUI.gameObject.SetActive(true);
			statUI.RefreshUI();
		}
	}

}
