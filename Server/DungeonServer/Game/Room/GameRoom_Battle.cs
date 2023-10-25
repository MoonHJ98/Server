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

            // 일단 서버에서 좌표 이동
            ObjectInfo info = player.ObjectInfo;
			info.PosDirInfo = movePacket.PosDirInfo;

			// 다른 플레이어한테도 알려준다
			S_Move resMovePacket = new S_Move();
			resMovePacket.PlayerId = player.ObjectInfo.ObjectId;
			resMovePacket.PosDirInfo = movePacket.PosDirInfo;

			Push(Broadcast, resMovePacket, resMovePacket.PlayerId);
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
			this.Push(Broadcast, resAttackPacket);
		}
		public void HandleDungeonDamaged(C_DungeonDamaged damagedPacket)
		{
			S_DungeonDamaged resPacket = new S_DungeonDamaged()
			{
				DefenderName = damagedPacket.DefenderName,
				Damage = damagedPacket.Damage
			};
			this.Push(Broadcast, resPacket);
		}
		public void HandleSpawn(C_Spawn spawnPacket)
		{

		}
		public void HandleMonsterMove(C_MonsterMove movePacket)
		{
		}
		public void HandleMonsterAttack(C_MonsterAttack monAttPacket)
		{
		}
		public void HandleDamaged(C_Damaged damagedPacket)
		{
		}
		public void HandleDeadEnd(C_DeadEnd deadEndPacket)
		{

		}
		public void HandleMonsterDead(GameObject monster)
		{
		}

		public void HandleUpdatePos(C_UpdatePos updatePosPacket)
		{
			
		}
	}
}

