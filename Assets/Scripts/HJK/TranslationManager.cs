using UnityEngine;
using WebSocketSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections;

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
    private const string Endpoint = "ws://metaai2.iptime.org:64444/translation";
    // 1107 추가된 부분
    public string CurrentRoomID { get; private set; } = string.Empty;

    public Action OnConnect, OnJoinRoom;

    private TranslationEventHandler eventHandler;

    private void Start()
    {
        eventHandler = TranslationEventHandler.Instance;
    }

    private void Update()
    {
        if (ws != null && ws.IsAlive)
            print("ws is alive");
    }

    private bool isConnecting = false;

    public void Connect()
    {
        if (ws != null && ws.IsAlive || isConnecting)
            return;

        isConnecting = true;
        ws = new WebSocket(Endpoint);
        ws.OnOpen += Ws_OnOpen;
        ws.OnMessage += OnMessageReceived;
        ws.OnError += Ws_OnError;
        ws.OnClose += Ws_OnClose;
        ws.Connect();
        print("connecting...");
    }

    private void Ws_OnError(object sender, ErrorEventArgs e)
    {
        Debug.LogError($"WebSocket 에러 발생: {e.Message}");
        if (e.Exception != null)
        {
            Debug.LogError($"예외 상세: {e.Exception.Message}");
            Debug.LogError($"스택 트레이스: {e.Exception.StackTrace}");
        }
    }

    private void Ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("WebSocket - Translation 연결 성공");
        OnConnect?.Invoke();
        OnConnect = null;
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

    public void RequestSpeech()
    {
        var message = new Dictionary<string, object>
        {
            { "type", "conversation.request_speech" }
        };
        Send(message);
    }

    public void ClearAudioBuffer()
    {
        var message = new Dictionary<string, object>
        {
            { "type", "conversation.buffer.clear_audio" }
        };
        Send(message);
    }

    public void DoneSpeech()
    {
        var message = new Dictionary<string, object>
        {
            { "type", "conversation.done_speech" }
        };
        Send(message);
    }

    public void LeaveRoom()
    {
        var message = new Dictionary<string, object>
        {
            { "type", "room.leave" }
        };
        Send(message);
        CurrentRoomID = string.Empty;
    }

    private void Send(Dictionary<string, object> message)
    {
        if (ws != null && ws.IsAlive)
        {
            ws.Send(JsonConvert.SerializeObject(message));
            print(message["type"].ToString() + "Send Successfully");
        }
        else
        {
            print(message["type"].ToString() + "Send failed...");
        }
    }

    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(e.Data);
       
        // 원본 데이터와 파싱된 데이터를 모두 출력
        Debug.Log($"Raw message: {e.Data}");
        Debug.Log($"Parsed data: {JsonConvert.SerializeObject(data, Formatting.Indented)}");

        // 서버 에러 처리 추가
        if (data["type"] as string == "server.error")
        {
            HandleServerError(Convert.ToInt32(data["code"]));
            return;
        }

        switch (data["type"] as string)
        {
            // 방 생성 후 입장 성공 시 room.joined 이벤트 처리
            case "room.joined":
                CurrentRoomID = data["roomid"] as string;
                OnJoinRoom?.Invoke();
                break;

            case "room.bye":
                CurrentRoomID = string.Empty;
                break;

            case "room.updated":
                bool isReady = data["ready"] as bool? ?? false;
                List<Dictionary<string, object>> users = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data["users"].ToString());
                
                // 방 상태 업데이트 이벤트 처리
                if (eventHandler != null)
                {
                    eventHandler.HandleRoomUpdate(isReady, users);
                }
                break;
        }

        if (eventHandler != null)
        {
            eventHandler.ProcessServerMessage(e.Data);
        }
    }

    private void HandleServerError(int errorCode)
    {
        string errorMessage = "";
        bool isCritical = false;

        switch (errorCode)
        {
            case 1:
                errorMessage = "치명적 오류, 연결이 중단되었습니다.";
                isCritical = true;
                break;
            case 2:
                errorMessage = "방 생성에 실패했습니다.";
                break;
            case 3:
                errorMessage = "방 참여에 실패했습니다.";
                break;
            case 4:
                errorMessage = "방 퇴장에 실패했습니다.";
                break;
            case 5:
                errorMessage = "발언권 획득에 실패했습니다.";
                break;
            case 6:
                errorMessage = "발언권이 없는 상태에서 음성 입력을 시도했습니다.";
                break;
            default:
                errorMessage = $"알 수 없는 에러가 발생했습니다. (에러 코드: {errorCode})";
                break;
        }

        Debug.LogError($"서버 에러: {errorMessage}");

        // UI에 에러 메시지 표시 (예: 팝업)
        ShowErrorMessage(errorMessage);

        // 치명적 에러인 경우 추가 처리
        if (isCritical)
        {
            HandleCriticalError();
        }
    }

    private void HandleCriticalError()
    {
        // 웹소켓 연결 종료
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }

        // 재연결 시도
        StartCoroutine(ReconnectCoroutine());
    }

    private void ShowErrorMessage(string message)
    {
        // 에러 메시지 UI 표시 로직
        // 예: ErrorPopup.Show(message);
        Debug.LogError(message);  // 임시로 콘솔에 출력

        // 필요한 경우 이벤트를 통해 다른 컴포넌트에 알림
        if (eventHandler != null)
        {
            eventHandler.OnErrorOccurred(message);
        }
    }

    private void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }

    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        isConnecting = false;
        Debug.Log($"WebSocket 연결 종료: {e.Reason}");
        
        // 필요한 경우 재연결 로직 구현
        if (e.Code != 1000) // 정상 종료가 아닌 경우
        {
            StartCoroutine(ReconnectCoroutine());
        }
    }

    private IEnumerator ReconnectCoroutine()
    {
        yield return new WaitForSeconds(5f); // 5초 후 재연결 시도
        Connect();
    }
}
