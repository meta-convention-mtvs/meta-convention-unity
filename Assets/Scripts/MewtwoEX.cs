using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using CHJ;
using Photon.Pun;

public class MewtwoEX : MonoBehaviour
{
    [SerializeField]
    public GameObject boothFactory;
    [SerializeField]
    public List<string> companyUuidList;

    public Transform[] boothPositionList;

    [Header("Mobility List")]
    public List<GameObject> mobilityObjects;

    [Header("Electronic List")]
    public List<GameObject> electronicObjects;

    private async void Start()
    {
        print("Mewtwo EX was called" + this.gameObject);

        BoothCategory? category = GetCurrentRoomCategory();
        if (!category.HasValue)
        {
            Debug.Log("Can't find category in boothCategory.");
            return;
        }

        companyUuidList = await GetUUIDListFromDatabase(category.Value);

        GameObject[] boothList = new GameObject[companyUuidList.Count];
        for(int i = 0; i < companyUuidList.Count; i++)
        {
            if(!string.IsNullOrEmpty(companyUuidList[i]))
            {
                GameObject go = Instantiate(boothFactory);
                go.transform.position = boothPositionList[i].transform.position;
                go.transform.rotation = boothPositionList[i].transform.rotation;
                go.GetComponent<UID>().SetUUID(companyUuidList[i]);

                // 미리 깔아놓는 오브젝트
                if (category == BoothCategory.Electronics)
                    go.GetComponent<BoothRuntimeCreate>().SetBoothModelingPrefab(electronicObjects[i]);
                else if(category == BoothCategory.Mobility)
                    go.GetComponent<BoothRuntimeCreate>().SetBoothModelingPrefab(mobilityObjects[i]);

                boothList[i] = go;
                boothPositionList[i].gameObject.SetActive(false);
            }  
        }
        //ApplyBoothDatasFromDatabaseInList(boothList);

        

    }

    public BoothCategory? GetCurrentRoomCategory()
    {
        return EnumUtility.GetEnumValue<BoothCategory>(PhotonNetwork.CurrentRoom.Name);
    }

    //ToDo: 부스 이름에 따라서 읽어오는 데이터를 바꿔야 한다.
    private async Task<List<string>> GetUUIDListFromDatabase(BoothCategory category)
    {
        var boothPositionData =  await AsyncDatabase.GetDataFromDatabase<ChargedBoothPosition>(DatabasePath.GetPublicBoothPositionDataPath(category));
        return boothPositionData.GetUUIDList();
    }

    #region 안씀
    private async Task ApplyAvatarDatasFromDatabaseInList(GameObject[] playerList)
    {
        List<UID> uidList = GetUidCompontentsIn(playerList);

        var characterTopBottomCustomizeDatas = await LoadAllDatasWithUID<CharacterTopBottomCustomizeData>(uidList);

        var dictionary = uidList.Zip(characterTopBottomCustomizeDatas, (key, value) => new { key, value }).ToDictionary(pair => pair.key, pair => pair.value);

        var tasks = dictionary.Keys.Select(uid => CreateAvatarsWithCharacterCustomizeDataAsync(uid, dictionary[uid]));
        var results = await Task.WhenAll(tasks);

        // 성공 여부 집계
        int successCount = results.Count(r => r);
        int failureCount = results.Length - successCount;

        Debug.Log($"성공: {successCount}, 실패: {failureCount}");

    }

    private async Task<bool> CreateAvatarsWithCharacterCustomizeDataAsync(UID uidComponent, CharacterTopBottomCustomizeData data)
    {
        try
        {
            var runtimeCreate = uidComponent.gameObject.GetComponent<RenderAvatarData>();
            runtimeCreate.CreateAvatar(data);

            if (data.isCustomTop)
            {
                var texture = await AsyncDatabase.GetImageFromDatabaseWithUid(uidComponent.uid, data.customImageFileName);
                runtimeCreate.OnLoadTexture(texture);
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"UID {uidComponent.uid}에서 아바타 생성 중 오류 발생: {ex.Message}");
            return false;
        }
    }
    #endregion

    private async Task ApplyBoothDatasFromDatabaseInList(GameObject[] boothList)
    {
        print("0");
        List<UID> uidList = GetUidCompontentsIn(boothList);

        print("0.1");
        BoothCustomizeData[] boothCustomizeDatas = await LoadAllDatasWithUUID<BoothCustomizeData>(uidList);

        print("1");

        var boothExtraDataLoadingTasks = uidList.Zip(boothCustomizeDatas, (uid, data) => BoothExtraData.LoadBoothExtraDataInDatabase(uid, data));

        BoothExtraData[] boothExtraDatas = await Task.WhenAll(boothExtraDataLoadingTasks);

        print("2");

        var successCount = uidList.Zip(boothExtraDatas, (uid, data) => RenderBoothDataWithExtraData(uid.GetComponent<RenderBoothData>(), data)).Count(r => r);

        uidList.Zip(boothExtraDatas, (uid, data) => RenderEmployeeWithExtraData(uid.GetComponent<CreateAIEmployee>(), data)).Count(r => r);


        print("3");
        Debug.Log(successCount);
        
    }



    private bool RenderBoothDataWithExtraData(RenderBoothData renderBoothData, BoothExtraData data)
    {
        renderBoothData.RenderBoothDataWith(data);
        renderBoothData.RenderBoothModeling(data);
        return true;
    }

    private bool RenderEmployeeWithExtraData(CreateAIEmployee createAIEmployee, BoothExtraData data)
    {
        Debug.Log("Hello");
        createAIEmployee.RenderAiEmployee(data.logoImage);
        return true;
    }

    async Task<T[]> LoadAllDatasWithUID<T>(List<UID> uidList) where T:class
    {
        var databaseTasks = new Task<T>[uidList.Count];
        for (int i = 0; i < uidList.Count; i++)
        {
            databaseTasks[i] = AsyncDatabase.GetDataFromDatabase<T>(DatabasePath.GetUserDataPath(uidList[i].uid, typeof(T).ToString()));
        }

        var datas = await Task.WhenAll(databaseTasks);

        return datas;
    }

    async Task<T[]> LoadAllDatasWithUUID<T>(List<UID> uidList) where T : class
    {
        var databaseTasks = new Task<T>[uidList.Count];
        for (int i = 0; i < uidList.Count; i++)
        {
            databaseTasks[i] = AsyncDatabase.GetDataFromDatabase<T>(DatabasePath.GetCompanyDataPath(uidList[i].uuid, typeof(T).ToString()));
        }

        var datas = await Task.WhenAll(databaseTasks);

        return datas;
    }

    List<UID> GetUidCompontentsIn(GameObject[] objectList)
    {
        var newUidList = new List<UID>();
        foreach(GameObject obj in objectList)
        {
            var uidComponent = obj.GetComponent<UID>();
            if (uidComponent != null)
                newUidList.Add(uidComponent);
        }
        return newUidList;
    }
}
