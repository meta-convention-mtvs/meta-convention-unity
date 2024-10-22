using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UICardMaker : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField instituteInput;
    public TMP_InputField majorInput;
    public TMP_InputField email_Input;
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
