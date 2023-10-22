using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager
{
	public string BaseUrl { get; set; } = "https://localhost:5001/api";

	public void SendPostRequest<T>(string url, object obj, Action<T> res)
	{
		Managers.Instance.StartCoroutine(CoSendWebRequest(url, UnityWebRequest.kHttpVerbPOST, obj, res));
	}
	IEnumerator CoSendWebRequest<T>(string url, string method, object obj, Action<T> res)
	{
		// obj : 해당하는 json오브젝트를 넣어주는 곳

		string sendUrl = $"{BaseUrl}/{url}";
		byte[] jsonBytes = null;
		if (obj != null)
		{
			string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
			jsonBytes = Encoding.UTF8.GetBytes(jsonStr);
		}
		using (var uwr = new UnityWebRequest(sendUrl, method))
		{
			// 요청 보내기
			uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);

			// 응답 받기
			uwr.downloadHandler = new DownloadHandlerBuffer();

			// 어떤 형태로 보낼건지
			uwr.SetRequestHeader("Content-Type", "application/json");

			yield return uwr.SendWebRequest();

			if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError)
				Debug.Log(uwr.error);
			else
			{
				// Json 텍스트를 객체로 만들기
				T resObj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text);
				res.Invoke(resObj);
			}
		}
	}
}
