using Google.Protobuf;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class CreatureController : MonoBehaviour
{
	public PosDirInfo targetPos = new PosDirInfo();
	public SpawnInfo spawnInfo = new SpawnInfo();

	public GameObject weapon = null;

	HpBar hpBar;

	// 몬스터를 각 클라이언트에게 보여주기 위한 키
	public string genPointKey;
	public GameObject genPoint;
	public List<GameObject> patrolPoints;

	// 서버에서 받은 위치, 방향 정보
	PosDirInfo posDirInfo = new PosDirInfo();
	StatInfo statInfo = new StatInfo();

	public Coroutine coUpdatePos;


	protected Coroutine coAttackCooltime = null;


	public PosDirInfo PosDirInfo
	{
		get { return posDirInfo; }
		set
		{
			if (posDirInfo.Equals(value) == false)
			{
				posDirInfo = value;

				UpdateAnimation();
			}
		}
	}
	public Vector3 TargetPos
	{
		get
		{
			return new Vector3(targetPos.PosX, targetPos.PosY, targetPos.PosZ);
		}
		set
		{
			Vector3 pos = new Vector3(targetPos.PosX, targetPos.PosY, targetPos.PosZ);

			if (pos.Equals(value))
				return;

			targetPos.PosX = value.x;
			targetPos.PosY = value.y;
			targetPos.PosZ = value.z;
			UpdateAnimation();
		}
	}
	public StatInfo Stat
	{
		get { return statInfo; }
		set
		{
			if (statInfo.Equals(value))
				return;
			statInfo = value;
			UpdateHpBar();
		}
	}
	public int Hp
	{
		get { return Stat.Hp; }
		set
		{
			Stat.Hp = value;
			UpdateHpBar();
		}
	}
	public int Id { get; set; }
	protected Vector3 dir = Vector3.zero;
	protected Vector3 lookDir = Vector3.zero;

	protected Animator animator;

	protected NavMeshAgent navMeshAgent;


	public CreatureState State
	{
		get { return posDirInfo.State; }
		set
		{
			if (posDirInfo.State == value)
				return;

			posDirInfo.State = value;
			UpdateAnimation();
		}
	}
	public Vector3 Position
	{
		get
		{
			return new Vector3(posDirInfo.PosX, posDirInfo.PosY, posDirInfo.PosZ);
		}
		set
		{
			Vector3 pos = new Vector3(posDirInfo.PosX, posDirInfo.PosY, posDirInfo.PosZ);

			if (pos.Equals(value))
				return;

			PosDirInfo.PosX = value.x;
			PosDirInfo.PosY = value.y;
			PosDirInfo.PosZ = value.z;
			//UpdateAnimation();
		}
	}
	public Vector3 LookDir
	{
		get
		{
			return new Vector3(posDirInfo.LookDirX, 0f, posDirInfo.LookDirZ);
		}
		set
		{
			Vector3 dir = new Vector3(posDirInfo.LookDirX, 0f, posDirInfo.LookDirZ);

			var length = Vector3.Distance(dir, value);
			if (length <= 0.25f)
				return;
			posDirInfo.LookDirX = value.x;
			posDirInfo.LookDirZ = value.z;

			if (spawnInfo != SpawnInfo.Monster)
				UpdateAnimation();
		}
	}

	void Start()
	{
		Init();


	}

	void UpdateHpBar()
	{
		if (hpBar == null)
			return;

		float ratio = 0f;
		if (Stat.MaxHp > 0)
		{
			ratio = (Hp / (float)Stat.MaxHp);

		}

		hpBar.SetHpBar(ratio);
	}

	void Update()
	{
		UpdateController();
		UpdateMovement();

	}
	protected virtual void UpdateController()
	{
		//Move();
	}

	public virtual void UpdateAnimation()
	{

	}

	protected virtual void UpdateMovement()
	{

	}


	public virtual void Init()
	{
		animator = GetComponent<Animator>();
		hpBar = this.gameObject.GetComponent<HpBar>();
		UpdateHpBar();
	}

	void LateUpdate()
	{
	}

	protected virtual void UpdateIdle()
	{

	}
	protected virtual void UpdateMove()
	{
	}
	protected virtual void UpdateAttack()
	{
	}
	protected virtual void UpdateDamaged()
	{

	}
	protected virtual void UpdateDead()
	{
	}
	protected virtual void UpdateDeadEnd()
	{

	}
	public void SetState(int state)
	{
		State = (CreatureState)state;
	}
	IEnumerator CoInputCooltime(float time)
	{
		yield return new WaitForSeconds(time);
		coAttackCooltime = null;
	}
	public IEnumerator CoUpdatePos()
	{
		if (State != CreatureState.Moving)
			yield break;

		float waitSeconds = 0.5f;
		yield return new WaitForSeconds(waitSeconds);

		C_UpdatePos updatePos = new C_UpdatePos()
		{
			SpawnInfo = spawnInfo,
			Id = Id,
			GenId = genPointKey,
			PosInfo = PosDirInfo
		};
		updatePos.PosInfo.LookDirX = 0f;
		updatePos.PosInfo.LookDirZ = 0f;

		Managers.Network.Send(updatePos);
		coUpdatePos = null;
	}
}
