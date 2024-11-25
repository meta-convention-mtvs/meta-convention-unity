using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Security.Permissions;
using Firebase.Firestore;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.UI;

[FirestoreData]
public class ai_training_data
{
    [FirestoreProperty]
    public string trainingData { get; set; }

}

public class AITrainingUI : MonoBehaviour
{
    public static FireStore instance;
    FirebaseFirestore store;
    public TMP_InputField aiTrainingData;
    string path;
    ai_training_data aiData = new ai_training_data();


    void Start()
    {
        store = FirebaseFirestore.DefaultInstance;

        // 테스트용 자동 로그인 시켜놓기
        // FireAuth.instance.LogIn("test@test.com", "12341234");
        // 테스트용
        //path = DatabasePath.GetCompanyDataPath("test11", "ai_training_data");

        // uuid 있으면 요걸루
        path = DatabasePath.GetCompanyDataPath(UuidMgr.Instance.currentUserInfo.companyUuid, "ai_training_data");

    }

    public void OnSetClickTrainingData()
    {
        aiData.trainingData = aiTrainingData.text;
        SaveAiTrainingData(aiData);
    }

    public void SaveAiTrainingData(ai_training_data aidata)
    {
        StartCoroutine(CoSaveAiTrainingData(aidata));
    }

    IEnumerator CoSaveAiTrainingData(ai_training_data aidata)
    {
        Task task = store.Document(path).SetAsync(aidata);
        yield return new WaitUntil(() => task.IsCompleted);
        if(task.Exception == null)
        {
            print("유저 정보 저장 성공");
        }else
        {
            print(" 유저 정보 저장 실패: " + task.Exception);
        }
    }

}
