using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerToTargetGroup : MonoBehaviour
{
    CinemachineTargetGroup targetGroup;
    CreatePlayer playerFactory;

    private void Start()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
        playerFactory = GameObject.FindWithTag("PlayerFactory").GetComponent<CreatePlayer>();
        if (playerFactory == null)
            Debug.LogError("Player Factory is null... set tag");
        playerFactory.OnPlayerCreate += SetFollowTargetAs;
    }

    void SetFollowTargetAs(GameObject player)
    {
        targetGroup.AddMember(player.transform.Find("PlayerCameraRoot"),1, 0);
    }
}
