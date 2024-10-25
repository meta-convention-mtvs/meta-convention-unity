using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIMgr : MonoBehaviour
{
    public GameObject settingUI;
    public GameObject btnUI;

    public string ssName = "screenshot.png"; 


    
    void Start()
    {
        
    }

    void Update()
    {
        // setting 버튼을 누르면 SettingUI activation
        // 
    }

    public void OnClickSettingBTN()
    {
        settingUI.SetActive(true);
        btnUI.SetActive(false);
    }

    public void OnClickCloseSettingBTN()
    {
        settingUI.SetActive(false);
        btnUI.SetActive(true);
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
