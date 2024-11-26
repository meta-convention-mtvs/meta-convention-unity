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
    private const string Endpoint = "ws://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/translation";
    //private const string Endpoint = "ws://198.19.249.3:3000/translation";
    // private const string Endpoint = "ws://metaai2.iptime.org:44444/translation";
    
    // 1107 추가된 부분
    public string CurrentRoomID { get; private set; } = string.Empty;

    public Action OnConnect, OnJoinRoom;

    private TranslationEventHandler eventHandler;

    public UnityMainThreadDispatcher dispatcher;
    private void Start()
    {
        eventHandler = TranslationEventHandler.Instance;
        dispatcher = GameObject.FindObjectOfType(typeof(UnityMainThreadDispatcher))as UnityMainThreadDispatcher;
    }

    private bool isConnecting = false;

    // 클래스 상단에 추가
    private Dictionary<int, string> accumulatedText = new Dictionary<int, string>();

    // 연결 상태 확인을 위한 프로퍼티 추가
    private bool _isConnected = false;
    public bool IsConnected => _isConnected;

    // 모든 코루틴 중지
    public void StopAllCoroutines()
    {
        if (dispatcher != null)
        {
            dispatcher.StopAllCoroutines();
        }
        base.StopAllCoroutines();
    }

    // 재연결 메서드
    public void Reconnect()
    {
        Debug.Log("[TranslationManager] Reconnect 시작");
        
        try
        {
            // 1. 누적 텍스트 초기화
            accumulatedText.Clear();
            
            // 2. 기존 웹소켓 정리
            if (ws != null)
            {
                Debug.Log("[TranslationManager] 기존 웹소켓 연결 종료");
                ws.Close();
                ws = null;
            }

            isConnecting = false;
            CurrentRoomID = string.Empty;
            
            Debug.Log("[TranslationManager] 새 연결 시도");
            Connect();
        }
        catch (Exception e)
        {
            Debug.LogError($"[TranslationManager] Reconnect 실패: {e.Message}\n{e.StackTrace}");
            throw;
        }
    }

    // 기존의 Connect 메서드 수정
    public void Connect()
    {
        // Debug.Log("[TranslationManager] Connect method called");
        if (ws != null && ws.IsAlive || isConnecting)
        {
            // Debug.Log("[TranslationManager] Already connected or connecting");
            return;
        }

        try 
        {
            isConnecting = true;
            ws = new WebSocket(Endpoint);
            ws.OnOpen += Ws_OnOpen;
            ws.OnMessage += OnMessageReceived;
            ws.OnError += Ws_OnError;
            ws.OnClose += Ws_OnClose;
            ws.Connect();
            // Debug.Log("[TranslationManager] Connecting...");
        }
        catch (Exception e)
        {
            Debug.LogError($"[TranslationManager] Connection error: {e.Message}");
            isConnecting = false;
            throw;
        }
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
        isConnecting = false;
        _isConnected = true;
        
        // OnConnect 이벤트 호출 추가
        if (OnConnect != null)
        {
            // Debug.Log("[TranslationManager] OnConnect 이벤트 발생");
            OnConnect.Invoke();
        }
    }

    public void CreateRoom(string userId, string language, string orgid)
    {
        var message = new Dictionary<string, object>
        {
            { "type", "room.create" },
            { "lang", language },
            { "userid", userId },
            { "orgid", orgid }
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

    // 이벤트 정의
    public event Action<string> OnRoomJoined;        // 방 입장 성공
    public event Action OnRoomLeft;                  // 방 퇴장
    public event Action<bool, List<Dictionary<string, object>>> OnRoomUpdated;  // 방 상태 업데이트
    public event Action<int, string, string> OnPartialTextReceived;
    public event Action<string> OnCompleteTextReceived;   // 완성된 텍스트 수신
    public event Action<string> OnPartialAudioReceived;   // 부분 오디오 수신
    public event Action OnCompleteAudioReceived;  // 완성된 오디오 수신
    // public event Action<string> OnSpeechApproved;         // 발언권 승인 (userId 전달) // 기존의 것
    public event Action<string> OnError;                  // 에러 발생
    public event Action<int, string, string> OnApprovedSpeech; // order, userid, lang 전달 // 새로 추가
    public event Action<int, string> OnInputAudioDone; // order와 text를 전달
    public event Action<int> OnInputAudioFailed; // order만 전달
    public event Action<string> OnRoomBye;  // roomId를 파라미터로 전달
    


    private void OnMessageReceived(object sender, MessageEventArgs e)
    {
        var message = e.Data;
        dispatcher.Enqueue(() =>
        {
            // Debug.Log($"[TranslationManager] Received message: {message}");
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
            string type = data["type"].ToString();

            switch (type)
            {
                case "server.error":
                    HandleServerError(Convert.ToInt32(data["code"]));
                    break;
                    
                case "room.joined":
                    print("OnMessageReceived: room.joined");
                    CurrentRoomID = data["roomid"] as string;
                    OnRoomJoined?.Invoke(CurrentRoomID);
                    break;
                    
                case "room.bye":
                    // string roomId = data["roomid"].ToString();
                    // Debug.Log($"[TranslationManager] Left room: {roomId}");
                    // CurrentRoomID = string.Empty;
                    OnRoomBye?.Invoke();
                    
                    // WebSocket 연결 종료
                    if (ws != null && _isConnected)
                    {
                        Debug.Log("[TranslationManager] Closing WebSocket connection after leaving room");
                        ws.Close();
                        _isConnected = false;
                        ws = null;
                    }
                    break;
                    
                case "room.updated":
                    bool isReady = data["ready"] as bool? ?? false;
                    List<Dictionary<string, object>> users = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(data["users"].ToString());
                    
                    Debug.Log($"[TranslationManager] Invoking OnRoomUpdated - Ready: {isReady}, Users: {users.Count}");
                    OnRoomUpdated?.Invoke(isReady, users);  // 이벤트 발생
                    break;
                    
                case "conversation.text.delta":
                    int order = Convert.ToInt32(data["order"]);
                    string delta = data["delta"] as string;
                    string userid = data.ContainsKey("userid") ? data["userid"] as string : string.Empty;
                    OnPartialTextReceived?.Invoke(order, delta, userid);
                    break;
                    
                case "conversation.text.done":
                    OnCompleteTextReceived?.Invoke(data["text"] as string);
                    break;
                    
                case "conversation.audio.delta":
                    OnPartialAudioReceived?.Invoke(data["delta"] as string);
                    break;
                    
                case "conversation.audio.done":
                    OnCompleteAudioReceived?.Invoke();
                    break;
                    
                case "conversation.approved_speech":
                    order = Convert.ToInt32(data["order"]);
                    userid = data.ContainsKey("userid") ? data["userid"] as string : string.Empty;
                    string lang = data.ContainsKey("lang") ? data["lang"] as string : string.Empty;
                    OnApprovedSpeech?.Invoke(order, userid, lang);
                    break;
                    
                case "conversation.input_audio.done":
                    order = Convert.ToInt32(data["order"]);
                    string text = data["text"] as string;
                    
                    // order별로 텍스트 누적
                    if (!accumulatedText.ContainsKey(order))
                    {
                        accumulatedText[order] = text;
                    }
                    else
                    {
                        accumulatedText[order] += text;
                    }
                    
                    // 누적된 텍스트로 이벤트 호출
                    OnInputAudioDone?.Invoke(order, accumulatedText[order]);
                    break;

                default:
                    Debug.LogWarning($"Unknown message type: {data["type"]}");
                    break;
            }
        });
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
                errorMessage = "방 성에 실패했습니다.";
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
        Debug.LogError(message);
        OnError?.Invoke(message);  // 직접 이벤트 발생
    }

    private void OnDestroy()
    {
        Debug.Log("[TranslationManager] OnDestroy - 리소스 정리");
        
        if (ws != null)
        {
            ws.OnOpen -= Ws_OnOpen;
            ws.OnClose -= Ws_OnClose;
            ws.OnMessage -= OnMessageReceived;  // OnWebSocketMessage에서 OnMessageReceived로 수정
            ws.OnError -= Ws_OnError;
            
            if (ws.IsAlive)
            {
                ws.Close();
            }
            
            ws = null;
        }
        
        _isConnected = false;
    }

    // Ws_OnClose 메서드 수정
    private void Ws_OnClose(object sender, CloseEventArgs e)
    {
        isConnecting = false;
        Debug.Log($"[TranslationManager] WebSocket connection closed: {e.Reason}");
        
        // 정상 종료가 아닌 경우에만 재연결 시도
        if (e.Code != 1000 && e.Code != 1001) // 1000: 정상 종료, 1001: 진행 중 종료
        {
            Debug.Log("[TranslationManager] Abnormal closure. Starting reconnection...");
            StartCoroutine(ReconnectCoroutine());
        }
    }

    // ReconnectCoroutine 수정
    private IEnumerator ReconnectCoroutine()
    {
        yield return new WaitForSeconds(5f); // 5초 대기
        
        if (!IsConnected && !isConnecting)
        {
            Debug.Log("[TranslationManager] Attempting reconnection...");
            Connect();
        }
    }

    private List<Dictionary<string, object>> _currentUsers = new List<Dictionary<string, object>>();

    public List<Dictionary<string, object>> GetCurrentUsers()
    {
        return _currentUsers;
    }
}
