using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RealtimeAPIClient : MonoBehaviour
{
    private ClientWebSocket ws;
    private Uri serverUri = new Uri("wss://api.openai.com/v1/realtime?model=gpt-4o-realtime-preview-2024-10-01");
    private CancellationTokenSource cts = new CancellationTokenSource();
    private bool isConnected = false; // 연결 상태를 확인하는 변수

    void Update()
    {
        // 'C' 키를 눌러서 WebSocket 연결 시작
        if (Input.GetKeyDown(KeyCode.C) && !isConnected)
        {
            StartWebSocketConnection();
        }

        // 'D' 키를 눌러서 WebSocket 연결 해제
        if (Input.GetKeyDown(KeyCode.D) && isConnected)
        {
            CloseWebSocketConnection();
        }
    }

    // WebSocket 연결을 시작하는 메서드
    private async void StartWebSocketConnection()
    {
        ws = new ClientWebSocket();

        // 요청 헤더 설정
        ws.Options.SetRequestHeader("Authorization", "Bearer YOUR_API_KEY");
        ws.Options.SetRequestHeader("OpenAI-Beta", "realtime=v1");

        try
        {
            await ws.ConnectAsync(serverUri, cts.Token);
            isConnected = true; // 연결 상태 업데이트
            Debug.Log("WebSocket connected!");

            // 서버에 메시지 전송
            var message = new
            {
                type = "response.create",
                response = new
                {
                    modalities = new[] { "text" },
                    instructions = "Please assist the user."
                }
            };

            string jsonMessage = JsonUtility.ToJson(message);
            await SendMessageAsync(jsonMessage);

            // 메시지 수신 대기
            await ReceiveMessagesAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection failed: {e.Message}");
        }
    }

    // WebSocket 연결을 끊는 메서드
    private async void CloseWebSocketConnection()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            isConnected = false; // 연결 상태 업데이트
            Debug.Log("WebSocket connection closed.");
        }
    }

    // 메시지 전송 메서드
    private async Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cts.Token);
        Debug.Log("Message sent to server");
    }

    // 메시지 수신 메서드
    private async Task ReceiveMessagesAsync()
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult result;

        while (ws.State == WebSocketState.Open)
        {
            result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Debug.Log($"Message received from server: {receivedMessage}");
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Debug.Log("WebSocket closed by server");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            }
        }
    }

    private void OnDestroy()
    {
        CloseWebSocketConnection();
        cts.Cancel();
    }
}
