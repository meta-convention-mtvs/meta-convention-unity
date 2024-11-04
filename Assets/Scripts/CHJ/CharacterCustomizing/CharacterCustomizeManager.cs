using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizeManager : MonoBehaviour
{

    public CharacterTemplet CharacterPrefabs;
    public UICharacterMaker ui_cm;
    public CharacterCustomizingCameraMove cameraMove;

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
