using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UICardMaker : MonoBehaviour
{
    public Text nameInput;
    public Text instituteInput;
    public Text majorInput;
    public Text email_Input;
    public Button saveButton;

    public Action<string, string, string, string> OnSaveClick;

    private void Start()
    {
        saveButton.onClick.AddListener(_OnSaveClick);
    }

    private void _OnSaveClick()
    {
        OnSaveClick?.Invoke(nameInput.text, instituteInput.text, majorInput.text, email_Input.text);
    }


}
