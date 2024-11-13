using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoothDisplay : MonoBehaviour
{
    public Button objectButton;
    public Button videoButton;
    public Button furnitureButton;
    public Button bannerButton;
    public Button bannerImageButton;
    public Slider objectSizeSlider;

    public Action OnObjectButtonClick, OnVideoButtonClick, OnBannerImageButtonClick;
    public Action OnFurnitureButtonClick, OnBannerButtonClick;
    public Action<float> OnObjectSliderChanged;

    private void Awake()
    {
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
        furnitureButton.onClick.AddListener(_OnFurnitureButtonClick);
        bannerImageButton.onClick.AddListener(_OnBannerImageButtonClick);
        bannerButton.onClick.AddListener(_OnBannerButtonClick);
        objectSizeSlider.onValueChanged.AddListener(_OnObjectSliderChanged);
    }

    void _OnObjectButtonClick()
    {
        OnObjectButtonClick?.Invoke();
    }

    void _OnVideoButtonClick()
    {
        OnVideoButtonClick?.Invoke();
    }

    void _OnFurnitureButtonClick()
    {
        OnFurnitureButtonClick.Invoke();
    }

    void _OnBannerButtonClick()
    {
        OnBannerButtonClick?.Invoke();
    }

    void _OnObjectSliderChanged(float f)
    {
        OnObjectSliderChanged?.Invoke(f);
    }

    void _OnBannerImageButtonClick()
    {
        OnBannerImageButtonClick?.Invoke();
    }
}
