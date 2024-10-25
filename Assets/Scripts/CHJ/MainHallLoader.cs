using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHallLoader : MonoBehaviourPunCallbacks
{

    bool isMoving = false;
    public void GoToHallRoom()
    {
        if (PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();
        isMoving = true;
    }

    public override void OnConnectedToMaster()
    {
        if (isMoving)
        {
            print("Connected to master: go to business Room");
            JoinOrCreateRoom("MainHall");
        }
    }

    void JoinOrCreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinedRoom()
    {
        if (isMoving)
        {
            print("Entered the Room");
            PhotonNetwork.LoadLevel("MainScene_CHJ");
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Enter room failed...");
        print(returnCode + message);
    }
}