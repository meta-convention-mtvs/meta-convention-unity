using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class CreatePlayer : MonoBehaviour
{
    public Transform[] playerStartPosition;
    public Action<GameObject> OnPlayerCreate;
    public enum RoomType
    {
        MainHall,
        BusinessRoom
    };
    public RoomType roomType;

    GameObject player;

    private void Start()
    {
        player = Create(roomType);
        if(roomType == RoomType.MainHall)
            DatabaseManager.Instance.GetData<Card>(onCardLoad);
        if(roomType == RoomType.MainHall || roomType == RoomType.BusinessRoom)
            StartCoroutine(WaitAndInvoke());
    }

    IEnumerator WaitAndInvoke()
    {
        yield return null;
        // player 객체를 카메라에 등록시킨다.
        OnPlayerCreate?.Invoke(player);
    }

    private void onCardLoad(Card myCard)
    {
        SaveCardInProperties(player.GetPhotonView().Owner, myCard);
    }

    public GameObject Create(RoomType roomType)
    {
        // 플레이어 prefab 생성
        int idx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        print(idx);
        if(idx >= playerStartPosition.Length)
        {
            idx = playerStartPosition.Length-1;
        }
        string resourceName = "";
        if (roomType == RoomType.MainHall)
            resourceName = "Player";
        else if (roomType == RoomType.BusinessRoom)
            resourceName = "Player_BusinessRoom";
        return PhotonNetwork.Instantiate(resourceName, playerStartPosition[idx].position, playerStartPosition[idx].rotation);
    }

    void SaveCardInProperties(Player player, Card myCard)
    {
        string[] emptyMeetingList = Enumerable.Repeat("", 100).ToArray();
        Hashtable myInformation = new Hashtable
        {
            {"id", myCard.id },
            {"nickname",myCard.nickname },
            {"institute", myCard.institute },
            {"major", myCard.major },
            {"email", myCard.email },
            {"phoneNumber",myCard.phoneNumber },
            {"MeetingList",emptyMeetingList },
            {"MeetingListIndex", 0 }
        };
      
        player.SetCustomProperties(myInformation);
    }


}
