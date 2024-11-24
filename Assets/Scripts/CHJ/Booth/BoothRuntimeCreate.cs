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
    private Texture2D bannerImage;
    private Texture2D brochureImage;
    private string videoURL;

    private BoothCustomizeData data;

    private bool isLogoLoaded;
    private bool isVideoLoaded;
    private bool isObjectLoaded;
    private bool isBannerLoaded;
    private bool isBrochureLoaded;

    private UID ownerUID;

    private void Awake()
    {
        renderBoothData = GetComponent<RenderBoothData>();
        ownerUID = GetComponent<UID>();

        ownerUID.OnUUIDChanged += LoadBoothCustomizeData;
    }

    void LoadBoothCustomizeData(string uid)
    {
        DatabaseManager.Instance.GetDataFrom<BoothCustomizeData>(uid, OnLoadBoothCustomizeData);
    }

    void OnLoadBoothCustomizeData(BoothCustomizeData data)
    {
        this.data = data;

        if (!string.IsNullOrEmpty(data.modelingPath))
            DatabaseManager.Instance.DownloadObjectFrom(ownerUID.uuid, data.modelingPath, OnLoadBoothModelingData);
        else
            isObjectLoaded = true;

        if (!string.IsNullOrEmpty(data.logoImagePath))
            DatabaseManager.Instance.DownloadLogoFrom(ownerUID.uuid, data.logoImagePath, OnLoadLogoImageData);
        else
            isLogoLoaded = true;

        if (!string.IsNullOrEmpty(data.videoURL))
            DatabaseManager.Instance.DownLoadVideoFrom(ownerUID.uuid, data.videoURL, OnLoadVideoData);
        else
            isVideoLoaded = true;

        if (!string.IsNullOrEmpty(data.bannerImagePath))
            DatabaseManager.Instance.DownloadBannerFrom(ownerUID.uuid, data.bannerImagePath, OnLoadBannerImageData);
        else
            isBannerLoaded = true;

        if (!string.IsNullOrEmpty(data.brochureImagePath))
            DatabaseManager.Instance.DownloadBrochureFrom(ownerUID.uuid, data.brochureImagePath, OnLoadBrochureImageData);
        else
            isBrochureLoaded = true;

        CheckAllDataLoaded();
    }
    
    void OnLoadBannerImageData(Texture2D texture)
    {
        isBannerLoaded = true;
        bannerImage = texture;
        CheckAllDataLoaded();
    }

    void OnLoadBrochureImageData(Texture2D texture)
    {
        isBrochureLoaded = true;
        brochureImage = texture;
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
        print("호출됨: Check All Data Loaded");
        if(isObjectLoaded && isVideoLoaded && isLogoLoaded && isBannerLoaded && isBrochureLoaded)
        {
            print("호출됨: RenderBoothData");
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
        extraData.hasBanner = data.hasBanner;
        extraData.bannerImage = bannerImage;
        extraData.hasBrochure = data.hasBrochure;
        extraData.brochureImage = brochureImage;
        extraData.homepageLink = data.homepageLink;
        return extraData;
    }
}
