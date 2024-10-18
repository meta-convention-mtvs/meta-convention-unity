using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public CinemachineVirtualCamera cinemachine;

    private void Start()
    {
        Create();
    }
    public void Create()
    {
        // 플레이어 prefab 생성
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 3, 0), Quaternion.identity);


        // Cinemachine 카메라가 캐릭터를 바라볼 수 있게 한다.
        cinemachine.Follow = player.transform.Find("PlayerCameraRoot") ;
    }
}
