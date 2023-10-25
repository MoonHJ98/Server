using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using Server.Game.Object;
using ServerCore;


class PacketHandler
{
	// 패킷 핸들러는 여러 스레드가 접근하는 임계영역
	public static void C_MoveHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_AttackHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_MonsterMoveHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_MonsterAttackHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_DamagedHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_LoginHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_EnterGameHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_EquipItemHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_MapZoneHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_SpawnHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_DeadEndHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_UpdatePosHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_DungeonDamagedHandler(PacketSession session, IMessage packet)
	{
	}
	public static void S_DungeonDamagedHandler(PacketSession session, IMessage packet)
	{
	}
	public static void C_MatchingLoginHandler(PacketSession session, IMessage packet)
	{
		C_MatchingLogin matchingPacket = packet as C_MatchingLogin;

		ClientSession clientSession = session as ClientSession;

		clientSession.HandleEnterGame(matchingPacket);
	}
}
