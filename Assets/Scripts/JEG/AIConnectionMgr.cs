using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class TestUserInfo
{
    //public string uIDP { get; set; }
    public string[] industry_type { get; set; }
    public string[] selected_interests { get; set; }
    public string situation_description { get; set; }
    public string language{ get; set; }

    public TestUserInfo(string[] ind_type, string[] selected_interests, string situ_desc, string lang)
    {
        this.industry_type = ind_type;
        this.selected_interests = selected_interests;
        this.situation_description = situ_desc;
        this.language = lang;
    }
   
}

[System.Serializable]
public class TestRecommendedCompany
{
    public string company_mission { get; set; }
    public string company_name { get; set; }
    public string items { get; set; }
    public string link { get; set; }
    public string logo_file_name { get; set; }
    public string reason { get; set; }

}

public class AIConnectionMgr : MonoBehaviour
{
    public string url;

    public Text companyName;

    public InputField industry_input;
    public InputField interest_input;
    public InputField situ_input;
    public InputField lang_input;

    public string[] industryTypes = new string[1];
    public string[] interests = new string[1];
    public string situDescription;
    public string lang;

    public void OnClickTestRequest()
    {
        // 테스트용 url
        url = "http://metaai2.iptime.org:65535/";
        RequestTestRequest(url);
    }

    public void RequestTestRequest(string url)
    {
        StartCoroutine(IRequestTestRecommendation(url));
    }

    IEnumerator IRequestTestRecommendation(string url)
    {
        using(UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.downloadHandler.text);
                companyName.text = www.downloadHandler.text;


            }
            else
            {
                Debug.LogError(www.error);
            }
        }
    }
    public void OnClickSendRecommendRequest()
    {
        // 테스트용 url 
        url = "http://metaai2.iptime.org:65535/recommendation";
        // 테스트용 데이터 셋팅
        // 나중에는 poll 로 받아온 데이터 셋팅
        //testUserInfo userInfo;

        RequestRecommendCompay( url);
    }

    public void RequestRecommendCompay(string url)
    {
        StartCoroutine(IRequestRecommend(url));
    }

    IEnumerator IRequestRecommend(string url)
    {
        // TODO: 받아온 유저의 관심분야 데이터를 jsonData로 변환해서 보내야 함..
        //string jsonData = JsonUtility.ToJson(userInfo);
        
        industryTypes[0] = industry_input.text;
        interests[0] = interest_input.text;
        situDescription = situ_input.text;
        lang = lang_input.text;

        TestUserInfo userInfo = new TestUserInfo(industryTypes, interests, situDescription, lang);

        string jsonData = JsonConvert.SerializeObject(userInfo, Formatting.None);
       

        //string jsonData = "{\"industry_type\":[\"항공\"], \"selected_interests\":[\"Vehicle Tech and Advanced Mobility\"],\"situation_description\":\"우주, 항공 산업에 관련되어 기체를 만드는 회사에 대해 궁금해\", \"language\":\"JP\" }";
        print(jsonData);
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {

            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                JObject jsonObject = JObject.Parse(www.downloadHandler.text);

                string receiveJsonData = jsonObject["result"].ToString();
                print("Received Json : " + receiveJsonData);



                List<TestRecommendedCompany> recommendedCompany = JsonConvert.DeserializeObject<List<TestRecommendedCompany>>(receiveJsonData);

                for (int i = 0; i < recommendedCompany.Count(); i++)
                {
                Debug.Log("Company Name: " + recommendedCompany[i].company_name);

                }
                companyName.text = recommendedCompany[0].company_name;

                // TODO: 받아온 데이터 UI 만들기..
                // 어떻게 보이게 할 건지, 플로우 확인 되면 작업 
            }else
            {
                Debug.LogError(www.error);
            }
        }
    }
}

