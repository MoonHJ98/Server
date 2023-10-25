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

		public GameRoom Room { get; set; }


	}
}
