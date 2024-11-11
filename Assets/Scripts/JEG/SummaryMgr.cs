using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SummaryMgr : MonoBehaviour
{
    string url;

    public Text summaryText;
    public class TakeSummary
    {
        public string summary { get; set; }
        public string full_script { get; set; }
    }

    public void OnClickToTakeSummary()
    {
        // 테스트용 url
        url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/summary";
        RequestSummary(url);
    }

    public void RequestSummary(string url)
    {
        StartCoroutine(IRequestSummary(url));
    }

    IEnumerator IRequestSummary(string url)
    {

        string jsonData = "{\"user_id\":\"none\", \"org_id\":\"abcd\",\"lang\":\"ko\"}";
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

                summaryText.text = takeSummary.summary;

                Debug.LogError(takeSummary);

            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }

}
