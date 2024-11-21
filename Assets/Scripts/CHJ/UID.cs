using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UID : MonoBehaviourPun
{
    public string uid;

    public UID(string id)
    {
        uid = id;
    }
    public UID()
    {
        uid = "";
    }

    public void SetUID(string id)
    {
        uid = id;
    }

    private void Awake()
    {
        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                this.uid = FireAuthManager.Instance.GetCurrentUser().UserId;
            }
            else
            {
                this.uid = (string)photonView.Owner.CustomProperties["id"];
            }
        }
    }
}
