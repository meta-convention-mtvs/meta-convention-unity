using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[FirestoreData]
public class UserInfo
{
    // ���� ������ ���� ����..
    // DB�� �ø� ������..
}

[FirestoreData]
public class CompanyInfo
{
    // ��� , �ν� ������ ���� ���� 
    // DB�� �ø� �� ��
}
public class FireMgr : MonoBehaviour
{
    public InputField inputEmail;
    public InputField inputPassword;

    public void OnClickSignUp()
    {
        FireAuth.instance.SignUp(inputEmail.text, inputPassword.text);
    }

    public void OnClickLogIn()
    {
        FireAuth.instance.LogIn(inputEmail.text, inputPassword.text);
    }

    public void OnClickLogOut()
    {
        FireAuth.instance.Logout();
    }

    // ���� ���� ��üȭ, poll �޾Ƽ�  UID ������� DB�� �����ϱ� 

}
