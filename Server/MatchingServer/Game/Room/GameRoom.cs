using Google.Protobuf;
using Google.Protobuf.Protocol;
using Org.BouncyCastle.Security;
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


			S_EnterMatching enterPacket = new S_EnterMatching();

			foreach (var player in _players)
			{
				enterPacket.UserNames.Add(player.ObjectInfo.Name);
			}


			foreach (Player p in _players)
			{
				p.Session.Send(enterPacket);
			}

		}
		public void UpdateUser()
		{
			S_EnterMatching enterPacket = new S_EnterMatching();

			foreach (var player in _players)
			{
				enterPacket.UserNames.Add(player.ObjectInfo.Name);
			}


			foreach (Player p in _players)
			{
				p.Session.Send(enterPacket);
			}
		}
		public void LeaveGame(int playerId)
		{
			Player player = _players.Find(p => p.ObjectInfo.ObjectId == playerId);
			if (player == null)
				return;

			_players.Remove(player);
			player.Room = null;

			UpdateUser();

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

