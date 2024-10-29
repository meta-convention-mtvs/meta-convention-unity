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
    Conversation,
    Memo,
    HUD
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public float UIAnimationTime = 1.0f;

    public GameObject popupUiFactory;

    public GameObject yesNoPopupUIFactory;

    public Canvas popupCanvas;

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
                HideUIInStack();
            }
        }
    }
    public void ShowUI(GameObject uiObject, UIType uiType)
    {
        UI ui = new UI(uiObject, uiType);

        if (IsShowUI(uiStack, ui))
        {
            switch (uiType)
            {
                case UIType.Option:
                case UIType.Conversation:
                case UIType.Normal:
                    PlayNoramlUIAnimation(ui, UIAnimationTime, true);
                    break;
            }
            uiStack.Push(ui);
        }
    }

    bool IsShowUI(Stack<UI> uiStack, UI currentUI)
    {
        if (uiStack.Count == 0)
            return true;

        UI uI = uiStack.Peek();
        if (HasHigherOrder(uI, currentUI))
            return true;
        else 
            return false;
    }

    public void HideUIInStack()
    {
        UI ui = uiStack.Pop();
        if (ui != null)
        {
            switch (ui.uiType)
            {
                case UIType.Conversation:
                case UIType.Normal:
                    PlayNoramlUIAnimation(ui, UIAnimationTime, false);
                    break;
            }
        }
    }

    void PlayNoramlUIAnimation(UI ui, float time, bool isSetActive)
    {
        CanvasGroup canvas = ui.UIObject.GetComponent<CanvasGroup>();
        if (canvas == null)
        {
            Debug.LogError("Canvas Group 이 붙여져 있지 않습니다 : " + ui.UIObject.name);
            return;
        }

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

        canvas.alpha = startAlpha;
        canvas.blocksRaycasts = isSetActive;
        canvas.DOFade(endAlpha, time).OnComplete(()=> canvas.blocksRaycasts = isSetActive);

    }

    bool HasHigherOrder(UI prevUI, UI currUI)
    {
        if ((int)prevUI.uiType > (int)currUI.uiType)
            return true;
        return false;
    }

    public void ShowPopupUI(string text) {
        GameObject go = Instantiate(popupUiFactory);
        go.transform.SetParent(popupCanvas.transform, false);
        go.GetComponent<SetPopupText>()?.SetText(text);
    }
}
