using CHJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompanyRecommend : MonoBehaviour
{
    public GameObject companyItemsFactory;
    public RectTransform Content;
    public string LoadingSceneName = "Start_Universe";
    public Button nextSceneButton;

    public void GetRecommendDataFromDatabase()
    {
        DatabaseManager.Instance.GetData<RecommendedCompanyListData>(SetRecommendField);
    }

    public void SetRecommendField(RecommendedCompanyListData companyInfos)
    {
        foreach(TestRecommendedCompany companyInfo in companyInfos.recommendedCompanyList)
        {
            GameObject go =Instantiate(companyItemsFactory, Content);
            go.GetComponent<CompanyRecommendItem>().SetItemText(companyInfo);
            if (companyInfo.category == null)
            {
                go.GetComponent<CompanyRecommendItem>().DisableButton();
            }
            else
            {
                print(companyInfo.category);
                string categoryString = companyInfo.category.Replace("_", " ");
                print(categoryString);

                go.GetComponent<CompanyRecommendItem>().SetButtonTransition(() =>
                {
                    ButtonOnClick(categoryString, LoadingSceneName, companyInfo.uuid);
                    if (nextSceneButton != null)
                        nextSceneButton.onClick?.Invoke();
                });

            }
        }
    }
    public void ButtonOnClick(string companyCategory, string LoadingSceneName, string uuid)
    {
        BoothCategory category = EnumUtility.GetEnumValue<BoothCategory>(companyCategory).Value;
        MainHallData.Instance.SetMainHallLoadingData(category, LoadingSceneName);
        MainHallData.Instance.SetTargetCompanyUuid(uuid);
    }
}
