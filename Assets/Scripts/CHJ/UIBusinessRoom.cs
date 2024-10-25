using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBusinessRoom : MonoBehaviour
{
    public Button qrcodeButton;
    public Button documentButton;
    public Button objectButton;
    public Button quitButton;

    public Action OnQrcode, OnDocument, OnObject, OnQuit;

    // Start is called before the first frame update
    void Start()
    {
        qrcodeButton.onClick.AddListener(_OnQrcode);
        documentButton.onClick.AddListener(_OnDocument);
        objectButton.onClick.AddListener(_OnObject);
        quitButton.onClick.AddListener(_OnQuit);
    }

    void _OnQrcode()
    {
        OnQrcode?.Invoke();
    }

    void _OnDocument()
    {
        OnDocument?.Invoke();
    }

    void _OnObject()
    {
        OnObject?.Invoke();
    }

    void _OnQuit()
    {
        OnQuit?.Invoke();
    }
}
