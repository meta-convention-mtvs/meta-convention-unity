using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoothDisplay : MonoBehaviour
{
    public Button objectButton;
    public Button videoButton;
    public Button brochureButton;
    public Button bannerButton;
    public Button brochureImageButton;
    public Button bannerImageButton;
    public Slider objectSizeSlider;

    public Action OnObjectButtonClick, OnVideoButtonClick, OnBrochureImageButtonCilck, OnBannerImageButtonClick;
    public Action OnBrochureButtonClick, OnBannerButtonClick;
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

        brochureButton.onClick.AddListener(_OnBrochureButtonClick);
        bannerButton.onClick.AddListener(_OnBannerButtonClick);
        bannerImageButton.onClick.AddListener(_OnBannerImageButtonClick);
        brochureImageButton.onClick.AddListener(_OnBrochureImageButtonClick);

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

    void _OnBrochureButtonClick()
    {
        OnBrochureButtonClick.Invoke();
    }

    void _OnBannerButtonClick()
    {
        OnBannerButtonClick?.Invoke();
    }

    void _OnObjectSliderChanged(float f)
    {
        OnObjectSliderChanged?.Invoke(f);
    }

    private void _OnBrochureImageButtonClick()
    {
        OnBrochureImageButtonCilck?.Invoke();
    }

    void _OnBannerImageButtonClick()
    {
        OnBannerImageButtonClick?.Invoke();
    }
}
