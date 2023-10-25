using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_MatchingBoard : UI_Base
{
	public UI_MoveDungeonSec MoveDungeonUI { get; set; }
	enum Buttons
	{
		MatchingStart,
		MatchingCancel
	}
	enum Texts
	{
		Player_1Text,
		Player_2Text
	}
	public override void Init()
	{
		Bind<Button>(typeof(Buttons));
		Bind<Text>(typeof(Texts));


		GetButton((int)Buttons.MatchingStart).gameObject.BindEvent(OnClickMatchingStartButton);
		GetButton((int)Buttons.MatchingCancel).gameObject.BindEvent(OnClickMatchingCancelButton);

		MoveDungeonUI = GetComponentInChildren<UI_MoveDungeonSec>();
		MoveDungeonUI.gameObject.SetActive(false);
	}
	public void RefreshUI()
	{
		Get<Text>((int)Texts.Player_1Text).text = "Empty";
		Get<Text>((int)Texts.Player_2Text).text = "Empty";

		if(MoveDungeonUI.gameObject.activeSelf == true)
			MoveDungeonUI.RefreshUI();
	}
	public void SetPlayer(string userName)
	{
		if(Get<Text>((int)Texts.Player_1Text).text == "Empty")
			Get<Text>((int)Texts.Player_1Text).text = userName;
		else if (Get<Text>((int)Texts.Player_2Text).text == "Empty")
			Get<Text>((int)Texts.Player_2Text).text = userName;
	}
	public bool ReadyToMoveDungeon()
	{
		if (Get<Text>((int)Texts.Player_1Text).text != "Empty" && Get<Text>((int)Texts.Player_2Text).text != "Empty")
			return true;


		MoveDungeonUI.gameObject.SetActive(false);
		return false;
	}
	public void OnClickMatchingStartButton(PointerEventData evt)
	{
		if (Get<Text>((int)Texts.Player_1Text).text == Managers.Objects.MyPlayer.gameObject.name || Get<Text>((int)Texts.Player_2Text).text == Managers.Objects.MyPlayer.gameObject.name)
			return;

		Managers.Network.ConnectToMatching();
	}
	public void OnClickMatchingCancelButton(PointerEventData evt)
	{
		RefreshUI();
		MoveDungeonUI.RefreshUI();
		MoveDungeonUI.gameObject.SetActive(false);
		Managers.Network.DisConnectMatching();
	}
}
