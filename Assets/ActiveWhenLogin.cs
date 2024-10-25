using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveWhenLogin : MonoBehaviour
{
    Button button;
    public bool isLogin;

    private void Start()
    {
        button = GetComponent<Button>();
    }
    private void Update()
    {
        isLogin = FireAuthManager.Instance.isLogIn;
        button.interactable = FireAuthManager.Instance.isLogIn;
    }
}
