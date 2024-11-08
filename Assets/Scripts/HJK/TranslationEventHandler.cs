using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// AI 통역 서버로부터 받은 메시지를 처리하는 싱글톤 핸들러 클래스
/// - 서버 응답 메시지 분류
/// - 텍스트 통역 처리
/// - 음성 통역 처리
/// - 실시간 통역 상태 관리
/// </summary>
public class TranslationEventHandler : Singleton<TranslationEventHandler>
{
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
        // 방 상태 업데이트 처리
        Debug.Log($"Room Update - Ready: {isReady}, Users: {users.Count}");
        
        // 필요한 경우 사용자 목록 처리
        foreach (var user in users)
        {
            string userId = user["userid"] as string;
            string lang = user["lang"] as string;
            Debug.Log($"User: {userId}, Language: {lang}");
        }

        // UI 업데이트나 다른 게임 로직에 필요한 처리를 여기에 추가
    }
}
