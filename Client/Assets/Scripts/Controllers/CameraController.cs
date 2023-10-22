using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public GameObject target;
	public Vector3 offset;

	// Start is called before the first frame update
	void Start()
	{

		offset.Set(0f, 10f, -10f);

	}

	// Update is called once per frame
	void Update()
	{
		if (target == null)
		{
			//Debug.Log("Camera target is null");
			return;
		}



		transform.position = target.transform.position + offset;
	}
}
