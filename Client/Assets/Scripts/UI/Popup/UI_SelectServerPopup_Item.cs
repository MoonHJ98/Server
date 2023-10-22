using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SelectServerPopup_Item: UI_Base
{
	public ServerInfo info { get; set; }

	enum Buttons
	{
		SelectServerButton
	}
	enum Texts
	{
		NameText
	}
	public override void Init()
	{
		Bind<Button>(typeof(Buttons));
		Bind<Text>(typeof(Texts));

		GetButton((int)Buttons.SelectServerButton).gameObject.BindEvent(OnClickButton);
	}

	public void RefreshUI()
	{
		if (info == null)
			return;

		GetText((int)Texts.NameText).text = info.Name;
	}
	void OnClickButton(PointerEventData evt)
	{
		Managers.Network.ConnectToGame(info);
		Managers.Scene.LoadScene(Define.Scene.Game);
		Managers.UI.ClosePopupUI();
	}
}
