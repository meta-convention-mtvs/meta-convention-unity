using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public Button quitButton;

    private void Start()
    {
        quitButton.onClick.AddListener(_OnQuitButtonClick);
    }


    void _OnQuitButtonClick()
    {
        if (BoothPositionReseter.Instance.isBoothPositionResetNeed)
        {
            BoothPositionReseter.Instance.OnSaveData += QuitApplication;
            BoothPositionReseter.Instance.SaveDataWhileQuit();
        }
    }

    void QuitApplication()
    {
        Application.Quit();
    }
}
