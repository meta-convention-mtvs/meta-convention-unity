using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompanyRecommend : MonoBehaviour
{
    public GameObject companyItemsFactory;
    public RectTransform Content;

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

        }
    }
}
