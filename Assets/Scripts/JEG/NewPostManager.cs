using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class NewPostManager : MonoBehaviour
{
    public string url;



    public void PostNewCompanyData()
    {
        StartCoroutine(IPostNewCompanyData());
    }

    IEnumerator IPostNewCompanyData()
    {
        string jsonData = JsonConvert.SerializeObject("", Formatting.None);
        using(UnityWebRequest www = UnityWebRequest.PostWwwForm(url, ""))
        {
            www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-type", "application/json");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                print(" connect is success!");    
            }else
            {
                Debug.LogError(www.error);
            }
        }
    }
}
