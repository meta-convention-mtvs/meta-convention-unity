using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeManager : MonoBehaviour
{
    [Header("��ũ��Ʈ�� ����ϴ� ��ü")]
    public string scriptInfo = "����ĳ����";
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
            // �� ���� Ȱ��ȭ�Ǿ��� ������Ʈ ��Ȱ��ȭ
            curObj.SetActive(false);
        }
        curObj = instanceObjects[customizingIdx];
        curObj.SetActive(true);
    }
}
