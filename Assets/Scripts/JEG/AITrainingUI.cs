using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Security.Permissions;
using Firebase.Firestore;
using TMPro;
using System.Threading.Tasks;

[FirestoreData]
public class AiTrainingData
{
    [FirestoreProperty]
    public string trainingData { get; set; }

    public AiTrainingData(string trainingData)
    {
        this.trainingData = trainingData;
    }

}

public class AITrainingUI : MonoBehaviour
{
    public TMP_InputField aiTrainingData;
    string path;
    AiTrainingData aiData = new AiTrainingData("");


    void Start()
    {

        // 테스트용 
        path = DatabasePath.GetCompanyDataPath("test11", "ai_training_data");
        // uuid 있으면 요걸루
        //path = DatabasePath.GetCompanyDataPath(UuidMgr.Instance.currentUserInfo.companyUuid, "ai_training_data");
        
    }

    public void  OnClickSetAiTrainingData()
    {
        aiData.trainingData = aiTrainingData.text;
        Task<bool> result = AsyncDatabase.SetDataToDatabase<AiTrainingData>(path, aiData);
       
    }
}
