using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game.Object
{
	public partial class Player
	{
		#region 공용 객체를 위한 클라이언트
		// 몬스터를 위한 클라이언트 용도
		public Dictionary<string, List<Monster>> monsters = new Dictionary<string, List<Monster>>();

		public List<Monster> DeadMonsters { get; set; } = new List<Monster>();

		public Monster FindMonster(string genPoint, int monsterId)
		{
			if (monsters.TryGetValue(genPoint, out var _monsters) == false)
				return null;

			var monster = _monsters.Find(args => args.ObjectInfo.ObjectId == monsterId);

			return monster;

		}

		public void AddMonster(ObjectInfo monInfo)
		{
			if (monsters.ContainsKey(monInfo.GenPoint))
			{
				monsters.TryGetValue(monInfo.GenPoint, out var monstersWithSameGenPoint);
				var monster = new Monster() { ObjectInfo = monInfo, StatInfo = monInfo.StatInfo, Room = this.Room };
				var zone = this.Room.GetZone(monster.PosDirInfo.PosX, monster.PosDirInfo.PosZ);
				if (zone != null)
					zone.Monsters.Add(monster);
				monstersWithSameGenPoint.Add(monster);
			}
			else
			{
				List<Monster> monstersWithSameGenPoint = new List<Monster>();
				var monster = new Monster() { ObjectInfo = monInfo, StatInfo = monInfo.StatInfo, Room = this.Room };
				var zone = this.Room.GetZone(monster.PosDirInfo.PosX, monster.PosDirInfo.PosZ);
				if (zone != null)
					zone.Monsters.Add(monster);
				monstersWithSameGenPoint.Add(monster);
				monsters.Add(monInfo.GenPoint, monstersWithSameGenPoint);
			}
		}

		public void DeleteMonster(string genPoint, int monsterId)
		{
			if (monsters.TryGetValue(genPoint, out var _monsters) == false)
				return;

			var monster = _monsters.Find(args => args.ObjectInfo.ObjectId == monsterId);
			if (monster != null)
			{
				var zone = this.Room.GetZone(monster.PosDirInfo.PosX, monster.PosDirInfo.PosZ);
				if (zone != null)
					zone.Monsters.Remove(monster);
				_monsters.Remove(monster);
			}
		}

		public void Update()
		{
			List<Monster> monsters = new List<Monster>();

			monsters = DeadMonsters.ToList();

			//if (monsters == null || monsters.Count <= 0)
			//	return;

			S_MonstersDead packet = new S_MonstersDead();

			foreach(var monster in monsters)
			{
				packet.Monsters.Add(monster.ObjectInfo);
			}
			GameRoom room = Room;
			if (room == null)
				return;

			if(packet.Monsters.Count >= 1)
				room.Push(room.Broadcast, packet);

			DeadMonsters = DeadMonsters.Except(monsters).ToList();

			room.PushAfter(500, Update);
		}
		#endregion
	}
}
