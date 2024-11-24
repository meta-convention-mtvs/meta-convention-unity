using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class UuidMgrtest : MonoBehaviour
{
    
    string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/translation/uuid";
    public Text genUuid;

    public InputField companyName;
    public Text findedUuid;

    public string myCompanyName;


    

    [System.Serializable]
    public class UuidCompany
    {
        public string company_name;
        public string uuid;
    }

    [System.Serializable]
    public class UuidCompanyList
    {
        public List<UuidCompany> companies;
    }

    private UuidCompanyList companyData;

    
    void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "uuidb.json");
        string jsonData = File.ReadAllText(filePath);

        companyData = JsonUtility.FromJson<UuidCompanyList>($"{{\"companies\":{jsonData}}}");
    }

    public void OnClickFindClosestCompanyUUID()
    {
        UuidCompany closestCompany = companyData.companies.OrderBy(c => LevenshteinDistance(companyName.text, c.company_name)).First();

        Debug.Log($"Closest Company UUID: {closestCompany.uuid}");

        findedUuid.text = closestCompany.uuid;
    }

    private int LevenshteinDistance(string a, string b)
    {
        int[,] dp = new int[a.Length + 1, b.Length + 1];

        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                dp[i, j] = Mathf.Min(
                    Mathf.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                    dp[i - 1, j - 1] + cost);
            }
        }
        return dp[a.Length, b.Length];
    }

    public void OnClickGenerateUuid()
    {
        StartCoroutine(IGetUuid(url));
    }

    IEnumerator IGetUuid(string url)
    {
        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + www.error);
        }
        else
        {
            Debug.Log("response: " + www.downloadHandler.text);
            genUuid.text = www.downloadHandler.text;
        }
    }


    public void OnClickMyCompanySet()
    {

    // 현재 유저의 정보에서 "소속 기업" 이름을 가져오고
    // 기업 이름의 uuid 를 받아서 uuid에 저장 한다.
    // 자신 소속의 기업이 없다면 uuid 를 생성해서 저장 한다.

    // fireAuth CurrentUser 를 이용 할 것
    // 유저의 정보를 모두 가져와서 캐싱 해 두는게 좋을까...? 
    
    }



}



