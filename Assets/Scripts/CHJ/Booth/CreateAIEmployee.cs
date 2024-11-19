using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class CreateAIEmployee : MonoBehaviourPun
{
    public Transform AIEmployeePosition;
    public GameObject aiEmployeeFactory;

    private void Start()
    {
        GameObject go = Instantiate(aiEmployeeFactory, this.transform);
        go.GetComponent<InteractableAIEmployeeObject>().InitializeUID(new UID((string)photonView.Owner.CustomProperties["id"]));
    }
}
