using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NetworkManager가 AIWebSocket 인스턴스 관리
public class NetworkManager : Singleton<NetworkManager>
{
    // Dictionary를 선언과 동시에 초기화
    private Dictionary<string, AIWebSocket> aiConnections = new Dictionary<string, AIWebSocket>();
    private Dictionary<string, VoiceManager> userVoiceManagers = new Dictionary<string, VoiceManager>();

    private void Start()
    {
        // Start에서의 초기화는 제거해도 됨
    }

    // 4. 기존 기능 유지
    public void AssignAIToUser(string userId, string aiId)
    {
        if (!aiConnections.ContainsKey(aiId))
        {
            Debug.LogError($"AI ID {aiId}를 찾을 수 없습니다.");
            return;
        }

        if (!userVoiceManagers.ContainsKey(userId))
        {
            Debug.LogError($"User ID {userId}를 찾을 수 없습니다.");
            return;
        }
        aiConnections[aiId].AssignSession(userId);
        userVoiceManagers[userId].SetCurrentAI(aiConnections[aiId]);
    }

    // 5. 새로운 등록 기능 추가
    public void RegisterAI(AIWebSocket aiWebSocket, string aiId)
    {
        if (aiWebSocket == null)
        {
            //Debug.LogError($"AIWebSocket이 null입니다. AI ID: {aiId}");
            return;
        }
        aiWebSocket.Initialize(aiId);
        aiConnections[aiId] = aiWebSocket;
        Debug.Log($"AI 등록 완료: {aiId}");
    }

    public void RegisterUser(VoiceManager voiceManager, string userId)
    {
        if (voiceManager == null)
        {
            //Debug.LogError($"VoiceManager가 null입니다. User ID: {userId}");
            return;
        }

        if (string.IsNullOrEmpty(userId))  // userId null 체크 추가
        {
            //Debug.LogError("User ID가 null이거나 비어있습니다.");
            return;
        }

        voiceManager.Initialize(userId);
        userVoiceManagers[userId] = voiceManager;
        Debug.Log($"사용자 등록 완료: {userId}");
    }
}
