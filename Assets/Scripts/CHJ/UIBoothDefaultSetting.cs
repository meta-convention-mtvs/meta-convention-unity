using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBoothDefaultSetting : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public Button[] boothTypeButton;
    public TMP_InputField companyName;
    public Button logoButton;
    public TMP_InputField homepageLink;
    public Button colorButton;

    public Action<int> OnDropdownChanged;
    public Action[] OnBoothButtonClick;
    public Action<string> OnCompanyNameChanged;
    public Action OnLogoButtonClick;
    public Action<string> OnHomepageNameChanged;
    public Action OnColorButtonClick;

    private void Awake()
    {
        OnBoothButtonClick = new Action[boothTypeButton.Length];
        BoothCustomizingManager boothManager = GameObject.FindObjectOfType<BoothCustomizingManager>();
        if(boothManager != null)
        {
            boothManager.SetUIDefaultSetting(this);
        }
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(_OnDropdownChanged);
        for (int i = 0; i < boothTypeButton.Length; i++)
        {
            // 이 변수는 값 참조를 위해서 필요함. 없으면 i가 바뀌면 계속 i 값 따라감.
            int j = i;
            boothTypeButton[i].onClick.AddListener(() => _OnBoothButtonClick(j));
        }
        companyName.onEndEdit.AddListener(_OnCompanyNameChanged);
        logoButton.onClick.AddListener(_OnLogoButtonClick);
        homepageLink.onEndEdit.AddListener(_OnHomepageNameChanged);
        colorButton.onClick.AddListener(_OnColorButtonClick);
    }

    void _OnDropdownChanged(int value)
    {
        OnDropdownChanged?.Invoke(value);
    }

    void _OnBoothButtonClick(int index)
    {
        print(index);
        OnBoothButtonClick[index]?.Invoke();
    }

    void _OnCompanyNameChanged(string s)
    {
        OnCompanyNameChanged?.Invoke(s);
    }

    void _OnLogoButtonClick()
    {
        OnLogoButtonClick?.Invoke();
    }

    void _OnHomepageNameChanged(string s)
    {
        OnHomepageNameChanged?.Invoke(s);
    }

    void _OnColorButtonClick()
    {
        OnColorButtonClick?.Invoke();
    }
}
