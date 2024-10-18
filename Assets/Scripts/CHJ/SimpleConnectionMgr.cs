using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConnectionMgr : MonoBehaviourPunCallbacks
{
    #region ���μ����� ����
    private void Start()
    {
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
    }
    #endregion

    #region �뿡 ����
    public void JoinOrCreateRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            PhotonNetwork.JoinOrCreateRoom("MainHall", roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region �� �ݹ��Լ�
    public override void OnJoinedRoom()
    {
        print("Entered the Room");
        PhotonNetwork.LoadLevel("MainScene_CHJ");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Enter room failed...");
        print(returnCode + message);
    }
    #endregion
}
