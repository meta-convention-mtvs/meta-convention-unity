using DG.Tweening.Plugins;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/*
[{'company_mission': "We deliver innovative solutions to the world's toughest "
                     'challenges.',
  'company_name': 'Lockheed Martin Corporation',
  'items': 'F-35 Lightning II: A fifth-generation multirole stealth fighter '
           'that combines advanced avionics with unmatched sensor '
           'capabilities.\n'
           'Orion Spacecraft: Designed for deep space missions, Orion will '
           'carry astronauts beyond low Earth orbit to destinations such as '
           'Mars.\n'
           'Aegis Combat System: An advanced naval weapon system that uses '
           'powerful radar and missile technology for defense against various '
           'threats.\n'
           'PAC-3 Missile Segment Enhancement: A missile defense system '
           'designed to intercept and destroy tactical ballistic missiles, '
           'cruise missiles, and aircraft.\n'
           'Hypervelocity Projectile: A next-generation projectile designed '
           'for high-speed, long-range precision strikes.\n'
           'Autonomous Black Hawk Helicopter: A UAV system that allows the '
           'Black Hawk helicopter to be operated autonomously over long '
           'distances.\n'
           'Cybersecurity Solutions: Innovative solutions to protect military, '
           'government, and commercial networks from cyber threats.\n'
           '21st Century Security®: A comprehensive approach to advancing '
           'defense strategies for NATO and allied nations.',
  'link': 'https://www.lockheedmartin.com/',
  'logo_file_name': 'Lockheed_Martin_Corporation',
  'reason': 'Lockheed Martin은 항공 및 우주 산업에서 혁신적인 기체를 만드는 선도적인 기업으로, 귀하의 관심사인 '
            "'Vehicle Tech and Advanced Mobility'와 밀접한 관련이 있습니다. 다양한 첨단 항공기 및 "
            '우주선 개발을 통해 우주 탐사와 방위 분야에서 중요한 역할을 수행하고 있어, 귀하의 상황 설명에 적합한 정보를 '
            '제공합니다.',
}
......
]

userinfo = UserInfo(
    industry_type=['항공'],
    selected_interests=['Vehicle Tech and Advanced Mobility'],
    situation_description='우주, 항공 산업에 관련되어 기체를 만드는 회사에 대해 궁금해',
    language='KO'
)

*/

[System.Serializable]
public class testUserInfo
{
    //public string uIDP { get; set; }
    public string[] industry_type { get; set; }
    public string[] selected_interests { get; set; }
    public string situation_description { get; set; }
    public string language{ get; set; }

   
}

[System.Serializable]
public class testRecommendedCompany
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

    public void OnClickTestRequest()
    {
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
        //string jsonData = JsonUtility.ToJson(userInfo);
        string jsonData = "{\"industry_type\":[\"항공\"], \"selected_interests\":[\"Vehicle Tech and Advanced Mobility\"],\"situation_description\":\"우주, 항공 산업에 관련되어 기체를 만드는 회사에 대해 궁금해\", \"language\":\"KO\" }";
        print(jsonData);
        using (UnityWebRequest www = UnityWebRequest.Post(url, ""))
        {

            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");
            
            yield return www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                // 가져온 데이터를 class로 바꾸서 사용 하기
                print("Received Json : " + responseText);

                testRecommendedCompany recommendedCompany = JsonUtility.FromJson<testRecommendedCompany>(responseText);

                Debug.Log("Company Name: " + recommendedCompany.company_name);
                companyName.text = recommendedCompany.company_name;

                // TODO: 받아온 데이터 UI 만들기..
                // 어떻게 보이게 할 건지, 플로우 확인 되면 작업 
            }else
            {
                Debug.LogError(www.error);
            }
        }
    }
}

