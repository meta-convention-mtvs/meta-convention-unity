using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextSceneButton : MonoBehaviour
{
    public Button btn_nextScene;
    public SimpleConnectionMgr connectMgr;

    private void Start()
    {
        btn_nextScene.onClick.AddListener(LoadScene);
    }
    public void LoadScene()
    {
        if(DatabaseManager.Instance.GetData<CharacterCustomizeData>(typeof(CharacterCustomizeData).ToString())!=null)
            connectMgr.JoinOrCreateRoom();  
    }
}
