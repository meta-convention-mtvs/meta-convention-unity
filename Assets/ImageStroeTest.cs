using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class ImageStroeTest : MonoBehaviour
{

    public Renderer myRenderer;
    public VideoPlayer videoPlayer;

    public string imageName;
    public string videoName;
    public void ShowPanel()
    {
        FileUploadManager.Instance.SetUpImageFileBrowser();
        FileUploadManager.Instance.ShowDialog(UploadImage);
    }

    void UploadImage(string[] paths)
    {
        string path = paths[0];
        imageName = Path.GetFileName(path);
        DatabaseManager.Instance.UploadImage(path);
    }

    public void DownloadImage()
    {
        DatabaseManager.Instance.DownloadImage(imageName, OnTextureLoad);
    }

    void OnTextureLoad(Texture2D texture)
    {
        myRenderer.material.mainTexture = texture;
    }

    public void ShowVideoPanel()
    {
        FileUploadManager.Instance.SetUpVideoFileBrowser();
        FileUploadManager.Instance.ShowDialog(UploadVideo);
    }

    void UploadVideo(string[] paths)
    {
        string path = paths[0];
        videoName = Path.GetFileName(path);
        DatabaseManager.Instance.UploadVideo(path);
    }

    public void DownloadVideo()
    {
        DatabaseManager.Instance.DownLoadVideo(videoName, OnVideoLoad);
    }

    void OnVideoLoad(string url)
    {
        videoPlayer.url = url;
        videoPlayer.Play();
    }
    
}
