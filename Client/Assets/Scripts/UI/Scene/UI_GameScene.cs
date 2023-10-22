using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameScene : UI_Scene
{
	public UI_Stat StatUI { get; private set; }
	public UI_Inventory InvenUI { get; private set; }

	public UI_InvenStat InvenStatButtonUI {  get; private set; }	


	public override void Init()
	{
		base.Init();

		StatUI = GetComponentInChildren<UI_Stat>();
		StatUI.gameObject.SetActive(false);

		InvenUI = GetComponentInChildren<UI_Inventory>();
		InvenUI.gameObject.SetActive(false);

		InvenStatButtonUI = GetComponentInChildren<UI_InvenStat>();
		InvenStatButtonUI.gameObject.SetActive(true);

	}
}