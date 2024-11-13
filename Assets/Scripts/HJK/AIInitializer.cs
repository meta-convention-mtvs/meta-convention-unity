using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInitializer : MonoBehaviour
{
    [SerializeField] public string aiId; // Inspector에서 설정 가능

    void Start()
    {
        AIWebSocket aiWebSocket = GetComponent<AIWebSocket>();
        if (aiWebSocket != null)
        {
            NetworkManager.Instance.RegisterAI(aiWebSocket, aiId);
        }
        else
        {
            //Debug.LogError("AIWebSocket 컴포넌트를 찾을 수 없습니다!");
        }
    }
}