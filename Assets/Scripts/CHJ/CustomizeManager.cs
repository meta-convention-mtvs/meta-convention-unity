using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{
    [Header("스크립트가 담당하는 객체")]
    public string scriptInfo = "남자캐릭터";
    [Space]
    public CustomizeTemplet myTemplet;

    List<GameObject> instanceObjects = new List<GameObject>();
    int customizingIdx;
    GameObject curObj;

    void Start()
    {
        LoadObject();
    }

    void LoadObject()
    {
        foreach(GameObject obj in myTemplet.gamePrefabs)
        {
            GameObject go = Instantiate(obj, this.gameObject.transform);
            go.SetActive(false);
            instanceObjects.Add(go);
        }
    }

    public void SetCustomizingIdx(int idx)
    {
        if (idx >= instanceObjects.Count || idx < 0)
            return;

        customizingIdx = idx;
        ChangeShowObject();
    }

    public int GetCustomizingIdx()
    {
        return customizingIdx;
    }
    public void IncCustomizingIdx()
    {
        customizingIdx++;
        customizingIdx %= instanceObjects.Count;
        ChangeShowObject();
    }

    public void DecCustomizingIdx()
    {
        customizingIdx--;
        if (customizingIdx < 0)
            customizingIdx = instanceObjects.Count - 1;
        ChangeShowObject();
    }

    public void HideObject()
    {
        if (curObj != null)
        {
            curObj.SetActive(false);
            curObj = null;
        }
    }

    void ChangeShowObject()
    {
        if (curObj != null)
        {
            // 그 전에 활성화되었던 오브젝트 비활성화
            curObj.SetActive(false);
        }
        curObj = instanceObjects[customizingIdx];
        curObj.SetActive(true);
    }
}
