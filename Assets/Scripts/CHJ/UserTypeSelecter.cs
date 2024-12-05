using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ricimi;

public class UserTypeSelecter : MonoBehaviour
{
    public Button individualButton;
    public Button companyButton;

    public SceneTransition individualSceneTransition;
    public SceneTransition companySceneTransition;

    public Toggle isNewCompany;

    private void Start()
    {
        individualButton.onClick.AddListener(OnIndividualButtonClick);
        companyButton.onClick.AddListener(OnCompanyButtonClick);
    }

    void OnIndividualButtonClick()
    {
        UserTypeData userType = new UserTypeData();
        userType.userType = UserTypeData.UserType.individual;
        DatabaseManager.Instance.SaveData<UserTypeData>(userType);
        individualSceneTransition.PerformTransition();
    }

    void OnCompanyButtonClick()
    {
        if (isNewCompany.isOn)
        {
            UuidMgr.Instance.GenerateUuid();
            print("신규 uuid 가 발행되었습니다");
        } else if (!isNewCompany.isOn)
        {
            UuidMgr.Instance.FindClosestCompanyUUID();
            print(" 기존 기업의 uuid 를 찾아서 할당 합니다.");
        }
        print(UuidMgr.Instance.currentUserInfo.companyName);
        print($" uuid : {UuidMgr.Instance.currentUserInfo.companyUuid}");
        UuidMgr.Instance.PrintUserInfo();
        UserTypeData userType = new UserTypeData();
        userType.userType = UserTypeData.UserType.company;
        DatabaseManager.Instance.SaveData<UserTypeData>(userType);
        companySceneTransition.PerformTransition();
    }
}

[FirestoreData]
class UserTypeData
{
    public enum UserType
    {
        company,
        individual
    }
    [FirestoreProperty]
    public UserType userType { get; set; }
}
