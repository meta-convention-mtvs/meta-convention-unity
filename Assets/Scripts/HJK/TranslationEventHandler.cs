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

    private PlayerTranslatorWithoutRPC playerTranslator;

    private string currentSpeakerId = "";  // 현재 발언자 ID 추가
    public string CurrentSpeakerId => currentSpeakerId;  // 읽기 전용 프로퍼티

    // 발언자 변경 시 발생하는 이벤트
    public event System.Action<string> OnSpeakerChanged;

    private void Start()
    {
        Debug.Log("[TranslationEventHandler] Start method called");
        playerTranslator = FindObjectOfType<PlayerTranslatorWithoutRPC>();
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
        manager.OnPartialTextReceived += DistributePartialTranslatedText;
        // manager.OnSpeechApproved += HandleApprovedSpeech; // 기존의 것
        manager.OnApprovedSpeech += HandleApprovedSpeech; // 새로 추가
        manager.OnInputAudioDone += HandleInputAudioDone;
        manager.OnPartialTextReceived += HandleTextDelta;
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
            manager.OnApprovedSpeech -= HandleApprovedSpeech;
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
        if (playerTranslator != null)
        {
            playerTranslator.ProcessAudioStream(base64Audio);
        }
    }

    private void DistributeCompleteTranslatedAudio()
    {
        if (playerTranslator != null)
        {
            playerTranslator.FinalizeAudioPlayback();
        }
        
        // HandleAudioDone의 로직을 여기로 통합
        Debug.Log("[TranslationEventHandler] Audio playback completed");
        currentSpeakerId = "";
        OnSpeakerChanged?.Invoke("");
    }
    // 부분 번역된 텍스트를 분배하는 메서드
    private void DistributePartialTranslatedText(int order, string partialText)
    {
        if (playerTranslator != null)
        {
            playerTranslator.UpdatePartialTranslatedText(order, partialText);
        }
    }

    // 기존의 것(HandleApprovedSpeech())

    // private void HandleApprovedSpeech(string userId)
    // {
    //     Debug.Log($"[TranslationEventHandler] Speech approved for user: {userId}");
    //     currentSpeakerId = userId;
    //     OnSpeakerChanged?.Invoke(userId);
    //     if (playerTranslator != null)
    //     {
    //         playerTranslator.OnSpeechApproved(userId);
    //     }
    // }

    private void HandleApprovedSpeech(int order, string userid, string lang)
    {
        currentSpeakerId = userId;
        // OnSpeakerChanged?.Invoke(userId); // 이건 아직 사용하지 않음
        if (playerTranslator != null)
        {
            playerTranslator.OnApprovedSpeech(order, userid, lang);
        }
    }

    private void HandleInputAudioDone(int order, string text)
    {
        if (playerTranslator != null)
        {
            playerTranslator.OnInputAudioDone(order, text);
        }
    }

    private void HandleTextDelta(int order, string delta)
    {
        if (playerTranslator != null)
        {
            playerTranslator.UpdatePartialTranslatedText(order, delta);
        }
    }

    private void HandleError(string errorMessage)
    {
        OnError?.Invoke(errorMessage);
        if (playerTranslator != null)
        {
            playerTranslator.HandleError(errorMessage);
        }
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

    // 발언자 상태 초기화 메서드
    public void ResetSpeaker()
    {
        currentSpeakerId = "";
        OnSpeakerChanged?.Invoke("");
    }
}
