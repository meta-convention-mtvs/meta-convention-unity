using System;
using System.Collections;
using System.Collections.Generic;
using TriLibCore;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(AudioSource))]
public class RenderBoothData : MonoBehaviour
{
    public GameObject boothLogo;
    public GameObject boothVideoWall;
    public GameObject banner;
    public GameObject brochure;

    private Renderer boothLogoRenderer;
    private Renderer boothVideoRenderer;
    private Renderer boothRenderer;
    private Renderer bannerRenderer;
    private Renderer brochureRenderer;

    private VideoPlayer videoPlayer;
    private RenderTexture videoRenderTexture;
    private AudioSource audioSource;

    private GameObject currentInstantiatedObject;

    private BoothExtraData extraData;

    private void Awake()
    {
        boothRenderer = GetComponent<Renderer>();
        bannerRenderer = banner.GetComponent<Renderer>();
        brochureRenderer = brochure.GetComponent<Renderer>();
        boothLogoRenderer = boothLogo.GetComponent<Renderer>();
        boothVideoRenderer = boothVideoWall.GetComponent<Renderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void RenderBoothDataWith(BoothExtraData extraData)
    {
        this.extraData = extraData;

        if (extraData.logoImage != null)
        {
            SetLogo(extraData.logoImage);
        }
        SetColor(extraData.color);
        if (!string.IsNullOrEmpty(extraData.videoURL))
        {
            SetVideo(extraData.videoURL);
        }
        if (currentInstantiatedObject != null)
        {
            currentInstantiatedObject.transform.localScale = new Vector3(extraData.modelingScale, extraData.modelingScale, extraData.modelingScale);
        }
        SetBanner(extraData.hasBanner, extraData.bannerImage, extraData.homepageLink);
        SetBrochure(extraData.hasBrochure, extraData.brochureImage);
    }

    public void RenderBoothModeling(BoothExtraData extraData)
    {
        this.extraData = extraData;

        if(currentInstantiatedObject != null)
        {
            Destroy(currentInstantiatedObject);
        }
        if(extraData.modelingPath != null)
            ObjectLoader.StartImporting(extraData.modelingPath, OnModelLoad);

    }

    private void OnModelLoad(AssetLoaderContext context)
    {
        ShowBoothModeling(context.RootGameObject, extraData);
    }

    void ShowBoothModeling(GameObject modeling, BoothExtraData extraData)
    {
        currentInstantiatedObject = modeling;
        if (currentInstantiatedObject != null)
        {
            currentInstantiatedObject.transform.localScale = new Vector3(extraData.modelingScale, extraData.modelingScale, extraData.modelingScale);
            currentInstantiatedObject.transform.SetParent(this.gameObject.transform, false);
        }
    }

    void SetLogo(Texture2D images)
    {      
        boothLogoRenderer.material.mainTexture = images;
        SetEmissionInImage(boothLogoRenderer, images);
    }

    void SetColor(Color color)
    {
        boothRenderer.material.color = color;
    }

    /// <summary>
    /// Video를 url을 받아서 재생합니다. 만약 로컬 비디오를 재생한다면 video path앞에 꼭 file://를 붙여서 주세요.
    /// </summary>
    /// <param name="path"></param>
    void SetVideo(string path)
    {
        // Render Texture를 만든다
        if(videoRenderTexture == null)
        {
            videoRenderTexture = new RenderTexture(1920, 1080, 32);
            videoRenderTexture.Create();
            videoPlayer.targetTexture = videoRenderTexture;
            boothVideoRenderer.material.mainTexture = videoRenderTexture;
            SetEmissionInImage(boothVideoRenderer, videoRenderTexture);
        }
        // Render Texture를 벽의 
        videoPlayer.url = path;
        videoPlayer.isLooping = true;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += PlayVideo;
    }

    void PlayVideo(VideoPlayer videoPlayer)
    {
        videoPlayer.Play();
    }

    void SetBanner(bool hasBanner, Texture2D bannerImage, string homepageURL)
    {
        if (hasBanner)
        {
            banner.SetActive(true);
            if (bannerImage != null)
            {
                bannerRenderer.material.mainTexture = bannerImage;
                SetEmissionInImage(bannerRenderer, bannerImage);
            }
            banner.GetComponent<InteractableBannerObject>().homepageURL = homepageURL;
        }
        else
        {
            banner.SetActive(false);
        }
    }

    void SetBrochure(bool hasBrochure, Texture2D brochureImage)
    {
        if (hasBrochure)
        {
            brochure.SetActive(true);
            if (brochureImage != null)
            {
                brochureRenderer.material.mainTexture = brochureImage;
                SetEmissionInImage(brochureRenderer, brochureImage);
            }
        }
        else
        {
            brochure.SetActive(false);
        }
    }

    void SetEmissionInImage(Renderer objectRenderer, Texture texture)
    {
        objectRenderer.material.SetColor("_EmissionColor", Color.white);
        objectRenderer.material.SetTexture("_EmissionMap", texture);
        objectRenderer.material.EnableKeyword("_EMISSION");
    }
}
