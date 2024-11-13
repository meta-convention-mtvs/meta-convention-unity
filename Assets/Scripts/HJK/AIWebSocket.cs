using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using UnityEngine.UI;
using System.Threading.Tasks;

// AIWebSocket 클래스: WebSocket을 통해 AI 서버와 통신하는 기능을 제공합니다.
public class AIWebSocket : MonoBehaviour
{
    public VoiceManager voiceManager;
    public TextItem uiText;
    private WebSocket ws;
    // 서버로부터 받은 메시지를 저장하는 큐
    // 메인 스레드에서 안전하게 처리하기 위해 사용됨
    private Queue<string> messageQueue = new Queue<string>();
    
    private bool isGenerating = false;
    private bool isConnected = false;

    public Text generatingStatusText; // Inspector에서 할당할 Text 컴포넌트

    [SerializeField]
    private string aiId;              // AI 식별자 추가
    [SerializeField]
    private string currentSessionId;  // 현재 세션 ID 추가
    
    // 새로운 프로퍼티 추가
    public string AiId => aiId;
    public string CurrentSessionId => currentSessionId;

    // 세션 할당 메서드 추가
    public void AssignSession(string userId)
    {
        currentSessionId = userId;
        Debug.Log($"AI {aiId}가 사용자 {userId}에게 할당됨");
    }

    // 세션 해제 메서드 추가
    public void UnassignSession()
    {
        currentSessionId = null;
        Debug.Log($"AI {aiId} 세션 해제됨");
    }

    // AI ID 초기화 메서드 추가
    public void Initialize(string newAiId)
    {
        aiId = newAiId;
        Debug.Log($"AI {aiId} 초기화됨");
    }

    public async void Connect(string companyUID)
    {
        ConnectToWebsocket("ws://ec2-3-36-111-173.ap-northeast-2.compute.amazonaws.com:6576/chat", messageQueue, companyUID);
        await WaitForConnection();
        if (isConnected)
        {
            await SendConfigUpdate(companyUID);
        }
    }

    public void ConnectToWebsocket(string websocketAddress, Queue<string> messageQueue, string companyUID)
    {
        ws = new WebSocket(websocketAddress);

        // WebSocket 연결 성공 시 호출되는 이벤트
        ws.OnOpen += (sender, e) => {
            Debug.Log("WebSocket 연결 성공");
            isConnected = true;
            // 연결 성공 후 초기 설정을 서버로 전송
            SendConfigUpdate(companyUID);
        };

        // 서버로부터 메시지를 받았을 때 호출되는 이벤트
        ws.OnMessage += (sender, e) => {
            // 스레드 안전성을 위해 messageQueue에 락을 걸고 데이터를 큐에 추가
            lock (messageQueue)
            {
                // 받은 메시지를 큐에 추가
                messageQueue.Enqueue(e.Data);
            }
        };

        // WebSocket 오류 발생 시 호출되는 이벤트
        ws.OnError += (sender, e) => {
            Debug.LogError($"WebSocket 오류: {e.Message}");
        };

        // WebSocket 연결이 종료될 때 호출되는 이벤트
        ws.OnClose += (sender, e) => {
            Debug.Log($"WebSocket 연결 종료: 코드={e.Code}, 이유={e.Reason}, 청산 요청={e.WasClean}");
            isConnected = false;
        };

        // WebSocket 연결 시작
        ws.Connect();
    }

    // OnDestroy 메서드: 오브젝트가 파괴될 때 WebSocket 연결을 종료합니다.
    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }

    // Update 메서드: 매 프레임마다 실행되며, 수신된 메시지를 처리합니다.
    void Update()
    {
        // 메시지 큐에 메시지가 있는 동안 계속 처리합니다.
        while (messageQueue.Count > 0)
        {
            string message;
            
            // 스레드 안전성을 위해 messageQueue에 락을 겁니다.
            lock(messageQueue)
            {
                // 큐에서 가장 오래된 메시지를 꺼냅니다.
                message = messageQueue.Dequeue();
            }
            
            // 꺼낸 메시지를 처리하는 메서드를 호출합니다.
            // 이 메서드는 메시지의 내용에 따라 적절한 동작을 수행할 것입니다.
            ProcessReceivedMessage(message);
        }
    }

    public async Task SendRequestAsync(dynamic request)
    {
        if (IsWebSocketConnected())
        {
            string jsonMessage = JsonConvert.SerializeObject(request);
            await Task.Run(() => ws.Send(jsonMessage));
            Debug.Log(request.type + " 메시지 전송: " + jsonMessage);
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }
    // SendConfigUpdate 메서드: 서버에 초기 설정을 전송합니다.
    //public async Task SendConfigUpdate()
    //{
    //    if (!isConnected)
    //    {
    //        Debug.LogError("WebSocket이 연결되지 않았습니다.");
    //        return;
    //    }
    //    var configUpdate = new
    //    {
    //        type = "config.update",
    //        org = "cf79ea17-a487-4b27-a20d-bbd11ff885da", // 실제 기업 ID로 교체
    //        llm = "realtime" // 현재 지원되는 LLM
    //    };
    //    await SendRequestAsync(configUpdate);
    //}
    public async Task SendConfigUpdate(string companyUID)
    {
        if (!isConnected)
        {
            Debug.LogError("WebSocket이 연결되지 않았습니다.");
            return;
        }

        // FireAuthManager에서 현재 사용자 ID 가져오기
        string userId = FireAuthManager.Instance.GetCurrentUser().UserId;

        var configUpdate = new
        {
            type = "config.update",
            org = companyUID,
            userid = userId,
            lang = "zh",  // ISO 639 Language Code 형식 사용
            llm = "realtime"
        };
        await SendRequestAsync(configUpdate);
    }

    // SendGenerateTextAudio 메서드: 텍스트와 오디오 생성 요청을 서버에 전송합니다.
    public async Task SendGenerateTextAudio(string text)
    {
        isGenerating = true;
        var request = new
        {
            type = "generate.text_audio",
            text = text
        };
        await SendRequestAsync(request);
    }
    // 매개변수가 없는 SendGenerateTextAudio 메소드
    public async Task SendGenerateTextAudio()
    {        
        isGenerating = true;
        var request = new
        {
            type = "generate.text_audio"
        };
        await SendRequestAsync(request);
        Debug.Log("audio with null text");
    }
    // SendGenerateOnlyText 메서드: 텍스트만 생성 요청을 서버에 전송합니다.
    public async Task SendGenerateOnlyText(string text)
    {
        var request = new
        {
            type = "generate.only_text",
            text = text
        };
        await SendRequestAsync(request);
    }

    // SendGenerateCancel 메서드: 생성 취소 요청을 서버에 전송합니다.
    public async Task SendGenerateCancel()
    {
        if (ws != null && ws.IsAlive)
        {
            if (!isGenerating)
            {
                Debug.Log("현재 생성 중인 작업이 없습니다.");
                return;
            }

            var request = new
            {
                type = "generate.cancel"
            };
            await SendRequestAsync(request);
            Debug.Log("generate.cancel 메시지 전송");
        }
    }

    // SendBufferAddAudio 메서드: 오디오 버퍼에 데이터를 추가하는 요청을 서버에 전송합니다.
    public async Task SendBufferAddAudio(string base64Audio)
    {
        Debug.Log($"[시간] SendBufferAddAudio 시작: Sessio  nId={currentSessionId}, AudioLength={base64Audio?.Length ?? 0}");

        if (string.IsNullOrEmpty(currentSessionId))
        {
            Debug.LogWarning("할당된 세션이 없습니다");
            return;
        }

        var request = new
        {
            type = "buffer.add_audio",
            audio = base64Audio
        };

        Debug.Log($"[시간] SendRequestAsync 호출 직전: RequestType={request.type}");
        await SendRequestAsync(request);
        Debug.Log("[시간] SendBufferAddAudio 완료");
    }

    // SendBufferClearAudio 메서드도 유지
    public async Task SendBufferClearAudio()
    {
        var request = new
        {
            type = "buffer.clear_audio"
        };
        await SendRequestAsync(request);
    }

    private IEnumerator ResetGeneratingStateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (isGenerating)
        {
            Debug.LogWarning("서버로부터 취소 응답을 받지 못했습니다. 상태를 강제로 리셋합니다.");
            isGenerating = false;
            uiText.AddText("\n[생성이 강제 중단되었습니다.]");
        }
    }

    // ProcessReceivedMessage 메서드: 서버로부터 받은 메시지를 처리합니다.
    private void ProcessReceivedMessage(string message)
    {
        var response = JsonConvert.DeserializeObject<dynamic>(message);

        if (response.type == "generated.audio.delta")
        {
            Debug.Log("받은 메시지: {\"type\":\"generated.audio.delta");
        }
        else Debug.Log("받은 메시지: " + message);

        //try
        //{
            
            
            if (response.type == "generated.text.delta")
            {
                uiText.AddText((string)response.delta);
                IsGenerating = true;
            }
            else if (response.type == "generated.text.done")
            {
                uiText.AddText("\n");
                IsGenerating = false;
            }
            else if (response.type == "generated.text.canceled" || response.type == "generated.audio.canceled")
            {                
                Debug.Log(response.type == "generated.text.canceled" ? "텍스트 생성이 취소되었습니다." : "오디오 생성이 취소되었습니다.");
                IsGenerating = false;
                StopAllCoroutines();
            }
            else if (response.type == "generated.audio.delta")
            {
                voiceManager.HandleAudioDelta((string)response.delta);
            }
            else if (response.type == "generated.audio.done")
            {
                Debug.Log("오디오 생성 완료");
            }
            else if (response.type == "server.error")
            {
                int errorCode = (int)response.code;
                Debug.LogError($"서버 오류: 코드={errorCode}");
                switch (errorCode)
                {
                    case 1:
                        Debug.LogError("치명적인 에러. 소켓 연결 종료.");
                        break;
                    case 2:
                        Debug.LogError("config.update를 통해 기업이 설정되지 않음.");
                        break;
                    case 3:
                        Debug.LogError("중복된 답변 생성 요청.");
                        break;
                    case 4:
                        Debug.LogError("필수 값 누락.");
                        break;
                    default:
                        Debug.LogError("알 수 없는 서버 오류.");
                        break;
                }
            }
            else
            {
                Debug.LogWarning($"알 수 없는 메시지 유형: {response.type}");
            }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError($"메시지 처리 중 예외 발생: {ex.Message}");
        //}
    }

    // IsConnected 메서드: WebSocket 연결 상태를 반환합니다.
    public bool IsWebSocketConnected()
    {
        return ws != null && ws.IsAlive;
    }

    // IsGenerating 상태를 외부에서 확인할 수 있는 프로퍼티 추가
    public bool IsGenerating
    {
        get { return isGenerating; }
        private set 
        { 
            isGenerating = value;
            UpdateGeneratingStatusUI();
        }
    }

    // UI 업데이트 메서드
    private void UpdateGeneratingStatusUI()
    {
       if (generatingStatusText != null)
       {
           generatingStatusText.text = "isGenerating: " + (isGenerating ? "true" : "false");
       }
       else
       {
           //Debug.LogError("generatingStatusText가 설정되지 않았습니다.");
       }
    }

    private async Task WaitForConnection()
    {
        int attempts = 0;
        while (!isConnected && attempts < 10)
        {
            await Task.Delay(500);
            attempts++;
        }
        if (!isConnected)
        {
            Debug.LogError("WebSocket 연결 실패");
        }
    }
}
