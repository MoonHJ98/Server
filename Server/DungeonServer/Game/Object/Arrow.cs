using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game.Object
{
	public class Arrow : GameObject
	{
		public GameObject Onwer { get; set; }

		public Arrow()
		{
			ObjectType = GameObjectType.Projectile;
		}
		
		// 화살이 마지막으로 때렸을때 소유자에게 보상을 주기 위함
		public override GameObject GetOnwer()
		{
			return Onwer;
		}
	}
}
