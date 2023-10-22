using Google.Protobuf.Protocol;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
	public class Zone
	{
		public ZoneInfo Info { get; set; } = new ZoneInfo();

		public HashSet<Player> Players { get; set; } = new HashSet<Player>();
		public HashSet<Monster> Monsters { get; set; } = new HashSet<Monster>();


		public Player FindOnePlayer(Func<Player, bool> condition)
		{
			foreach (Player p in Players)
			{
                if (condition.Invoke(p))
                {
					return p;
                }
            }
			return null;
		}
		public List<Player> FindAllPlayers(Func<Player, bool> condition)
		{
			List<Player> findList = new List<Player>();
			foreach (Player p in Players)
			{
				if (condition.Invoke(p))
				{
					findList.Add(p);
				}
			}
			return findList;
		}
		public Monster FindOneMonster(Func<Monster, bool> condition)
		{
			foreach (Monster m in Monsters)
			{
				if (condition.Invoke(m))
				{
					return m;
				}
			}
			return null;
		}
		public List<Monster> FindAllMonsters(Func<Monster, bool> condition)
		{
			List<Monster> findList = new List<Monster>();
			foreach (Monster m in Monsters)
			{
				if (condition.Invoke(m))
				{
					findList.Add(m);
				}
			}
			return findList;
		}
	}
}
