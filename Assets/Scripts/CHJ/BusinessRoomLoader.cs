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

        // FireAuthManager를 통해서 Firebase UserId 가져옴
        string firebaseUserId = FireAuthManager.Instance.GetCurrentUser().UserId;
        
        // 룸 입장 후 통역 서버 연결 초기화
        TranslationManager.Instance.Connect();
        TranslationManager.Instance.CreateRoom(firebaseUserId, "ko");  // UserId와 언어 설정

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Enter room failed...");
        print(returnCode + message);
    }
}
