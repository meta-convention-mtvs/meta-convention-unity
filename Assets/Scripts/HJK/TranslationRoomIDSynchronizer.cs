using Photon.Pun;
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
        // 웹소켓 연결 설정
        TranslationManager.Instance.OnConnect += CreateRoom;
        TranslationManager.Instance.OnRoomJoined += JoinRoom;
        TranslationManager.Instance.Connect();

        Debug.Log($"[TranslationRoomIDSynchronizer] 초기화 상태 - IsMine: {photonView.IsMine}, Owner: {photonView.Owner?.UserId ?? "null"}, IsMasterClient: {PhotonNetwork.IsMasterClient}");
    }

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
    private void RequestReset(string requesterId)
    {
        Debug.Log($"[TranslationRoomIDSynchronizer] 리셋 요청 - RequesterId: {requesterId}, IsMine: {photonView.IsMine}, Owner: {photonView.Owner?.UserId ?? "null"}");
        
        if (isResetting)
        {
            Debug.Log("[TranslationRoomIDSynchronizer] 이미 리셋 중입니다.");
            return;
        }
        
        isResetting = true;
        if (resetCoroutine != null)
        {
            Debug.Log("[TranslationRoomIDSynchronizer] 기존 리셋 코루틴 중지");
            StopCoroutine(resetCoroutine);
        }
        
        Debug.Log("[TranslationRoomIDSynchronizer] 새 리셋 프로세스 시작");
        resetCoroutine = StartCoroutine(ResetProcess(requesterId));
    }

    private IEnumerator ResetProcess(string requesterId)
    {
        Debug.Log($"[ResetProcess] 시작 - RequesterId: {requesterId}");
        
        // 방장 체크를 PhotonView.Owner 대신 Translation 서버의 유저 정보로 판단
        var users = TranslationManager.Instance.GetCurrentUsers();
        if (users != null && users.Count > 0)
        {
            // 첫 번째 유저를 방장으로 간주
            var firstUser = users[0];
            string firstUserId = firstUser["userid"].ToString();
            
            Debug.Log($"[ResetProcess] 방장 체크 - RequesterId: {requesterId}, FirstUser: {firstUserId}");
            
            if (string.Equals(requesterId, firstUserId))
            {
                Debug.Log("[ResetProcess] 방장이 리셋 요청함");
                TranslationManager.Instance.Reconnect();
                yield break;
            }
        }
        
        Debug.Log("[ResetProcess] 방장이 아닌 사용자의 리셋 요청");
        
        Debug.Log("[ResetProcess] 완료");
        isResetting = false;
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
