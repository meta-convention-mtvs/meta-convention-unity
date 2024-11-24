using Firebase.Firestore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
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

[FirestoreData]
public class TestRecommendedCompany
{
    [FirestoreProperty]
    public string company_mission { get; set; }
    [FirestoreProperty]
    public string company_name { get; set; }
    [FirestoreProperty]
    public string items { get; set; }
    [FirestoreProperty]
    public string link { get; set; }
    [FirestoreProperty]
    public string logo_file_name { get; set; }

}

public class AIConnectionMgr : MonoBehaviour
{
    public string url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/recommendation";

    public TMP_InputField situ_input;

    public UICompanyRecommend ui_cr;
    #region 현재 안씀
    //   public static string[] fields = new string[49] {"3D Printing",
    //"5G Technologies",
    //"AR/VR/XR",
    //"Accessibility",
    //"Accessories",
    //"AgTech",
    //"Artificial Intelligence",
    //"Audio",
    //"Cloud Computing",
    //"Construction Tech",
    //"Content and EntertaInment",
    //"Cybersecurity",
    //"Defense",
    //"Digital Health",
    //"Drones",
    //"Education Tech",
    //"Energy Transition",
    //"Energy/Power",
    //"Enterprise",
    //"Fashion Tech",
    //"Fintech",
    //"Fitness",
    //"Food Tech",
    //"Gaming and Esports",
    //"Home Entertainment and Office Hardware",
    //"Imaging",
    //"Innovation",
    //"Investing",
    //"IoT/Sensors",
    //"Lifestyle",
    //"Marketing and Advertising",
    //"Metaverse",
    //"NFT",
    //"Retail/E-Commerce",
    //"Robotics",
    //"Smart Cities",
    //"Smart Home and Appliances",
    //"Sourcing and Manufacturing",
    //"Space Tech",
    //"Sports",
    //"Startups",
    //"Streaming",
    //"Style",
    //"Supply and Logistics",
    //"Sustainability",
    //"Technology",
    //"Travel and Tourism",
    //"Vehicle Tech and Advanced Mobility",
    //"Video"};
    #endregion
    #region 서머리 테스트 함수(사용 안함)
    // 테스트용
    //public void OnClickTestRequest()
    //{
    //    // 테스트용 url
    //    url = "http://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/summary";
    //    RequestTestRequest(url);
    //}

    //public void RequestTestRequest(string url)
    //{
    //    StartCoroutine(IRequestTestRecommendation(url));
    //}

    //IEnumerator IRequestTestRecommendation(string url)
    //{

    //    string jsonData = "{\"user_id\":\"none\", \"org_id\":\"abcd\",\"lang\":\"ko\"}";
    //    using (UnityWebRequest www = UnityWebRequest.PostWwwForm(url,""))
    //    {
    //        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
    //        www.downloadHandler = new DownloadHandlerBuffer();
    //        www.SetRequestHeader("Content-Type", "application/json");
    //        yield return www.SendWebRequest();

    //        if (www.result == UnityWebRequest.Result.Success)
    //        {
    //            JObject jsonObject = JObject.Parse(www.downloadHandler.text);

    //            string receiveJsonData = jsonObject["result"].ToString();
    //            print("Received Json : " + receiveJsonData);

    //            Debug.LogError(www.downloadHandler.text);
    //            companyName.text = www.downloadHandler.text;


    //        }
    //        else
    //        {
    //            Debug.LogError(www.error);
    //        }
    //    }
    //}
    #endregion


    public void OnClickSendRecommendRequest()
    {
        RequestRecommendCompay( situ_input.text);
    }

    public void RequestRecommendCompay(string situationText)
    {
        StartCoroutine(IRequestRecommend(situationText));
    }

    IEnumerator IRequestRecommend(string situationText)
    {
        // TODO: 받아온 유저의 관심분야 데이터를 jsonData로 변환해서 보내야 함..
        // checkcheckbox에서 List 받아와서 interests 에 항목 넣어서 셋팅
        //string jsonData = JsonUtility.ToJson(userInfo);


        string jsonData = JsonConvert.SerializeObject(GetTestUserInfo(situationText, LanguageSingleton.Instance.language), Formatting.None);
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

                RecommendedCompanyListData data = new RecommendedCompanyListData();
                data.recommendedCompanyList = recommendedCompany;

                ui_cr.SetRecommendField(data);
                DatabaseManager.Instance.SaveData<RecommendedCompanyListData>(data);

            }else
            {
                Debug.LogError(www.error);
            }
        }
    }


    TestUserInfo GetTestUserInfo(string situDescription, string lang)
    {
        TestUserInfo userInfo = new TestUserInfo(new string[]{ "-" }, new string[] { "-" }, situDescription, lang);
        return userInfo;
    }

    
}

[FirestoreData]
public class RecommendedCompanyListData
{
    [FirestoreProperty]
    public List<TestRecommendedCompany> recommendedCompanyList { get; set; }
}

