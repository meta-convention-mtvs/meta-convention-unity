using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Ricimi;

public class UICardMaker : MonoBehaviour
{
    public InputField nameInput;
    public InputField instituteInput;
    public InputField majorInput;
    public InputField phoneNumberInput;
    public Button saveButton;
    public Text cardErrorText;


    public Action<string, string, string, string, string> OnSaveClick;

    private void Start()
    {
        saveButton.onClick.AddListener(_OnSaveClick);
    }

    private void _OnSaveClick()
    {
        if (nameInput.text == "" || instituteInput.text == "" || majorInput.text == "" || phoneNumberInput.text == "")
        {
            cardErrorText.text = "모든 필드를 채워주세요";
            return;
        }
        UuidMgr.Instance.currentUserInfo.userName = nameInput.text;
        UuidMgr.Instance.currentUserInfo.companyName = instituteInput.text;
        UuidMgr.Instance.PrintUserInfo();
        OnSaveClick?.Invoke(nameInput.text, instituteInput.text, majorInput.text, phoneNumberInput.text, UuidMgr.Instance.currentUserInfo.companyUuid);
        saveButton.gameObject.GetComponent<SceneTransition>().PerformTransition();
    }


}
