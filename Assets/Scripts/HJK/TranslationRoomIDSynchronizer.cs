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

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

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

    public override void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        base.OnOwnershipTransfered(targetView, previousOwner);
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
        if (isResetting)
        {
            return;
        }

        isResetting = true;
        StartCoroutine(ResetProcess(requesterId));
    }

    private IEnumerator ResetProcess(string requesterId)
    {
        var users = TranslationManager.Instance.GetCurrentUsers();
        if (users != null && users.Count > 0)
        {
            var firstUser = users[0];
            string firstUserId = firstUser["userid"].ToString();
            
            // 방장인 경우: 직접 리셋 수행
            if (string.Equals(firstUserId, TranslationManager.Instance.UserId))
            {
                Debug.Log("[ResetProcess] 방장이 리셋 실행");
                yield return new WaitForSeconds(0.5f);
                TranslationManager.Instance.Reconnect();
                // 새로운 방 생성
                TranslationManager.Instance.OnConnect += () => {
                    CreateRoom();
                    TranslationManager.Instance.OnConnect -= CreateRoom;
                };
            }
            // 방장이 아닌 경우: 방장의 리셋 완료 대기
            else
            {
                Debug.Log("[ResetProcess] 방장의 리셋 대기 중");
                yield return new WaitForSeconds(2f);  // 방장의 리셋 및 새 방 생성 대기
                TranslationManager.Instance.OnRoomJoined += (roomId) => {
                    JoinRoom(roomId);
                    TranslationManager.Instance.OnRoomJoined -= JoinRoom;
                };
            }
        }
        
        isResetting = false;
        Debug.Log("[ResetProcess] 완료");
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
