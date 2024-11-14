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

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == INVITE_TO_ROOM_ID)
        {
            object[] data = (object[])photonEvent.CustomData;

            string uid = (string)data[0];

            if (uid == FireAuthManager.Instance.GetCurrentUser().UserId)
            {
                roomName = (string)data[1];
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
