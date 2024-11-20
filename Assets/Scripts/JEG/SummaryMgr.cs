using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SummaryMgr : MonoBehaviourPun
{

    public string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/summary";
    public GameObject SummaryUIFactory;

    public class TakeSummary
    {
        public string summary { get; set; }
        public string full_script { get; set; }
    }
    private void Start()
    {
        OnClickTakeUserID();
    }
    public void OnClickTakeUserID()
    {
        // 방이름을 기업이름으로 세팅함
        if (PhotonNetwork.CurrentRoom.Name == FireAuthManager.Instance.GetCurrentUser().UserId)
        {
            Dictionary<int, Player> OtherPlayers = GetOtherPlayer(PhotonNetwork.CurrentRoom.Players);
            if (OtherPlayers.Count > 1) 
            {
                Debug.Log("Too many players are in the Room");
            }

            foreach (Player player in OtherPlayers.Values)
            {
                if (player != null)
                {
                    RequestSummary((string)player.CustomProperties["id"], (string)PhotonNetwork.LocalPlayer.CustomProperties["id"], CashedDataFromDatabase.Instance.playerLanguage.language, OnDataLoaded);
                }
            }
            RequestSummary(FireAuthManager.Instance.GetCurrentUser().UserId, "cf79ea17-a487-4b27-a20d-bbd11ff885da", CashedDataFromDatabase.Instance.playerLanguage.language, OnDataLoaded);
        }
            //GetRequestJson(PhotonNetwork.CurrentRoom., PhotonNetwork.CurrentRoom.Players[PhotonNetwork.CurrentRoom.MasterClientId].CustomProperties["id"]);
    }

    public Dictionary<int, Player> GetOtherPlayer(Dictionary<int, Player> Players)
    {
        Dictionary<int, Player> result = new Dictionary<int, Player>();
        foreach (int key in Players.Keys)
        {
            if (Players[key].IsLocal)
            {
                continue;
            }
            result.Add(key, Players[key]);
        }
        return result;
    }
    // MeetingInfo 함수에 user UID, company UID 넣으면 됨
    public string GetRequestJson(string userId, string companyId, string lang)
    {
        string meetingJson = "{\"user_id\":\"" + userId + "\", \"org_id\":\"" + companyId + "\", \"lang\":\"" + lang + "\"}";
        print(meetingJson);
        return meetingJson;
    }

    public void OnClickToTakeSummary()
    {

        RequestSummary("none", "abcd","ko", OnDataLoaded);
        //RequestSummary("none", "abcd", OnDataLoaded );
    }

    //콜백함수: 데이터가 로딩되면 실행됨
    public void OnDataLoaded(TakeSummary summary)
    {
        // string t가 받아온 데이터
        // 데이터를 띄우는 로직
        GameObject go = Instantiate(SummaryUIFactory);
        go.GetComponent<UIAISummary>()?.SetSummaryText(summary.summary);
        go.GetComponent<UIAISummary>()?.SetAllText(summary.full_script);

    }
    public void RequestSummary(string visitorUID, string companyUID, string language, Action<TakeSummary> OnReceived)
    {
        string jsonData = GetRequestJson(visitorUID, companyUID, language);
        StartCoroutine(IRequestSummary(jsonData, OnReceived));
    }

    IEnumerator IRequestSummary(string jsonRequestData, Action<TakeSummary> OnReceived)
    {

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

                OnReceived?.Invoke(takeSummary);

                Debug.Log(takeSummary);

            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }

}
