using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server.Game.Object
{
	public class Monster : GameObject
	{ 
		public Monster()
		{
			ObjectType = GameObjectType.Monster;
		}
		public override void OnDamaged(GameObject attacker, int damage)
		{
			base.OnDamaged(attacker, damage);
		}

		public override void OnDead(GameObject attacker)
		{
			base.OnDead(attacker);

			var owner = attacker.GetOnwer();
			if (owner.ObjectType == GameObjectType.Player)
			{
				var reward = GetRandonReward();

				if (reward == null)
					return;

				Player player = owner as Player;
				DbThread.RewardPlayer(player, reward, player.Room);
			}

		}

		RewardData GetRandonReward()
		{
			DataManager.MonsterDict.TryGetValue(ObjectInfo.Name, out var monsterData);

			if (monsterData == null)
				return null;

			//int rand = new Random().Next(0, 101);

			//int sum = 0;
			//foreach (var reward in monsterData.rewards)
			//{
			//	sum += reward.probability;
			//	if (rand <= sum)
			//		return reward;
			//}
			//return null;

			int rand = new Random().Next(0, monsterData.rewards.Count);
			return monsterData.rewards[rand];

			
		}
	}
}
