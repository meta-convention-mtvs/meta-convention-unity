using Firebase.Auth;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FireAuthManager : Singleton<FireAuthManager>
{

    public bool isLogIn;

   void Start()
    {
        FirebaseAuth.DefaultInstance.StateChanged += OnChangeAuthState;
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public FirebaseUser GetCurrentUser()
    {
        return FirebaseAuth.DefaultInstance.CurrentUser;
    }
    void OnChangeAuthState(object sender, EventArgs e)
    {
        // 만약, 유저 정보가 있다면
        if (FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            print(FirebaseAuth.DefaultInstance.CurrentUser.Email + " , " + FirebaseAuth.DefaultInstance.CurrentUser.UserId);
            // 로그인 되어 있음
            isLogIn = true;
            print("로그인 상태");
        }
        // 그렇지 않으면
        else
        {
            isLogIn = false;
            print("로그 아웃 상태");
            // 로그 아웃
        }
    }

    public void SignUp(string email, string password, Action onSuccess, Action<string> onFailed)
    {
        StartCoroutine(CoSignUp(email, password, onSuccess, onFailed));
    }

    IEnumerator CoSignUp(string email, string password, Action onSuccess, Action<string> onFailed)
    {
        // 회원 가입 시도
        Task<AuthResult> task = FirebaseAuth.DefaultInstance.CreateUserWithEmailAndPasswordAsync(email, password);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없다면
        if (task.Exception == null)
        {
            onSuccess?.Invoke();
            print("회원 가입 성공");
        }
        else
        {
            onFailed?.Invoke(task.Exception.ToString());
            print("회원 가입 실패 : " + task.Exception);
        }
    }

    public void LogIn(string email, string password, Action onSuccess, Action<string> onFailed)
    {
        StartCoroutine(CoLogin(email, password, onSuccess, onFailed));
    }

    IEnumerator CoLogin(string email, string password, Action onSuccess, Action<string> onFailed)
    {
        // 로그인 시도
        Task<AuthResult> task = FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없다면
        if (task.Exception == null)
        {
            onSuccess?.Invoke();
            print("로그인 성공");
        }
        else
        {
            onFailed?.Invoke(task.Exception.ToString());
            print("로그인 실패 : " + task.Exception);
        }
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        print("로그 아웃!");
    }
}