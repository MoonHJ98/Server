using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Data;
using Server.DB;
using Server.Game.Object;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;

namespace Server.Game
{
	// JobQueue를 적용한 시점에서 아래 함수들을 GameRoom이외의 공간에서 사용하게되면 문제가 됨
	public partial class GameRoom : JobSerializer
	{
		public void HandleEquipItem(Player player, C_EquipItem equipPacket)
		{
			if (player == null)
				return;

			player.HandleEquipItem(equipPacket);
		}
	}
}

