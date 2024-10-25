using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;

public class DocumentWriter : MonoBehaviourPun
{
    public InputField meetingDate;
    public InputField meetingTime;
    public InputField companyName;
    public InputField customerName;

    public InputField document;

    void Start()
    {
        meetingDate.text = DateTime.Today.Date.ToString();
        meetingTime.text = DateTime.Today.Hour + ":" + DateTime.Today.Minute + ":" + DateTime.Today.Second;

        document.onValueChanged.AddListener(SetString);
    }


    void SetString(string s)
    {
        photonView.RPC(nameof(RpcSetString), RpcTarget.Others, s);
    }

    [PunRPC]
    void RpcSetString(string s)
    {
        document.text = s;
    }
}
