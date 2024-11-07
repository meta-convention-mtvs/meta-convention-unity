using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Firestore;
using System.Threading.Tasks;

public class DatabaseManager : Singleton<DatabaseManager>
{
    FirebaseFirestore store;

    private void Start()
    {
        store = FirebaseFirestore.DefaultInstance;
    }

    public void SaveData<T>(T info) where T : class
    {
        StartCoroutine(CoSaveUserInfo<T>(info));
    }
    IEnumerator CoSaveUserInfo<T>(T info) where T : class
    {
        // 저장 경로 USER/ID/내정보
        string path = "USER/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + "Data/" + typeof(T).ToString();
        // 정보 저장 요청
        Task task = FirebaseFirestore.DefaultInstance.Document(path).SetAsync(info);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없으면 
        if (task.Exception == null)
        {
            print("유저 정보 저장 성공");
        }
        else
        {
            print("유저 정보 저장 실패 : " + task.Exception);
        }
    }



    public void GetData<T>(Action<T> OnComplete)  where T :class
    {
        StartCoroutine(CoLoadUserInfo<T>( OnComplete));
    }
    IEnumerator CoLoadUserInfo<T>(Action<T> onComplete) where T: class
    {
        // 저장 경로 USER/ID/내 정보
        string path = "USER/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + "Data/" + typeof(T).ToString();
        // 정보 조회 요청
        Task<DocumentSnapshot> task = FirebaseFirestore.DefaultInstance.Document(path).GetSnapshotAsync();
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약 예외가 없다면
        if (task.Exception == null)
        {
            print("회원 정보 불러오기 성공!");
            // 불러온 정보를 UserInfo 변수에 저장
            T loadInfo = task.Result.ConvertTo<T>();
            // 불러온 정보를 전달
            if (onComplete != null)
            {
                onComplete(loadInfo);
            }
            else
            {
                onComplete(null);
            }
        }
        else
        {
            print("유저 정보 불러오기 실패 : " + task.Exception);
        }
    }

    public T GetData<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string data = PlayerPrefs.GetString(key);
            print(key + " : " + data);
            return (T)JsonUtility.FromJson<T>(data);
        }

        return default(T);
    }

}
