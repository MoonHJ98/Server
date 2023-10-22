
using Google.Protobuf.Protocol;
using Google.Protobuf;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using static System.Net.Mime.MediaTypeNames;


public class PacketHandler
{
	// Step 4
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;


	}
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;

	}
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;

	}
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;

	}
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		S_Move movePacket = packet as S_Move;

	}
	public static void S_AttackHandler(PacketSession session, IMessage packet)
	{
		S_Attack attackPacket = packet as S_Attack;

	}

	public static void S_MonsterMoveHandler(PacketSession session, IMessage packet)
	{

		S_MonsterMove movePacket = packet as S_MonsterMove;

	}
	public static void S_MonsterAttackHandler(PacketSession session, IMessage packet)
	{
		S_MonsterAttack attackPacket = packet as S_MonsterAttack;

	}
	public static void S_DeadHandler(PacketSession session, IMessage packet)
	{
		S_Dead deadPacket = packet as S_Dead;

	}
	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changeHpPacket = packet as S_ChangeHp;

	}

	// Step 1
	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{

		ServerSession serverSession = session as ServerSession;
		C_Login loginPacket = new C_Login()
		{
			UniqueId = $"DummyClient_{serverSession.DummyId.ToString("0000")}"
		};
		serverSession.Send(loginPacket);
	}

	// Step 2
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = (S_Login)packet;
		ServerSession serverSession = (ServerSession)session;

		// TODO : 로비 UI에서 캐릭터 보여주고, 선택할 수 있도록
		if (loginPacket.Players == null || loginPacket.Players.Count == 0)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{serverSession.DummyId.ToString("0000")}";
			serverSession.Send(createPacket);
		}
		else
		{
			// 무조건 첫번째 로그인
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = info.Name;
			serverSession.Send(enterGamePacket);
		}
	}

	// Step 3
	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createOkPacket = (S_CreatePlayer)packet;
		ServerSession serverSession = (ServerSession)session;

		if (createOkPacket.Player == null)
		{
			// 생략
		}
		else
		{
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = createOkPacket.Player.Name;
			serverSession.Send(enterGamePacket);
		}
	}
	public static void S_ItemListHandler(PacketSession session, IMessage packet)
	{
		S_ItemList itemListPacket = packet as S_ItemList;

	}
	public static void S_AddItemHandler(PacketSession session, IMessage packet)
	{
		S_AddItem addItemPacket = packet as S_AddItem;
	}
	public static void S_EquipItemHandler(PacketSession session, IMessage packet)
	{
		S_EquipItem equipItemPacket = packet as S_EquipItem;

	}
	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat changeStatPacket = packet as S_ChangeStat;
	}
	public static void S_DeadEndHandler(PacketSession session, IMessage packet)
	{

	}
	public static void S_MonstersDeadHandler(PacketSession session, IMessage packet)
	{
		S_MonstersDead deadPacket = packet as S_MonstersDead;
	}
}
