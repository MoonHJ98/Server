using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class MyPlayerController : CreatureController
{
	CharacterController characterController;

	public int WeaponDamage { get; private set; }
	public int ArmourDefense { get; private set; }

	bool attack2 = false;
	bool attack3 = false;

	public override void Init()
	{
		base.Init();
		spawnInfo = SpawnInfo.Player;
		RefreshAdditionStat();
		characterController = GetComponent<CharacterController>();

		var camera = GameObject.Find("Main Camera");
		camera.GetComponent<CameraController>().target = this.gameObject;
	}

	public override void UpdateAnimation()
	{
		if (animator == null)
			return;
		base.UpdateAnimation();

		if (State == CreatureState.Idle)
		{
			animator.SetInteger("ani", (int)CreatureState.Idle);
			C_Move movePacket = new C_Move();
			movePacket.PosDirInfo = PosDirInfo;
			movePacket.PosDirInfo.LookDirX = 0f;
			movePacket.PosDirInfo.LookDirZ = 0f;

			Managers.Network.Send(movePacket);
		}
		else if (State == CreatureState.Moving)
		{
			animator.SetInteger("ani", (int)CreatureState.Moving);
			C_Move movePacket = new C_Move();
			movePacket.PosDirInfo = PosDirInfo;
			Managers.Network.Send(movePacket);
		}
		else if (State == CreatureState.Attack)
		{
			if (weapon == null)
				animator.SetInteger("ani", (int)CreatureState.Attack);

			C_Attack attackPacket = new C_Attack()
			{
				Info = new AttackInfo(),
			};

			if (weapon == null)
				attackPacket.Info.AttackId = (int)CreatureState.Attack;
			Managers.Network.Send(attackPacket);

		}
		else if (State == CreatureState.SwordAttack)
		{
			animator.SetInteger("ani", (int)CreatureState.SwordAttack);
			C_Attack attackPacket = new C_Attack()
			{
				Info = new AttackInfo(),
			};

			attackPacket.Info.AttackId = (int)CreatureState.SwordAttack;
			Managers.Network.Send(attackPacket);
		}
		else if (State == CreatureState.SwordAttack2)
		{
			animator.SetInteger("ani", (int)CreatureState.SwordAttack2);
			C_Attack attackPacket = new C_Attack()
			{
				Info = new AttackInfo(),
			};

			attackPacket.Info.AttackId = (int)CreatureState.SwordAttack2;
			Managers.Network.Send(attackPacket);
		}
		else if (State == CreatureState.SwordAttack3)
		{
			animator.SetInteger("ani", (int)CreatureState.SwordAttack3);
			C_Attack attackPacket = new C_Attack()
			{
				Info = new AttackInfo(),
			};

			attackPacket.Info.AttackId = (int)CreatureState.SwordAttack3;
			Managers.Network.Send(attackPacket);
		}
	}

	protected override void UpdateController()
	{
		base.UpdateController();

		GetUIKeyInput();


		if (characterController.isGrounded == false)
		{
			State = CreatureState.Moving;
		}

		switch (State)
		{
			case CreatureState.Idle:
				GetDirInput();
				GetAttackInput();
				StopCoroutine("CoUpdatePos");
				coUpdatePos = null;
				break;
			case CreatureState.Falling:
				break;
			case CreatureState.Moving:
				GetDirInput();
				GetAttackInput();
				break;
			case CreatureState.Attack:
				GetAttackInput();
				StopCoroutine("CoUpdatePos");
				coUpdatePos = null;
				break;
			case CreatureState.Dameged:
				break;
			case CreatureState.Dead:
				break;
			default:
				break;
		}
	}
	void GetUIKeyInput()
	{
		if (Input.GetKeyDown(KeyCode.I))
		{
			var gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			UI_Inventory invenUI = gameSceneUI.InvenUI;

			if (invenUI.gameObject.activeSelf)
			{
				invenUI.gameObject.SetActive(false);
			}
			else
			{
				invenUI.gameObject.SetActive(true);
				invenUI.RefreshUI();
			}
		}
		else if (Input.GetKeyDown(KeyCode.C))
		{
			var gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
			UI_Stat statUI = gameSceneUI.StatUI;

			if (statUI.gameObject.activeSelf)
			{
				statUI.gameObject.SetActive(false);
			}
			else
			{
				statUI.gameObject.SetActive(true);
				statUI.RefreshUI();
			}
		}

	}
	protected override void UpdateMovement()
	{
		switch (State)
		{
			case CreatureState.Idle:
				lookDir = Vector3.zero;

				break;
			case CreatureState.Falling:
				UpdateMove();
				break;
			case CreatureState.Moving:
				UpdateMove();
				break;
			case CreatureState.Attack:
				lookDir = Vector3.zero;
				Attack();
				break;
			case CreatureState.SwordAttack:
				lookDir = Vector3.zero;
				Attack1();
				break;
			case CreatureState.SwordAttack2:
				lookDir = Vector3.zero;
				Attack2();
				break;
			case CreatureState.SwordAttack3:
				lookDir = Vector3.zero;
				Attack3();
				break;
			case CreatureState.Dameged:
				break;
			case CreatureState.Dead:
				break;
			default:
				break;
		}
	}

	void GetDirInput()
	{
		if (State != CreatureState.Idle && State != CreatureState.Moving)
			return;

		if (characterController.isGrounded == false)
			return;



		dir.Set(0f, 0f, 0f);
		if (Input.GetKey(KeyCode.UpArrow))
		{
			dir.Set(0f, 0f, 1f);
			lookDir.Set(0f, 0f, 1f);
			State = CreatureState.Moving;

			if (Input.GetKey(KeyCode.RightArrow))
			{
				dir.Set(1f, 0f, 1f);
				lookDir.Set(1f, 0f, 1f);
				State = CreatureState.Moving;
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				dir.Set(-1f, 0f, 1f);
				lookDir.Set(-1f, 0f, 1f);
			}
			if (Input.GetKey(KeyCode.DownArrow))
			{
				dir.Set(0f, 0f, 0f);
				State = CreatureState.Idle;
			}
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			dir.Set(0f, 0f, -1f);
			lookDir.Set(0f, 0f, -1f);
			State = CreatureState.Moving;
			if (Input.GetKey(KeyCode.RightArrow))
			{
				dir.Set(1f, 0f, -1f);
				lookDir.Set(1f, 0f, -1f);
				State = CreatureState.Moving;
			}
			else if (Input.GetKey(KeyCode.LeftArrow))
			{
				dir.Set(-1f, 0f, -1f);
				lookDir.Set(-1f, 0f, -1f);
				State = CreatureState.Moving;

			}
			if (Input.GetKey(KeyCode.UpArrow))
			{
				dir.Set(0f, 0f, 0f);
				State = CreatureState.Idle;
			}
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			dir.Set(1f, 0f, 0f);
			lookDir.Set(1f, 0f, 0f);
			State = CreatureState.Moving;

			if (Input.GetKey(KeyCode.LeftArrow))
			{
				dir.Set(0f, 0f, 0f);
				State = CreatureState.Idle;
			}
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			dir.Set(-1f, 0f, 0f);
			lookDir.Set(-1f, 0f, 0f);
			State = CreatureState.Moving;
		}
		else
		{
			State = CreatureState.Idle;
			dir.Set(0f, 0f, 0f);
		}
	}
	protected virtual void GetAttackInput()
	{
		if (coAttackCooltime == null && Input.GetKey(KeyCode.Z))
		{
			//// TODO : 쿨타임 정해놓고 마구잡이로 못보내도록 설정하기
			//C_Attack attackPacket = new C_Attack()
			//{
			//	Info = new AttackInfo(),
			//};

			//if (weapon == null)
			//	attackPacket.Info.AttackId = (int)CreatureState.Attack;
			//else
			//	attackPacket.Info.AttackId = (int)CreatureState.SwordAttack;
			//Managers.Network.Send(attackPacket);
			StartCoroutine("CoInputCooltime", 0.3f);

			if (weapon == null)
				State = CreatureState.Attack;
			else
				State = CreatureState.SwordAttack;
		}
	}

	protected override void UpdateMove()
	{
		if (State != CreatureState.Moving)
			return;

		base.UpdateMove();

		dir.y += Physics.gravity.y * Time.deltaTime;

		characterController.Move(dir.normalized * 6f * Time.deltaTime);
		transform.LookAt(transform.position + lookDir);

		// 서버로 보내기 위한 PosDirInfo 데이터 갱신 
		Position = transform.position;
		LookDir = lookDir;

		if (coUpdatePos == null)
			StartCoroutine("CoUpdatePos");

	}
	protected virtual void Attack()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("punch_1"))
		{
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
			{
				State = CreatureState.Idle;

				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
					State = CreatureState.Moving;
			}
		}
	}
	public void Attack1()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("slash_1"))
		{
			if (coAttackCooltime == null && Input.GetKey(KeyCode.Z))
			{
				StartCoroutine("CoInputCooltime", 0.3f);
				attack2 = true;
				attack3 = false;
			}

			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
			{
				if (attack2 == true)
				{
					State = CreatureState.SwordAttack2;
					attack2 = false;
					attack3 = false;
					return;
				}
				State = CreatureState.Idle;

				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
					State = CreatureState.Moving;
			}
		}
	}
	public void Attack2()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("slash_2"))
		{
			if (coAttackCooltime == null && Input.GetKey(KeyCode.Z))
			{
				StartCoroutine("CoInputCooltime", 0.3f);
				attack2 = false;
				attack3 = true;
			}

			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
			{
				if (attack3 == true)
				{
					State = CreatureState.SwordAttack3;
					attack2 = false;
					attack3 = false;
					return;
				}
				State = CreatureState.Idle;

				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
					State = CreatureState.Moving;
			}
		}
	}
	public void Attack3()
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("slash_3"))
		{
			if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
			{
				State = CreatureState.Idle;

				if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
					State = CreatureState.Moving;
			}
		}
	}


	public void RefreshAdditionStat()
	{
		WeaponDamage = 0;
		ArmourDefense = 0;

		foreach (var item in Managers.Inventory.Items.Values)
		{
			if (item.Equipped == false)
				continue;

			switch (item.ItemType)
			{
				case ItemType.Weapon:
					WeaponDamage += ((Weapon)item).Damage;
					break;
				case ItemType.Armour:
					ArmourDefense += ((Armour)item).Defence;
					break;

			}
		}
	}

}
