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
		public int RoomId { get; set; }

		List<Player> _players = new List<Player>();

		public void Update(float deltaTime)
		{
			Flush();
		}
		public void EnterGame(Player newPlayer)
		{
			if (newPlayer == null)
				return;

			_players.Add(newPlayer);
			newPlayer.Room = this;

			newPlayer.RefreshAdditionStat();

			// 본인한테 정보 전송
			{
				// 유저 정보 전송
				S_EnterGame enterPacket = new S_EnterGame();
				enterPacket.ServerInfo = ServerConnecInfo.Dungeon;
				enterPacket.Player = newPlayer.ObjectInfo;
				enterPacket.Player.EquippedWeaponTemplateId = -1;
				var equippedWeapon = newPlayer.Inventory.Find(args => args.Equipped == true && args.ItemType == ItemType.Weapon);
				if(equippedWeapon != null)
					enterPacket.Player.EquippedWeaponTemplateId = equippedWeapon.TemplateId;
				Thread.Sleep(200);
				newPlayer.Session.Send(enterPacket);

				S_Spawn spawnPacket = new S_Spawn();
				foreach(var player in _players)
				{
					if (player == newPlayer)
						continue;
					spawnPacket.Objects.Add(player.ObjectInfo);
				}
				newPlayer.Session.Send(spawnPacket);
			}

			// 타인한테 정보 전송
			{
				S_Spawn spawnPacket = new S_Spawn();
				spawnPacket.Objects.Add(newPlayer.ObjectInfo);

				foreach (Player p in _players)
				{
					if (newPlayer != p)
						p.Session.Send(spawnPacket);
				}
			}
		}
		public void LeaveGame(int playerId)
		{
			Player player = _players.Find(p => p.ObjectInfo.ObjectId == playerId);
			if (player == null)
				return;

			player.OnLeaveGame();

			_players.Remove(player);
			player.Room = null;

			// 본인한테 정보 전송
			{
				S_LeaveGame leavePacket = new S_LeaveGame();
				player.Session.Send(leavePacket);
			}

		}
		public void Broadcast(IMessage packet)
		{

			foreach (Player p in _players)
			{
				p.Session.Send(packet);
			}

		}
		public void Broadcast(IMessage packet, int playerId)
		{

			foreach (Player p in _players)
			{
				if (p.ObjectInfo.ObjectId == playerId)
					continue;
				p.Session.Send(packet);
			}

		}
	}
}

