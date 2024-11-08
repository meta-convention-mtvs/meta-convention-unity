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

    public void ProcessServerMessage(string message)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);

        switch (data["type"] as string)
        {
            case "conversation.text.delta":
                DistributePartialTranslatedText(data);
                break;
            case "conversation.audio.delta":
                DistributePartialTranslatedAudio(data);
                break;
            case "conversation.text.done":
                DistributeCompleteTranslatedText(data);
                break;
            case "conversation.audio.done":
                DistributeCompleteTranslatedAudio(data);
                break;
            case "conversation.approved_speech":
                HandleApprovedSpeech(data);
                break;
            default:
                Debug.Log("Unknown message type: " + data["type"]);
                break;
        }
    }

    private void DistributePartialTranslatedText(Dictionary<string, object> data)
    {
        // 텍스트 전달 로직
    }

    private void DistributePartialTranslatedAudio(Dictionary<string, object> data)
    {
        if (data.TryGetValue("audio", out object audioObj))
        {
            string base64Audio = audioObj as string;
            var translators = FindObjectsOfType<PlayerTranslator>();
            foreach (var translator in translators)
            {
                translator.ProcessAudioStream(base64Audio);
            }
        }
    }

    private void DistributeCompleteTranslatedText(Dictionary<string, object> data)
    {
        // 완성된 텍스트 전달 로직
    }

    private void DistributeCompleteTranslatedAudio(Dictionary<string, object> data)
    {
        var translators = FindObjectsOfType<PlayerTranslator>();
        foreach (var translator in translators)
        {
            translator.FinalizeAudioPlayback();
        }
    }

    public void HandleRoomUpdate(bool isReady, List<Dictionary<string, object>> users)
    {
        bool previousState = isRoomReady;
        isRoomReady = isReady;
        currentUsers = users;

        // 로그 출력
        Debug.Log($"Room Update - Ready: {isReady}, Users: {users.Count}");
        
        // 필요한 경우 사용자 목록 처리
        foreach (var user in users)
        {
            string userId = user["userid"] as string;
            string lang = user["lang"] as string;
            Debug.Log($"User: {userId}, Language: {lang}");
        }

        // Ready 상태가 변경되었을 때만 이벤트 발생
        if (previousState != isReady)
        {
            OnRoomReadyStateChanged?.Invoke(isReady);
            UpdateUI(isReady);
        }

        // 사용자 수에 따른 추가 처리
        HandleUserCountChange(users.Count);
    }

    private void UpdateUI(bool isReady)
    {
        // 발언 가능 상태 UI 업데이트
        var translators = FindObjectsOfType<PlayerTranslator>();
        foreach (var translator in translators)
        {
            if (translator.photonView.IsMine)
            {
                translator.UpdateSpeakUI(isReady);
                break;
            }
        }
    }

    private void HandleUserCountChange(int userCount)
    {
        // 사용자 수에 따른 처리
        if (userCount < 2)
        {
            Debug.Log("통역을 시작하려면 다른 언어 사용자가 필요합니다.");
        }
        else
        {
            Debug.Log("통역 준비가 완료되었습니다.");
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
        var translators = FindObjectsOfType<PlayerTranslator>();
        foreach (var translator in translators)
        {
            if (translator.photonView.Owner.UserId == userId)
            {
                translator.OnSpeechApproved();
                break;
            }
        }
    }

    public void OnErrorOccurred(string errorMessage)
    {
        OnError?.Invoke(errorMessage);

        // 에러에 따른 UI 업데이트
        var translators = FindObjectsOfType<PlayerTranslator>();
        foreach (var translator in translators)
        {
            if (translator.photonView.IsMine)
            {
                translator.HandleError(errorMessage);
            }
        }
    }
}
