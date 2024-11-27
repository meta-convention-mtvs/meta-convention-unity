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


    public void ShowUI(GameObject uiObject, UIType uiType)
    {
        GeneralUI myUi = uiObject.GetComponent<GeneralUI>();
        if (myUi != null)
        {
            myUi.Show();
        }
    }

    public void ShowPopupUI(string koText, string enText) {
        GameObject go = Instantiate(popupUiFactory);
        go.transform.SetParent(popupCanvas.transform, false);
        if (LanguageSingleton.Instance.language == "ko")
            go.GetComponent<SetPopupText>()?.SetText(koText);
        else if (LanguageSingleton.Instance.language == "en")
            go.GetComponent<SetPopupText>()?.SetText(enText);
    }
}
