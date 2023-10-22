using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

class PacketHandler
{
	public static void S_EnterGameHandler(PacketSession session, IMessage packet)
	{
		S_EnterGame enterGamePacket = packet as S_EnterGame;
		ServerSession serverSession = session as ServerSession;

		if (enterGamePacket.Player.ObjectId == 1)
		{
			//Managers.Network.ConnectToGame();
			//Managers.Scene.LoadScene(Define.Scene.Game);
			Managers.Objects.AddClientForServer();
			return;
		}

		// 여기서 만들면 네트워크 스레드에서 생성하는것임.
		// 게임컨텐츠에는 메인스레드에서만 접근할 수 있음
		// ClientPacketManager에서 패킷을 만듣 후 PacketHandler에서 바로 실행되는 것이 아니라
		// PacketQueue에 넣은 후 메인스레드에서 생성해줘야함
		Managers.Objects.Add(enterGamePacket.Player, myPlayer: true);

	}
	public static void S_LeaveGameHandler(PacketSession session, IMessage packet)
	{
		S_LeaveGame leaveGamePacket = packet as S_LeaveGame;
		ServerSession serverSession = session as ServerSession;

		Managers.Objects.RemoveMyPlayer();
	}
	public static void S_SpawnHandler(PacketSession session, IMessage packet)
	{
		S_Spawn spawnPacket = packet as S_Spawn;
		ServerSession serverSession = session as ServerSession;


		foreach (ObjectInfo info in spawnPacket.Objects)
		{
			if (info.SpawnInfo == SpawnInfo.Player)
				Managers.Objects.Add(info, myPlayer: false);
			else if (info.SpawnInfo == SpawnInfo.Monster)
				Managers.Objects.AddMonstersForClient(info);

		}
	}
	public static void S_DespawnHandler(PacketSession session, IMessage packet)
	{
		S_Despawn despawnPacket = packet as S_Despawn;
		ServerSession serverSession = session as ServerSession;

		foreach (var info in despawnPacket.Objects)
		{
			if (info.SpawnInfo == SpawnInfo.Player)
				Managers.Objects.Remove(info.ObjectId);
			else if (info.SpawnInfo == SpawnInfo.Monster)
				Managers.Objects.RemoveMonster(info.GenPoint, info.ObjectId);

		}
	}
	public static void S_MoveHandler(PacketSession session, IMessage packet)
	{
		// 나는 서버한테 내가 이동했다는 패킷 받을 필요없음
		// 클라 움직이면 서버에 C_Move 전송
		// 서버에서 룸에서 플레이어들한테 S_Move 브로드캐스트
		// 이미 클라에서 내꺼 움직인 상태니까 받을 필요 없음 
		S_Move movePacket = packet as S_Move;
		ServerSession serverSession = session as ServerSession;

		GameObject obj = Managers.Objects.Find(movePacket.PlayerId);
		if (obj == null)
			return;

		CreatureController cc = obj.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.PosDirInfo = movePacket.PosDirInfo;
		cc.UpdateAnimation();

	}
	public static void S_AttackHandler(PacketSession session, IMessage packet)
	{
		S_Attack attackPacket = packet as S_Attack;
		ServerSession serverSession = session as ServerSession;

		GameObject obj = Managers.Objects.Find(attackPacket.PlayerId);
		if (obj == null)
			return;

		CreatureController cc = obj.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.SetState(attackPacket.Info.AttackId);
	}

	public static void S_MonsterMoveHandler(PacketSession session, IMessage packet)
	{

		S_MonsterMove movePacket = packet as S_MonsterMove;
		ServerSession serverSession = session as ServerSession;

		GameObject obj = Managers.Objects.FindMonster(movePacket.MonsterInfo.GenPoint, movePacket.MonsterInfo.ObjectId);
		if (obj == null)
			return;

		CreatureController cc = obj.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.PosDirInfo = movePacket.MonsterInfo.PosDirInfo;
		cc.targetPos = movePacket.MonsterInfo.Target;
	}
	public static void S_MonsterAttackHandler(PacketSession session, IMessage packet)
	{
		S_MonsterAttack attackPacket = packet as S_MonsterAttack;
		ServerSession serverSession = session as ServerSession;

		GameObject obj = Managers.Objects.FindMonster(attackPacket.MonsterInfo.GenPoint, attackPacket.MonsterInfo.ObjectId);
		if (obj == null)
			return;

		CreatureController cc = obj.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.SetState(attackPacket.AttackInfo.AttackId);
		cc.targetPos = attackPacket.MonsterInfo.Target;
	}
	public static void S_DeadHandler(PacketSession session, IMessage packet)
	{
		S_Dead deadPacket = packet as S_Dead;
		ServerSession serverSession = session as ServerSession;

		GameObject obj = null;

		if (string.IsNullOrEmpty(deadPacket.GenId) == false)
			obj = Managers.Objects.FindMonster(deadPacket.GenId, deadPacket.ObjectId);
		else
			obj = Managers.Objects.Find(deadPacket.ObjectId);

		if (obj == null)
			return;

		CreatureController cc = obj.GetComponent<CreatureController>();
		if (cc == null)
			return;

		cc.SetState(deadPacket.State);
	}
	public static void S_ChangeHpHandler(PacketSession session, IMessage packet)
	{
		S_ChangeHp changeHpPacket = packet as S_ChangeHp;
		ServerSession serverSession = session as ServerSession;

		// 플레이어
		if (string.IsNullOrEmpty(changeHpPacket.DefenderGenId))
		{
			GameObject obj = Managers.Objects.Find(changeHpPacket.DefenderId);
			if (obj == null)
				return;

			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				return;

			cc.Stat.Hp = changeHpPacket.DefenderHp;

		}
		else // 몬스터
		{
			GameObject obj = Managers.Objects.FindMonster(changeHpPacket.DefenderGenId, changeHpPacket.DefenderId);
			if (obj == null)
				return;

			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				return;

			cc.Hp = changeHpPacket.DefenderHp;
		}

	}

	public static void S_ConnectedHandler(PacketSession session, IMessage packet)
	{
		//Debug.Log("S_ConnectedHandler");

		// 유니티에서 적절하게 유니크 키를 제공
		// 같은 로컬 머신에서 클라를 여러개 띄우기 위한 코드
		string path = Application.dataPath;

		C_Login loginPacket = new C_Login()
		{
			UniqueId = path.GetHashCode().ToString(),
			AccountName = Managers.Network.AccountName,
			Token = Managers.Network.Token
		};
		Managers.Network.Send(loginPacket);
	}
	public static void S_LoginHandler(PacketSession session, IMessage packet)
	{
		S_Login loginPacket = packet as S_Login;
		Debug.Log($"LogInOK({loginPacket.LoginOk})");

		// TODO : 로비에서 캐릭터 보여주고 선택할 수 있도록
		if (loginPacket.Players == null || loginPacket.Players.Count == 0)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
		}
		else
		{
			// 이미 있을 경우 무조건 첫번째로 로그인
			LobbyPlayerInfo info = loginPacket.Players[0];
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = info.Name;
			Managers.Network.Send(enterGamePacket);
		}
	}
	public static void S_CreatePlayerHandler(PacketSession session, IMessage packet)
	{
		S_CreatePlayer createOkPacket = packet as S_CreatePlayer;

		if (createOkPacket.Player == null)
		{
			C_CreatePlayer createPacket = new C_CreatePlayer();
			createPacket.Name = $"Player_{Random.Range(0, 10000).ToString("0000")}";
			Managers.Network.Send(createPacket);
		}
		else
		{
			C_EnterGame enterGamePacket = new C_EnterGame();
			enterGamePacket.Name = createOkPacket.Player.Name;
			Managers.Network.Send(enterGamePacket);
		}
	}
	public static void S_ItemListHandler(PacketSession session, IMessage packet)
	{
		S_ItemList itemListPacket = packet as S_ItemList;



		Managers.Inventory.Clear();

		// 클라 메모리에 아이템 정보 저장
		foreach (var itemInfo in itemListPacket.Items)
		{
			Item item = Item.MakeItem(itemInfo);
			Managers.Inventory.Add(item);
		}

		if (Managers.Objects.MyPlayer != null)
			Managers.Objects.MyPlayer.RefreshAdditionStat();

		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.StatUI.RefreshUI();
	}
	public static void S_AddItemHandler(PacketSession session, IMessage packet)
	{
		S_AddItem addItemPacket = packet as S_AddItem;

		// 클라 메모리에 아이템 정보 저장
		foreach (var itemInfo in addItemPacket.Items)
		{
			Item item = Item.MakeItem(itemInfo);
			Managers.Inventory.Add(item);
		}
		UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

		gameSceneUI.InvenUI.RefreshUI();
		gameSceneUI.StatUI.RefreshUI();

		if (Managers.Objects.MyPlayer != null)
			Managers.Objects.MyPlayer.RefreshAdditionStat();
	}
	public static void S_EquipItemHandler(PacketSession session, IMessage packet)
	{
		S_EquipItem equipItemPacket = packet as S_EquipItem;

		if (Managers.Objects.MyPlayer == null)
		{
			if (equipItemPacket.ItemType != ItemType.Weapon)
				return;

			var player = Managers.Objects.Find(equipItemPacket.PlayerId);

			Managers.Objects.UpdateWeapon(player, equipItemPacket);
			return;
		}

		if (Managers.Objects.MyPlayer.Id == equipItemPacket.PlayerId)
		{
			Item item = Managers.Inventory.Get(equipItemPacket.ItemId);
			if (item == null)
				return;

			item.Equipped = equipItemPacket.Equipped;

			UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;

			gameSceneUI.InvenUI.RefreshUI();
			gameSceneUI.StatUI.RefreshUI();

			if (Managers.Objects.MyPlayer != null)
				Managers.Objects.MyPlayer.RefreshAdditionStat();
		}
		else
		{

			if (equipItemPacket.ItemType != ItemType.Weapon)
				return;

			var player = Managers.Objects.Find(equipItemPacket.PlayerId);

			Managers.Objects.UpdateWeapon(player, equipItemPacket);
		
		}

	}
	public static void S_ChangeStatHandler(PacketSession session, IMessage packet)
	{
		S_ChangeStat changeStatPacket = packet as S_ChangeStat;
	}
	public static void S_DeadEndHandler(PacketSession session, IMessage packet)
	{
		S_DeadEnd deadEndPacket = packet as S_DeadEnd;
		if (deadEndPacket.Info.SpawnInfo == SpawnInfo.Monster)
			Managers.Objects.RemoveMonster(deadEndPacket.Info.GenPoint, deadEndPacket.Info.ObjectId);
		else if (deadEndPacket.Info.SpawnInfo == SpawnInfo.Player)
			Managers.Objects.Remove(deadEndPacket.Info.ObjectId);

	}
	public static void S_MonstersDeadHandler(PacketSession session, IMessage packet)
	{
		S_MonstersDead deadPacket = packet as S_MonstersDead;

		foreach (var monster in deadPacket.Monsters)
		{
			var obj = Managers.Objects.FindMonster(monster.GenPoint, monster.ObjectId);
			if (obj == null)
				return;

			CreatureController cc = obj.GetComponent<CreatureController>();
			if (cc == null)
				return;
			cc.SetState((int)CreatureState.Dead);
		}
	}
}
