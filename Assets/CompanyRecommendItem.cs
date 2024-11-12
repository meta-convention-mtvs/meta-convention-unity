using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyRecommendItem : MonoBehaviour
{
    public Text companyName;
    public Text companyMission;
    public Text companyItem;
    public Text companyLink;
    public Text companyReason;

    public void SetItemText(TestRecommendedCompany companyInfo)
    {
        if(companyName != null)
            companyName.text = companyInfo.company_name;
        if(companyMission != null)
            companyMission.text = companyInfo.company_mission;
        if(companyItem != null)
            companyItem.text = companyInfo.items;
        if(companyLink != null)
            companyLink.text = companyInfo.link;
        if(companyReason != null)
            companyReason.text = companyInfo.reason;
    }
}
