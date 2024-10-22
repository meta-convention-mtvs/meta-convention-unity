using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject cardInfoUI;
    public GameObject infoTextUI;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    Stack<GameObject> uiStack;

    private void Start()
    {
        uiStack = new Stack<GameObject>();
    }

    private void Update()
    {
        // 만약에 눌린 키가 esc라면
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideUI();
        }
    }
    public void ShowUI(GameObject UI)
    {
        UI.SetActive(true);
        uiStack.Push(UI);
    }

    public void HideUI()
    {
        GameObject uiToHide = uiStack.Pop();
        if (uiToHide != null)
            uiToHide.SetActive(false);
    }
}
