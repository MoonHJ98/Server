using Google.Protobuf.Protocol;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Game.Room
{

	public class VisionCube
	{
		public Player Owner { get; private set; }
		public HashSet<GameObject> PreviousObjects { get; private set; } = new HashSet<GameObject>();

		public VisionCube(Player owner)
		{
			Owner = owner;
		}

		public HashSet<GameObject> GatherObjects()
		{
			if (Owner == null || Owner.Room == null)
				return null;

			HashSet<GameObject> objects = new HashSet<GameObject>();

			PosDirInfo pos = Owner.PosDirInfo;
			List<Zone> zones = Owner.Room.GetAdjacentZones(pos);

			foreach (Zone zone in zones)
			{
				foreach (Player player in zone.Players)
				{
					float dx = player.PosDirInfo.PosX - pos.PosX;
					float dz = player.PosDirInfo.PosZ - pos.PosZ;

					// 유효 범위에서 벗어남
					if (Math.Abs(dx) > GameRoom.VisionRange)
						continue;
					if (Math.Abs(dz) > GameRoom.VisionRange)
						continue;

					objects.Add(player);

				}
				foreach (Monster monster in zone.Monsters)
				{
					float dx = monster.PosDirInfo.PosX - pos.PosX;
					float dz = monster.PosDirInfo.PosZ - pos.PosZ;

					// 유효 범위에서 벗어남
					if (Math.Abs(dx) > GameRoom.VisionRange)
						continue;
					if (Math.Abs(dz) > GameRoom.VisionRange)
						continue;

					objects.Add(monster);
				}
			}

			return objects;
		}
		public void Update()
		{
			if(Owner == null || Owner.Room == null)
				return;

			HashSet<GameObject> currentObjects = GatherObjects();

			// 기존에 없었는데 새로 생겼으면 Spawn
			var added = currentObjects.Except(PreviousObjects).ToList();
			if(added.Count > 0)
			{
				S_Spawn spawnPacket = new S_Spawn();
				foreach(GameObject obj in added)
				{
					if (obj == Owner.Room.ClientForServer)
						continue;
					ObjectInfo info = new ObjectInfo();
					info.MergeFrom(obj.ObjectInfo);
					spawnPacket.Objects.Add(info);
				}
				Owner.Session.Send(spawnPacket);
			}

			// 기존에 있었는데 사라지면 Despawn
			
			var removed = PreviousObjects.Except(currentObjects).ToList();
			if (removed.Count > 0)
			{
				S_Despawn despawnPacket = new S_Despawn();
				foreach (GameObject obj in removed)
				{
					ObjectInfo info = new ObjectInfo();
					info.MergeFrom(obj.ObjectInfo);
					despawnPacket.Objects.Add(info);
				}
				Owner.Session.Send(despawnPacket);
			}

			PreviousObjects = currentObjects;

			Owner.Room.PushAfter(500, Update);
		}
	}
}
