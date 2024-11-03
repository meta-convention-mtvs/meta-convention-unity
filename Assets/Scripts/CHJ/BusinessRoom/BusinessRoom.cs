using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessRoom : MonoBehaviour
{
    public UIBusinessRoom ui_br;
    public GameObject documentUI;
    public GameObject qrcodeUI;
    public GameObject quitUI;

    MainHallLoader hallLoader;

    private void Start()
    {
        ui_br.OnQrcode += ShowQrcode;
        ui_br.OnDocument += WriteDocument;
        ui_br.OnObject += ShowObject;
        ui_br.OnQuit += QuitRoom;
    }

    void ShowQrcode()
    {
        UIManager.Instance.ShowUI(qrcodeUI, UIType.Normal);
    }

    void WriteDocument()
    {
        // 회의록 ui를 띄운다.
        UIManager.Instance.ShowUI(documentUI, UIType.Normal);
    }

    void ShowObject()
    {
        // 데이터 베이스에 올려놓은 오브젝트를 가져온다
        // 상대방도 가져온다.
        // 그것을 띄운다.
    }

    void QuitRoom()
    {
        UIManager.Instance.ShowUI(quitUI, UIType.Normal);
    }
}
