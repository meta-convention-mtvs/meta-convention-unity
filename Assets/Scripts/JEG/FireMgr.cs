using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[FirestoreData]
public class UserInfo
{
    [FirestoreProperty]
    public string uID { get; set; }
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public bool isMan { get; set; }
    [FirestoreProperty]

    public List<string> interests { get; set; }
    [FirestoreProperty]

    public List<string> visitBooth { get; set; }
    [FirestoreProperty]
    public List<string> likeBooth { get; set; }
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

    public InputField userName;
    public Toggle isMan;

    public Text loadedUserName;
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

    public void OnClickSaveUserInfo()
    {
        UserInfo info = new UserInfo();
        info.uID = FireAuth.instance.auth.CurrentUser.UserId;
        info.name = userName.text;
        info.isMan = isMan;
        //info.interests = new List<string>();
        //info.visitBooth = new List<string>();
        //info.likeBooth = new List<string>();

        FireStore.instance.SaveUserInfo(info);
    }
    // 가입 정보 구체화, poll 받아서  UID 기반으로 DB에 저장하기 

    public void OnClickLoadUserInfo()
    {
        FireStore.instance.LoadUserInfo(OnCompleteLoadUserInfo);
    }

    void OnCompleteLoadUserInfo(UserInfo info)
    {
        loadedUserName.text = info.name;
        print(info.name);
        print(info.isMan);
        for (int i = 0; i< info.interests.Count; i++)
        {
            print(info.interests[i]);
        }
    }

}
