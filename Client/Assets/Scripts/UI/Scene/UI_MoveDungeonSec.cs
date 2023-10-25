using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Define;

public class UI_MoveDungeonSec: UI_Base
{
	public float time = 6f;
	enum Texts
	{
		MoveDungeonSec
	}
	public override void Init()
	{
		Bind<Text>(typeof(Texts));
	}
	public void RefreshUI()
	{
		Get<Text>((int)Texts.MoveDungeonSec).text = "5";
		time = 6f;
	}
	public void SetTime(int sec)
	{
		if (sec < 0)
		{
			this.gameObject.transform.parent.gameObject.GetComponent<UI_MatchingBoard>().RefreshUI();
			this.gameObject.SetActive(false);
			this.gameObject.transform.parent.gameObject.SetActive(false);
			Managers.Objects.Clear();
			Managers.Network.DisConnectMatching();
			Managers.Network.DisConnectServer();
			Managers.Network.ConnectToDungeon();
			Managers.Scene.LoadScene(Scene.Dungeon);
			return;
		}

		Get<Text>((int)Texts.MoveDungeonSec).text = sec.ToString();
	}
	public string GetTime()
	{
		return Get<Text>((int)Texts.MoveDungeonSec).text;
	}
}
