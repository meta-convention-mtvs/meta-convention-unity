using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class RealtimeVoiceClient : MonoBehaviour
{
    private ClientWebSocket ws;
    private Uri serverUri = new Uri("wss://api.openai.com/v1/realtime?model=gpt-4o-realtime-preview-2024-10-01");
    private CancellationTokenSource cts = new CancellationTokenSource();
    private bool isConnected = false;

    private AudioSource audioSource;
    private MemoryStream audioStream;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

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

        // 'M' Ű�� ������ ����ũ�� ���� ����
        if (Input.GetKeyDown(KeyCode.M) && isConnected)
        {
            StartRecording();
        }

        // 'S' Ű�� ������ ���� ���� �� ������ ����
        if (Input.GetKeyDown(KeyCode.S) && isConnected)
        {
            StopRecordingAndSend();
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
            isConnected = true;
            Debug.Log("WebSocket connected!");

            // �����κ��� �޽��� ���� ����
            await ReceiveMessagesAsync(); // await �߰�
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
            isConnected = false;
            Debug.Log("WebSocket connection closed.");
        }
    }

    // ���� ����
    private void StartRecording()
    {
        audioStream = new MemoryStream();
        AudioClip audioClip = Microphone.Start(null, false, 10, 24000); // 24kHz ���ø�
        Debug.Log("Recording started...");
    }

    // ���� ���� �� ������ ����
    private void StopRecordingAndSend()
    {
        Microphone.End(null);
        byte[] audioData = audioStream.ToArray();
        SendAudioToServer(audioData);
        Debug.Log("Recording stopped and sent to server.");
    }

    // ������ ����� ������ ����
    private async void SendAudioToServer(byte[] audioData)
    {
        // ����� �����͸� Base64�� ���ڵ��Ͽ� ������ ����
        string encodedAudio = Convert.ToBase64String(audioData);

        var eventMessage = new
        {
            type = "conversation.item.create",
            item = new
            {
                type = "message",
                role = "user",
                content = new[]
                {
                    new { type = "input_audio", audio = encodedAudio }
                }
            }
        };

        string jsonMessage = JsonUtility.ToJson(eventMessage);
        await SendMessageAsync(jsonMessage);
    }

    // �޽��� ���� �޼���
    private async Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cts.Token);
        Debug.Log("Message sent to server");
    }

    // ���� �޽��� ���� �޼���
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
                // ������ ������ ó�� (��: �ؽ�Ʈ ����)
            }
            else if (result.MessageType == WebSocketMessageType.Binary)
            {
                Debug.Log("Audio message received from server.");
                PlayAudioResponse(buffer, result.Count);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                Debug.Log("WebSocket closed by server");
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            }
        }
    }

    // �����κ��� ���� ����� �����͸� ���
    private void PlayAudioResponse(byte[] audioData, int length)
    {
        byte[] receivedAudio = new byte[length];
        Array.Copy(audioData, receivedAudio, length);

        // PCM �����͸� float �迭�� ��ȯ
        float[] audioFloatData = ConvertByteArrayToFloatArray(receivedAudio);

        // AudioClip ���� �� ���
        AudioClip audioClip = AudioClip.Create("ReceivedAudio", audioFloatData.Length, 1, 24000, false);
        audioClip.SetData(audioFloatData, 0);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    // PCM ����Ʈ �迭�� float �迭�� ��ȯ�ϴ� �޼���
    private float[] ConvertByteArrayToFloatArray(byte[] byteArray)
    {
        int floatCount = byteArray.Length / 2; // 16-bit PCM�̹Ƿ� 2 ����Ʈ�� 1 float
        float[] floatArray = new float[floatCount];

        for (int i = 0; i < floatCount; i++)
        {
            short value = BitConverter.ToInt16(byteArray, i * 2);
            floatArray[i] = value / 32768f; // short ���� float�� ����ȭ
        }

        return floatArray;
    }

    private void OnDestroy()
    {
        CloseWebSocketConnection();
        cts.Cancel();
    }
}
