using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoothCustomizingManager : MonoBehaviour
{
    public CustomizeTemplet myTemplet;

    private List<GameObject> instanceObjects = new List<GameObject>();
    private GameObject curObj;
    private int customizingIdx;

    void Start()
    {
        LoadObject();
    }

    void LoadObject()
    {
        foreach (GameObject obj in myTemplet.gamePrefabs)
        {
            GameObject go = Instantiate(obj, this.gameObject.transform);
            go.SetActive(false);
            instanceObjects.Add(go);
        }
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
