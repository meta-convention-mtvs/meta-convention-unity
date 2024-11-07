using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;
using Photon.Realtime;
using StarterAssets;

public class DocumentWriter : MonoBehaviourPun
{
    public InputField title;
    public InputField meetingDate;
    public InputField companyName;
    public InputField customerName;

    public InputField document;

    CreatePlayer playerFactory;
    StarterAssetsInputs inputSystem;

    void Start()
    {
        meetingDate.text = DateTime.Today.Date.ToString();

        // playerfactory에서 플레이어 객체에서 StarterAssetsInputs 받아오기
        playerFactory = GameObject.FindWithTag("PlayerFactory").GetComponent<CreatePlayer>();
        if (playerFactory == null)
            Debug.LogError("Player Factory is null... set tag");
        playerFactory.OnPlayerCreate += SetInputSystem;

    }

    private void Update()
    {
        // 만약 4개 input field 중 한 개라도 활성화 되어있다면
        if(title.isFocused || meetingDate.isFocused || companyName.isFocused || customerName.isFocused || document.isFocused)
        {
            inputSystem?.OnChatToggle(true);
        }
        else
        {
            inputSystem?.OnChatToggle(false);
        }
        // inputSystem 끈다
        // 아니라면
        // inputSystem 활성화한다.
    }
    void SetInputSystem(GameObject player)
    {
        inputSystem = player.GetComponent<StarterAssetsInputs>();
    }

    void SetDocumentString(string s)
    {
        photonView.RPC(nameof(RpcSetDocumentString), RpcTarget.OthersBuffered, s);
    }

    void SetTitleString(string s)
    {
        photonView.RPC(nameof(RpcSetTitleString), RpcTarget.OthersBuffered, s);
    }

    [PunRPC]
    void RpcSetDocumentString(string s)
    {
        document.text = s;
    }

    [PunRPC]
    void RpcSetTitleString(string s)
    {
        title.text = s;
    }
}
