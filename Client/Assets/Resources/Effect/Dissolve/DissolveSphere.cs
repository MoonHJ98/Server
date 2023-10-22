using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveSphere : MonoBehaviour
{

	Material mat;
	float amount = 0;

	public bool dissolveStart = false;

	private void Start()
	{
		mat = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		if (transform.parent.gameObject.GetComponent<CreatureController>().State == CreatureState.DeadEffect)
			dissolveStart = true;

		if (dissolveStart)
		{

			amount += Time.deltaTime * 0.25f;

			mat.SetFloat("_DissolveAmount", amount);

			if (amount > 0.9f)
			{
				dissolveStart = false;
				transform.parent.gameObject.GetComponent<CreatureController>().State = CreatureState.DeadEnd;
			}
		}

	}
}