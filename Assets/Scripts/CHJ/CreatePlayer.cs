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
        // �÷��̾� prefab ����
        GameObject player = PhotonNetwork.Instantiate("Player", new Vector3(0, 3, 0), Quaternion.identity);


        // Cinemachine ī�޶� ĳ���͸� �ٶ� �� �ְ� �Ѵ�.
        cinemachine.Follow = player.transform.Find("PlayerCameraRoot") ;
    }
}
