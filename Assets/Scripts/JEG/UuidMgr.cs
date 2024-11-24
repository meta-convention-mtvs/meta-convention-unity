using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerInfo
{
    public string uid { get; set; }
    public string userName { get; set; }
    public string companyName { get; set; }
    public string companyUuid { get; set; }
    public string language { get; set; }

    public PlayerInfo(string uid, string userName, string companyName, string companyUuid)
    {
        this.uid = uid;
        this.userName = userName;
        this.companyName = companyName;
        this.companyUuid = companyUuid;
        this.language = "ko";
    }
}
public class UuidMgr : MonoBehaviour
{
    public static UuidMgr Instance;

    string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/translation/uuid";
    
    public PlayerInfo currentUserInfo = new PlayerInfo("방","구","뿡","뿡");
    public Card cashedCard;

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // company name , uuid 연결하기 위해 json 파일 가져오기
        string filePath = Path.Combine(Application.streamingAssetsPath, "uuidb.json");
        string jsonData = File.ReadAllText(filePath);

        companyData = JsonUtility.FromJson<UuidCompanyList>($"{{\"companies\":{jsonData}}}");
    }

    public void FindClosestCompanyUUID()
    {
        UuidCompany closestCompany = companyData.companies.OrderBy(c => LevenshteinDistance(currentUserInfo.companyName, c.company_name)).First();

        Debug.Log($"Closest Company UUID: {closestCompany.uuid}");

        currentUserInfo.companyUuid = closestCompany.uuid;
        cashedCard.uuid = currentUserInfo.companyUuid;
        DatabaseManager.Instance.SaveData<Card>(cashedCard);
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

    public void GenerateUuid()
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
            currentUserInfo.companyUuid = www.downloadHandler.text;
            cashedCard.uuid = currentUserInfo.companyUuid;
            DatabaseManager.Instance.SaveData<Card>(cashedCard);
        }
    }

    public void PrintUserInfo()
    {
        Debug.Log($"User ID: {currentUserInfo.uid}");
        Debug.Log($"User Name: {currentUserInfo.userName}");
        Debug.Log($"Company Name: {currentUserInfo.companyName}");
        Debug.Log($"Company UUID: {currentUserInfo.companyUuid}");
    }
}



