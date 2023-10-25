using Google.Protobuf;
using Google.Protobuf.Protocol;
using Org.BouncyCastle.Asn1.X509;
using Server;
using Server.DB;
using Server.Game;
using Server.Game.Object;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

class PacketHandler
{
	// 패킷 핸들러는 여러 스레드가 접근하는 임계영역
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
		C_Move movePacket = packet as C_Move;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;


		// 멀티 스레드 환경에서 클라이언트 세션의 마이플레이어를 직접 비교하는 건 위험
		// 여기서 null체크에서 통과한더라도 아래에 내려갔을 때 다른 스레드에서 null로 만들 수 있음
		// 그래서 위에처럼 지역변수로 받고 실행
		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		// 플레이어에게 데이터를 밀어 넣는 것은 여러곳에서 발생할 경우 멀티스레드 환경에서
		// 문제가 발생할 수 있기 때문에 한 곳에 모아 놓는다.
		room.Push(room.HandleMove, player, movePacket);
	}
	public static void C_AttackHandler(PacketSession session, IMessage packet)
	{
		C_Attack attackPacket = packet as C_Attack;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleAttack, player, attackPacket);
	}

	public static void C_MonsterMoveHandler(PacketSession session, IMessage packet)
	{
		C_MonsterMove movePacket = packet as C_MonsterMove;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleMonsterMove, movePacket);

	}

	public static void C_MonsterAttackHandler(PacketSession session, IMessage packet)
	{
		C_MonsterAttack attackPacket = packet as C_MonsterAttack;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleMonsterAttack, attackPacket);
	}
	public static void C_DamagedHandler(PacketSession session, IMessage packet)
	{
		C_Damaged damagedPacket = packet as C_Damaged;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;


		room.Push(room.HandleDamaged, damagedPacket);
	}
	public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
		C_Login logInPacket = packet as C_Login;
		ClientSession clientSession = session as ClientSession;

		clientSession.HandleLogin(logInPacket);

	}
	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
	{
		C_EnterGame enterPacket = packet as C_EnterGame;
		ClientSession clientSession = session as ClientSession;

		clientSession.HandleEnterGame(enterPacket);
	}
	public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		C_CreatePlayer createPlayerPacket = packet as C_CreatePlayer;
		ClientSession clientSession = session as ClientSession;

		clientSession.HandleCreatePlayer(createPlayerPacket);
	}
	public static void C_EquipItemHandler(PacketSession session, IMessage packet)
	{
		C_EquipItem equipItemPacket = packet as C_EquipItem;
		ClientSession clientSession = session as ClientSession;
		
		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleEquipItem, player, equipItemPacket);
	}
	public static void C_MapZoneHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_SpawnHandler(PacketSession session, IMessage packet)
	{
		C_Spawn spawnPacket = packet as C_Spawn;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleSpawn, spawnPacket);
	}
	public static void C_DeadEndHandler(PacketSession session, IMessage packet)
	{
		C_DeadEnd deadEndPacket = packet as C_DeadEnd;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleDeadEnd, deadEndPacket);
	}
	public static void C_UpdatePosHandler(PacketSession session, IMessage packet)
	{
		C_UpdatePos updatePosPacket = packet as C_UpdatePos;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleUpdatePos, updatePosPacket);
	}
	public static void C_DungeonDamagedHandler(PacketSession session, IMessage packet)
	{
		C_DungeonDamaged damagedPacket = packet as C_DungeonDamaged;
		ClientSession clientSession = session as ClientSession;

		Player player = clientSession.MyPlayer;

		if (player == null)
			return;
		GameRoom room = player.Room;
		if (room == null)
			return;

		room.Push(room.HandleDungeonDamaged, damagedPacket);
	}
	public static void C_MatchingLoginHandler(PacketSession session, IMessage packet)
	{
	}
}
