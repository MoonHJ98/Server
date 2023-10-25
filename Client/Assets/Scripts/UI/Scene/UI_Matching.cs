using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Matching : UI_Base
{
	public UI_MatchingBoard MatchingBoardUI { get; private set; }
	enum Buttons
	{
		MatchingButton
	}
	public override void Init()
	{
		Bind<Button>(typeof(Buttons));

		GetButton((int)Buttons.MatchingButton).gameObject.BindEvent(OnClickMatchingButton);

		MatchingBoardUI = GetComponentInChildren<UI_MatchingBoard>();
		MatchingBoardUI.gameObject.SetActive(false);

	}
	public void OnClickMatchingButton(PointerEventData evt)
	{
		MatchingBoardUI.gameObject.SetActive(!MatchingBoardUI.gameObject.activeSelf);
	}
}
