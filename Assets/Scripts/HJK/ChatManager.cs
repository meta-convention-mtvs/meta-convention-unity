using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField chatInputField;  // TextMeshPro 입력 필드
    public TextMeshProUGUI chatDisplay;    // TextMeshPro 텍스트 컴포넌트
    public AIWebSocket aiWebSocket;         // WebSocket 연결 스크립트 참조

    void Start()
    {
        if (aiWebSocket == null)
        {
            aiWebSocket = GetComponent<AIWebSocket>();
            if (aiWebSocket == null)
            {
                Debug.LogError("AIWebSocket 컴포넌트를 찾을 수 없습니다.");
                return;
            }
        }
        
        // 엔터 키 입력 이벤트 추가
        chatInputField.onSubmit.AddListener(delegate { OnSendChat(); });
    }

    void Update()
    {
        // 엔터 키를 눌렀을 때 메시지 전송 (InputField가 포커스된 상태에서)
        if (Input.GetKeyDown(KeyCode.Return) && chatInputField.isFocused)
        {
            OnSendChat();
        }
        
        if (aiWebSocket != null)
        {
            // 디버깅용: WebSocket 상태 로그 출력을 주석 처리하거나 필요 시 사용
            // Debug.Log($"WebSocket 상태: {(aiWebSocket.IsConnected() ? "연결됨" : "연결 끊김")}");
        }
    }

    public void OnSendChat()
    {
        if (aiWebSocket == null)
        {
            Debug.LogError("AIWebSocket이 설정되지 않았습니다.");
            return;
        }

        string userMessage = chatInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            aiWebSocket.SendGenerateTextAudio(userMessage); // generate.text_audio 메시지 전송
            chatDisplay.text += "\nuser: " + userMessage;
            chatInputField.text = "";
            chatInputField.ActivateInputField();
        }
    }

    public void OnReceiveAIResponse(string aiMessage)
    {
        chatDisplay.text += "\nAI: " + aiMessage;
    }
}
