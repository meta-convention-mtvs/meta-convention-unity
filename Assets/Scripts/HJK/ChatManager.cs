using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ChatManager : MonoBehaviour
{
    public InputField chatInputField;  // Legacy 입력 필드
    public Text chatDisplay;    // Legacy 텍스트 컴포넌트
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
        // AIWebSocket이 설정되지 않았는지 확인
        if (aiWebSocket == null)
        {
            Debug.LogError("AIWebSocket이 설정되지 않았습니다.");
            return;
        }

        // 사용자 메시지 가져오기
        string userMessage = chatInputField.text;
        
        // 메시지가 비어있지 않은 경우에만 처리
        if (!string.IsNullOrEmpty(userMessage))
        {
            // 서버로 텍스트 및 오디오 생성 요청 전송
            aiWebSocket.SendGenerateTextAudio(userMessage);
            
            // 채팅 디스플레이에 사용자 메시지 추가
            chatDisplay.text += "\nuser: " + userMessage;
            
            // 입력 필드 초기화
            chatInputField.text = "";
            
            // 입력 필드에 포커스 설정
            chatInputField.ActivateInputField();
        }
    }

    public void OnReceiveAIResponse(string aiMessage)
    {
        chatDisplay.text += "\nAI: " + aiMessage;
    }
}
