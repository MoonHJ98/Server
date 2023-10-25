using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
	public enum Floor
	{
		Floor = 6,
	}
	public enum Scene
	{
		Unknown,
		Login,
		Lobby,
		Game,
		Dungeon,
	}

	public enum Sound
	{
		Bgm,
		Effect,
		MaxCount,
	}

	public enum UIEvent
	{
		Click,
		Drag,
	}
}

