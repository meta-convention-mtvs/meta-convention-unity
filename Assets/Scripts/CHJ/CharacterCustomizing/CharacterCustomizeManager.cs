using Firebase.Firestore;
using System.Linq;
using UnityEngine;

public class CharacterCustomizeManager : MonoBehaviour
{

    public CharacterTemplet CharacterPrefabs;
    public UICharacterMaker ui_cm;
    public CharacterCustomizingCameraMove cameraMove;

    // Debug mode
    public SkinnedMeshRenderer customTShirts;

    private GameObject[,] instantiatedMaleCharacter;
    private GameObject[,] instantiatedFemaleCharacter;

    private GameObject currentShowObject;
    private int topClothesIndex;
    private int bottomClothesIndex;
    private bool isMan = true;
    private bool isIndexChange;
    private int totalTopClothes;
    private int totalBottomClothes;

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
        string path = paths[0];

        Texture2D texture = ImageUtility.LoadTexture(path);
        customTShirts.materials[1].mainTexture = texture;

        if (isMan)
        {
            var topGameObjects = instantiatedMaleCharacter[topClothesIndex, bottomClothesIndex].GetComponentsInChildren<SkinnedMeshRenderer>().Where(go => go != null && go.gameObject.name.Contains("top"));

            foreach(var go in topGameObjects)
            {
                ChangeClothes(instantiatedMaleCharacter[topClothesIndex, bottomClothesIndex], go, customTShirts);
            }
        }
        
    }
    void ChangeClothes(GameObject player, SkinnedMeshRenderer originalClothes, SkinnedMeshRenderer newClothes)
    {
        GameObject go = new GameObject();
        go.transform.SetParent(player.transform);

        SkinnedMeshRenderer mesh = go.AddComponent<SkinnedMeshRenderer>();
        mesh.rootBone = originalClothes.rootBone;
        mesh.bones = originalClothes.bones;
        mesh.localBounds = originalClothes.localBounds;
        mesh.sharedMesh = newClothes.sharedMesh;
        mesh.sharedMaterials = newClothes.sharedMaterials;

        originalClothes.gameObject.SetActive(false);
    }

    #region 상하의 인덱스 조절 함수
    void IncTopIndex()
    {
        topClothesIndex++;
        topClothesIndex %= totalTopClothes;
        isIndexChange = true;
        cameraMove.SetTopCamera();
    }

    void DecTopIndex()
    {
        topClothesIndex--;
        if(topClothesIndex < 0)
            topClothesIndex = (totalTopClothes - 1);
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

        DatabaseManager.Instance.SaveData<CharacterTopBottomCustomizeData>(data);
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

    public CharacterTopBottomCustomizeData()
    {

    }

    public CharacterTopBottomCustomizeData(bool isMan, int topIndex, int bottomIndex)
    {
        this.isMan = isMan;
        this.topIndex = topIndex;
        this.bottomIndex = bottomIndex;
    }
}
