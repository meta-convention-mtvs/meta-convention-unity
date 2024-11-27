using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIRoomQuit : MonoBehaviour
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
        else
        {
            QuitApplication();
        }
    }

    void QuitApplication()
    {
#if UNITY_EDITOR
        // 에디터에서 Play Mode 종료
        EditorApplication.isPlaying = false;
#else
        // 실제 빌드에서 애플리케이션 종료
        Application.Quit();
#endif
    }
}
