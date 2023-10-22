using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class MyMonsterController : CreatureController
{
	Coroutine coPatrol;
	Coroutine coChaseTarget;

	private NavMeshAgent agent;

	public GameObject targetPlayer = null;

	float noticeDistance = 4f;
	float followDistanceLimit = 7.5f;
	public float attackDistance = 2f;

	bool sendDeadPacketOnce = true;

	Vector3 Destination { get; set; }

	public override void Init()
	{
		base.Init();
		agent = GetComponent<NavMeshAgent>();
		agent.obstacleAvoidanceType = 0;
		spawnInfo = SpawnInfo.Monster;

	}
	protected override void UpdateController()
	{
		base.UpdateController();

	}
	protected override void UpdateMovement()
	{
		base.UpdateMovement();

		if (coChaseTarget == null)
		{
			coChaseTarget = StartCoroutine("CoChaseTarget");
		}
		// 서버에 위치 업데이트하기 위한 코드
		if (coUpdatePos == null)
			StartCoroutine("CoUpdatePos");

		switch (State)
		{
			case CreatureState.Idle:
				LookDir = Vector3.zero;
				UpdateIdle();
				StopCoroutine("CoUpdatePos");
				coUpdatePos = null;
				break;
			case CreatureState.Falling:
				LookDir = Vector3.zero;
				UpdateMove();
				break;
			case CreatureState.Moving:
				UpdateMove();
				break;
			case CreatureState.Attack:
				LookDir = Vector3.zero;
				UpdateAttack();
				StopCoroutine("CoUpdatePos");
				coUpdatePos = null;
				break;
			case CreatureState.Dameged:
				UpdateDamaged();
				break;
			case CreatureState.Dead:
				LookDir = Vector3.zero;
				UpdateDead();
				break;
			case CreatureState.DeadEffect:
				break;
			case CreatureState.DeadEnd:
				UpdateDeadEnd();
				break;
			default:
				break;
		}
	}

	public override void UpdateAnimation()
	{
		if (animator == null)
			return;

		if (State == CreatureState.Idle)
		{
			animator.SetInteger("ani", (int)CreatureState.Idle);
			C_MonsterMove movePacket = new C_MonsterMove()
			{
				MonsterInfo = new ObjectInfo()
			};
			LookDir = Vector3.zero;
			movePacket.MonsterInfo.PosDirInfo = PosDirInfo;
			movePacket.MonsterInfo.GenPoint = genPointKey;
			movePacket.MonsterInfo.ObjectId = Id;
			Managers.Network.Send(movePacket);
		}
		else if (State == CreatureState.Moving)
		{
			animator.SetInteger("ani", (int)CreatureState.Moving);

			C_MonsterMove movePacket = new C_MonsterMove()
			{
				MonsterInfo = new ObjectInfo()
			};
			movePacket.MonsterInfo.PosDirInfo = PosDirInfo;
			movePacket.MonsterInfo.GenPoint = genPointKey;
			movePacket.MonsterInfo.ObjectId = Id;
			movePacket.MonsterInfo.Target = new PosDirInfo();
			movePacket.MonsterInfo.Target.PosX = TargetPos.x;
			movePacket.MonsterInfo.Target.PosY = TargetPos.y;
			movePacket.MonsterInfo.Target.PosZ = TargetPos.z;

			Managers.Network.Send(movePacket);

		}
		else if (State == CreatureState.Attack)
		{
			animator.SetInteger("ani", (int)CreatureState.Attack);
			LookDir = Vector3.zero;
			C_MonsterAttack attackPacket = new C_MonsterAttack()
			{
				AttackInfo = new AttackInfo() { AttackId = (int)CreatureState.Attack },
				MonsterInfo = new ObjectInfo() { GenPoint = genPointKey, ObjectId = Id, PosDirInfo = PosDirInfo }
			};
			attackPacket.MonsterInfo.Target = new PosDirInfo();
			attackPacket.MonsterInfo.Target.PosX = TargetPos.x;
			attackPacket.MonsterInfo.Target.PosY = TargetPos.y;
			attackPacket.MonsterInfo.Target.PosZ = TargetPos.z;
			Managers.Network.Send(attackPacket);

		}
		else if (State == CreatureState.Dead)
		{
			animator.SetInteger("ani", (int)CreatureState.Dead);
		}
		else
		{

		}
	}

	protected override void UpdateIdle()
	{
		base.UpdateIdle();


		// 대기중 따라갈 플레이어 찾으면 바로 이동
		if (SearchPlayer() != null)
		{
			if (coPatrol != null)
			{
				StopCoroutine("CoPatrol");
				coPatrol = null;
			}
			return;
		}

		// 패트롤
		if (coPatrol == null && agent != null)
		{
			LookDir = agent.transform.forward.normalized;
			Position = agent.transform.position;
			coPatrol = StartCoroutine("CoPatrol");
		}

	}
	protected override void UpdateMove()
	{
		if (State != CreatureState.Moving)
			return;

		base.UpdateMove();

		// 지금 바라보는 방향, 위치를 PosDir에 업데이트
		LookDir = agent.transform.forward.normalized;
		Position = transform.position;



		// 패트롤
		if (targetPlayer == null)
		{
			if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
			{
				agent.velocity = Vector3.zero;
				State = CreatureState.Idle;
				return;
			}

			if (SearchPlayer() != null)
			{
				if (coPatrol != null)
				{
					StopCoroutine("CoPatrol");
					coPatrol = null;
				}
				//agent.isStopped = true;
				agent.velocity = Vector3.zero;
				State = CreatureState.Moving;
			}
		}
		else // 추적
		{
			agent.SetDestination(targetPlayer.transform.position);
			agent.isStopped = false;
			float distanceWithPlayer = Vector3.Distance(targetPlayer.transform.position, transform.position);

			if (distanceWithPlayer > followDistanceLimit)
			{
				targetPlayer = null;
				agent.velocity = Vector3.zero;
				agent.isStopped = true;
				State = CreatureState.Idle;

			}
		}

		if (targetPlayer != null)
		{
			float distanceWithPlayer = Vector3.Distance(targetPlayer.transform.position, transform.position);

			if (distanceWithPlayer <= attackDistance)
			{
				State = CreatureState.Attack;
			}
		}

	}
	protected override void UpdateAttack()
	{
		if (targetPlayer == null)
		{
			State = CreatureState.Idle;
			return;
		}
		base.UpdateAttack();


		TargetPos = targetPlayer.transform.position;

		float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;




		if (Vector3.Distance(targetPlayer.transform.position, transform.position) > attackDistance)
		{
			if (normalizedTime >= 0.9f)
			{
				State = CreatureState.Idle;
				targetPlayer = null;
				coPatrol = null;
			}
		}

	}
	protected override void UpdateDamaged()
	{
		base.UpdateDamaged();
	}

	protected override void UpdateDead()
	{
		base.UpdateDead();
		this.gameObject.GetComponent<BoxCollider>().enabled = false;
		float normalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		if (normalizedTime >= 1f)
			State = CreatureState.DeadEffect;
	}
	protected override void UpdateDeadEnd()
	{
		base.UpdateDeadEnd();

		if (sendDeadPacketOnce == true)
		{
			C_DeadEnd packet = new C_DeadEnd()
			{
				Info = new ObjectInfo()
				{
					GenPoint = genPointKey,
					ObjectId = Id,
					SpawnInfo = SpawnInfo.Monster,
				}
			};
			sendDeadPacketOnce = false;
			Managers.Network.Send(packet);
		}
		Managers.Objects.RemoveMonster(genPointKey, Id);
	}
	IEnumerator CoPatrol()
	{
		int waitSeconds = Random.Range(1, 4);
		yield return new WaitForSeconds(waitSeconds);

		int patrolPointIndex = Random.Range(0, patrolPoints.Count);
		agent.SetDestination(patrolPoints[patrolPointIndex].transform.position);
		TargetPos = patrolPoints[patrolPointIndex].transform.position;
		agent.updateRotation = false;
		transform.LookAt(patrolPoints[patrolPointIndex].transform.position);
		LookDir = agent.transform.forward.normalized;
		State = CreatureState.Moving;

		agent.updateRotation = true;


		agent.isStopped = false;
		if (coPatrol != null)
			coPatrol = null;
	}
	IEnumerator CoChaseTarget()
	{
		float waitSeconds = 0.25f;
		yield return new WaitForSeconds(waitSeconds);

		if(targetPlayer != null)
			TargetPos = targetPlayer.transform.position;

		if (coChaseTarget != null)
			coChaseTarget = null;
	}
	GameObject SearchPlayer()
	{
		float minFollowDistance = noticeDistance;

		foreach (var player in Managers.Objects.GetPlayers().Values)
		{
			float distanceWithPlayer = Vector3.Distance(player.transform.position, transform.position);

			if (distanceWithPlayer > noticeDistance)
				continue;

			if (minFollowDistance > distanceWithPlayer)
			{
				minFollowDistance = distanceWithPlayer;
				targetPlayer = player;
			}

		}

		if (targetPlayer != null)
		{
			if (minFollowDistance < attackDistance)
			{
				// 회전
				agent.updateRotation = false;
				transform.LookAt(targetPlayer.transform.position);
				agent.updateRotation = true;
				State = CreatureState.Attack;
				LookDir = agent.transform.forward.normalized;
			}
			else
			{
				// 회전
				agent.updateRotation = false;
				transform.LookAt(targetPlayer.transform.position);
				agent.updateRotation = true;
				State = CreatureState.Moving;
				LookDir = agent.transform.forward.normalized;
			}
		}


		return targetPlayer;
	}
}
