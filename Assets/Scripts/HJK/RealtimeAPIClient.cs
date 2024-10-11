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

        // 'M' 키를 눌러서 마이크로 녹음 시작
        if (Input.GetKeyDown(KeyCode.M) && isConnected)
        {
            StartRecording();
        }

        // 'S' 키를 눌러서 녹음 종료 및 서버로 전송
        if (Input.GetKeyDown(KeyCode.S) && isConnected)
        {
            StopRecordingAndSend();
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
            isConnected = true;
            Debug.Log("WebSocket connected!");

            // 서버로부터 메시지 수신 시작
            await ReceiveMessagesAsync(); // await 추가
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
            isConnected = false;
            Debug.Log("WebSocket connection closed.");
        }
    }

    // 녹음 시작
    private void StartRecording()
    {
        audioStream = new MemoryStream();
        AudioClip audioClip = Microphone.Start(null, false, 10, 24000); // 24kHz 샘플링
        Debug.Log("Recording started...");
    }

    // 녹음 종료 및 서버로 전송
    private void StopRecordingAndSend()
    {
        Microphone.End(null);
        byte[] audioData = audioStream.ToArray();
        SendAudioToServer(audioData);
        Debug.Log("Recording stopped and sent to server.");
    }

    // 서버로 오디오 데이터 전송
    private async void SendAudioToServer(byte[] audioData)
    {
        // 오디오 데이터를 Base64로 인코딩하여 서버로 전송
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

    // 메시지 전송 메서드
    private async Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.UTF8.GetBytes(message);
        await ws.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, cts.Token);
        Debug.Log("Message sent to server");
    }

    // 서버 메시지 수신 메서드
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
                // 서버의 응답을 처리 (예: 텍스트 응답)
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

    // 서버로부터 받은 오디오 데이터를 재생
    private void PlayAudioResponse(byte[] audioData, int length)
    {
        byte[] receivedAudio = new byte[length];
        Array.Copy(audioData, receivedAudio, length);

        // PCM 데이터를 float 배열로 변환
        float[] audioFloatData = ConvertByteArrayToFloatArray(receivedAudio);

        // AudioClip 생성 및 재생
        AudioClip audioClip = AudioClip.Create("ReceivedAudio", audioFloatData.Length, 1, 24000, false);
        audioClip.SetData(audioFloatData, 0);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    // PCM 바이트 배열을 float 배열로 변환하는 메서드
    private float[] ConvertByteArrayToFloatArray(byte[] byteArray)
    {
        int floatCount = byteArray.Length / 2; // 16-bit PCM이므로 2 바이트당 1 float
        float[] floatArray = new float[floatCount];

        for (int i = 0; i < floatCount; i++)
        {
            short value = BitConverter.ToInt16(byteArray, i * 2);
            floatArray[i] = value / 32768f; // short 값을 float로 정규화
        }

        return floatArray;
    }

    private void OnDestroy()
    {
        CloseWebSocketConnection();
        cts.Cancel();
    }
}
