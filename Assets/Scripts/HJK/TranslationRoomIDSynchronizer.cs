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
        TranslationManager.Instance.OnConnect += CreateRoom;
        TranslationManager.Instance.OnRoomJoined += JoinRoom;
        TranslationManager.Instance.Connect();
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
        Debug.Log($"[TranslationRoomIDSynchronizer] RequestReset 호출됨 - RequesterId: {requesterId}");
        
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
        Debug.Log("[ResetProcess] 시작");
        isResetting = true;

        // 1. 진행 중인 작업 중단
        Debug.Log("[ResetProcess] 코루틴 중지 시도");
        TranslationManager.Instance.StopAllCoroutines();
        
        // 2. TranslationEventHandler 리셋
        var handler = TranslationEventHandler.Instance;
        if (handler == null)
        {
            Debug.LogError("[ResetProcess] TranslationEventHandler가 null입니다.");
            isResetting = false;
            yield break;
        }
        
        Debug.Log("[ResetProcess] 스피커 및 방 상태 리셋");
        handler.ResetSpeaker();
        handler.SetRoomReadyState(false);
        
        // 3. 방장 체크 및 재연결
        Debug.Log($"[ResetProcess] 방장 체크 - RequesterId: {requesterId}, OwnerId: {photonView.Owner.UserId}");
        if (photonView.Owner.UserId == requesterId)
        {
            try
            {
                Debug.Log("[ResetProcess] 방장이 재연결 시도");
                TranslationManager.Instance.Reconnect();
            }
            catch (Exception e)
            {
                Debug.LogError($"[ResetProcess] 재연결 중 예외 발생: {e.Message}");
                handler.HandleError("재연결 실패. 다시 시도해주세요.");
                isResetting = false;
                yield break;
            }

            // 연결 대기 로직을 try-catch 밖으로 이동
            float waitTime = 0;
            bool connectionSuccess = false;
            
            while (waitTime < RESET_TIMEOUT)
            {
                Debug.Log($"[ResetProcess] 연결 대기 중... ({waitTime}/{RESET_TIMEOUT})");
                if (TranslationManager.Instance.IsConnected)
                {
                    connectionSuccess = true;
                    Debug.Log("[ResetProcess] 연결 성공");
                    break;
                }
                waitTime += Time.deltaTime;
                yield return null;
            }

            if (!connectionSuccess)
            {
                Debug.LogError("[ResetProcess] 재연결 시간 초과");
                handler.HandleError("재연결 시간 초과. 다시 시도해주세요.");
                isResetting = false;
                yield break;
            }

            try
            {
                // 새 방 생성
                TranslationManager.Instance.CreateRoom(requesterId, 
                    CashedDataFromDatabase.Instance.playerLanguage.language);
            }
            catch (Exception e)
            {
                Debug.LogError($"[ResetProcess] 방 생성 중 예외 발생: {e.Message}");
                handler.HandleError("방 생성 실패. 다시 시도해주세요.");
                isResetting = false;
                yield break;
            }
        }
        
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
}
