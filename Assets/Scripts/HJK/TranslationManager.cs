using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;

/// <summary>
/// AI 통역 서버와의 웹소켓 통신을 관리하는 싱글톤 매니저 클래스
/// - 서버 연결 관리
/// - 음성 데이터 전송
/// - 발화 상태 관리
/// - 방 생성 및 참여 처리
/// </summary>
public class TranslationManager : Singleton<TranslationManager>
{
    private WebSocket ws;
    private const string Endpoint = "ws://metaai2.iptime.org:4444/translation";

    private bool isCurrentlySpeaking = false;
    private string currentSpeakerId = "";

    public bool CanSpeak(string userId)
    {
        return !isCurrentlySpeaking || currentSpeakerId == userId;
    }

    public void Connect()
    {
        if (ws != null && ws.IsAlive)
            return;

        ws = new WebSocket(Endpoint);
        ws.OnMessage += OnMessageReceived;
        ws.Connect();
    }

    public void CreateRoom(string userId, string language)
    {
        var message = new Dictionary<string, object>
        {
            { "type", "room.create" },
            { "lang", language },
            { "userid", userId }
        };
        Send(message);
    }

    public void JoinRoom(string roomId, string userId, string language)
    {
        var message = new Dictionary<string, object>
        {
            { "type", "room.join" },
            { "roomid", roomId },
            { "lang", language },
            { "userid", userId }
        };
        Send(message);
    }

    public void SendAudioData(string audioData)
    {
        var message = new Dictionary<string, object>
        {
            { "type", "conversation.buffer.add_audio" },
            { "audio", audioData }
        };
        Send(message);
    }

    private void Send(Dictionary<string, object> message)
    {
        if (ws != null && ws.IsAlive)
            ws.Send(JsonUtility.ToJson(message));
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        TranslationEventHandler.Instance.ProcessServerMessage(e.Data);
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
