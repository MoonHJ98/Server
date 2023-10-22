using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<string, Stat> StatDict { get; private set; } = new Dictionary<string, Stat>();
	public Dictionary<int, Skill> SkillDict { get; private set; } = new Dictionary<int, Skill>();
	public Dictionary<int, ItemData> ItemDict { get; private set; } = new Dictionary<int, ItemData>();
	public Dictionary<string, MonsterData> MonsterDict { get; private set; } = new Dictionary<string, MonsterData>();


	public void Init()
    {
        StatDict = LoadJson<Data.StatData, string, Stat>("StatData").MakeDict();
		SkillDict = LoadJson<Data.SkillData, int, Skill>("SkillData").MakeDict();
		ItemDict = LoadJson<ItemLoader, int, ItemData>("ItemData").MakeDict();
		MonsterDict = LoadJson<MonsterLoader, string, MonsterData>("MonsterData").MakeDict();

	}

	// 유니티의 Json라이브러리는 enum값은 int로 저장
	// 현재 아이템 데이터 시트에 enum값을 문자열로 정의 해놨기 때문에 데이터(WeaponType, ArmourType)가 None으로 들어옴
	// 서버에서 사용하는 newtonsoft의 라이브러리를 사용하는걸로 바꾼다
	// 서버 -> 종속성 -> 패키지 -> newtonsoft 속성 클릭 -> 경로에 dll -> net45 폴더에 dll파일을
	// 유니티 Assets\Libs 폴더에 넣어준 후 그 라이브러리를 사용한다.
	Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
		//return JsonUtility.FromJson<Loader>(textAsset.text);
		return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(textAsset.text);

	}
}
