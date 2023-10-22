using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowOff : MonoBehaviour
{
    void Update()
    {
        if(transform.parent.gameObject.GetComponent<CreatureController>().State == CreatureState.DeadEffect)
        {
			Renderer renderer = GetComponent<Renderer>();

			if (renderer != null)
            {
				// 오브젝트에 캐스트되는 그림자 끄기
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				// 오브젝트에 떨어지는 그림자 끄기
				renderer.receiveShadows = false;

			}
		}
    }
}
