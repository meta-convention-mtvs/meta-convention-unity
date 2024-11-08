using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusinessRoomLoader : MonoBehaviourPunCallbacks
{
    public void GoToBusinessRoom()
    {
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master: go to business Room");
        JoinOrCreateRoom(PhotonNetwork.NickName + "Room");
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
        print("Entered the Room");
        PhotonNetwork.LoadLevel("BusinessRoomScene");

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Enter room failed...");
        print(returnCode + message);
    }
}
