using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
	public void PlayerPunch()
	{
		var obj = GameObject.Find("PunchTrigger");
		if (obj == null)
			return;

		obj.GetComponent<BoxCollider>().enabled = true;
	}
	public void PlayerPunchEnd()
	{
		var obj = GameObject.Find("PunchTrigger");
		if (obj == null)
			return;


		obj.GetComponent<BoxCollider>().enabled = false;

	}
	public void Slash_1()
	{
		Transform[] allChildren = this.gameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren)
		{
			if (child.name == "SwordEffect_1")
			{
				GameObject go = Managers.Resource.Instantiate($"Effect/CFXR4 Sword Trail FIRE (360 Spiral)");
				go.transform.SetParent(child, true);
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
			}
		}
	}
	public void Slash_2()
	{
		Transform[] allChildren = this.gameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren)
		{
			if (child.name == "SwordEffect_2")
			{
				GameObject go = Managers.Resource.Instantiate($"Effect/CFXR4 Sword Trail FIRE (360 Spiral)");
				go.transform.SetParent(child, true);
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
			}
		}
	}
	public void Slash_3()
	{
		Transform[] allChildren = this.gameObject.transform.GetComponentsInChildren<Transform>();
		foreach (Transform child in allChildren)
		{
			if (child.name == "SwordEffect_3")
			{
				GameObject go = Managers.Resource.Instantiate($"Effect/CFXR4 Sword Trail FIRE (360 Spiral)");
				go.transform.SetParent(child, true);
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
			}
		}
	}

}
