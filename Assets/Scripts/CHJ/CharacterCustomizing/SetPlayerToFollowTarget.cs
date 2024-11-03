using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerToFollowTarget : MonoBehaviour
{
    CinemachineVirtualCamera playerFollowCamera;
    CreatePlayer playerFactory;

    private void Start()
    {
        playerFollowCamera = GetComponent<CinemachineVirtualCamera>();
        playerFactory = GameObject.FindWithTag("PlayerFactory").GetComponent<CreatePlayer>();
        if (playerFactory == null)
            Debug.LogError("Player Factory is null... set tag");
        playerFactory.OnPlayerCreate += SetFollowTargetAs;
    }

    void SetFollowTargetAs(GameObject player)
    {
        playerFollowCamera.Follow = player.transform.Find("PlayerCameraRoot");
    }
}
