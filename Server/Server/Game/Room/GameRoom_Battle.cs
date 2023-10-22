using Google.Protobuf;
using Google.Protobuf.Protocol;
using Org.BouncyCastle.Bcpg;
using Server.Data;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using static Server.Game.Object.GameObject;

namespace Server.Game
{
	// JobQueue를 적용한 시점에서 아래 함수들을 GameRoom이외의 공간에서 사용하게되면 문제가 됨
	public partial class GameRoom : JobSerializer
	{
		public void HandleMove(Player player, C_Move movePacket)
		{
			// 이동 전 좌표의 존
			Zone now = player.Room.GetZone(player.PosDirInfo.PosX, player.PosDirInfo.PosZ);
			// 이동 후 좌표의 존
			Zone after = player.Room.GetZone(movePacket.PosDirInfo.PosX, movePacket.PosDirInfo.PosZ);
            if (now != after)
            {
				if(now != null)
					now.Players.Remove(player);
				if(after != null)
					after.Players.Add(player);
            }

            // 일단 서버에서 좌표 이동
            ObjectInfo info = player.ObjectInfo;
			info.PosDirInfo = movePacket.PosDirInfo;

			// 다른 플레이어한테도 알려준다
			S_Move resMovePacket = new S_Move();
			resMovePacket.PlayerId = player.ObjectInfo.ObjectId;
			resMovePacket.PosDirInfo = movePacket.PosDirInfo;

			Push(Broadcast, player.PosDirInfo, resMovePacket, resMovePacket.PlayerId);
		}
		public void HandleAttack(Player player, C_Attack attackPacket)
		{
			ObjectInfo info = player.ObjectInfo;

			// TODO : 스킬 사용 가능 여부 체크

			// 통과
			info.PosDirInfo.State = (CreatureState)attackPacket.Info.AttackId;

			S_Attack resAttackPacket = new S_Attack()
			{
				Info = new AttackInfo()
			};
			resAttackPacket.PlayerId = player.ObjectInfo.ObjectId;
			// 어떤 공격인지(펀치, 검, 활...)
			resAttackPacket.Info.AttackId = attackPacket.Info.AttackId;
			this.Push(Broadcast, player.PosDirInfo, resAttackPacket);
		}
		public void HandleSpawn(C_Spawn spawnPacket)
		{
			foreach (var info in spawnPacket.Objects)
			{
				if(info.SpawnInfo == SpawnInfo.Monster)
					ClientForServer.AddMonster(info);
			}
		}
		public void HandleMonsterMove(C_MonsterMove movePacket)
		{

			S_MonsterMove resMovePacket = new S_MonsterMove();
			resMovePacket.MonsterInfo = movePacket.MonsterInfo;

			var monster = ClientForServer.FindMonster(movePacket.MonsterInfo.GenPoint, movePacket.MonsterInfo.ObjectId);
			if (monster == null)
				return;

			monster.ObjectInfo.Target = movePacket.MonsterInfo.Target;
			// 이동 전 좌표의 존
			Zone now = monster.Room.GetZone(monster.PosDirInfo.PosX, monster.PosDirInfo.PosZ);
			// 이동 후 좌표의 존
			Zone after = monster.Room.GetZone(movePacket.MonsterInfo.PosDirInfo.PosX, movePacket.MonsterInfo.PosDirInfo.PosZ);
			if (now != after)
			{
				if (now != null)
					now.Monsters.Remove(monster);
				if (after != null)
					after.Monsters.Add(monster);
			}

			monster.PosDirInfo = movePacket.MonsterInfo.PosDirInfo;

			Push(Broadcast, monster.PosDirInfo, resMovePacket, ClientForServer.ObjectInfo.ObjectId);
		}
		public void HandleMonsterAttack(C_MonsterAttack monAttPacket)
		{

			S_MonsterAttack resAttackPacket = new S_MonsterAttack()
			{
				AttackInfo = monAttPacket.AttackInfo,
				MonsterInfo = monAttPacket.MonsterInfo,
			};

			var monster = ClientForServer.FindMonster(monAttPacket.MonsterInfo.GenPoint, monAttPacket.MonsterInfo.ObjectId);

			Push(Broadcast, monster.PosDirInfo, resAttackPacket, ClientForServer.ObjectInfo.ObjectId);
		}
		public void HandleDamaged(C_Damaged damagedPacket)
		{
			GameObject attacker = null;
			GameObject deffender = null;
			// 플레이어에서 찾아야함
			if (string.IsNullOrEmpty(damagedPacket.AttackerGenId))
			{
				attacker = _players.Find(args => args.ObjectInfo.ObjectId == damagedPacket.AttackerId);
			}
			else // 몬스터에서 찾아야함
			{
				attacker = ClientForServer.FindMonster(damagedPacket.AttackerGenId, damagedPacket.AttackerId);
			}

			// 플레이어에서 찾아야함
			if (string.IsNullOrEmpty(damagedPacket.DefenderGenId))
			{
				deffender = _players.Find(args => args.ObjectInfo.ObjectId == damagedPacket.DefenderId);
			}
			else // 몬스터에서 찾아야함
			{
				deffender = ClientForServer.FindMonster(damagedPacket.DefenderGenId, damagedPacket.DefenderId);
			}

			if (attacker == null || deffender == null)
				return;


			deffender.OnDamaged(attacker, attacker.TotalAttack);
		}
		public void HandleDeadEnd(C_DeadEnd deadEndPacket)
		{
			GameObject obj = null;
			if(deadEndPacket.Info.SpawnInfo == SpawnInfo.Player)
			{
				obj = _players.Find((p) => p.ObjectInfo.ObjectId == deadEndPacket.Info.ObjectId);
				if(obj != null)
					Push(LeaveGame, deadEndPacket.Info.ObjectId);
			}
			else if(deadEndPacket.Info.SpawnInfo == SpawnInfo.Monster)
			{
				obj = ClientForServer.FindMonster(deadEndPacket.Info.GenPoint, deadEndPacket.Info.ObjectId);
				if (obj != null)
				{
					ClientForServer.DeleteMonster(deadEndPacket.Info.GenPoint, deadEndPacket.Info.ObjectId);
				}
			}
		}
		public void HandleMonsterDead(GameObject monster)
		{
			ClientForServer.DeadMonsters.Add(monster as Monster);
		}

		public void HandleUpdatePos(C_UpdatePos updatePosPacket)
		{
			if(updatePosPacket.SpawnInfo == SpawnInfo.Player)
			{
				var player = _players.Find((p) => p.ObjectInfo.ObjectId == updatePosPacket.Id);
				if(player != null)
				{
					player.PosDirInfo = updatePosPacket.PosInfo;
				}
			}
			else if(updatePosPacket.SpawnInfo == SpawnInfo.Monster)
			{
				var monster = ClientForServer.FindMonster(updatePosPacket.GenId, updatePosPacket.Id);
				if(monster != null)
				{
					monster.PosDirInfo = updatePosPacket.PosInfo;
				}
			}
		}
	}
}

