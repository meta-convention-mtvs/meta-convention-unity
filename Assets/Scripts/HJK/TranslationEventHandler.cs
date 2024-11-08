using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

/// <summary>
/// AI 통역 서버로부터 받은 메시지를 처리하는 싱글톤 핸들러 클래스
/// - 서버 응답 메시지 분류
/// - 텍스트 통역 처리
/// - 음성 통역 처리
/// - 실시간 통역 상태 관리
/// </summary>
public class TranslationEventHandler : Singleton<TranslationEventHandler>
{
    private bool isRoomReady = false;
    public bool IsRoomReady => isRoomReady;  // 읽기 전용 프로퍼티

    // 현재 방의 사용자 정보를 저장
    private List<Dictionary<string, object>> currentUsers = new List<Dictionary<string, object>>();
    
    // Ready 상태 변경 시 호출될 이벤트
    public event System.Action<bool> OnRoomReadyStateChanged;

    // 에러 발생 시 호출될 이벤트
    public event System.Action<string> OnError;

    public PlayerTranslator playerTranslator;

    private void Start()
    {
        Debug.Log("[TranslationEventHandler] Start method called");
        playerTranslator = FindObjectOfType<PlayerTranslator>();
        var manager = TranslationManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[TranslationEventHandler] TranslationManager instance is null!");
            return;
        }
        
        Debug.Log("[TranslationEventHandler] Subscribing to events");
        manager.OnRoomUpdated += HandleRoomUpdate;
        manager.OnPartialAudioReceived += DistributePartialTranslatedAudio;
        manager.OnCompleteAudioReceived += DistributeCompleteTranslatedAudio;
        manager.OnSpeechApproved += HandleApprovedSpeech;
        manager.OnError += HandleError;
        
        Debug.Log("[TranslationEventHandler] Events subscribed successfully");
    }

    private void OnDestroy()
    {
        var manager = TranslationManager.Instance;
        if (manager != null)
        {
            // 이벤트 구독 해제
            manager.OnRoomUpdated -= HandleRoomUpdate;
            manager.OnPartialAudioReceived -= DistributePartialTranslatedAudio;
            manager.OnCompleteAudioReceived -= DistributeCompleteTranslatedAudio;
            manager.OnSpeechApproved -= HandleApprovedSpeech;
            manager.OnError -= HandleError;
        }
    }

    private void HandleRoomUpdate(bool isReady, List<Dictionary<string, object>> users)
    {
        bool previousState = isRoomReady;
        isRoomReady = isReady;
        currentUsers = users;

        // 디버깅을 위한 상세 로그 추가
        Debug.Log($"[HandleRoomUpdate] Ready: {isReady}, Users Count: {users.Count}");
        Debug.Log($"[HandleRoomUpdate] Current Users Detail:");
        foreach (var user in users)
        {
            string userId = user["userid"] as string;
            string lang = user["lang"] as string;
            Debug.Log($"- User ID: {userId}, Language: {lang}");
        }

        // Ready 상태가 변경되었을 때만 이벤트 발생
        if (previousState != isReady)
        {
            Debug.Log($"[HandleRoomUpdate] Ready state changed from {previousState} to {isReady}");
            OnRoomReadyStateChanged?.Invoke(isReady);
            UpdateUI(isReady);
        }

        // 사용자 수에 따른 추가 처리
        HandleUserCountChange(users.Count);
    }

    private void DistributePartialTranslatedAudio(string base64Audio)
    {
        //var translators = FindObjectsOfType<PlayerTranslator>();
        //foreach (var translator in translators)
        //{
        //    translator.ProcessAudioStream(base64Audio);
        //}
        playerTranslator.ProcessAudioStream(base64Audio);
    }

    private void DistributeCompleteTranslatedAudio(string base64Audio)
    {
        //var translators = FindObjectsOfType<PlayerTranslator>();
        //foreach (var translator in translators)
        //{
        //    translator.FinalizeAudioPlayback();
        //}
        playerTranslator.FinalizeAudioPlayback();
    }

    private void HandleApprovedSpeech(string userId)
    {
        //var translators = FindObjectsOfType<PlayerTranslator>();
        //foreach (var translator in translators)
        //{
        //    if (translator.photonView.Owner.UserId == userId)
        //    {
        //        translator.OnSpeechApproved();
        //        break;
        //    }
        //}
        playerTranslator.OnSpeechApproved();
    }

    private void HandleError(string errorMessage)
    {
        OnError?.Invoke(errorMessage);

        //var translators = FindObjectsOfType<PlayerTranslator>();
        //foreach (var translator in translators)
        //{
        //    if (translator.photonView.IsMine)
        //    {
        //        translator.HandleError(errorMessage);
        //    }
        //}
        playerTranslator.HandleError(errorMessage);
    }

    private void UpdateUI(bool isReady)
    {
        // 발언 가능 상태 UI 업데이트
        //var translators = FindObjectsOfType<PlayerTranslator>();
        //foreach (var translator in translators)
        //{
        //    if (translator.photonView.IsMine)
        //    {
        //        translator.UpdateSpeakUI(isReady);
        //        break;
        //    }
        //}
        playerTranslator.UpdateSpeakUI(isReady);
    }

    private void HandleUserCountChange(int userCount)
    {
        Debug.Log($"[HandleUserCountChange] Current user count: {userCount}");
        
        // 사용자 수에 따른 처리
        if (userCount < 2)
        {
            Debug.Log("[HandleUserCountChange] 통역을 시작하려면 다른 언어 사용자가 필요합니다.");
        }
        else
        {
            Debug.Log("[HandleUserCountChange] 통역 준비가 완료되었습니다.");
        }
    }

    // 현재 방의 사용자 수 반환
    public int GetCurrentUserCount()
    {
        return currentUsers.Count;
    }

    // 특정 언어를 사용하는 사용자가 있는지 확인
    public bool HasUserWithLanguage(string language)
    {
        return currentUsers.Any(user => (user["lang"] as string) == language);
    }

    // 특정 사용자의 언어 가져오기
    public string GetUserLanguage(string userId)
    {
        var user = currentUsers.FirstOrDefault(u => (u["userid"] as string) == userId);
        return user?["lang"] as string;
    }

    private void HandleApprovedSpeech(Dictionary<string, object> data)
    {
        string userId = data["userid"] as string;
        //var translators = FindObjectsOfType<PlayerTranslator>();
        //foreach (var translator in translators)
        //{
        //    if (translator.photonView.Owner.UserId == userId)
        //    {
        //        translator.OnSpeechApproved();
        //        break;
        //    }
        //}
        playerTranslator.OnSpeechApproved();
    }
}
