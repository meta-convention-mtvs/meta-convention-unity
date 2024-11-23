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

public class MewtwoEX : MonoBehaviour
{
    public GameObject boothFactory;
    public string[] companyUuidList;
    public Transform[] boothPositionList;

    private void Start()
    {
        //GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        //psycowave(playerList);

        GameObject[] boothList = new GameObject[companyUuidList.Length];
        for(int i = 0; i < companyUuidList.Length; i++)
        {
            GameObject go = Instantiate(boothFactory, boothPositionList[i]);
            go.GetComponent<UID>().SetUUID(companyUuidList[i]);
            boothList[i] = go;
        }
        ApplyBoothDatasFromDatabaseInList(boothList);

    }

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
                var texture = await AsyncDatabase.GetTextureFromDatabaseWithUid(uidComponent.uid, data.customImageFileName);
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

    private async Task ApplyBoothDatasFromDatabaseInList(GameObject[] boothList)
    {
        List<UID> uidList = GetUidCompontentsIn(boothList);

        BoothCustomizeData[] boothCustomizeDatas = await LoadAllDatasWithUUID<BoothCustomizeData>(uidList);

        var boothExtraDataLoadingTasks = uidList.Zip(boothCustomizeDatas, (uid, data) => BoothExtraData.LoadBoothExtraDataInDatabase(uid, data));

        BoothExtraData[] boothExtraDatas = await Task.WhenAll(boothExtraDataLoadingTasks);

        var successCount = uidList.Zip(boothExtraDatas, (uid, data) => RenderBoothDataWithExtraData(uid.GetComponent<RenderBoothData>(), data)).Count(r => r);

        uidList.Zip(boothExtraDatas, (uid, data) => RenderEmployeeWithExtraData(uid.GetComponent<CreateAIEmployee>(), data)).Count(r => r);
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
