using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviourPunCallbacks
{
    public string roomName;
    public string sceneName = "BusinessRoomScene";

    const byte INVITE_TO_ROOM_ID = 1;

    public void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        print("Event 수신");
        byte eventCode = photonEvent.Code;

        if (eventCode == INVITE_TO_ROOM_ID)
        {
            object[] data = (object[])photonEvent.CustomData;

            string uid = (string)data[0];
            print("event code 1번");
            if (uid == FireAuthManager.Instance.GetCurrentUser().UserId)
            {
                print("룸 초대됨");
                roomName = (string)data[1];
                PhotonNetwork.LeaveRoom();
            }
        }
    }

}
