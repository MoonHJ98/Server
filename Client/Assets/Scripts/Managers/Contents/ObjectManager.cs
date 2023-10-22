using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectManager
{
	public MyPlayerController MyPlayer { get; set; }

	// Dictionary를 여러개로 할 수 도 있음 몬스터, 수집품...
	Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
	//List<GameObject> monsters = new List<GameObject>();

	Dictionary<string, List<GameObject>> monsters = new Dictionary<string, List<GameObject>>();
	int monsterId = 1;

	// 서버용 클라이언트가 몬스터 추가할때
	public void AddMonster(string genPoint, GameObject _monster, int monsterKey = 0)
	{
		// 몬스터용 클라
		if(MyPlayer == null)
		{
			if(monsters.TryGetValue(genPoint, out var monstersWithSameGen))
			{
				monstersWithSameGen.Add(_monster);
				monsterId++;
			}
			else
			{
				monstersWithSameGen = new List<GameObject>();
				monstersWithSameGen.Add(_monster);
				monsters.Add(genPoint, monstersWithSameGen);
				monsterId++;
			}
		}
		else // 일반 클라
		{
			if (monsters.TryGetValue(genPoint, out var monstersWithSameGen))
			{
				monstersWithSameGen.Add(_monster);
			}
			else
			{
				monstersWithSameGen = new List<GameObject>();
				monstersWithSameGen.Add(_monster);
				monsters.Add(genPoint, monstersWithSameGen);
			}
		}
	}

	public void RemoveMonster(string genPoint, int key)
	{
		monsters.TryGetValue(genPoint, out var _monsters);

		if (_monsters == null)
			return;

		var monster = _monsters.Find(args => args.GetComponent<CreatureController>().Id == key);

		if (monster == null)
			return;

		_monsters.Remove(monster);
		Managers.Resource.Destroy(monster);
	}

	public GameObject FindMonster(string genPoint, int key)
	{
		monsters.TryGetValue(genPoint, out var _monsters);

		if (_monsters == null)
			return null;

		var monster = _monsters.Find(args => args.GetComponent<CreatureController>().Id == key);

		if (monster == null)
			return null;

		return monster;
	}
	public Dictionary<string, List<GameObject>> GetMonsters() { return monsters; }
	public Dictionary<int, GameObject> GetPlayers() { return players; }

	public void UpdateWeapon(GameObject player, S_EquipItem equipItemPacket)
	{
		if (equipItemPacket.Equipped)
		{
			if (player.GetComponent<CreatureController>().weapon != null)
				return;

			if (equipItemPacket.ItemTemplateId == 1)
			{
				GameObject go = Managers.Resource.Instantiate($"Creature/sword-1");
				Transform[] allChildren = player.GetComponent<PlayerController>().GetComponentsInChildren<Transform>();
				foreach (Transform child in allChildren)
				{
					if (child.name == "SwordSlot")
					{
						go.transform.SetParent(child, true);
						go.transform.localPosition = Vector3.zero;
						go.transform.localRotation = Quaternion.identity;
						player.GetComponent<PlayerController>().weapon = go;
					}
				}
				
				player.GetComponent<CreatureController>().weapon = go;
			}
		}
		else
		{
			Managers.Resource.Destroy(player.GetComponent<CreatureController>().weapon);
			player.GetComponent<CreatureController>().weapon = null;
			
		}
	}
	public void Add(ObjectInfo info, bool myPlayer = false)
	{
		if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
			return;

		if (myPlayer == true)
		{
			GameObject player = Managers.Resource.Instantiate("Creature/MyPlayer");
			player.name = info.Name;
			player.transform.position = new Vector3(info.PosDirInfo.PosX, info.PosDirInfo.PosY, info.PosDirInfo.PosZ);


			MyPlayer = player.GetComponent<MyPlayerController>();
			MyPlayer.Id = info.ObjectId;
			player.transform.position = new Vector3(info.PosDirInfo.PosX, info.PosDirInfo.PosY, info.PosDirInfo.PosZ);
			Vector3 dir = new Vector3(info.PosDirInfo.LookDirX, player.transform.rotation.y, info.PosDirInfo.LookDirZ);
			player.transform.LookAt(player.transform.position + dir);
			MyPlayer.PosDirInfo = info.PosDirInfo;
			MyPlayer.State = info.PosDirInfo.State;
			MyPlayer.Stat = info.StatInfo;

			if (info.EquippedWeaponTemplateId != -1)
			{
				if (info.EquippedWeaponTemplateId == 1)
				{
					GameObject go = Managers.Resource.Instantiate($"Creature/sword-1");
					Transform[] allChildren = MyPlayer.GetComponentsInChildren<Transform>();
					foreach (Transform child in allChildren)
					{
						if (child.name == "SwordSlot")
						{
							go.transform.SetParent(child, true);
							go.transform.localPosition = Vector3.zero;
							go.transform.localRotation = Quaternion.identity;
							MyPlayer.weapon = go;
						}
					}
				}
			}	
			players.Add(info.ObjectId, player);
		}
		else
		{
			GameObject player = Managers.Resource.Instantiate("Creature/Player");
			player.name = info.Name;
			player.transform.position = new Vector3(info.PosDirInfo.PosX, info.PosDirInfo.PosY, info.PosDirInfo.PosZ);
			Vector3 dir = new Vector3(info.PosDirInfo.LookDirX, player.transform.rotation.y, info.PosDirInfo.LookDirZ);
			player.transform.LookAt(player.transform.position + dir);


			var pc = player.GetComponent<PlayerController>();
			pc.Init();
			pc.Id = info.ObjectId;
			pc.PosDirInfo = info.PosDirInfo;
			pc.State = info.PosDirInfo.State;
			pc.Stat = info.StatInfo;
			pc.UpdateAnimation();

			if (info.EquippedWeaponTemplateId != -1)
			{
				if (info.EquippedWeaponTemplateId == 1)
				{
					GameObject go = Managers.Resource.Instantiate($"Creature/sword-1");
					Transform[] allChildren = pc.GetComponentsInChildren<Transform>();
					foreach (Transform child in allChildren)
					{
						if (child.name == "SwordSlot")
						{
							go.transform.SetParent(child, true);
							go.transform.localPosition = Vector3.zero;
							go.transform.localRotation = Quaternion.identity;
							pc.weapon = go;
						}
					}
				}
			}

			players.Add(info.ObjectId, player);
		}

	}
	public void AddMonstersForClient(ObjectInfo info)
	{
		GameObject monster = Managers.Resource.Instantiate("Creature/Goblin_rouge_b");
		monster.name = "Goblin_rouge_b" + "_" + info.ObjectId.ToString();

		monster.transform.position = new Vector3(info.PosDirInfo.PosX, info.PosDirInfo.PosY, info.PosDirInfo.PosZ);
		Vector3 dir = new Vector3(info.PosDirInfo.LookDirX, 0f, info.PosDirInfo.LookDirZ);
		monster.transform.LookAt(monster.transform.position + dir);


		var monsterController = monster.GetComponent<MonsterController>();
		monsterController.Init();
		monsterController.genPointKey = info.GenPoint;
		monsterController.Id = info.ObjectId;
		monsterController.PosDirInfo = info.PosDirInfo;
		monsterController.Stat = info.StatInfo;
		monsterController.State = info.PosDirInfo.State;
		monsterController.targetPos = info.Target;
		if (info.StatInfo.Hp <= 0)
		{
			monsterController.State = CreatureState.Dead;
			monsterController.UpdateAnimation();
		}

		AddMonster(info.GenPoint, monster, monsterController.Id);
	}
	public void Add(int id, GameObject obj)
	{
		players.Add(id, obj);
	}

	public void AddClientForServer()
	{

		var scene = GameObject.Find("GameScene");

		switch (scene.GetComponent<GameScene>().SceneType)
		{
			case Define.Scene.Unknown:
				break;
			case Define.Scene.Login:
				break;
			case Define.Scene.Lobby:
				break;
			case Define.Scene.Game:
				LoadMonsters("Map_001");
				break;
			default:
				break;
		}

	}

	void LoadMonsters(string mapName)
	{
		GameObject map = GameObject.Find(mapName);

		if (map == null)
		{
			Debug.Log("Failed to find map for spawn monster");
			return;
		}

		C_Spawn spawnMonsterPacket = new C_Spawn();
		C_MapZone zonePacket = new C_MapZone();



		for (int i = 0; i < map.transform.childCount; i++)
		{
			var genPoint = map.transform.GetChild(i).gameObject;
			var zone = map.transform.GetChild(i).gameObject;

			// 몬스터 로드
			if(genPoint.name.StartsWith("Gen_"))
			{
				string genPointName = genPoint.name;
				

				Dictionary<int, GameObject> _monsters = new Dictionary<int, GameObject>();
				List<GameObject> patrolPoints = new List<GameObject>();

				// 패트롤 포인트 가져오기
				for(int j = 0; j < genPoint.transform.childCount; j++)
				{
					var patrolPoint = genPoint.transform.GetChild(j).gameObject;

					if(patrolPoint.name.StartsWith("Point_"))
					{
						patrolPoints.Add(patrolPoint);
					}
				}
				
				// 몬스터 생성
				for (int k = 0; k < 1;  k++)
				{
					GameObject monster = Managers.Resource.Instantiate("Creature/My_Goblin_rouge_b");

					monster.name = "My_Goblin_rouge_b" + k.ToString();
					monster.transform.position = new Vector3(genPoint.transform.position.x, genPoint.transform.position.y, genPoint.transform.position.z);

					var rangeX = Random.Range(-1, 1);
					var rangeZ = Random.Range(-1, 1);
					Vector3 dir = new Vector3(rangeX, 0f, rangeZ);
					monster.transform.LookAt(monster.transform.position + dir);

					var monsterController = monster.GetComponent<MyMonsterController>();
					monsterController.Init();
					monsterController.Id = monsterId;
					monsterController.genPointKey = genPointName;
					monsterController.patrolPoints = patrolPoints;
					monsterController.Position = monster.transform.position;
					monsterController.LookDir = dir;
					monsterController.State = CreatureState.Idle;

					Managers.Data.StatDict.TryGetValue("My_Goblin_rouge_b", out var stat);

					monsterController.Stat.Level = stat.level;
					monsterController.Stat.Hp = stat.hp;
					monsterController.Stat.MaxHp = stat.maxHp;
					monsterController.Stat.Damage = stat.damage;
					monsterController.Stat.Defense = stat.defense;
					monsterController.Stat.Exp = stat.exp;
					monsterController.Stat.TotalExp = stat.totalExp;



					// 패킷 전송을 위한 데이터 삽입
					ObjectInfo monsterInfo = new ObjectInfo() { GenPoint = genPointName, ObjectId = monsterId, PosDirInfo = monsterController.PosDirInfo, StatInfo = monsterController.Stat };
					monsterInfo.Name = "My_Goblin_rouge_b";
					monsterInfo.SpawnInfo = SpawnInfo.Monster;

					spawnMonsterPacket.Objects.Add(monsterInfo);

					AddMonster(genPointName, monster);

				}

			}

			// 존 로드
			if(zone.name.StartsWith("Zones"))
			{
				for(int l = 0; l <  zone.transform.childCount; ++ l)
				{
					var zoneObj = zone.transform.GetChild(l).gameObject;
					
					var leftX = zoneObj.transform.position.x - zoneObj.transform.localScale.x * 0.5f;
					var rightX = zoneObj.transform.position.x + zoneObj.transform.localScale.x * 0.5f;
					var forwardZ = zoneObj.transform.position.z + zoneObj.transform.localScale.z * 0.5f;
					var backwardZ = zoneObj.transform.position.z - zoneObj.transform.localScale.z * 0.5f;

					ZoneInfo zoneInfo = new ZoneInfo()
					{
						Name = zoneObj.name,
						LeftX = leftX,
						RightX = rightX,
						ForwardZ = forwardZ,
						BackwardZ = backwardZ
					};

					zonePacket.Zones.Add(zoneInfo);
				}
			}
		}
		// 존 정보 서버로 전송
		Managers.Network.Send(zonePacket);
		// 몬스터들 서버로 전송
		Managers.Network.Send(spawnMonsterPacket);



	}
	public void Remove(int id)
	{
		if (MyPlayer != null && MyPlayer.Id == id)
			return;

		GameObject obj = Find(id);
		if (obj == null)
			return;

		players.Remove(id);
		Managers.Resource.Destroy(obj);
	}
	public void RemoveMyPlayer()
	{
		if (MyPlayer == null)
			return;

		Remove(MyPlayer.Id);
		MyPlayer = null;
	}
	public void Clear()
	{
		foreach (var obj in players.Values)
			Managers.Resource.Destroy(obj);

		players.Clear();
	}

	public GameObject Find(int id)
	{
		GameObject obj = null;
		players.TryGetValue(id, out obj);
		return obj;

	}
}
