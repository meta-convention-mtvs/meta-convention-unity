using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class UuidMgr : MonoBehaviour
{
    string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/translation/uuid";
    public Text genUuid;

    public InputField companyName;
    public Text findedUuid;


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
            for (int j = 1; j <= b.Length; j ++)
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
    // url 주소로 get 요청으로 갔을 때 받는 문자열 을 반환 한다. 
    // 그게 나의 uuid... 
    IEnumerator IGetUuid(string url)
    {
        UnityWebRequest www = new UnityWebRequest(url, "GET");
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("Error: " + www.error);
        }else
        {
            Debug.Log("response: " + www.downloadHandler.text);
            genUuid.text = www.downloadHandler.text;
        }
    }

}

// 유저 데이터 저장
// 유저 데이터에서 기업 이름으로 uuid 할당하기
// .. 유저는 uuid 가지고만 있으면 될.. 듯?
// TODO: user class, 데이터에  uuid 항목 만들기
// TODO: uuid 할당하는 함수 만들기 (기업명 -> uuid)



// 부스 셋팅 있다 없다 true false 
// 기업 uuid 기반으로 기업 데이터 저장
// 기업의 부스, 기업 정보 데이터가 있다면 사용하고..
// 없으면 저장 시키기
// 경로 변경만 잘하면.. 그대로 이용 할 수 있..는...ㄱ... 아님./. ? 
// 일단 항목 잘 살리면서 유지 할 것


// 트레이닝 정보도.. uuid 기반으로 저장, 접근 해야 함
// 우리가 건드릴게 있나? 
// 그냥 받은거 있으면 uuid 기반으로 경로 잘 정해서 저장 해주기





