using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : Singleton<DatabaseManager>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public void SaveData<T>(T data)
    {
        string saveData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(data.ToString(), saveData);
        print(data + " : " + saveData);
        PlayerPrefs.Save();
    }

    public T GetData<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string data = PlayerPrefs.GetString(key);
            print(key + " : " + data);
            return (T)JsonUtility.FromJson<T>(data);
        }
        return default(T);
    }


}
