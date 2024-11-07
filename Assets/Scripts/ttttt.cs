using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ttttt : MonoBehaviourPun
{

    string roomID = string.Empty;

    private void Start()
    {
        // 이 스크립트는 공용 오브젝트에 붙을 것이다 => owner이면 isMine은 true, 아니면 false
        if (photonView.IsMine)
        {
            string userID = FireAuthManager.Instance.GetCurrentUser().UserId;
            // 내가 owner이다 room create를 요청하자
            TranslationManager.Instance.CreateRoom(userID, "ko");
        }
    }

    private void Update()
    {
        // 내가 방장이면..
        // ToDo: 이 부분을 구현해주세요...
        if (photonView.IsMine)
        {
            // room joined 메시지가 올 때까지 기다린다.
            // 만약 room joined 메시지를 받으면, RPC로 roomID를 전달해준다.
        }
        // 내가 방장이 아니라면...
        else
        {
            // 만약 roomID가 empty가 아니라면... (RPC로 roomID를 받으면..)
            if (roomID != string.Empty)
            {
                // room joine을 시도한다
                string userID = FireAuthManager.Instance.GetCurrentUser().UserId;
                TranslationManager.Instance.JoinRoom(roomID, userID, "ko");
            }
        }
    }

    [PunRPC]
    void SetRoomID(string newRoomID)
    {
        roomID = newRoomID;
    }
}
