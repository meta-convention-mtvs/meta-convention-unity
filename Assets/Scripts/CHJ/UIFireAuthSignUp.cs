using Ricimi;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFireAuthSignUp : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public Button signUpButton;
    public TMP_Text signUpErrorText;
    public Action OnLoginClick;

    public Popup myPopup;

    private void Start()
    {
        signUpButton.onClick.AddListener(_OnSignUpClick);
    }

    void _OnSignUpClick()
    {
        FireAuthManager.Instance.SignUp(emailInput.text, passwordInput.text, myPopup.Close, (errorText) => signUpErrorText.text = "Sign up failed...");
    }
}