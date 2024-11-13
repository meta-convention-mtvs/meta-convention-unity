using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

[RequireComponent(typeof(RenderBoothData))]
public class BoothRuntimeCreate : MonoBehaviourPun
{

    // Booth Runtime Create에서 할 일:
    // 부스 정보를 읽어온다.
    // 부스 정보를 바탕으로 데이터베이스에서 데이터를 읽어온다.
    // 읽은 데이터를 바탕으로 데이터를 가공하여 RenderBoothData에게 넘겨준다.
    private RenderBoothData renderBoothData;

    private string boothModelingPath;
    private Texture2D logoImage;
    private string videoURL;

    private BoothCustomizeData data;

    private bool isLogoLoaded;
    private bool isVideoLoaded;
    private bool isObjectLoaded;

    private string OwnerUID;

    private void Start()
    {
        renderBoothData = GetComponent<RenderBoothData>();
        OwnerUID = (string)photonView.Owner.CustomProperties["id"];
        LoadBoothCustomizeData(OwnerUID);
    }

    void LoadBoothCustomizeData(string uid)
    {
        DatabaseManager.Instance.GetDataFrom<BoothCustomizeData>(uid, OnLoadBoothCustomizeData);
        
    }

    void OnLoadBoothCustomizeData(BoothCustomizeData data)
    {
        this.data = data;

        if (!string.IsNullOrEmpty(data.modelingPath))
            DatabaseManager.Instance.DownloadObjectFrom(OwnerUID, data.modelingPath, OnLoadBoothModelingData);
        else
            isObjectLoaded = true;

        if (!string.IsNullOrEmpty(data.logoImagePath))
            DatabaseManager.Instance.DownloadImageFrom(OwnerUID, data.logoImagePath, OnLoadLogoImageData);
        else
            isLogoLoaded = true;

        if (!string.IsNullOrEmpty(data.videoURL))
            DatabaseManager.Instance.DownLoadVideoFrom(OwnerUID, data.videoURL, OnLoadVideoData);
        else
            isVideoLoaded = true;

        CheckAllDataLoaded();
    }

    void OnLoadBoothModelingData(string path)
    {
        isObjectLoaded = true;
        boothModelingPath = path;
        CheckAllDataLoaded();
    }

    void OnLoadLogoImageData(Texture2D texture)
    {
        isLogoLoaded = true;
        logoImage = texture;
        CheckAllDataLoaded();
    }

    void OnLoadVideoData(string url)
    {
        isVideoLoaded = true;
        videoURL = url;
        CheckAllDataLoaded();
    }

    void CheckAllDataLoaded()
    {
        if(isObjectLoaded && isVideoLoaded && isLogoLoaded)
        {
            BoothExtraData extraData = GetBoothExtraData(data);

            renderBoothData.RenderBoothDataWith(extraData);
            renderBoothData.RenderBoothModeling(extraData);
        }
    }
    BoothExtraData GetBoothExtraData(BoothCustomizeData data)
    {
        BoothExtraData extraData = new BoothExtraData();
        extraData.boothType = data.boothType;
        extraData.color = data.color.GetColor();
        extraData.logoImage = logoImage;
        extraData.modelingScale = data.modelingScale;
        extraData.modelingPath = boothModelingPath;
        extraData.videoURL = videoURL;
        return extraData;
    }
}
