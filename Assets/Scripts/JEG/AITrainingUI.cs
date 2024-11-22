using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Security.Permissions;
using UnityEngine.UI;

public class AITrainingUI : MonoBehaviour
{
    public InputField aiTrainingData;
    string path;    

    void Start()
    {
        path = DatabasePath.GetCompanyDataPath(UuidMgr.Instance.currentUserInfo.companyUuid, "ai_training_data");
    }

    void Update()
    {

    }

    public void OnClickSetAiTrainingData(string path1, string aiData)
    {
        path1 = path;
        aiData = aiTrainingData.text;
        AsyncDatabase.SetDataToDatabase<string>(path1, aiData);
    }
}
