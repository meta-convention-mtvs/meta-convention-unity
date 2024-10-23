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
        DatabaseManager.Instance.GetData<CharacterCustomizeData>(JoinRoom);
        
    }

    void JoinRoom(CharacterCustomizeData data)
    {
        if(data!= null)
            connectMgr.JoinOrCreateRoom();
    }
}
