using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class KeyValuePair
{
    public string key;
    public string value;
}

public class LanguageSelector : MonoBehaviour
{
    public KeyValuePair[] languageDictionary;
    public TMP_Dropdown dropdown;
    
    int languageIndex = -1;

    private void Start()
    {
        SetDropdownValue(dropdown, languageDictionary);
        dropdown.onValueChanged.AddListener(OnLanguageChanged);
    }

    void SetDropdownValue(TMP_Dropdown dropdown, KeyValuePair[] languageDictionary)
    {

        List<TMP_Dropdown.OptionData> optionDatas = new List<TMP_Dropdown.OptionData>();
        foreach (KeyValuePair item in languageDictionary) {
            optionDatas.Add(new TMP_Dropdown.OptionData(item.value));
        }
        dropdown.AddOptions(optionDatas);
    }

    void OnLanguageChanged(int languageIndex)
    {
        this.languageIndex = languageIndex;
    }

    public void OnSaveData()
    {
        if(languageIndex != -1)
        {
            Language languageData = new Language();
            languageData.language = languageDictionary[languageIndex].key;
            LanguageSingleton.Instance.language = languageData.language;
            DatabaseManager.Instance.SaveData<Language>(languageData);
        }
    }

}

public class LanguageSingleton : Singleton<LanguageSingleton>
{
    public string language;
}

[FirestoreData]
public class Language
{
    [FirestoreProperty]
    public string language { get; set; }
}
