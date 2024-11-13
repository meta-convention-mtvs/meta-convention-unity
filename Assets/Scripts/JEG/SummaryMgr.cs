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
<<<<<<< Updated upstream
    string url;

    public Text summaryText;
=======
    public string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/summary";
    public GameObject SummaryUIFactory;

>>>>>>> Stashed changes
    public class TakeSummary
    {
        public string summary { get; set; }
        public string full_script { get; set; }
    }

<<<<<<< Updated upstream
    public void OnClickToTakeSummary()
    {
        // 테스트용 url
        url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/summary";
        RequestSummary(url,ttttt );
    }

    public void ttttt(string t)
    {

    }
    public void RequestSummary(string url, Action<string> OnReceived)
    {
        StartCoroutine(IRequestSummary(url, OnReceived));
    }

    IEnumerator IRequestSummary(string url, Action<string> OnReceived)
    {

        string jsonData = "{\"user_id\":\"none\", \"org_id\":\"abcd\",\"lang\":\"ko\"}";
=======
    public void OnClickTakeUserID()
    {
        // 내가 방장이라면
        if (photonView.IsMine)
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
                    RequestSummary((string)player.CustomProperties["id"], (string)PhotonNetwork.LocalPlayer.CustomProperties["id"], OnDataLoaded);
                }
            }
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
    public string GetRequestJson(string userId, string companyId)
    {
        string meetingJson = "{\"user_id\":\"" + userId + "\", \"org_id\":\"" + companyId + "\", \"lang\":\"ko\"}";
        return meetingJson;
    }

    public void OnClickToTakeSummary()
    {

        RequestSummary("none", "abcd", OnDataLoaded);
        //RequestSummary("none", "abcd", OnDataLoaded );
    }

    //콜백함수: 데이터가 로딩되면 실행됨
    public void OnDataLoaded(string receivedSummary)
    {
        // string t가 받아온 데이터
        // 데이터를 띄우는 로직
        GameObject go = Instantiate(SummaryUIFactory);
        go.GetComponent<UIAISummary>()?.SetSummaryText(receivedSummary);

    }
    public void RequestSummary(string visitorUID, string companyUID, Action<string> OnReceived)
    {
        string jsonData = GetRequestJson(visitorUID, companyUID);
        StartCoroutine(IRequestSummary(jsonData, OnReceived));
    }

    IEnumerator IRequestSummary(string jsonRequestData, Action<string> OnReceived)
    {
>>>>>>> Stashed changes
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string receiveJsonData = www.downloadHandler.text;
                print("Received Json : " + receiveJsonData);

                TakeSummary takeSummary = JsonConvert.DeserializeObject<TakeSummary>(receiveJsonData);

                OnReceived?.Invoke(takeSummary.summary);

<<<<<<< Updated upstream
                Debug.LogError(takeSummary);
=======
                Debug.Log(takeSummary);
>>>>>>> Stashed changes

            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }

}
