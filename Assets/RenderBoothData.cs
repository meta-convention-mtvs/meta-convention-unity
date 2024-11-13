using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer), typeof(AudioSource))]
public class RenderBoothData : MonoBehaviour
{
    public GameObject boothLogo;
    public GameObject boothVideoWall;

    private Renderer boothLogoRenderer;
    private Renderer boothVideoRenderer;
    private Renderer boothRenderer;

    private VideoPlayer videoPlayer;
    private RenderTexture videoRenderTexture;
    private AudioSource audioSource;

    private GameObject currentInstantiatedObject;

    private void Awake()
    {
        boothRenderer = GetComponent<Renderer>();
        boothLogoRenderer = boothLogo.GetComponent<Renderer>();
        boothVideoRenderer = boothVideoWall.GetComponent<Renderer>();
        videoPlayer = GetComponent<VideoPlayer>();
        audioSource = GetComponent<AudioSource>();
    }

    public void RenderBoothDataWith(BoothExtraData extraData)
    {
        SetLogo(extraData.logoImage);
        SetColor(extraData.color);
        SetVideo(extraData.videoURL);
        if (currentInstantiatedObject != null)
        {
            currentInstantiatedObject.transform.localScale = new Vector3(extraData.modelingScale, extraData.modelingScale, extraData.modelingScale);
        }
    }

    public void RenderBoothModeling(BoothExtraData extraData)
    {
        if(currentInstantiatedObject != null)
        {
            Destroy(currentInstantiatedObject);
        }
        currentInstantiatedObject = ObjectLoader.ImportObj(extraData.modelingPath);
        if(currentInstantiatedObject != null)
            currentInstantiatedObject.transform.localScale = new Vector3(extraData.modelingScale, extraData.modelingScale, extraData.modelingScale);
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


}
