using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIMgr : MonoBehaviour
{
    public Button screenshotButton;

    public GameObject settingUI;
    private void Start()
    {
        screenshotButton.onClick.AddListener(() => OnClickScreenShot());
    }
    
    public void OnClickScreenShot()
    {
        screenshotButton.interactable = false;
        string directoryPath = Application.persistentDataPath + "/Screenshots";
        if (!System.IO.Directory.Exists(directoryPath))
        {
            System.IO.Directory.CreateDirectory(directoryPath);  // 폴더가 없으면 생성
        }
        string filePath = directoryPath + "/" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";
        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log("Editor Screenshot saved to: " + filePath);
        UIManager.Instance.ShowPopupUI("스크린샷이 저장되었습니다.", "The screenshot has been saved.");
        StartCoroutine(ReenableButton());
    }

    IEnumerator ReenableButton()
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        screenshotButton.interactable = true;       // 버튼 다시 활성화
    }

    public void OnClickSetting()
    {
        if (settingUI.activeInHierarchy)
        {
            settingUI.SetActive(false);
        } else if (!settingUI.activeInHierarchy)
        {
            settingUI.SetActive(true);
        }
    } 

}
