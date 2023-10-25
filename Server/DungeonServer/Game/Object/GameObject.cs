using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game.Object
{
	public class GameObject
	{
		public enum GameObjectType { None, Player, Monster, Projectile }
		public GameObjectType ObjectType { get; set; }
		public ObjectInfo ObjectInfo { get; set; } = new ObjectInfo() { PosDirInfo = new PosDirInfo(), StatInfo = new StatInfo(), Target = new PosDirInfo() };
		public PosDirInfo PosDirInfo 
		{
			set
			{
				if (ObjectInfo.PosDirInfo.Equals(value))
					return;
				ObjectInfo.PosDirInfo = value;
			}
			get
			{
				return ObjectInfo.PosDirInfo;
			}
		}
		public PosDirInfo Target
		{
			set
			{
				if (ObjectInfo.Target.Equals(value))
					return;
				ObjectInfo.Target = value;
			}
			get
			{
				return ObjectInfo.Target;
			}
		}
		public StatInfo StatInfo
		{
			get
			{
				return ObjectInfo.StatInfo;
			}
			set
			{
				if (ObjectInfo.StatInfo.Equals(value))
					return;
				ObjectInfo.StatInfo = value;
			}
		}
		public virtual int TotalAttack { get { return StatInfo.Damage; } }
		public virtual int TotalDefense { get { return StatInfo.Defense; } }

		public GameRoom Room { get; set; }

		public virtual void OnDamaged(GameObject attacker, int damage)
		{
			if(Room == null) 
				return;

			damage = Math.Max((damage - TotalDefense), 0);

			StatInfo.Hp = Math.Max(StatInfo.Hp - damage, 0);

			S_ChangeHp changePacket = new S_ChangeHp();
			changePacket.DefenderId = ObjectInfo.ObjectId;

			// 몬스터라면 젠포인터 넣어주기
			if (string.IsNullOrEmpty(ObjectInfo.GenPoint) == false)
				changePacket.DefenderGenId = ObjectInfo.GenPoint;

			changePacket.DefenderHp = StatInfo.Hp;

			Room.Push(Room.Broadcast, changePacket);


			if (StatInfo.Hp <= 0)
			{
				OnDead(attacker);
			}
		}
		public virtual void OnDead(GameObject attacker)
		{
			if (Room == null)
				return;

			if (ObjectType == GameObjectType.Player)
			{
				S_Dead packet = new S_Dead();
				if (ObjectType == GameObjectType.Monster)
					packet.GenId = ObjectInfo.GenPoint;

				packet.ObjectId = ObjectInfo.ObjectId;
				packet.State = (int)CreatureState.Dead;

				var room = Room;
				room.Push(room.Broadcast, packet);
			}
			else if(ObjectType == GameObjectType.Monster)
			{
				var room = Room;
				room.Push(room.HandleMonsterDead, this);
				
			}

			//// 죽었는데 플레이어일 경우
			//if (ObjectType == GameObjectType.Player)
			//{
			//	room.Push(room.LeaveGame, packet.ObjectId);
			//}
			//else // 몬스터일 경우
			//{
			//	room.ClientForServer.DeleteMonster(packet.GenId, packet.ObjectId);
			//}
				
		}

		public virtual GameObject GetOnwer()
		{
			return this;
		}
	}
}
