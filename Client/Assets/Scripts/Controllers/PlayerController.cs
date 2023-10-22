using DG.Tweening;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Define;

public class PlayerController : CreatureController
{

	// Start is called before the first frame update
	public override void Init()
	{
		base.Init();
		spawnInfo = SpawnInfo.Player;


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
			if (weapon == null)
				animator.SetInteger("ani", (int)CreatureState.Attack);
		}
		else if (State == CreatureState.SwordAttack)
		{
			animator.SetInteger("ani", (int)CreatureState.SwordAttack);
		}
		else if (State == CreatureState.SwordAttack2)
		{
			animator.SetInteger("ani", (int)CreatureState.SwordAttack2);
		}
		else if (State == CreatureState.SwordAttack3)
		{
			animator.SetInteger("ani", (int)CreatureState.SwordAttack3);
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
				var length = Vector3.Distance(transform.position, Position);
				if (length >= 0.25f)
					transform.position = Position;
				break;
			case CreatureState.Falling:
				UpdateMove();
				break;
			case CreatureState.Moving:
				UpdateMove();
				break;
			case CreatureState.Attack:
				break;
			case CreatureState.Dameged:
				break;
			case CreatureState.Dead:
				break;
			case CreatureState.DeadEffect:
				break;
			case CreatureState.DeadEnd:
				break;
			default:
				break;
		}
	}

	protected override void UpdateMove()
	{
		if (State != CreatureState.Moving)
			return;

		base.UpdateMove();

		dir.y += Physics.gravity.y * Time.deltaTime;

		//transform.position = new Vector3(Position.x, Position.y, Position.z);
		transform.position = transform.position + LookDir.normalized * 6f * Time.deltaTime;

		var length = Vector3.Distance(transform.position, Position);

		//if (length >= 2f)
		//	transform.position = Position;

		transform.LookAt(transform.position + LookDir);

		//Debug.Log(transform.position);

	}

	void LateUpdate()
	{

	}
}
