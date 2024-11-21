using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MewtwoEX : MonoBehaviour
{
    GameObject[] loadedPlayerList;

    private void Start()
    {
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");
        UpdateData(playerList);
    }

    public async Task UpdateData(GameObject[] newPlayerList)
    {
        if (loadedPlayerList != null)
            await psycowave(newPlayerList.Except(loadedPlayerList).ToArray());
        else
            await psycowave(newPlayerList);
        loadedPlayerList = newPlayerList;
    }

    private async Task psycowave(GameObject[] playerList)
    {
        List<UID> uidList = GetUidCompontentsIn(playerList);

        var characterTopBottomCustomizeDatas = await LoadAllDatasWithUID<CharacterTopBottomCustomizeData>(uidList);

        var dictonary = uidList.Zip(characterTopBottomCustomizeDatas, (key, value) => new { key, value }).ToDictionary(pair => pair.key, pair => pair.value);

        foreach(UID uidComponent in dictonary.Keys)
        {
            CharacterTopBottomCustomizeData data = dictonary[uidComponent];
            ModelingRuntimeCreate runtimeCreate = uidComponent.gameObject.GetComponent<ModelingRuntimeCreate>();
            runtimeCreate.CreateAvatar(data);
            if (data.isCustomTop)
            {
                var url = await GetImageDownloadUrl(uidComponent.uid, data.customImageFileName);
                var texture = await GetTextureFromDatabase(url.ToString());
                runtimeCreate.OnLoadTexture(texture);
            }
            
        }
    }


    async Task<T[]> LoadAllDatasWithUID<T>(List<UID> uidList) where T:class
    {
        var databaseTasks = new Task<T>[uidList.Count];
        for (int i = 0; i < uidList.Count; i++)
        {
            databaseTasks[i] = GetDataFromDatabase<T>(uidList[i].uid);
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

    async Task<T> GetDataFromDatabase<T>(string uid) where T : class
    {
        string path = "USER/" + uid + "/" + "Data/" + typeof(T).ToString();

        Task<DocumentSnapshot> task = FirebaseFirestore.DefaultInstance.Document(path).GetSnapshotAsync();
        await task;

        if (task.Exception == null)
        {
            print("회원 정보 불러오기 성공!");
            T loadInfo = task.Result.ConvertTo<T>();
            return loadInfo;
        }
        else
        {
            print("Exception in Database loading: " + typeof(T) + " " + task.Exception);
            return null;
        }
    }

    async Task<Uri> GetImageDownloadUrl(string uid, string imageFileName)
    {
        var storageRef = FirebaseStorage.DefaultInstance.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("images/" + uid + "/" + imageFileName);

        Task<Uri> task = fileRef.GetDownloadUrlAsync();
        await task;

        if(task.Exception == null)
        {
            print("이미지 url 불러오기 성공");
            return task.Result;
        }
        else
        {
            print("Exception in Image Url loading: " + uid + ", " + imageFileName + " " + task.Exception);
            return null;
        }
    }

    async Task<Texture2D> GetTextureFromDatabase(string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        // 비동기 요청 보내기
        var operation = request.SendWebRequest();

        // 요청이 완료될 때까지 대기
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        // 요청 결과 확인
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error: {request.error}");
            return null;
        }

        return ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}
