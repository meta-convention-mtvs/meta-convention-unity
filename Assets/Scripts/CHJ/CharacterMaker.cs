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
    CharacterCustomzieData data;

    bool isMan = true;
    CustomizeManager currentCustomizeManager;

    private void Start()
    {
        currentCustomizeManager = maleCustomMgr;
        ui_cm.OnPrevClick += IncCustomizingIdx;
        ui_cm.OnNextClick += DecCustomizingIdx;
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
        data = new CharacterCustomzieData();
        data.isMan = isMan;
        data.customizingIdx = currentCustomizeManager.GetCustomizingIdx();

        DatabaseManager.Instance.SaveData(data);

        SceneManager.LoadScene(2);
    }
}

[System.Serializable]
public class CharacterCustomzieData
{
    public bool isMan;
    public int customizingIdx;
}
