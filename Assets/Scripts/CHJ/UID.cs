using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UID : MonoBehaviourPun
{
    public string uid;
    public string uuid;

    public Action<string> OnUIDChanged;
    public Action<string> OnUUIDChanged;
    
    public void SetUID(string id)
    {
        uid = id;
        OnUIDChanged?.Invoke(uid);
    }

    public void SetUUID(string id)
    {
        uuid = id;
        OnUUIDChanged?.Invoke(uuid);
    }

    private void Start()
    {
        if (photonView != null)
        {
            if (photonView.IsMine)
            {
                this.uid = FireAuthManager.Instance.GetCurrentUser().UserId;
                photonView.RPC(nameof(RPCSetUID), RpcTarget.OthersBuffered, this.uid);
                OnUIDChanged?.Invoke(this.uid);

                this.uuid = CashedDataFromDatabase.Instance.playerInfo.uuid;
                photonView.RPC(nameof(RPCSetUUID), RpcTarget.OthersBuffered, this.uuid);
                OnUUIDChanged?.Invoke(this.uuid);
            }
        }
    }

    [PunRPC]
    void RPCSetUID(string id)
    {
        uid = id;
        OnUIDChanged?.Invoke(uid);
    }

    [PunRPC]
    void RPCSetUUID(string id)
    {
        uuid = id;
        OnUUIDChanged?.Invoke(uuid);
    }
}
