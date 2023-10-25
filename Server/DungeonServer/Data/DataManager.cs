using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server.Data
{
	// ILoader : Generic 문법
	public interface ILoader<Key, Value>
	{
		Dictionary<Key, Value> MakeDict();
	}

	public class DataManager
	{
		public static Dictionary<string, Stat> StatDict { get; private set; } = new Dictionary<string, Stat>();
		public static Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();
		public static Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
		public static Dictionary<string, MonsterData> MonsterDict { get; private set; } = new Dictionary<string, MonsterData>();

		public static void LoadData()
		{
			StatDict = LoadJson<StatData, string, Stat>("StatData").MakeDict();
			SkillDict = LoadJson<SkillData, int, Skill>("SkillData").MakeDict();
			ItemDict = LoadJson<ItemLoader, int, ItemData>("ItemData").MakeDict();
			MonsterDict = LoadJson<MonsterLoader, string, MonsterData>("MonsterData").MakeDict();
		}

		static Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
		{
			string text = File.ReadAllText($"{ConfigManager.Config.dataPath}/{path}.json");
			return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(text);
		}
	}
}
