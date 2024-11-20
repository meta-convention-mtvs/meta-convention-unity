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
