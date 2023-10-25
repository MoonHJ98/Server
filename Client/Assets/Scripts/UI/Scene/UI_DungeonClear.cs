using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_DungeonClear : UI_Base
{
	enum Buttons
	{
		ClearButton
	}
	public override void Init()
	{
		Bind<Button>(typeof(Buttons));

		GetButton((int)Buttons.ClearButton).gameObject.BindEvent(OnClickClearButton);

	}
	public void OnClickClearButton(PointerEventData evt)
	{
		var gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		UI_DungeonClear dungeonClearUI = gameSceneUI.DungeonClearUI;

		dungeonClearUI.gameObject.SetActive(false);

		Managers.Objects.Clear();
		Managers.Network.DisConnectDungeon();
		Managers.Network.ConnectToGame();
		Managers.Scene.LoadScene(Scene.Game);
	}
}
