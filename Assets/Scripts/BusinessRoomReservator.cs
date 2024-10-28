using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BusinessRoomReservator : MonoBehaviourPunCallbacks
{
    // 버튼을 누르면
    // 그 사람의 Player Custom properties에 추가한다.

    public BusinessRoomLoader businessRoomLoader;
    Queue<string> meetingQueue;

    private void Start()
    {
        meetingQueue = new Queue<string>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MakeAppointmentWith(photonView.Owner);
        }        
    }

    public void MakeAppointmentWith(Player player)
    {
        string[] meetingList = (string[])player.CustomProperties["MeetingList"];
        int meetingListIndex = (int)player.CustomProperties["MeetingListIndex"];

        meetingList[meetingListIndex++] = photonView.Owner.UserId;

        Hashtable newProperties = new Hashtable();
        newProperties.Add("MeetingList", meetingList);
        newProperties.Add("MeetingListIndex", meetingListIndex);
        player.SetCustomProperties(newProperties);
    }

    Player FindPlayerWithId(string id) {
        Player[] playerList = PhotonNetwork.PlayerList;

        for(int i = 0; i< playerList.Length; i++)
        {
            if (playerList[i].UserId == id)
                return playerList[i];
        }
        return null;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("MeetingList"))
        {
            string[] newMeetingList = (string[])changedProps["MeetingList"];
            int newMeetingListIndex = (int)changedProps["MeetingListIndex"];

            for (int i = 0; i < newMeetingListIndex; i++)
                print(newMeetingList[i]);
            if (newMeetingListIndex > 0)
            {
                meetingQueue.Enqueue(newMeetingList[newMeetingListIndex - 1]);
                //StartAppointmentIn(meetingQueue);
            }
        }
    }

    void StartAppointmentIn(Queue<string> queue)
    {
        if(queue.Count > 0)
        { 
            string playerId = queue.Dequeue();
            Player targetPlayer = FindPlayerWithId(playerId);
            if(targetPlayer != null)
            {
                photonView.RPC(nameof(GoToBusinessRoom), targetPlayer);
                businessRoomLoader.GoToBusinessRoom();
            }
        }
    }

    void CancelAppointmentIn(Queue<string> queue)
    {
        if(queue.Count > 0)
        {
            queue.Dequeue();
        }
    }

    [PunRPC]
    void GoToBusinessRoom()
    {
        businessRoomLoader.GoToBusinessRoom();
    }
}
