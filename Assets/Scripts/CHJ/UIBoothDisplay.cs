using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoothDisplay : MonoBehaviour
{
    public Button objectButton;
    public Button videoButton;
    public Button[] furnitureButtons;
    public Button bannerButton;

    public Action OnObjectButtonClick, OnVideoButtonClick, OnBannerButtonClick;
    public Action[] OnFurnitureButtonClick;

    private void Awake()
    {
        OnFurnitureButtonClick = new Action[furnitureButtons.Length];
        BoothCustomizingManager boothManager = GameObject.FindObjectOfType<BoothCustomizingManager>();
        if (boothManager != null)
        {
            boothManager.SetUIDisplay(this);
        }
    }

    private void Start()
    {
        objectButton.onClick.AddListener(_OnObjectButtonClick);
        videoButton.onClick.AddListener(_OnVideoButtonClick);
        for(int i = 0; i<furnitureButtons.Length; i++)
        {
            // 이 변수는 값 참조를 위해서 필요함. 없으면 i가 바뀌면 계속 i 값 따라감.
            int j = i;
            furnitureButtons[i].onClick.AddListener(() => _OnFurnitureButtonClick(j));
        }
        bannerButton.onClick.AddListener(_OnBannerButtonClick);
    }

    void _OnObjectButtonClick()
    {
        OnObjectButtonClick?.Invoke();
    }

    void _OnVideoButtonClick()
    {
        OnVideoButtonClick?.Invoke();
    }

    void _OnFurnitureButtonClick(int index)
    {
        OnFurnitureButtonClick[index]?.Invoke();
    }

    void _OnBannerButtonClick()
    {
        OnBannerButtonClick?.Invoke();
    }
}
