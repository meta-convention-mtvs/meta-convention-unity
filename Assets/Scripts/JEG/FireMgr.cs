using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[FirestoreData]
public class UserInfo
{
    // 유저 인포를 정리 하자..
    // DB에 올릴 정보들..
}

[FirestoreData]
public class CompanyInfo
{
    // 기업 , 부스 정보를 정리 하자 
    // DB에 올릴 것 들
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

    // 가입 정보 구체화, poll 받아서  UID 기반으로 DB에 저장하기 

}
