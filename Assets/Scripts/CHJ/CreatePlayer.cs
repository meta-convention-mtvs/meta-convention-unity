using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachine;
    public Transform[] playerStartPosition;
    GameObject player;

    private void Start()
    {
        player = Create();
        DatabaseManager.Instance.GetData<Card>(onCardLoad);

        CameraFollow(player, cinemachine);
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
        if(idx > playerStartPosition.Length)
        {
            idx = playerStartPosition.Length;
        }
        return PhotonNetwork.Instantiate("Player", playerStartPosition[idx].position, Quaternion.identity);
    }

    void SaveCardInProperties(Player player, Card myCard)
    {
        Hashtable myInformation = new Hashtable
        {
            {"id", myCard.id },
            {"nickname",myCard.nickname },
            {"institute", myCard.institute },
            {"major", myCard.major },
            {"email", myCard.email }
        };

        player.SetCustomProperties(myInformation);
    }

    public void CameraFollow(GameObject player, CinemachineVirtualCamera cinemachine)
    {
        // Cinemachine 카메라가 캐릭터를 바라볼 수 있게 한다.
        cinemachine.Follow = player.transform.Find("PlayerCameraRoot");
    }
}
