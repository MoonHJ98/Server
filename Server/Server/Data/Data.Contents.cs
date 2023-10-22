using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Server.Data
{
	#region Stat

	[Serializable]
	public class Stat
	{
		public string name;
		public int hp;
		public int maxHp;
		public int damage;
		public int defense;
		public int level;
		public int exp;
		public int totalExp;
	}

	[Serializable]
	public class StatData : ILoader<string, Stat>
	{
		public List<Stat> stats = new List<Stat>();

		public Dictionary<string, Stat> MakeDict()
		{
			Dictionary<string, Stat> dict = new Dictionary<string, Stat>();
			foreach (Stat stat in stats)
				dict.Add(stat.name, stat);
			return dict;
		}
	}
	#endregion

	#region Skill

	[Serializable]
	public class Skill
	{
		public int id;
		public string name;
		public float cooldown;
		public int damage;
		public SkillType skillType;
		public Projectile projectile;
		
	}

	[Serializable]
	public class SkillData : ILoader<int, Skill>
	{
		public List<Skill> skills = new List<Skill>();

		public Dictionary<int, Skill> MakeDict()
		{
			Dictionary<int, Skill> dict = new Dictionary<int, Skill>();
			foreach (Skill skill in skills)
				dict.Add(skill.id, skill);
			return dict;
		}
	}
	#endregion

	#region Item

	[Serializable]
	public class ItemData
	{
		// 템플릿 아이디
		public int id;
		public string name;
		public ItemType itemType;
		public string iconPath; // 아이템 이미지 경로
	}

	[Serializable]

	public class WeaponData : ItemData
	{
		public WeaponType weaponType;
		public int damage;
	}

	[Serializable]
	public class ArmourData : ItemData
	{
		public ArmourType armourType;
		public int defence;
	}

	[Serializable]
	public class ConsumableData : ItemData
	{
		public ConsumableType consumableType;
		public int maxCount;
	}


	[Serializable]
	public class ItemLoader : ILoader<int, ItemData>
	{
		public List<WeaponData> weapons = new List<WeaponData>();
		public List<ArmourData> armours = new List<ArmourData>();
		public List<ConsumableData> consumables = new List<ConsumableData>();

		public Dictionary<int, ItemData> MakeDict()
		{
			Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
			foreach (ItemData item in weapons)
			{
				item.itemType = ItemType.Weapon;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in armours)
			{
				item.itemType = ItemType.Armour;
				dict.Add(item.id, item);
			}
			foreach (ItemData item in consumables)
			{
				item.itemType = ItemType.Consumable;
				dict.Add(item.id, item);
			}
			return dict;
		}
	}
	#endregion

	#region Monster

	[Serializable]
	public class RewardData
	{
		public int probability; // 100분율
		public int itemTemplateId;
		public int count;
	}

	[Serializable]
	public class MonsterData
	{
		public string name;
		public StatInfo stat;
		public List<RewardData> rewards;
	}



	[Serializable]
	public class MonsterLoader : ILoader<string, MonsterData>
	{
		public List<MonsterData> monsters = new List<MonsterData>();

		public Dictionary<string, MonsterData> MakeDict()
		{
			Dictionary<string, MonsterData> dict = new Dictionary<string, MonsterData>();
			foreach (MonsterData monster in monsters)
				dict.Add(monster.name, monster);
			return dict;
		}
	}
	#endregion
}
