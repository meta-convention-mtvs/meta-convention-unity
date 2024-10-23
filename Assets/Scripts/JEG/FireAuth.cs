using Firebase.Auth;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FireAuth : MonoBehaviour
{
    public static FireAuth instance;
    public FirebaseAuth auth;

    public GameObject loginUI;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += OnChangeAuthState;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            loginUI.SetActive(!loginUI.activeInHierarchy);
        }
    }

    void OnChangeAuthState(object sender, EventArgs e)
    {
        // ����, ���� ������ �ִٸ�
        if(auth.CurrentUser != null)
        {
            print(auth.CurrentUser.Email + " , " + auth.CurrentUser.UserId);
            // �α��� �Ǿ� ����
            print("�α��� ����");
        }
        // �׷��� ������
        else
        {
            print("�α� �ƿ� ����");
            // �α� �ƿ�
        }
    }

    public void SignUp(string email, string password)
    {
        StartCoroutine(CoSignUp(email, password));
    }

    IEnumerator CoSignUp(string email, string password)
    {
        // ȸ�� ���� �õ�
        Task<AuthResult> task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        // ����� �Ϸ� �� �� ���� ��ٸ���.
        yield return new WaitUntil(() => task.IsCompleted);
        // ���࿡ ���ܰ� ���ٸ�
        if(task.Exception == null)
        {
            print("ȸ�� ���� ����");
        }
        else
        {
            print("ȸ�� ���� ���� : " + task.Exception);
        }
    }

    public void LogIn(string email, string password)
    {
        StartCoroutine(CoLogin(email, password));
    }

    IEnumerator CoLogin(string email, string password)
    {
        // �α��� �õ�
        Task<AuthResult> task = auth.SignInWithEmailAndPasswordAsync(email, password);
        // ����� �Ϸ� �� �� ���� ��ٸ���.
        yield return new WaitUntil(() => task.IsCompleted);
        // ���࿡ ���ܰ� ���ٸ�
        if (task.Exception == null)
        {
            print("�α��� ����");
        }
        else
        {
            print("�α��� ���� : " + task.Exception);
        }
    }

    public void Logout()
    {
        auth.SignOut();
        print("�α� �ƿ�!");
    }
}
