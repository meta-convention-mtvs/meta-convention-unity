using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BusinessRoomQueueManager : MonoBehaviourPunCallbacks
{
    public UIBusinessRoomQueueManager ui_br;
    Queue<string> meetingQueue;

    string roomName;

    private void Start()
    {
        meetingQueue = new Queue<string>();
        ui_br.OnYesButtonClick += () => { StartAppointmentIn(meetingQueue); };
        ui_br.OnNoButtonClick += () => { CancelAppointmentIn(meetingQueue); };
    }
    Player FindPlayerWithId(string id)
    {
        Player[] playerList = PhotonNetwork.PlayerList;

        for (int i = 0; i < playerList.Length; i++)
        {
            if (playerList[i].UserId == id)
                return playerList[i];
        }
        return null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer != PhotonNetwork.LocalPlayer)
            return;

        if (changedProps.ContainsKey("MeetingList"))
        {
            // newMeetingList 는 Index에 값 한개만 추가됨
            string[] newMeetingList = (string[])changedProps["MeetingList"];
            int newMeetingListIndex = (int)changedProps["MeetingListIndex"];

            // 모든 meetingList를 출력한다.
            for (int i = 0; i < newMeetingListIndex; i++)
                print(newMeetingList[i] + newMeetingListIndex);
            
            // meetingList의 데이터를 Queue에 넣는다.
            if (newMeetingListIndex > 0)
            {
                print("yes");
                meetingQueue.Enqueue(newMeetingList[newMeetingListIndex - 1]);
                UIManager.Instance.ShowUI(ui_br.gameObject, UIType.Option);
            }
        }
    }

    void StartAppointmentIn(Queue<string> queue)
    {
        if (queue.Count > 0)
        {
            string playerId = queue.Dequeue();
            Player targetPlayer = FindPlayerWithId(playerId);
            if (targetPlayer != null)
            {
                roomName = FireAuthManager.Instance.GetCurrentUser().UserId;
                photonView.RPC(nameof(GoToBusinessRoom), targetPlayer, roomName);
                PhotonNetwork.LeaveRoom();
            }
        }
    }

    void CancelAppointmentIn(Queue<string> queue)
    {
        if (queue.Count > 0)
        {
            queue.Dequeue();
        }
    }

    [PunRPC]
    void GoToBusinessRoom(string roomName)
    {
        this.roomName = roomName;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to master: go to business Room");
        JoinOrCreateRoom(roomName);
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
        print("Entered the Room : " + PhotonNetwork.CurrentRoom.Name);
        if(PhotonNetwork.CurrentRoom.Name == FireAuthManager.Instance.GetCurrentUser().UserId)
        {            
            PhotonNetwork.CurrentRoom.SetMasterClient(PhotonNetwork.LocalPlayer);
        }
        PhotonNetwork.LoadLevel("BusinessRoomScene");

    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Enter room failed...");
        print(returnCode + message);
    }
}
