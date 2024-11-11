using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICompanyRecommend : MonoBehaviour
{
    public GameObject companyItemsFactory;
    public RectTransform Content;
    void SetRecommendField(TestRecommendedCompany companyInfo)
    {
        GameObject go =Instantiate(companyItemsFactory, Content);
        go.GetComponent<CompanyRecommendItem>().SetItemText(companyInfo);
    }
}
