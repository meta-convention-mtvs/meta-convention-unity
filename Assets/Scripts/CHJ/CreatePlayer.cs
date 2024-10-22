using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachine;
    public bool isPlayerExchangeCard;


    private void Start()
    {
        GameObject player = Create();
        Card myCard = DatabaseManager.Instance.GetData<Card>(typeof(Card).ToString());
        SaveCardInProperties(player.GetPhotonView().Owner, myCard);
        CameraFollow(player, cinemachine);
    }
    public GameObject Create()
    {
        // 플레이어 prefab 생성
        return PhotonNetwork.Instantiate("Player", new Vector3(0, 3, 0), Quaternion.identity);
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
