using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;

public class FireStore : MonoBehaviour
{
    public static FireStore instance;

    FirebaseFirestore store;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        store = FirebaseFirestore.DefaultInstance;
    }

    void Update()
    {
        
    }

    public void SaveUserInfo(UserInfo info)
    {
        StartCoroutine(CoSaveUserInfo(info));
    }
    IEnumerator CoSaveUserInfo(UserInfo info)
    {
        // 저장 경로 USER/ID/내정보
        string path = "USER/" + FireAuth.instance.auth.CurrentUser.UserId;
        // 정보 저장 요청
        Task task = store.Document(path).SetAsync(info);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없으면 
        if ( task.Exception == null)
        {
            print("유저 정보 저장 성공");
        }
        else
        {
            print("유저 정보 저장 실패 : " + task.Exception);
        }
    }

    public void LoadUserInfo(Action<UserInfo> onComplete)
    {
        StartCoroutine(CoLoadUserInfo(onComplete));
    }
    IEnumerator CoLoadUserInfo(Action<UserInfo> onComplete)
    {
        // 저장 경로 USER/ID/내 정보
        string path = "USER/" + FireAuth.instance.auth.CurrentUser.UserId;
        // 정보 조회 요청
        Task<DocumentSnapshot> task = store.Document(path).GetSnapshotAsync();
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약 예외가 없다면
        if(task.Exception == null)
        {
            print("회원 정보 불러오기 성공!");
            // 불러온 정보를 UserInfo 변수에 저장
            UserInfo loadInfo = task.Result.ConvertTo<UserInfo>();
            // 불러온 정보를 전달
            if(onComplete != null)
            {
                onComplete(loadInfo);
            }
        }
        else
        {
            print("유저 정보 불러오기 실패 : " + task.Exception);
        }
    }
}
