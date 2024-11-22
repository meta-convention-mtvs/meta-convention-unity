using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UID : MonoBehaviourPun
{
    public string uid;
    public string uuid;

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

    public void SetUUID(string id)
    {
        uuid = id;
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
