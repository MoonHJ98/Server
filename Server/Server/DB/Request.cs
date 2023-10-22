using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.DB
{
	public class Request
	{

	}
	public class Result
	{

	}
	
	public class CreatePlayer : Request
	{
		public string accountName;
		public string playerName;
		public int level;
		public int hp;
		public int maxHp;
		public int damage;
		public int defense;
		public int exp;
		public int totalExp;
	}
	public class CreatePlayerResult : Result
	{
		public enum DBResult { Success, Fail, Already_Exists}
		public DBResult result;
		public LobbyPlayerInfo playerInfo;
	}
	public class GetPlayerByAccount : Request
	{
		public string accountName;
	}
	public class GetPlayerByAccountResult : Result
	{
		public enum DBResult { Success, Fail }
		public DBResult result;
		public List<LobbyPlayerInfo> playerList = new List<LobbyPlayerInfo>();
	}
	public class SetStat : Request
	{
		public string playerName;
		public StatInfo statInfo;
	}
	public class SetStatResult : Result
	{
		public enum DBResult { Success, Fail }
		public DBResult result;
	}
	public class GetItems : Request
	{
		public string playerName;
	}
	public class GetItemsResult : Result
	{
		public enum DBResult { Success, Fail }
		public DBResult result;
		public List<ItemInfo> items = new List<ItemInfo>();
	}
	public class AddItem : Request
	{
		public int itemTemplateId;
		public int count;
		public int slot;
		public string owner;
	}
	public class AddItemResult : Result
	{
		public enum DBResult { Success, Fail }
		public DBResult result;
		public int itemId;
		public int itemTemplateId;
		public int count;
		public int slot;
	}
	public class ChangeItemEquipState : Request
	{
		public int itemId;
		public bool equipped;
	}
	public class ChangeItemEquipStateResult : Result
	{
		public int itemId;
		public bool equipped;
		public enum DBResult { Success, Fail }
		public DBResult result;
	}
}
