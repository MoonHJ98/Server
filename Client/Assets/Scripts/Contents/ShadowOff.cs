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
				// ������Ʈ�� ĳ��Ʈ�Ǵ� �׸��� ����
				renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

				// ������Ʈ�� �������� �׸��� ����
				renderer.receiveShadows = false;

			}
		}
    }
}
