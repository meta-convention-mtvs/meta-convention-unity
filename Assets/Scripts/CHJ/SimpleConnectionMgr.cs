using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConnectionMgr : MonoBehaviourPunCallbacks
{

    #region 메인서버에 연결
    private void Start()
    {
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
        JoinOrCreateRoom("MainHall");
    }
    #endregion

    #region 룸에 입장, 나가기
    public void JoinOrCreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            roomOptions.PublishUserId = true;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region 룸 콜백함수
    public override void OnJoinedRoom()
    {
        print("Entered the Room");
        PhotonNetwork.LoadLevel("MainScene_CHJ");
    }


    #endregion
}
