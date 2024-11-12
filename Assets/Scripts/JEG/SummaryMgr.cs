using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SummaryMgr : MonoBehaviour
{
    public string url;

    public InputField visitorId;
    public InputField companyId;
    
    public Text summaryText;
    public class TakeSummary
    {
        public string summary { get; set; }
        public string full_script { get; set; }
    }

    public void OnClickTakeUserID()
    {
        GetRequestJson(visitorId.text, companyId.text);
    }

    // MeetingInfo 함수에 user UID, company UID 넣으면 됨
    public string GetRequestJson(string userId, string companyId)
    {
        string meetingJson = "{\"user_id\":\"" + userId + "\", \"org_id\":\"" + companyId + "\", \"lang\":\"ko\"}";
        return meetingJson;
    }

    public void OnClickToTakeSummary()
    {

        RequestSummary("siYXI5m13JXOwximFb6Av5TEzGs2", "cf79ea17-a487-4b27-a20d-bbd11ff885da", OnDataLoaded);
        //RequestSummary("none", "abcd", OnDataLoaded );
    }

    //콜백함수: 데이터가 로딩되면 실행됨
    public void OnDataLoaded(string t)
    {
        // string t가 받아온 데이터
        // 데이터를 띄우는 로직
    }
    public void RequestSummary(string visitorUID, string companyUID, Action<string> OnReceived)
    {
        string jsonData = GetRequestJson(visitorUID, companyUID);
        StartCoroutine(IRequestSummary(jsonData, OnReceived));
    }

    IEnumerator IRequestSummary(string jsonRequestData, Action<string> OnReceived)
    {
        // 테스트용 url
        string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/summary";

        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonRequestData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string receiveJsonData = www.downloadHandler.text;
                print("Received Json : " + receiveJsonData);

                TakeSummary takeSummary = JsonConvert.DeserializeObject<TakeSummary>(receiveJsonData);

                OnReceived?.Invoke(takeSummary.summary);

                Debug.LogError(takeSummary);
                summaryText.text = takeSummary.summary;

            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }

}
