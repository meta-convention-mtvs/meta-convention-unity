using Firebase.Firestore;
using Ricimi;
using System.IO;
using System.Linq;
using UnityEngine;

public class CharacterCustomizeManager : MonoBehaviour
{

    public CharacterTemplet CharacterPrefabs;
    public UICharacterMaker ui_cm;
    public CharacterCustomizingCameraMove cameraMove;
    public Customization_GenderSelection customization_GenderSelection;

    // Debug mode
    public SkinnedMeshRenderer customTShirts;

    private GameObject[,] instantiatedMaleCharacter;
    private GameObject[,] instantiatedFemaleCharacter;

    private GameObject currentShowObject;
    private int topClothesIndex;
    private int bottomClothesIndex;
    private bool isMan = true;
    private bool isIndexChange;
    // Top Clothes의 마지막 번호는 커스텀 티를 위한 것이다.
    private int totalTopClothes;
    private int totalBottomClothes;
    private bool isImageUploaded;
    private string imageFilePath;

    private void Awake()
    {
        totalTopClothes = CharacterPrefabs.maleCharacterPrefabs.Count;
        totalBottomClothes = CharacterPrefabs.maleCharacterPrefabs[0].column.Length;
        instantiatedMaleCharacter = new GameObject[totalTopClothes, totalBottomClothes];
        instantiatedFemaleCharacter = new GameObject[totalTopClothes, totalBottomClothes];
    }

    private void Start()
    {
        ui_cm.OnTopPrevClick += DecTopIndex;
        ui_cm.OnTopNextClick += IncTopIndex;
        ui_cm.OnBottomPrevClick += DecBottomIndex;
        ui_cm.OnBottomNextClick += IncBottomIndex;
        ui_cm.OnGenderClick += ChangeGender;
        ui_cm.OnSaveClick += SaveData;
        LoadPrefabs();
        ShowCharacter();
    }

    private void Update()
    {
        if (isIndexChange)
        {
            ShowCharacter();
            isIndexChange = false;
        }
    }

    void LoadPrefabs()
    {
        for (int i = 0; i < totalTopClothes; i++)
        {
            for (int j = 0; j < totalBottomClothes; j++)
            {
                GameObject go = Instantiate(CharacterPrefabs.maleCharacterPrefabs[i].column[j], transform);
                go.SetActive(false);
                instantiatedMaleCharacter[i, j] = go;
            }
        }

        for (int i = 0; i < totalTopClothes; i++)
        {
            for (int j = 0; j < totalBottomClothes; j++)
            {
                GameObject go = Instantiate(CharacterPrefabs.femaleCharacterPrefabs[i].column[j], transform);
                go.SetActive(false);
                instantiatedFemaleCharacter[i, j] = go;
            }
        }
    }

    GameObject GetCharacterInIndex(int topIndex, int bottomIndex, bool isMan)
    {
        if (isMan)
        {
            return instantiatedMaleCharacter[topIndex, bottomIndex];
        }
        else
        {
            return instantiatedFemaleCharacter[topIndex, bottomIndex];
        }
    }

    void ShowCharacter()
    {
        if (currentShowObject != null)
            currentShowObject.SetActive(false);
        currentShowObject = GetCharacterInIndex(topClothesIndex, bottomClothesIndex, isMan);
        if(topClothesIndex == totalTopClothes - 1)
        {
            ChangeClothes(currentShowObject, customTShirts);
        }
        currentShowObject.SetActive(true);
    }

    public void UploadTshirtsImage()
    {
        // image texture 를 읽어온다.
        // 읽어온 texture를 적용시킨다.
        FileUploadManager.Instance.SetUpImageFileBrowser();
        FileUploadManager.Instance.ShowDialog(ImageFileLoad);
    }

    void ImageFileLoad(string[] paths)
    {
        //image 파일이 올라감
        string path = paths[0];
        imageFilePath = path;

        // texture 불러옴
        Texture2D texture = ImageUtility.LoadTexture(path);
        texture.wrapMode = TextureWrapMode.Clamp;
        customTShirts.materials[0].mainTexture = texture;

        // 이미지 파일 올라갔다.
        isImageUploaded = true;

        // 렌더링을 다시 해주기
        isIndexChange = true;
        topClothesIndex = totalTopClothes - 1;

        // UI에서 커스텀 티셔츠로 세팅하기
        customization_GenderSelection.SetCustomTshirts();

    }

    void ChangeClothes(GameObject player, SkinnedMeshRenderer newClothes)
    {
        var originalClothes = player.GetComponentsInChildren<SkinnedMeshRenderer>().Where(go => go != null && go.gameObject.name.Contains("top"));

        GameObject go = new GameObject();
        go.transform.SetParent(player.transform);
        SkinnedMeshRenderer mesh = go.AddComponent<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer clothes in originalClothes)
        {
            mesh.rootBone = clothes.rootBone;
            mesh.bones = clothes.bones;
            mesh.localBounds = clothes.localBounds;
            mesh.sharedMesh = newClothes.sharedMesh;
            mesh.sharedMaterials = newClothes.sharedMaterials;

            clothes.gameObject.SetActive(false);
        }
    }

    #region 상하의 인덱스 조절 함수
    void IncTopIndex()
    {
        topClothesIndex++;
        if (isImageUploaded)
            topClothesIndex %= totalTopClothes;
        else
            topClothesIndex %= totalTopClothes - 1;
        isIndexChange = true;
        cameraMove.SetTopCamera();
    }

    void DecTopIndex()
    {
        topClothesIndex--;
        if (topClothesIndex < 0) {
            if (isImageUploaded)
                topClothesIndex = (totalTopClothes - 1);
            else
                topClothesIndex = (totalTopClothes - 2);
        }
        isIndexChange = true;
        cameraMove.SetTopCamera();
    }
    void IncBottomIndex()
    {
        bottomClothesIndex++;
        bottomClothesIndex %= totalBottomClothes;
        isIndexChange = true;
        cameraMove.SetBottomCamera();
    }
    void DecBottomIndex()
    {
        bottomClothesIndex--;
        if(bottomClothesIndex < 0)
            bottomClothesIndex = (totalBottomClothes - 1);
        isIndexChange = true;
        cameraMove.SetBottomCamera();
    }
    #endregion
    void ChangeGender()
    {
        isMan = !isMan;
        isIndexChange = true;
        cameraMove.SetMainCamera();
    }

    void SaveData()
    {
        CharacterTopBottomCustomizeData data = new CharacterTopBottomCustomizeData();
        data.isMan = isMan;
        data.topIndex = topClothesIndex;
        data.bottomIndex = bottomClothesIndex;
        data.isCustomTop = topClothesIndex == totalTopClothes - 1 ? true : false;
        data.customImageFileName = Path.GetFileName(imageFilePath);
        DatabaseManager.Instance.SaveData<CharacterTopBottomCustomizeData>(data);
        if(data.isCustomTop)
            DatabaseManager.Instance.UploadImage(imageFilePath);
    }
}

[FirestoreData]
public class CharacterTopBottomCustomizeData
{
    [FirestoreProperty]
    public bool isMan { get; set; }
    [FirestoreProperty]
    public int topIndex { get; set; }
    [FirestoreProperty]
    public int bottomIndex { get; set; }
    [FirestoreProperty]
    public bool isCustomTop { get; set; }
    [FirestoreProperty]
    public string customImageFileName { get; set; }

    public static CharacterTopBottomCustomizeData GetRandomCharacterData()
    {
        CharacterTopBottomCustomizeData newData = new CharacterTopBottomCustomizeData();

        newData.isMan = Random.Range(0, 2) == 0;
        newData.topIndex = Random.Range(0, 4);
        newData.bottomIndex = Random.Range(0, 4);
        return newData;
    }

}
