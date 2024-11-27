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

    private TranslationManager translationManager;

    private void Start()
    {
        translationManager = TranslationManager.Instance;
        
        // PhotonView 소유권 설정
        if (photonView.IsMine && photonView.Owner == null)
        {
            photonView.RequestOwnership();
            Debug.Log("[TranslationRoomIDSynchronizer] PhotonView 소유권 요청");
        }

        translationManager.OnConnect += CreateRoom;
        translationManager.OnRoomJoined += JoinRoom;
        translationManager.Connect();
    }
    private void OnDestroy()
    {
        if (translationManager != null)
        {
            translationManager.OnConnect -= CreateRoom;
            translationManager.OnRoomJoined -= JoinRoom;
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("[TranslationRoomIDSynchronizer] Photon room left, leaving AI server room");
        
        // AI 서버 방 퇴장 요청
        if (translationManager != null && !string.IsNullOrEmpty(translationManager.CurrentRoomID))
        {
            translationManager.LeaveRoom();
        }
    }

    // public override void OnOwnershipTransferred(PhotonView targetView, Player previousOwner)
    // {
    //     base.OnOwnershipTransferred(targetView, previousOwner);
    // }

    void CreateRoom()
    {
        Debug.Log("[TranslationRoomIDSynchronizer] CreateRoom 실행됨");
        // 이 스크립트는 공용 오브젝트에 붙을 것이다 => owner이면 isMine은 true, 아니면 false
        if (PhotonNetwork.CurrentRoom.Name == FireAuthManager.Instance.GetCurrentUser().UserId)
        {
            string userID = FireAuthManager.Instance.GetCurrentUser().UserId;
            // 내가 owner이다 room create를 요청하자
            TranslationManager.Instance.CreateRoom(userID, CashedDataFromDatabase.Instance.playerLanguage.language, CashedDataFromDatabase.Instance.playerInfo.uuid);
        }
    }

    void JoinRoom(string roomID)
    {
        Debug.Log("[TranslationRoomIDSynchronizer] JoinRoom 실행됨");
        // 내가 방장이면..
        // ToDo: 이 부분을 구현해주세요...
        if (PhotonNetwork.CurrentRoom.Name == FireAuthManager.Instance.GetCurrentUser().UserId)
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
        // 마스터 클라이언트만 처리하도록
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[TranslationRoomIDSynchronizer] 마스터 클라이언트가 아님, 리셋 요청 무시");
            return;
        }

        Debug.Log($"[TranslationRoomIDSynchronizer] 마스터 클라이언트가 리셋 프로세스 시작");
        StartCoroutine(ResetProcess(requesterId));
    }

    private IEnumerator ResetProcess(string requesterId)
    {
        Debug.Log("[ResetProcess] 시작");
        isResetting = true;
        
        // 1. 현재 방에서 나가기 (메시지 초기화는 LeaveCurrentRoom에서 처리)
        if (!string.IsNullOrEmpty(TranslationManager.Instance.CurrentRoomID))
        {
            Debug.Log("[ResetProcess] 기존 방에서 나가기");
            photonView.RPC("LeaveCurrentRoom", RpcTarget.All);
            
            yield return new WaitUntil(() => string.IsNullOrEmpty(TranslationManager.Instance.CurrentRoomID));
            Debug.Log("[ResetProcess] 방 나가기 완료");
        }
        
        // 2. 웹소켓 재연결
        Debug.Log("[ResetProcess] 웹소켓 재연결");
        TranslationManager.Instance.Reconnect();
        
        // 3. 웹소켓 재연결 대기
        yield return new WaitUntil(() => TranslationManager.Instance.IsConnected);
        Debug.Log("[ResetProcess] 웹소켓 재연결 완료");
        
        // 4. 안정화 대기
        yield return new WaitForSeconds(1.0f);
        
        // 5. 새로운 방 생성
        if (string.IsNullOrEmpty(TranslationManager.Instance.CurrentRoomID))
        {
            Debug.Log("[ResetProcess] 새로운 방 생성 시작");
            CreateRoom();
        }
        
        isResetting = false;
        Debug.Log("[ResetProcess] 완료");
    }

    [PunRPC]
    private void LeaveCurrentRoom()
    {
        Debug.Log("[TranslationRoomIDSynchronizer] 현재 방 나가기 실행");
        
        // 1. 메시지 버블 초기화
        var playerTranslator = FindObjectOfType<PlayerTranslator>();
        if (playerTranslator != null)
        {
            Debug.Log("[TranslationRoomIDSynchronizer] 메시지 초기화 시작");
            playerTranslator.ClearMessages();
        }
        
        // 2. 방 나가기
        TranslationManager.Instance.LeaveRoom();
        
        Debug.Log("[TranslationRoomIDSynchronizer] 방 나가기 및 메시지 초기화 완료");
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
