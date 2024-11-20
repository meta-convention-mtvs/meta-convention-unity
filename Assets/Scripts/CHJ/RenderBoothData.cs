using System.Collections;
using System.Collections.Generic;
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
        if(currentInstantiatedObject != null)
        {
            Destroy(currentInstantiatedObject);
        }
        currentInstantiatedObject = ObjectLoader.ImportObj(extraData.modelingPath);
        if (currentInstantiatedObject != null)
        {
            currentInstantiatedObject.transform.localScale = new Vector3(extraData.modelingScale, extraData.modelingScale, extraData.modelingScale);
            currentInstantiatedObject.transform.SetParent(this.gameObject.transform, false);
        }
    }
    void SetLogo(Texture2D images)
    {
       
        boothLogoRenderer.material.mainTexture = images;
        boothLogoRenderer.material.SetColor("_EmissionColor", Color.white);
        boothLogoRenderer.material.SetTexture("_EmissionMap", images);
        boothLogoRenderer.material.EnableKeyword("_EMISSION");
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
            boothVideoRenderer.material.SetTexture("_EmissionMap", videoRenderTexture);
            boothVideoRenderer.material.SetColor("_EmissionColor", Color.white);
            boothVideoRenderer.material.EnableKeyword("_EMISSION");
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
            bannerRenderer.material.mainTexture = bannerImage;
            bannerRenderer.material.SetColor("_EmissionColor", Color.white);
            bannerRenderer.material.SetTexture("_EmissionMap", bannerImage);
            bannerRenderer.material.EnableKeyword("_EMISSION");
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
            brochureRenderer.material.mainTexture = brochureImage;
            brochureRenderer.material.SetColor("_EmissionColor", Color.white);
            brochureRenderer.material.SetTexture("_EmissionMap", brochureImage);
            brochureRenderer.material.EnableKeyword("_EMISSION");
        }
        else
        {
            brochure.SetActive(false);
        }
    }

}
