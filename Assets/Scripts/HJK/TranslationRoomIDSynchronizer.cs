using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TranslationRoomIDSynchronizer : MonoBehaviourPunCallbacks
{
    private bool isResetting = false;
    private const float RESET_TIMEOUT = 10f;
    private Coroutine resetCoroutine;

    private void Start()
    {
        // PhotonView 소유권 설정
        if (photonView.IsMine && photonView.Owner == null)
        {
            photonView.RequestOwnership();
            Debug.Log("[TranslationRoomIDSynchronizer] PhotonView 소유권 요청");
        }

        TranslationManager.Instance.OnConnect += CreateRoom;
        TranslationManager.Instance.OnRoomJoined += JoinRoom;
        TranslationManager.Instance.Connect();
    }

    // public override void OnOwnershipTransferred(PhotonView targetView, Player previousOwner)
    // {
    //     base.OnOwnershipTransferred(targetView, previousOwner);
    // }

    void CreateRoom()
    {
        // 이 스크립트는 공용 오브젝트에 붙을 것이다 => owner이면 isMine은 true, 아니면 false
        if (photonView.IsMine)
        {
            string userID = FireAuthManager.Instance.GetCurrentUser().UserId;
            // 내가 owner이다 room create를 요청하자
            TranslationManager.Instance.CreateRoom(userID, CashedDataFromDatabase.Instance.playerLanguage.language);
        }
    }

    void JoinRoom(string roomID)
    {
        // 내가 방장이면..
        // ToDo: 이 부분을 구현해주세요...
        if (photonView.IsMine)
        {
            //print(roomID);
            // TranslationManager에서 roomID를 받았는지 확인하고 받았다면 다른 참가자들에게 전달
            if (roomID != string.Empty)
            {
                print("Room ID: " + roomID);
                // RPC를 통해 모든 클라이언트에게 roomID 전달
                photonView.RPC(nameof(SetRoomID), RpcTarget.Others, roomID);
            }
        }
    }

    [PunRPC]
    void SetRoomID(string newRoomID)
    {
        string userID = FireAuthManager.Instance.GetCurrentUser().UserId;
        TranslationManager.Instance.JoinRoom(newRoomID, userID, CashedDataFromDatabase.Instance.playerLanguage.language);
    }

    // 리셋 요청 처리
    [PunRPC]
    public void RequestReset(string requesterId)
    {
        Debug.Log($"[TranslationRoomIDSynchronizer] RequestReset RPC 수신됨 - RequesterId: {requesterId}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
        
        // 마스터 클라이언트만 리셋 프로세스를 실행
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[TranslationRoomIDSynchronizer] 마스터 클라이언트가 리셋 프로세스 시작");
            StartCoroutine(ResetProcess(requesterId));
        }
        else
        {
            Debug.Log("[TranslationRoomIDSynchronizer] 마스터 클라이언트의 리셋 프로세스 대기");
        }
    }

    private IEnumerator ResetProcess(string requesterId)
    {
        Debug.Log("[ResetProcess] 시작");
        isResetting = true;
        
        // 1. 현재 방에서 나가기
        if (!string.IsNullOrEmpty(TranslationManager.Instance.CurrentRoomID))
        {
            // 모든 클라이언트에게 방 나가기 알림
            photonView.RPC("LeaveCurrentRoom", RpcTarget.All);
            
            // room.bye 이벤트를 기다림 (최대 5초)
            float waitTime = 0f;
            while (waitTime < 5f && !string.IsNullOrEmpty(TranslationManager.Instance.CurrentRoomID))
            {
                yield return new WaitForSeconds(0.1f);
                waitTime += 0.1f;
            }
        }
        
        // 2. 웹소켓 재연결
        Debug.Log("[ResetProcess] 웹소켓 재연결");
        TranslationManager.Instance.Reconnect();
        
        // 웹소켓이 다시 연결될 때까지 대기
        yield return new WaitUntil(() => TranslationManager.Instance.IsConnected);
        Debug.Log("[ResetProcess] 웹소켓 재연결 완료");
        
        // 3. 잠시 대기
        yield return new WaitForSeconds(0.5f);
        
        // 4. 새로운 방 생성
        Debug.Log("[ResetProcess] 새로운 방 생성 시작");
        CreateRoom();
        
        isResetting = false;
        Debug.Log("[ResetProcess] 완료");
    }

    [PunRPC]
    private void LeaveCurrentRoom()
    {
        Debug.Log("[TranslationRoomIDSynchronizer] 현재 방 나가기 실행");
        TranslationManager.Instance.LeaveRoom();
    }

    private class TranslationState
    {
        public string RoomId;
        public bool IsConnected;
        // 필요한 상태 정보 추가
    }

    private TranslationState SaveCurrentState()
    {
        return new TranslationState
        {
            RoomId = TranslationManager.Instance.CurrentRoomID,
            IsConnected = TranslationManager.Instance.IsConnected
        };
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        
        // Photon Room 입장 후 소유권 설정
        if (photonView.Owner == null && PhotonNetwork.IsMasterClient)
        {
            photonView.RequestOwnership();
            Debug.Log("[TranslationRoomIDSynchronizer] PhotonView 소유권 요청");
        }
        
        Debug.Log($"[TranslationRoomIDSynchronizer] 방 입장 완료 - IsMasterClient: {PhotonNetwork.IsMasterClient}, Owner: {photonView.Owner?.UserId ?? "null"}");
    }
}
