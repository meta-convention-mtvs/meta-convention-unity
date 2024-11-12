using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

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
                    currentInstantiatedObject = ObjectLoader.ImportObj(boothCustomizeData.boothObjectPath);
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

    // to do: booth도 올릴 수 있어야 한다.
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

    void SetModelingScale(float size)
    {
        boothCustomizeData.modelingScale = size;
        boothDataChanged = true;
    }
    void SetColor(Vector3 HSV)
    {
        boothCustomizeData.color = new FireStoreColor(HSV.x, HSV.y, HSV.z);
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

        boothCustomizeData.boothObjectPath = Path.GetFileName(boothCustomizeData.boothObjectPath);
        boothCustomizeData.logoImagePath = Path.GetFileName(boothCustomizeData.logoImagePath);
        boothCustomizeData.modelingPath = Path.GetFileName(boothCustomizeData.modelingPath);
        if (boothCustomizeData.videoURL != null && boothCustomizeData.videoURL.StartsWith("file://"))
        {
            boothCustomizeData.videoURL = Path.GetFileName(boothCustomizeData.videoURL.Substring("file://".Length));
        }
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
}

public class BoothExtraData
{
    public BoothType boothType;
    public Color color;
    public Texture2D logoImage;
    public string modelingPath;
    public float modelingScale;
    public string videoURL;
}

public enum BoothCategory
{
    Mobility,
    DigitalHealth,
    ARXRVR,
    ThreeDPrinting,
    Lifestyle,
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