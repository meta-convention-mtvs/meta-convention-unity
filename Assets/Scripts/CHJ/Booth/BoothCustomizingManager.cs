using Firebase.Firestore;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
using UnityEngine;

public class BoothCustomizingManager : MonoBehaviour
{
    public GameObject[] boothPrefabs;
    //public UIBoothCustomzing ui_bc;
    public ColorPicker colorPicker;
    [Space]
    public BoothCustomizeData boothCustomizeData;

    private bool boothDataChanged;
    private bool boothTypeChanged;
    private bool boothObjectModelingChanged;

    private int currentIndex;
    private List<GameObject> instantiatedObject;

    // Obj파일을 불러오면 바로 인스턴스화됩니다. 새로운 obj 파일을 불러오면 이전에 불러왔던 파일을 없애주고 불러와야 합니다.
    private GameObject currentInstantiatedObject;

    private void Start()
    {
        boothCustomizeData = new BoothCustomizeData();
        // 1로 초기화시킨다.
        boothCustomizeData.modelingScale = 1;
        boothCustomizeData.color = new FireStoreColor(0, 0, 0);
        LoadGamePrefab(boothPrefabs);
    }

    public void SetUIDefaultSetting(UIBoothDefaultSetting defaultSetting)
    {
        defaultSetting.OnDropdownChanged += SetBoothCategory;
        defaultSetting.OnBoothButtonClick[0] += () => SetBoothType(0);
        defaultSetting.OnBoothButtonClick[1] += () => SetBoothType((BoothType)1);
        defaultSetting.OnBoothButtonClick[2] += () => SetBoothType((BoothType)2);
        defaultSetting.OnImportBoothModelingClick += ShowBoothFileUploader;
        defaultSetting.OnCompanyNameChanged += SetCompanyName;
        defaultSetting.OnLogoButtonClick += ShowImageFileUploader;
        defaultSetting.OnHomepageNameChanged += SetHomepageLink;
        defaultSetting.OnColorButtonClick += ShowColorPicker;
    }

    public void SetUIDisplay(UIBoothDisplay display)
    {
        display.OnObjectButtonClick += ShowModelingFileUploder;
        display.OnVideoButtonClick += ShowVideoFileUploder;
        display.OnObjectSliderChanged += SetModelingScale;
        display.OnBannerButtonClick += SetBanner;
        display.OnBannerImageButtonClick += SetBannerImage;
        display.OnBrochureButtonClick += SetBrochure;
        display.OnBrochureImageButtonCilck += SetBrochureImage;
    }
    private void Update()
    {
        if (boothTypeChanged)
        {
            currentIndex = (int)boothCustomizeData.boothType - 1;
            if(currentIndex == -1)
            {
                instantiatedObject[0].SetActive(false);
                instantiatedObject[1].SetActive(false);
                if (File.Exists(boothCustomizeData.boothObjectPath))
                {
                    if (currentInstantiatedObject != null)
                    {
                        Destroy(currentInstantiatedObject);
                    }
                    //currentInstantiatedObject = ObjectLoader.ImportObj(boothCustomizeData.boothObjectPath);
                    ObjectLoader.StartImporting(boothCustomizeData.boothObjectPath, OnBoothObjectLoad);
                }

            }
            else
            {
                if (currentInstantiatedObject != null)
                {
                    Destroy(currentInstantiatedObject);
                }
                instantiatedObject[currentIndex].SetActive(true);
                instantiatedObject[1 - currentIndex].SetActive(false);
                instantiatedObject[currentIndex].GetComponent<RenderBoothData>().RenderBoothDataWith(GetBoothExtraData(boothCustomizeData));
            }
            boothTypeChanged = false;
        }
        if (boothDataChanged)
        {
            if(currentIndex > -1)
                instantiatedObject[currentIndex].GetComponent<RenderBoothData>().RenderBoothDataWith(GetBoothExtraData(boothCustomizeData));
            boothDataChanged = false;
        }

        if(boothObjectModelingChanged)
        {
            if (currentIndex > -1)
                instantiatedObject[currentIndex].GetComponent<RenderBoothData>().RenderBoothModeling(GetBoothExtraData(boothCustomizeData));
            boothObjectModelingChanged = false;
        }
    }

    void OnBoothObjectLoad(AssetLoaderContext context)
    {
        currentInstantiatedObject = context.RootGameObject;
    }

    void LoadGamePrefab(GameObject[] prefab)
    {
        instantiatedObject = new List<GameObject>();
        foreach(GameObject obj in prefab)
        {
            GameObject go = Instantiate(obj);
            go.SetActive(false);
            instantiatedObject.Add(go);
        }
    }
    void SetBoothType(BoothType type)
    {
        boothCustomizeData.boothType = type;
        boothTypeChanged = true;
    }

    void SetBoothCategory(int index)
    {
        boothCustomizeData.category = (BoothCategory)index;
    }

    void SetCompanyName(string s)
    {
        boothCustomizeData.companyName = s;
    }

    void SetHomepageLink(string s)
    {
        boothCustomizeData.homepageLink = s;
    }
    void ShowColorPicker()
    {
        colorPicker.ShowColorPicker(SetColor);
    }

    void ShowImageFileUploader()
    {
        FileUploadManager.Instance.SetUpImageFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetLogoImage);
    }

    void ShowModelingFileUploder()
    {
        FileUploadManager.Instance.SetUpObjFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetModeling);
    }

    void ShowBoothFileUploader()
    {
        FileUploadManager.Instance.SetUpObjFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetBoothObject);
    }

    void ShowVideoFileUploder()
    {
        FileUploadManager.Instance.SetUpVideoFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetVideoClip);
    }

    void SetBanner()
    {
        boothCustomizeData.hasBanner = !boothCustomizeData.hasBanner;
        boothDataChanged = true;
    }

    void SetBannerImage()
    {
        FileUploadManager.Instance.SetUpImageFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetBannerImage);
    }

    void SetBrochure()
    {
        boothCustomizeData.hasBrochure = !boothCustomizeData.hasBrochure;
        boothDataChanged = true;
    }

    void SetBrochureImage()
    {
        FileUploadManager.Instance.SetUpImageFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetBrochureImage);
    }

    void SetModelingScale(float size)
    {
        boothCustomizeData.modelingScale = size;
        boothDataChanged = true;
    }
    void SetColor(Vector3 HSV)
    {
        Color color = Color.HSVToRGB(HSV.x, HSV.y, HSV.z);
        boothCustomizeData.color = new FireStoreColor(color.r, color.g, color.b);
        boothDataChanged = true;
    }
    void SetModeling(string[] paths)
    {
        string path = paths[0];
        boothCustomizeData.modelingPath = path;
        boothObjectModelingChanged = true;
    }

    void SetBoothObject(string[] paths)
    {
        string path = paths[0];
        boothCustomizeData.boothObjectPath = path;
        boothTypeChanged = true;
    }
    void SetLogoImage(string[] paths)
    {
        string path = paths[0];
        boothCustomizeData.logoImagePath = path;
        boothDataChanged = true;
    }

    void SetBannerImage(string[] paths)
    {
        string path = paths[0];
        boothCustomizeData.bannerImagePath = path;
        boothDataChanged = true;
    }

    void SetBrochureImage(string[] paths)
    {
        string path = paths[0];
        boothCustomizeData.brochureImagePath = path;
        boothDataChanged = true;
    }

    void SetVideoClip(string[] paths)
    {
        string path = paths[0];
        boothCustomizeData.videoURL = "file://" + path;
        boothDataChanged = true;
    }

    BoothExtraData GetBoothExtraData(BoothCustomizeData data)
    {
        BoothExtraData extraData = new BoothExtraData();
        extraData.boothType = data.boothType;
        extraData.color = data.color.GetColor();
        extraData.logoImage = ImageUtility.LoadTexture(data.logoImagePath);
        extraData.modelingScale = data.modelingScale;
        extraData.modelingPath = data.modelingPath;
        extraData.videoURL = data.videoURL;
        extraData.hasBanner = data.hasBanner;
        extraData.bannerImage = ImageUtility.LoadTexture(data.bannerImagePath);
        extraData.hasBrochure = data.hasBrochure;
        extraData.brochureImage = ImageUtility.LoadTexture(data.brochureImagePath);
        extraData.homepageLink = data.homepageLink;
        return extraData;
    }

    public void SaveBoothData()
    {
        if(boothCustomizeData.boothType == BoothType.Blank && boothCustomizeData.boothObjectPath != null)
        {
            DatabaseManager.Instance.UploadObject(boothCustomizeData.boothObjectPath);
        }

        if(boothCustomizeData.logoImagePath != null)
        {
            DatabaseManager.Instance.UploadImage(boothCustomizeData.logoImagePath);
        }

        if(boothCustomizeData.modelingPath != null)
        {
            DatabaseManager.Instance.UploadObject(boothCustomizeData.modelingPath);
        }

        if(boothCustomizeData.videoURL != null)
        {
            DatabaseManager.Instance.UploadVideo(boothCustomizeData.videoURL.Substring("file://".Length));
        }

        if(boothCustomizeData.bannerImagePath != null)
        {
            DatabaseManager.Instance.UploadImage(boothCustomizeData.bannerImagePath);
        }

        if(boothCustomizeData.brochureImagePath != null)
        {
            DatabaseManager.Instance.UploadImage(boothCustomizeData.brochureImagePath);

        }
        boothCustomizeData.boothObjectPath = Path.GetFileName(boothCustomizeData.boothObjectPath);
        boothCustomizeData.logoImagePath = Path.GetFileName(boothCustomizeData.logoImagePath);
        boothCustomizeData.modelingPath = Path.GetFileName(boothCustomizeData.modelingPath);
        boothCustomizeData.bannerImagePath = Path.GetFileName(boothCustomizeData.bannerImagePath);
        boothCustomizeData.brochureImagePath = Path.GetFileName(boothCustomizeData.brochureImagePath);

        if (boothCustomizeData.videoURL != null && boothCustomizeData.videoURL.StartsWith("file://"))
        {
            boothCustomizeData.videoURL = Path.GetFileName(boothCustomizeData.videoURL.Substring("file://".Length));
        }

        // 데이터 베이스에 저장하는 코드 (수정 필요)
        DatabaseManager.Instance.SaveData<BoothCustomizeData>(boothCustomizeData);

    }

    public enum BoothCustomzieDataSaveError
    {
        EmptyCompanyName,
        EmptyBoothObject
    }
    public bool CanSaveData(ref BoothCustomzieDataSaveError error)
    {
        if (string.IsNullOrEmpty(boothCustomizeData.companyName))
        {
            // error code 1
            error = BoothCustomzieDataSaveError.EmptyCompanyName;
            return false;
        }
        
        if(boothCustomizeData.boothType == BoothType.Blank && boothCustomizeData.boothObjectPath == null)
        {
            error = BoothCustomzieDataSaveError.EmptyBoothObject;
            return false;
        }

        return true;
    }
}

// Firebase에 저장해 놓을 데이터
[FirestoreData]
public class BoothCustomizeData
{
    [FirestoreProperty]
    public BoothCategory category { get; set; }
    [FirestoreProperty]
    public string boothObjectPath { get; set; }
    [FirestoreProperty]
    public string companyName { get; set; }
    [FirestoreProperty]
    public string homepageLink { get; set; }
    [FirestoreProperty]
    public BoothType boothType { get; set; }
    [FirestoreProperty]
    public FireStoreColor color { get; set; }
    [FirestoreProperty]
    public string logoImagePath { get; set; }
    [FirestoreProperty]
    public string modelingPath { get; set; }
    [FirestoreProperty]
    public float modelingScale { get; set; }
    [FirestoreProperty]
    public string videoURL { get; set; }
    [FirestoreProperty]
    public bool hasBanner { get; set; }
    [FirestoreProperty]
    public string bannerImagePath { get; set; }
    [FirestoreProperty]
    public bool hasBrochure { get; set; }
    [FirestoreProperty]
    public string brochureImagePath { get; set; }

    public bool hasLogoImage => string.IsNullOrEmpty(logoImagePath);

    public bool hasBannerImage => string.IsNullOrEmpty(bannerImagePath);

    public bool hasBrochureImage => string.IsNullOrEmpty(brochureImagePath);

    public bool hasVideoUrl => string.IsNullOrEmpty(videoURL);

    public bool hasModelingPath => string.IsNullOrEmpty(modelingPath);
}

public class BoothExtraData
{
    public BoothType boothType;
    public string homepageLink;
    public Color color;
    public Texture2D logoImage;
    public string modelingPath;
    public float modelingScale;
    public string videoURL;
    public bool hasBanner;
    public Texture2D bannerImage;
    public bool hasBrochure;
    public Texture2D brochureImage;

    public BoothExtraData(BoothCustomizeData data)
    {
        this.boothType = data.boothType;
        this.homepageLink = data.homepageLink;
        this.color = data.color.GetColor();
        this.modelingScale = data.modelingScale;
        this.hasBanner = data.hasBanner;
        this.hasBrochure = data.hasBrochure;
    }

    public BoothExtraData()
    {

    }
}

public enum BoothCategory
{
    Mobility,
    DigitalHealth,
    ARXRVR,
    ThreeDPrinting,
    Robotics,
    Technology
}

public enum BoothType
{
    Blank,
    Cubic,
    Round
}

[FirestoreData]
public class FireStoreColor
{
    [FirestoreProperty]
    public float r { get; set; }
    [FirestoreProperty]
    public float g { get; set; }
    [FirestoreProperty]
    public float b { get; set; }

    public FireStoreColor()
    {

    }

    public FireStoreColor(float r, float g, float b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public Color GetColor()
    {
        return new Color(r, g, b);
    }
}