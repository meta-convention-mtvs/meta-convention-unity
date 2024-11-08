using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

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
    // 1107 추가된 부분
    public string CurrentRoomID { get; private set; } = string.Empty;

    public Action OnConnect, OnJoinRoom;

    public void Connect()
    {
        if (ws != null && ws.IsAlive)
            return;

        ws = new WebSocket(Endpoint);
        ws.OnOpen += Ws_OnOpen;
        ws.OnMessage += OnMessageReceived;
        ws.OnError += Ws_OnError;
        ws.Connect();
        print("connecting...");
    }

    private void Ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError(e.ToString());
    }

    private void Ws_OnOpen(object sender, EventArgs e)
    {
        print("connected: " + e.ToString());
        OnConnect?.Invoke();
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
        {
            ws.Send(JsonUtility.ToJson(message));
            print("Send Successfully");
        }
        print("Send failed...");
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        // 1107 추가된 부분
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);

        print(data);

        // 방 생성 후 입장 성공 시 room.joined 이벤트 처리
        if (data["type"] as string == "room.joined")
        {
            CurrentRoomID = data["roomid"] as string;
            OnJoinRoom?.Invoke();
        }
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
