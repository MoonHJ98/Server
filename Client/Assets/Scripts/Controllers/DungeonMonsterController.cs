using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class DungeonMonsterController : CreatureController
{
	Coroutine coChaseTarget;

	private NavMeshAgent agent;

	public GameObject targetPlayer = null;

	float noticeDistance = 4f;
	float followDistanceLimit = 7.5f;
	public float attackDistance = 2f;
	

	Vector3 Destination { get; set; }

	public override void Init()
	{
		base.Init();
		agent = GetComponent<NavMeshAgent>();
		agent.obstacleAvoidanceType = 0;
		spawnInfo = SpawnInfo.Monster;
		statInfo.Hp = 100;
		statInfo.MaxHp = 100;
		statInfo.Damage = 10;
		UpdateHpBar();

		Managers.Objects.dungeonMonsters.Add(this.gameObject);
	}
	protected override void UpdateController()
	{
		base.UpdateController();

	}
	protected override void UpdateMovement()
	{
		base.UpdateMovement();

		switch (State)
		{
			case CreatureState.Idle:
				LookDir = Vector3.zero;
				UpdateIdle();
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

		}
		else if (State == CreatureState.Moving)
		{
			animator.SetInteger("ani", (int)CreatureState.Moving);


		}
		else if (State == CreatureState.Attack)
		{
			animator.SetInteger("ani", (int)CreatureState.Attack);
			LookDir = Vector3.zero;
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

		SearchPlayer();


	}
	protected override void UpdateMove()
	{
		if (State != CreatureState.Moving)
			return;

		base.UpdateMove();

		// 지금 바라보는 방향, 위치를 PosDir에 업데이트
		LookDir = agent.transform.forward.normalized;
		Position = transform.position;



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


		if (targetPlayer != null)
		{
			float distance = Vector3.Distance(targetPlayer.transform.position, transform.position);

			if (distance <= attackDistance)
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

		Managers.Objects.dungeonMonsters.Remove(this.gameObject);
		Destroy(this.gameObject);
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
