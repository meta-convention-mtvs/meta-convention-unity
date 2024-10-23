using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private AIWebSocket aiWebSocket;
    [SerializeField] private VoiceManager voiceManager;

    void Start()
    {
        // 1. AI 등록
        string aiId = "AI_001";  // 또는 동적으로 생성
        NetworkManager.Instance.RegisterAI(aiWebSocket, aiId);

        // 2. 사용자 등록
        string userId = "User_001";  // 또는 동적으로 생성
        NetworkManager.Instance.RegisterUser(voiceManager, userId);

        // 3. AI와 사용자 연결
        NetworkManager.Instance.AssignAIToUser(userId, aiId);
    }
}
