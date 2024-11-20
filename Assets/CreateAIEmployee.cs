using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CreateAIEmployee : MonoBehaviourPun
{
    public Transform AIEmployeePosition;

    private void Start()
    {
        if(photonView.IsMine)
        {
            PhotonNetwork.Instantiate("AIEmployee", AIEmployeePosition.position, AIEmployeePosition.rotation);
        }
    }
}
