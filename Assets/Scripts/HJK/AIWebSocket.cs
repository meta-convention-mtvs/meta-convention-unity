using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// AIWebSocket 클래스: WebSocket을 통해 AI 서버와 통신하는 기능을 제공합니다.
public class AIWebSocket : MonoBehaviour
{
    public ChatManager chatManager;
    public VoiceManager voiceManager;

    private WebSocket ws;
    // 서버로부터 받은 메시지를 저장하는 큐
    // 메인 스레드에서 안전하게 처리하기 위해 사용됨
    private Queue<string> messageQueue = new Queue<string>();
    
    private bool isGenerating = false;
    private bool isCancelled = false;
    
    // Start 메서드: WebSocket 연결을 초기화하고 이벤트 핸들러를 설정합니다.
    void Start()
    {
        ws = new WebSocket("ws://metaai2.iptime.org:44444");

        // WebSocket 연결 성공 시 호출되는 이벤트
        ws.OnOpen += (sender, e) => {
            Debug.Log("WebSocket 연결 성공");
            // 연결 성공 후 초기 설정을 서버로 전송
            SendConfigUpdate(); 
        };

        // 서버로터 메시지를 받았을 때 호출되는 이벤트
        // WebSocket 메시지 수신 이벤트 핸들러
        ws.OnMessage += (sender, e) => {
            // 서버로부터 받은 데이터를 로그에 출력
            Debug.Log("서버로부터 수신한 데이터: " + e.Data);
            
            // 스레드 안전성을 위해 messageQueue에 락을 걸고 데이터를 큐에 추가
            lock(messageQueue)
            {
                // 받은 메시지를 큐에 추가
                // 이렇게 하면 메인 스레드에서 안전하게 메시지를 처리할 수 있음
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

    // SendConfigUpdate 메서드: 서버에 초기 설정을 전송합니다.
    public void SendConfigUpdate()
    {
        if (ws != null && ws.IsAlive)
        {
            var configUpdate = new
            {
                type = "config.update",
                org = "cf79ea17-a487-4b27-a20d-bbd11ff885da", // 실제 기업 ID로 교체
                llm = "realtime" // 현재 지원되는 LLM
            };
            string jsonMessage = JsonConvert.SerializeObject(configUpdate);
            ws.Send(jsonMessage);
            Debug.Log("config.update 메시지 전송");
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    // SendGenerateTextAudio 메서드: 텍스트와 오디오 생성 요청을 서버에 전송합니다.
    public void SendGenerateTextAudio(string text)
    {
        if (ws != null && ws.IsAlive)
        {
            var request = new
            {
                type = "generate.text_audio",
                text = text
            };
            string jsonMessage = JsonConvert.SerializeObject(request);
            ws.Send(jsonMessage);
            Debug.Log("generate.text_audio 메시지 전송: " + jsonMessage);
            isGenerating = true;
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    // SendGenerateOnlyText 메서드: 텍스트만 생성 요청을 서버에 전송합니다.
    public void SendGenerateOnlyText(string text)
    {
        if (ws != null && ws.IsAlive)
        {
            var request = new
            {
                type = "generate.only_text",
                text = text
            };
            string jsonMessage = JsonConvert.SerializeObject(request);
            ws.Send(jsonMessage);
            Debug.Log("generate.only_text 메시지 전송: " + jsonMessage);
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    // SendGenerateCancel 메서드: 생성 취소 요청을 서버에 전송합니다.
    public void SendGenerateCancel()
    {
        if (ws != null && ws.IsAlive)
        {
            var request = new
            {
                type = "generate.cancel"
            };
            string jsonMessage = JsonConvert.SerializeObject(request);
            ws.Send(jsonMessage);
            Debug.Log("generate.cancel 메시지 전송: " + jsonMessage);
            isCancelled = true;
            isGenerating = false;

            // 메시지 큐 초기화
            lock(messageQueue)
            {
                messageQueue.Clear();
            }
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    // SendBufferAddAudio 메서드: 오디오 버퍼에 데이터를 추가하는 요청을 서버에 전송합니다.
    public void SendBufferAddAudio(string base64AudioData)
    {
        if (ws != null && ws.IsAlive)
        {
            var request = new
            {
                type = "buffer.add_audio",
                audio = base64AudioData
            };
            string jsonMessage = JsonConvert.SerializeObject(request);
            ws.Send(jsonMessage);
            Debug.Log("buffer.add_audio 메시지 전송");
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    // SendBufferClearAudio 메서드: 오디오 버퍼를 초기화하는 요청을 서버에 전송합니다.
    public void SendBufferClearAudio()
    {
        if (ws != null && ws.IsAlive)
        {
            var request = new
            {
                type = "buffer.clear_audio"
            };
            string jsonMessage = JsonConvert.SerializeObject(request);
            ws.Send(jsonMessage);
            Debug.Log("buffer.clear_audio 메시지 전송");
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    // ProcessReceivedMessage 메서드: 서버로부터 받은 메시지를 처리합니다.
    private void ProcessReceivedMessage(string message)
    {
        try
        {
            var response = JsonConvert.DeserializeObject<dynamic>(message);
            
            if (response.type == "generated.text.delta")
            {
                chatManager.OnReceiveAIResponse((string)response.delta);
            }
            else if (response.type == "generated.text.done")
            {
                chatManager.OnReceiveAIResponse("\nAI: " + (string)response.text);
                isGenerating = false;
                ResetCancelledState();
            }
            else if (response.type == "generated.audio.delta")
            {
                voiceManager.HandleAudioDelta((string)response.delta);
            }
            else if (response.type == "generated.audio.done")
            {
                Debug.Log("오디오 생성 완료");
            }
            else if (response.type == "generated.text.canceled" || response.type == "generated.audio.canceled")
            {
                Debug.Log(response.type == "generated.text.canceled" ? "텍스트 생성이 취소되었습니다." : "오디오 생성이 취소되었습니다.");
                isGenerating = false;
                ResetCancelledState();
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
        }
        catch (Exception ex)
        {
            Debug.LogError($"메시지 처리 중 예외 발생: {ex.Message}");
        }
    }

    // IsConnected 메서드: WebSocket 연결 상태를 반환합니다.
    public bool IsConnected()
    {
        return ws != null && ws.IsAlive;
    }

    // 매개변수가 없는 SendGenerateTextAudio 메소드
    public void SendGenerateTextAudio()
    {
        if (ws != null && ws.IsAlive)
        {
            var request = new
            {
                type = "generate.text_audio"
            };
            string jsonMessage = JsonConvert.SerializeObject(request);
            ws.Send(jsonMessage);
            Debug.Log("generate.text_audio 메시지 전송 (빈 텍스트): " + jsonMessage);
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

    public bool IsGenerating()
    {
        return isGenerating;
    }

    public bool IsCancelled()
    {
        return isCancelled;
    }

    public void ResetCancelledState()
    {
        isCancelled = false;
    }
}
