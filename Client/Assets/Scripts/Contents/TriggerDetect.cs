using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetect : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{ 
		if (this.gameObject == null || other.gameObject == null)
			return; 

		C_Damaged packet = new C_Damaged();

		var attackController = this.gameObject.transform.parent.gameObject.GetComponent<CreatureController>();
		if (attackController == null)
			return;

		var deffenderController = other.gameObject.GetComponent<CreatureController>();
		if (deffenderController == null)
			return;

		//other.gameObject.GetComponent<CreatureController>().State = CreatureState.Dead;

		packet.AttackerId = attackController.Id;
		packet.AttackType = (int)attackController.State;
		if (string.IsNullOrEmpty(attackController.genPointKey) == false)
			packet.DefenderGenId = attackController.genPointKey;

		packet.DefenderId = deffenderController.Id;
		if(string.IsNullOrEmpty(deffenderController.genPointKey) == false)
			packet.DefenderGenId = deffenderController.genPointKey;

		Managers.Network.Send(packet);

	}
}
