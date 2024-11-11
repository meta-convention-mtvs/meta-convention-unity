using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UIBoothCustomzing : MonoBehaviour
{
    public Button BlankRoom;
    public Button CubicRoom;
    public Button RoundRoom;

    public Button LogoButton;
    public Button ColorButton;

    public Button ObjectButton;
    public Button VideoButton;

    public Action OnBlankRoom, OnCubicRoom, OnRoundRoom;

    public Action OnLogoButton;
    public Action OnColorButton;
    public Action OnObjectButton;
    public Action OnVideoButton;

    private void Start()
    {
        BlankRoom.onClick.AddListener(_OnBlankRoom);
        CubicRoom.onClick.AddListener(_OnCubicRoom);
        RoundRoom.onClick.AddListener(_OnRoundRoom);
        LogoButton.onClick.AddListener(_OnLogoButton);
        ColorButton.onClick.AddListener(_OnColorButton);
        ObjectButton.onClick.AddListener(_OnObjectButton);
        VideoButton.onClick.AddListener(_OnVideoButton);
    }

    void _OnBlankRoom()
    {
        OnBlankRoom?.Invoke();
    }

    void _OnCubicRoom()
    {
        OnCubicRoom?.Invoke();
    }

    void _OnRoundRoom()
    {
        OnRoundRoom?.Invoke();
    }

    void _OnLogoButton()
    {
        OnLogoButton?.Invoke();
    }

    void _OnColorButton()
    {
        OnColorButton?.Invoke();
    }

    void _OnObjectButton()
    {
        OnObjectButton?.Invoke();
    }

    void _OnVideoButton()
    {
        OnVideoButton?.Invoke();
    }
}
