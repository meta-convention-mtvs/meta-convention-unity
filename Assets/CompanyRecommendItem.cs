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
        companyName.text = companyInfo.company_name;
        companyMission.text = companyInfo.company_mission;
        companyItem.text = companyInfo.items;
        companyLink.text = companyInfo.link;
        companyReason.text = companyInfo.reason;
    }
}
