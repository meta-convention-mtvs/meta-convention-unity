using Firebase.Auth;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FireAuth : MonoBehaviour
{
    public static FireAuth instance;
    public FirebaseAuth auth;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnChangeAuthState;
    }

    void OnChangeAuthState(object sender, EventArgs e)
    {
        // 만약, 유저 정보가 있다면
        if(auth.CurrentUser != null)
        {
            print(auth.CurrentUser.Email + " , " + auth.CurrentUser.UserId);
            // 로그인 되어 있음
            print("로그인 상태");
        }
        // 그렇지 않으면
        else
        {
            print("로그 아웃 상태");
            // 로그 아웃
        }
    }

    public void SignUp(string email, string password)
    {
        StartCoroutine(CoSignUp(email, password));
    }

    IEnumerator CoSignUp(string email, string password)
    {
        // 회원 가입 시도
        Task<AuthResult> task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없다면
        if(task.Exception == null)
        {
            print("회원 가입 성공");
        }
        else
        {
            print("회원 가입 실패 : " + task.Exception);
        }
    }

    public void LogIn(string email, string password)
    {
        StartCoroutine(CoLogin(email, password));
    }

    IEnumerator CoLogin(string email, string password)
    {
        // 로그인 시도
        Task<AuthResult> task = auth.SignInWithEmailAndPasswordAsync(email, password);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없다면
        if (task.Exception == null)
        {
            print("로그인 성공");
        }
        else
        {
            print("로그인 실패 : " + task.Exception);
        }
    }

    public void Logout()
    {
        auth.SignOut();
        print("로그 아웃!");
    }
}
