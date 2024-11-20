using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWebPage : MonoBehaviour
{

    public void OpenWebsiteURL()
    {
        string url = "https://www.naver.com/"; // 열고자 하는 URL
        Application.OpenURL(url);
    }
}
