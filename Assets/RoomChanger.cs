using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum RoomType
{
    MainHall,
    BusinessRoom
}

public class RoomChanger : MonoBehaviourPunCallbacks
{
    RoomType targetRoomType;
    Dictionary<RoomType, string> roomNameDictionary;

    private void Start()
    {
        roomNameDictionary = new Dictionary<RoomType, string>();
        roomNameDictionary[RoomType.MainHall] = "MainHall";
        roomNameDictionary[RoomType.BusinessRoom] = PhotonNetwork.NickName + "BusinessRoom";
    }

    public void ChangeRoom(RoomType roomType)
    {
        targetRoomType = roomType;
        if(PhotonNetwork.InRoom)
            PhotonNetwork.LeaveRoom();

    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master: go to " + targetRoomType.ToString());
        JoinOrCreateRoom(roomNameDictionary[targetRoomType]);
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
