using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class BusinessRoomReservator : MonoBehaviour
{
    public void MakeAppointmentWith(Player player)
    {
        string[] meetingList = (string[])player.CustomProperties["MeetingList"];
        int meetingListIndex = (int)player.CustomProperties["MeetingListIndex"];

        meetingList[meetingListIndex++] = PhotonNetwork.LocalPlayer.UserId;

        Hashtable newProperties = new Hashtable();
        newProperties.Add("MeetingList", meetingList);
        newProperties.Add("MeetingListIndex", meetingListIndex);
        player.SetCustomProperties(newProperties);

        UIManager.Instance.ShowPopupUI("약속을 잡았습니다! 승인을 기다려주세요");
    }  
}
