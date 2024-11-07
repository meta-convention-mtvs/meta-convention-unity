using Ricimi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFireAuthLogin : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button loginButton;
    public TMP_Text loginErrorText;
    public Action OnLoginClick;

    public Popup myPopup;

    private void Start()
    {
        loginButton.onClick.AddListener(_OnLoginClick);
    }

    void _OnLoginClick()
    {
        FireAuthManager.Instance.LogIn(emailInput.text, passwordInput.text, myPopup.Close, (errorText) => loginErrorText.text ="login failed...");
    }
}
