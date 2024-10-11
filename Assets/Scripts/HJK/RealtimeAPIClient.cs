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
    private bool isConnected = false; // ���� ���¸� Ȯ���ϴ� ����

    void Update()
    {
        // 'C' Ű�� ������ WebSocket ���� ����
        if (Input.GetKeyDown(KeyCode.C) && !isConnected)
        {
            StartWebSocketConnection();
        }

        // 'D' Ű�� ������ WebSocket ���� ����
        if (Input.GetKeyDown(KeyCode.D) && isConnected)
        {
            CloseWebSocketConnection();
        }
    }

    // WebSocket ������ �����ϴ� �޼���
    private async void StartWebSocketConnection()
    {
        ws = new ClientWebSocket();

        // ��û ��� ����
        ws.Options.SetRequestHeader("Authorization", "Bearer YOUR_API_KEY");
        ws.Options.SetRequestHeader("OpenAI-Beta", "realtime=v1");

        try
        {
            await ws.ConnectAsync(serverUri, cts.Token);
            isConnected = true; // ���� ���� ������Ʈ
            Debug.Log("WebSocket connected!");

            // ������ �޽��� ����
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

            // �޽��� ���� ���
            await ReceiveMessagesAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"WebSocket connection failed: {e.Message}");
        }
    }

    // WebSocket ������ ���� �޼���
    private async void CloseWebSocketConnection()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            isConnected = false; // ���� ���� ������Ʈ
            Debug.Log("WebSocket connection closed.");
        }
    }

    // �޽��� ���� �޼���
    private async Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cts.Token);
        Debug.Log("Message sent to server");
    }

    // �޽��� ���� �޼���
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
