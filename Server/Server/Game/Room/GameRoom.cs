using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;

namespace Server.Game
{
	// JobQueue를 적용한 시점에서 아래 함수들을 GameRoom이외의 공간에서 사용하게되면 문제가 됨
	public partial class GameRoom : JobSerializer
	{
		public const int VisionRange = 15;
		public int RoomId { get; set; }

		List<Player> _players = new List<Player>();
		public Player ClientForServer { get; set; }

		public List<Zone> Zones { get; set; } = new List<Zone>();

		public Zone GetZone(float posX, float posZ)
		{
			if (Zones.Count == 0)
				return null;

			foreach (var zone in Zones)
			{
				if ((posX >= zone.Info.LeftX && posX < zone.Info.RightX) &&
					(posZ >= zone.Info.BackwardZ && posZ < zone.Info.ForwardZ))
					return zone;
			}

			return null;
		}
		public void Update(float deltaTime)
		{
			Flush();
		}
		public void EnterGameForServer(Player clientForServer)
		{
			if (clientForServer == null)
				return;
			clientForServer.Room = this;
			S_EnterGame enterPacket = new S_EnterGame();
			enterPacket.Player = clientForServer.ObjectInfo;

			clientForServer.Session.Send(enterPacket);
			clientForServer.Update();
			_players.Add(clientForServer);
		}
		public void EnterGame(Player newPlayer)
		{
			if (newPlayer == null)
				return;

			_players.Add(newPlayer);
			newPlayer.Room = this;

			newPlayer.RefreshAdditionStat();

			// 플레이어가 있는 존의 플레이어 리스트에 플레이어 추가
			var zone = GetZone(newPlayer.PosDirInfo.PosX, newPlayer.PosDirInfo.PosZ);
			if (zone != null)
				zone.Players.Add(newPlayer);

			// 본인한테 정보 전송
			{
				// 유저 정보 전송
				S_EnterGame enterPacket = new S_EnterGame();
				enterPacket.Player = newPlayer.ObjectInfo;
				enterPacket.Player.EquippedWeaponTemplateId = -1;
				var equippedWeapon = newPlayer.Inventory.Find(args => args.Equipped == true && args.ItemType == ItemType.Weapon);
				if(equippedWeapon != null)
					enterPacket.Player.EquippedWeaponTemplateId = equippedWeapon.TemplateId;
				Thread.Sleep(100);
				newPlayer.Session.Send(enterPacket);

				newPlayer.Vision.Update();

				
				
			}

			// 타인한테 정보 전송
			{
				S_Spawn spawnPacket = new S_Spawn();
				spawnPacket.Objects.Add(newPlayer.ObjectInfo);
				ClientForServer.Session.Send(spawnPacket);
				//foreach (Player p in _players)
				//{
				//	if (newPlayer != p)
				//		p.Session.Send(spawnPacket);
				//}
			}
		}
		public void LeaveGame(int playerId)
		{
			Player player = _players.Find(p => p.ObjectInfo.ObjectId == playerId);
			if (player == null)
				return;

			// 플레이어가 있는 존의 플레이어 리스트에 플레이어 삭제
			GetZone(player.PosDirInfo.PosX, player.PosDirInfo.PosZ).Players.Remove(player);

			player.OnLeaveGame();

			_players.Remove(player);
			player.Room = null;

			// 본인한테 정보 전송
			{
				S_LeaveGame leavePacket = new S_LeaveGame();
				player.Session.Send(leavePacket);
			}

			// 몬스터용 클라한테 정보 전송
			{
				S_Despawn despawnPacket = new S_Despawn();
				despawnPacket.Objects.Add(player.ObjectInfo);

				ClientForServer.Session.Send(despawnPacket);

			}
		}
		public void Broadcast(PosDirInfo pos, IMessage packet, int myIdForExcept)
		{
			List<Zone> zones = GetAdjacentZones(pos);

			foreach (Zone zone in zones)
			{
				foreach (Player p in zone.Players)
				{
					if (p.ObjectInfo.ObjectId == myIdForExcept)
						continue;

					// 이 코드 없으면 몬스터용 클라에서 플레이어 위치 정보를 못받음
					if (p == ClientForServer)
					{
						p.Session.Send(packet);
						continue;
					}

					float dx = p.PosDirInfo.PosX - pos.PosX;
					float dz = p.PosDirInfo.PosZ - pos.PosZ;

					// 유효 범위에서 벗어남
					if (Math.Abs(dx) > GameRoom.VisionRange)
						continue;
					if (Math.Abs(dz) > GameRoom.VisionRange)
						continue;



					// IO함수는 내부적으로 시스템콜을 발생 -> 유저레벨에서 커널레벨로 이동 -> 컨텍스트 스위칭 발생
					// => Send할때 마다 컨텍스트 스위칭이 발생 -> 부하가 많아짐
					// => GameLogic스레드에서 처리하지 않고 Network 스레드에서 처리한다.
					p.Session.Send(packet);
				}
			}
			//foreach (Player p in _players)
			//{
			//
			//	if (p.ObjectInfo.ObjectId == myIdForExcept)
			//		continue;
			//
			//	// IO함수는 내부적으로 시스템콜을 발생 -> 유저레벨에서 커널레벨로 이동 -> 컨텍스트 스위칭 발생
			//	// => Send할때 마다 컨텍스트 스위칭이 발생 -> 부하가 많아짐
			//	// => GameLogic스레드에서 처리하지 않고 Network 스레드에서 처리한다.
			//	p.Session.Send(packet);
			//}
		}
		public void Broadcast(PosDirInfo pos, IMessage packet)
		{
			List<Zone> zones = GetAdjacentZones(pos);

			foreach (Zone zone in zones)
			{
				foreach (Player p in zone.Players)
				{
					if (p == ClientForServer)
					{
						p.Session.Send(packet);
						continue;
					}

					float dx = p.PosDirInfo.PosX - pos.PosX;
					float dz = p.PosDirInfo.PosZ - pos.PosZ;

					// 유효 범위에서 벗어남
					if (Math.Abs(dx) > GameRoom.VisionRange)
						continue;
					if (Math.Abs(dz) > GameRoom.VisionRange)
						continue;
					p.Session.Send(packet);
				}
			}
			//foreach (Player p in _players)
			//{
			//	p.Session.Send(packet);
			//}
		}
		public void Broadcast(IMessage packet)
		{

			foreach (Player p in _players)
			{
				if (p == ClientForServer)
				{
					p.Session.Send(packet);
					continue;
				}
				p.Session.Send(packet);
			}

		}

		public List<Zone> GetAdjacentZones(PosDirInfo pos, int range = VisionRange)
		{
			HashSet<Zone> zones = new HashSet<Zone>();

			Zone zone = null;
			var forwardLefX = pos.PosX - range;
			var forwardLefZ = pos.PosZ + range;
			zone = GetZone(forwardLefX, forwardLefZ);
			if (zone != null)
				zones.Add(zone);


			var forwardRightX = pos.PosX + range;
			var forwardRightZ = pos.PosZ + range;
			zone = GetZone(forwardRightX, forwardRightZ);
			if (zone != null)
				zones.Add(zone);


			var backwardLefX = pos.PosX - range;
			var backwardLefZ = pos.PosZ - range;
			zone = GetZone(backwardLefX, backwardLefZ);
			if (zone != null)
				zones.Add(zone);


			var backwardRightX = pos.PosX + range;
			var backwardRightZ = pos.PosZ - range;
			zone = GetZone(backwardRightX, backwardRightZ);
			if (zone != null)
				zones.Add(zone);


			return zones.ToList();
		}

		public void HandleZone(C_MapZone zonePacket)
		{
			foreach (var zoneInfo in zonePacket.Zones)
			{
				Zone zone = new Zone();
				zone.Info.MergeFrom(zoneInfo);
				zone.Players.Add(ClientForServer);
				Zones.Add(zone);
			}
		}

	}
}

