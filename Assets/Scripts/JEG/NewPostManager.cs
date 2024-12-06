using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System;
using UnityEngine;
using UnityEngine.Networking;

public class NewPostManager : Singleton<NewPostManager>
{
    public string baseUrl = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/recommendation/";

    public void PostNewCompanyData(string uuid, object data)
    {
        string fullUrl = $"{baseUrl}{uuid}"; // URL에 UUID를 추가
        StartCoroutine(IPostNewCompanyData(fullUrl, data));
    }

    IEnumerator IPostNewCompanyData(string fullUrl, object data)
    {
        // JSON 데이터 직렬화
        string jsonData = JsonConvert.SerializeObject(data, Formatting.None);

        using (UnityWebRequest www = new UnityWebRequest(fullUrl, "POST"))
        {
            // JSON 데이터 업로드
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();

            // 헤더 설정
            www.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송
            yield return www.SendWebRequest();

            // 요청 결과 처리
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Connect is success!");
                Debug.Log($"Response: {www.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"Error: {www.error}");
            }
        }
    }
}

[Serializable]
public class NewCompanyDataForAIBoothRecommendation
{
    public string company_uuid;
    public string company_name;
    public string description;
    public string category;

    public NewCompanyDataForAIBoothRecommendation(string company_uuid, string company_name, string description, string category)
    {
        this.company_uuid = company_uuid;
        this.company_name = company_name;
        this.description = description;
        this.category = category;
    }
}