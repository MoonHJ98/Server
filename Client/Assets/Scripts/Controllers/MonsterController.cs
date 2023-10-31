using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class MonsterController : CreatureController
{
	// Start is called before the first frame update
	NavMeshAgent agent;

	public override void Init()
	{
		base.Init();
		agent = GetComponent<NavMeshAgent>();
		agent.obstacleAvoidanceType = 0;
		spawnInfo = SpawnInfo.Monster;

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
		}
		else if (State == CreatureState.Dead)
		{
			animator.SetInteger("ani", (int)CreatureState.Dead);
		}
		else
		{

		}
	}
	protected override void UpdateMovement()
	{
		base.UpdateMovement();
		switch (State)
		{
			
			case CreatureState.Idle:
				agent.velocity = Vector3.zero;
				agent.isStopped = true;
				break;
			case CreatureState.Falling:
				break;
			case CreatureState.Moving:
				UpdateMove();
				break;
			case CreatureState.Attack:
				UpdateAttack();
				break;
			case CreatureState.Dameged:
				break;
			case CreatureState.Dead:
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

	protected override void UpdateMove()
	{
		if (State != CreatureState.Moving)
			return;

		if (animator.GetCurrentAnimatorStateInfo(0).IsName("power_attack"))
			return;

        if (targetPos == null)
        {
			return;
        }
        base.UpdateMove();

		dir.y += Physics.gravity.y * Time.deltaTime;

		Vector3 target = new Vector3(targetPos.PosX, targetPos.PosY, targetPos.PosZ);
		Vector3 targetDir = target - transform.position;
		//
		//if(targetDir.magnitude >= 1.5f)
		//	transform.position += targetDir.normalized * 3.5f * Time.deltaTime;

		agent.isStopped = false;

		agent.SetDestination(target);

		transform.LookAt(target);


	}
	protected override void UpdateAttack()
	{
		base.UpdateAttack();
		Vector3 target = new Vector3(targetPos.PosX, targetPos.PosY, targetPos.PosZ);
		transform.LookAt(target);
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
		//Managers.Objects.RemoveMonster(genPointKey, Id);
	}

	void LateUpdate()
	{

	}
}

