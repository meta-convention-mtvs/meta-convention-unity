using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

[FirestoreData]
public class UserInfo
{
    [FirestoreProperty]
    public string uID { get; set; }
    [FirestoreProperty]
    public bool isCompany { get; set; }
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
    [FirestoreProperty]
    public string uID { get; set; }
    [FirestoreProperty]
    public bool isCompany { get; set; }
    [FirestoreProperty]
    public string company_name { get; set; }
    [FirestoreProperty]
    public string company_mission { get; set; }
    [FirestoreProperty]
    public string company_website { get; set; }
    [FirestoreProperty]
    public List<Product> products { get; set; }
}
[FirestoreData]
public class Product
{
    [FirestoreProperty]
    public string name { get; set; }
    [FirestoreProperty]
    public string description { get; set; }
    [FirestoreProperty]
    public List<Resource> resources { get; set; }
}
[FirestoreData]
public class Resource
{
    [FirestoreProperty]
    public string type { get; set; }
    [FirestoreProperty]
    public string description { get; set; }
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
        info.isCompany = false;
        info.name = userName.text;
        info.isMan = isMan;
        //info.interests = new List<string>();
        //info.visitBooth = new List<string>();
        //info.likeBooth = new List<string>();

        FireStore.instance.SaveUserInfo(info);
    }
    // 가입 정보 구체화, poll 받아서  UID 기반으로 DB에 저장하기 

    public void OoClickSaveCompanyInfo()
    {
        CompanyInfo info = new CompanyInfo();
        info.uID = FireAuth.instance.auth.CurrentUser.UserId;
        info.isCompany = true;
        
        
    }

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

    public void OnClickUpload()
    {
        byte[] data = File.ReadAllBytes("D:\\UnityProjects\\meta-convention-unity\\Assets\\Materials\\");
        string path = "ProfileImage/" + FireAuth.instance.auth.CurrentUser.UserId + ".png";

        FireStorage.instance.Upload(data, path);
    }
    
    public void OnClickDownload()
    {
        string path = "ProfileImage/" + FireAuth.instance.auth.CurrentUser.UserId + ".png";

        FireStorage.instance.DownLoad(path, OnCompleteDownload);
    }

    public RawImage profileImage;
    void OnCompleteDownload(byte[] data)
    {
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(data);
        profileImage.texture = texture;
    }

}
