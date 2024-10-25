using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFireAuthLogin : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public Button loginButton;

    public Action OnLoginClick;

    private void Start()
    {
        loginButton.onClick.AddListener(_OnLoginClick);
    }

    void _OnLoginClick()
    {
        OnLoginClick?.Invoke();
    }
}
