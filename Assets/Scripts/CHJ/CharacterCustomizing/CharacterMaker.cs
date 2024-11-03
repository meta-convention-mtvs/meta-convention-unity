using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterMaker : MonoBehaviour
{
    public CustomizeManager maleCustomMgr;
    public CustomizeManager femaleCustomMgr;
    public UICharacterMaker ui_cm;

    [SerializeField]
    CharacterCustomizeData data;

    bool isMan = true;
    CustomizeManager currentCustomizeManager;

    private void Start()
    {
        currentCustomizeManager = maleCustomMgr;
        currentCustomizeManager.SetCustomizingIdx(0);
        ui_cm.OnNextClick += IncCustomizingIdx;
        ui_cm.OnPrevClick += DecCustomizingIdx;
        ui_cm.OnSaveClick += SaveCharacterInfo;
        ui_cm.OnGenderClick += ChangeGender;
    }

    // 성별이 바뀌었을 때
    public void ChangeGender()
    {
        isMan = !isMan;
        if (isMan)
        {
            maleCustomMgr.SetCustomizingIdx(0);
            femaleCustomMgr.HideObject();
            currentCustomizeManager = maleCustomMgr;
        }
        else
        {
            maleCustomMgr.HideObject();
            femaleCustomMgr.SetCustomizingIdx(0);
            currentCustomizeManager = femaleCustomMgr;
        }


    }

    public void SetCustomizingIdx(int idx)
    {
        currentCustomizeManager.SetCustomizingIdx(idx);
    }

    public void IncCustomizingIdx()
    {
        currentCustomizeManager.IncCustomizingIdx();
    }

    public void DecCustomizingIdx()
    {
        currentCustomizeManager.DecCustomizingIdx();
    }

    public void SaveCharacterInfo()
    {
        data = new CharacterCustomizeData();
        data.isMan = isMan;
        data.customizingIdx = currentCustomizeManager.GetCustomizingIdx();

        DatabaseManager.Instance.SaveData<CharacterCustomizeData>(data);
    }
}

[FirestoreData]
public class CharacterCustomizeData
{
    [FirestoreProperty]
    public bool isMan { get; set; }
    [FirestoreProperty]
    public int customizingIdx { get; set; }

    public CharacterCustomizeData()
    {

    }

    public CharacterCustomizeData(bool isMan, int customizingIdx)
    {
        this.isMan = isMan;
        this.customizingIdx = customizingIdx;
    }
}
