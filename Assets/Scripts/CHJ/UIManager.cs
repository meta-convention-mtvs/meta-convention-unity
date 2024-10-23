using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UI
{
    public GameObject UIObject;
    public UIType uiType;

    public UI(GameObject obj, UIType type)
    {
        UIObject = obj;
        uiType = type;
    }
}

public enum UIType
{
    OptionPopUp,
    Option,
    Normal,
    Memo,
    HUD
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public float UIAnimationTime = 1.0f;

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

    Stack<UI> uiStack;

    private void Start()
    {
        uiStack = new Stack<UI>();
    }

    private void Update()
    {
        GetEscapeInput();
    }
    
    void GetEscapeInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(uiStack.Count > 0)
            {
                HideUI();
            }
        }
    }
    public void ShowUI(GameObject uiObject, UIType uiType)
    {
        UI ui = new UI(uiObject, uiType);
        if(uiStack.Count > 0)
        {
            UI previousUI = uiStack.Peek();
            if (HasHigherOrder(previousUI, ui))
            {
                switch (uiType) {
                    case UIType.Normal:
                        PlayNoramlUIAnimation(ui, UIAnimationTime, true);
                        break;
                }
                uiStack.Push(ui);
            }
        }
        else
        {
            switch (uiType)
            {
                case UIType.Normal:
                    PlayNoramlUIAnimation(ui, UIAnimationTime, true);
                    break;
            }
            uiStack.Push(ui);
        }

        
    }
    void HideUI()
    {
        UI ui = uiStack.Pop();
        if (ui != null)
        {
            switch (ui.uiType)
            {
                case UIType.Normal:
                    PlayNoramlUIAnimation(ui, UIAnimationTime, false);
                    break;
            }
        }
    }

    void PlayNoramlUIAnimation(UI ui, float time, bool isSetActive)
    {
        CanvasGroup canvas = ui.UIObject.GetComponent<CanvasGroup>();
        float startAlpha, endAlpha;
        if (isSetActive)
        {
            startAlpha = 0;
            endAlpha = 1;
        }
        else
        {
            startAlpha = 1;
            endAlpha = 0;
        }
        if (canvas != null)
        {
            canvas.alpha = startAlpha;
            canvas.DOFade(endAlpha, time).OnComplete(()=> canvas.blocksRaycasts = isSetActive);
        }
    }

    bool HasHigherOrder(UI prevUI, UI currUI)
    {
        if ((int)prevUI.uiType > (int)currUI.uiType)
            return true;
        return false;
    }
}
