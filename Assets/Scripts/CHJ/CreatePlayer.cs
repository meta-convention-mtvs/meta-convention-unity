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

    GameObject player;

    private void Start()
    {
        player = Create();
        DatabaseManager.Instance.GetData<Card>(onCardLoad);

        StartCoroutine(WaitAndInvoke());
    }

    IEnumerator WaitAndInvoke()
    {
        yield return null;
        OnPlayerCreate?.Invoke(player);
    }
    private void onCardLoad(Card myCard)
    {
        SaveCardInProperties(player.GetPhotonView().Owner, myCard);
    }
    public GameObject Create()
    {
        // 플레이어 prefab 생성
        int idx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        print(idx);
        if(idx >= playerStartPosition.Length)
        {
            idx = playerStartPosition.Length-1;
        }
        return PhotonNetwork.Instantiate("Player", playerStartPosition[idx].position, playerStartPosition[idx].rotation);
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
            {"MeetingList",emptyMeetingList },
            {"MeetingListIndex", 0 }
        };

        
        player.SetCustomProperties(myInformation);
    }


}
