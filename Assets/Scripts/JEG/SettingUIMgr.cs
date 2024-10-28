using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIMgr : MonoBehaviour
{
    public GameObject settingUI;

    public string ssName = "screenshot.png"; 

    public void OnClickSettingBTN()
    {
        UIManager.Instance.ShowUI(settingUI, UIType.Normal);
    }

    public void OnClickCloseSettingBTN()
    {
        CanvasGroup canvas = settingUI.GetComponent<CanvasGroup>();
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;
    }

    public void OnClickScreenShot()
    {
        string path = Application.persistentDataPath + "/" + ssName;
        ScreenCapture.CaptureScreenshot(ssName);
        Debug.Log("ScreenShot saved to : " + ssName);
    }
#if UNITY_EDITOR
    [MenuItem("Tools/Take Editor Screenshot")]
    public static void TakeEditorScreenshot()
    {
        string path = Application.dataPath + "/../Editor_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        ScreenCapture.CaptureScreenshot(path);
        Debug.Log("Editor Screenshot saved to: " + path);
    }
#endif

}
