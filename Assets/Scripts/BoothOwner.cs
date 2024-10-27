using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoothOwner : MonoBehaviourPun
{
    public void SetBoothOwnerAs()
    {
        photonView.RequestOwnership();
    }
}
