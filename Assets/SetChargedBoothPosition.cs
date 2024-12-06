using CHJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UuidMgr;

public class SetChargedBoothPosition : MonoBehaviour
{
    public ChargedBoothPosition newData, mobilityPosition, electronicsPosition;
    public bool[] isChargedList;

    public string mobilityCompanyList = "Lockheed Martin Corporation\nSwift Navigation\nShenzhen Smiley Technology Co., Ltd\nSheeva.AI\nRolling Wireless\nRidecell\nRAM Mounts\nMotrex\nBrunswick Corporation\nBotronics\nBHTC\nWeRide\nVector\nValeo\nUrtopia\nTweddle Group";

    public string electronicsCompanyList = "Apple\nSynaptics\nSilicon Labs\nShenzhen Zhonghenghua Technology Co., Ltd.\nShenzhen Shenchuang Hi-Tech Electronics Co., Ltd.\nShenzhen Jiteng Network Technology Co., Ltd\nShenzhen Grandtop Electronics Co., Ltd.\nShenzhen Appphone Electronics Co., Ltd.\nMoon Tech Electronics\nMIPS\nMars International\nLG Electronics\nams OSRAM\nAllegro MicroSystems\nTexas Instruments\nTaoglas";
    private UuidCompanyList companyData;


    private void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "uuidb.json");
        string jsonData = File.ReadAllText(filePath);

        companyData = JsonUtility.FromJson<UuidCompanyList>($"{{\"companies\":{jsonData}}}");

        string[] mobilityCompany = mobilityCompanyList.Split('\n');
        string[] electronicsCompany = electronicsCompanyList.Split("\n");

        mobilityPosition = new ChargedBoothPosition();
        mobilityPosition.BoothPositionList = new List<ChargedBoothData>();
        for (int i = 0; i < isChargedList.Length; i++)
        {
            if (i < mobilityCompany.Length)
            {
                UuidCompany findCompany = companyData.companies.Find(company => company.company_name == mobilityCompany[i]);
                if (findCompany != null)
                    mobilityPosition.BoothPositionList.Add(new ChargedBoothData(true, findCompany.uuid));
                else
                    mobilityPosition.BoothPositionList.Add(new ChargedBoothData(false, ""));

            }
            else
            {
                mobilityPosition.BoothPositionList.Add(new ChargedBoothData(false, ""));
            }
        }

        electronicsPosition = new ChargedBoothPosition();
        electronicsPosition.BoothPositionList = new List<ChargedBoothData>();
        for (int i = 0; i < isChargedList.Length; i++)
        {
            if (i < electronicsCompany.Length)
            {
                UuidCompany findCompany = companyData.companies.Find(company => company.company_name == electronicsCompany[i]);
                if (findCompany != null)
                    electronicsPosition.BoothPositionList.Add(new ChargedBoothData(true, findCompany.uuid));
                else
                    electronicsPosition.BoothPositionList.Add(new ChargedBoothData(false, ""));

            }
            else
            {
                electronicsPosition.BoothPositionList.Add(new ChargedBoothData(false, ""));
            }
        }
        newData = new ChargedBoothPosition();

        newData.BoothPositionList = new List<ChargedBoothData>();
        for(int i = 0; i < isChargedList.Length; i++)
        {
            newData.BoothPositionList.Add(new ChargedBoothData(isChargedList[i], ""));
        }

        FireAuthManager.Instance.OnLogin += ResetChargedBoothPosition;
    }

    void ResetChargedBoothPosition()
    {
        DatabaseManager.Instance.SavePublicData<ChargedBoothPosition>(newData);

        foreach(BoothCategory value in Enum.GetValues(typeof(BoothCategory)))
        {
            if (value == BoothCategory.Mobility)
            {
                AsyncDatabase.SetDataToDatabase(DatabasePath.GetPublicBoothPositionDataPath(value), mobilityPosition);
            }
            else if (value == BoothCategory.Electronics)
            {
                AsyncDatabase.SetDataToDatabase(DatabasePath.GetPublicBoothPositionDataPath(value), electronicsPosition);
            }
            else
            {
                AsyncDatabase.SetDataToDatabase(DatabasePath.GetPublicBoothPositionDataPath(value), newData);
            }
        }
    }
}
