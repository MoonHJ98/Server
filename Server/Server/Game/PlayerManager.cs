﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Server.Game.Object;

namespace Server.Game
{
    public class PlayerManager
	{
		public static PlayerManager Instance { get; } = new PlayerManager();

		object _lock = new object();
		Dictionary<int, Player> _players = new Dictionary<int, Player>();
		public int _playerId = 1;
		
		public Player AddClientForServer()
		{
			Player client = new Player();

			lock (_lock)
			{
				client.ObjectInfo.ObjectId = _playerId;
				_playerId++;
			}

			return client;
		}
		public Player Add()
		{
			Player player = new Player();

			lock (_lock)
			{
				player.ObjectInfo.ObjectId = _playerId;
				_players.Add(_playerId, player);
				_playerId++;
			}

			return player;
		}

		public bool Remove(int playerId)
		{
			lock (_lock)
			{
				return _players.Remove(playerId);
			}
		}

		public Player Find(int playerId)
		{
			lock (_lock)
			{
				Player player = null;
				if (_players.TryGetValue(playerId, out player))
					return player;

				return null;
			}
		}
	}
}
