using System;
using System.Collections;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class AIWebSocket : MonoBehaviour
{
    public ChatManager chatManager;
    public VoiceManager voiceManager;

    private WebSocket ws;

    void Start()
    {
        ws = new WebSocket("ws://metaai2.iptime.org:44444");

        ws.OnOpen += (sender, e) => {
            Debug.Log("WebSocket 연결 성공");
            SendConfigUpdate(); // 연결 성공 후 초기 설정 전송
        };

        ws.OnMessage += (sender, e) => {
            Debug.Log("서버로부터 수신한 데이터: " + e.Data);
            ProcessReceivedMessage(e.Data);
        };

        ws.OnError += (sender, e) => {
            Debug.LogError($"WebSocket 오류: {e.Message}, 코드: {e.Code}");
        };

        ws.OnClose += (sender, e) => {
            Debug.Log($"WebSocket 연결 종료: 코드={e.Code}, 이유={e.Reason}, 청산 요청={e.WasClean}");
        };

        ws.Connect();
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }

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
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

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
            Debug.Log("generate.cancel 메시지 전송");
        }
        else
        {
            Debug.LogError("WebSocket이 연결되지 않았거나 활성 상태가 아닙니다.");
        }
    }

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
            }
            else if (response.type == "generated.audio.delta")
            {
                voiceManager.HandleAudioDelta((string)response.delta);
            }
            else if (response.type == "generated.audio.done")
            {
                Debug.Log("오디오 생성 완료");
            }
            else if (response.type == "generated.text.canceled")
            {
                Debug.Log("텍스트 생성이 취소되었습니다.");
            }
            else if (response.type == "generated.audio.canceled")
            {
                Debug.Log("오디오 생성이 취소되었습니다.");
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

    public bool IsConnected()
    {
        return ws != null && ws.IsAlive;
    }
}
