using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class UuidMgr : MonoBehaviour
{
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

        string inputCompanyName = "Lockheed Martine Corporation";
        string closestUUID = FindClosestCompanyUUID(inputCompanyName);

        Debug.Log($"Closest Company UUID: {closestUUID}");
    }

    public string FindClosestCompanyUUID(string inputCompanyName)
    {
        UuidCompany closestCompany = companyData.companies.OrderBy(c => LevenshteinDistance(inputCompanyName, c.company_name)).First();
        return closestCompany.uuid;
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



    // Update is called once per frame
    void Update()
    {
        
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



