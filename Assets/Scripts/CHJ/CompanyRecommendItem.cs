using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompanyRecommendItem : MonoBehaviour
{
    public Text companyName;
    public Text companyItem;
    public Text companyLink;
    public Text companyMission;
    public Image companyLogo;

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

        if (!string.IsNullOrEmpty(companyInfo.logo_file_name))
        {
            Sprite loadedSprite = Resources.Load<Sprite>("Logos/"+companyInfo.logo_file_name);
            if (loadedSprite != null)
            {
                companyLogo.sprite = loadedSprite;
            }
        }
    }
}
