using System.Collections;
using System.Collections.Generic;
using System.Net.Cache;
using UnityEngine;
using UnityEngine.Networking;

public class WebManager : MonoBehaviour
{
    private string apiUrl = "http://localhost:3001/api/unity-data";

    // 유니티의 버튼을 누르면, 웹에 신호가 간다...
    // 아.. 이거 개인 서버 용 연결임! 
    public void SendDataToServer(string jsonData)
    {
        StartCoroutine(PostData(jsonData));
    }

    IEnumerator PostData(string jsonData)
    {
        UnityWebRequest www = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + www.error);
        }else
        {
            Debug.Log("response: " + www.downloadHandler.text);
        }
    }
}
