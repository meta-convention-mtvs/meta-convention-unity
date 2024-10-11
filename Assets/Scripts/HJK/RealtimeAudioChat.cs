using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class RealtimeAudioChat : MonoBehaviour
{
    // ClientWebSocket instance to manage connection
    private ClientWebSocket ws;
    // API URL for connecting to the realtime WebSocket server
    private Uri apiUrl = new Uri("wss://api.openai.com/v1/realtime?model=gpt-4o-realtime-preview-2024-10-01");
    private string apiKey = "YOUR_API_KEY"; // Replace with your API Key

    // Queue to store audio data before sending it to the server
    private Queue<byte[]> audioBufferQueue = new Queue<byte[]>();
    private bool isConnected = false;
    private CancellationTokenSource cancellationTokenSource;

    void Start()
    {
        // Initialize WebSocket connection
        ws = new ClientWebSocket();
        cancellationTokenSource = new CancellationTokenSource();

        // Set headers for authentication
        ws.Options.SetRequestHeader("Authorization", "Bearer " + apiKey);
        ws.Options.SetRequestHeader("OpenAI-Beta", "realtime=v1");

        ConnectWebSocket();
    }

    // Connect to the WebSocket server asynchronously
    private async void ConnectWebSocket()
    {
        try
        {
            await ws.ConnectAsync(apiUrl, cancellationTokenSource.Token);
            Debug.Log("Connected to server.");
            isConnected = true;
            ReceiveMessages(); // Start listening for messages from the server
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket Error: " + e.Message);
        }
    }

    // Listen for messages from the server
    private async void ReceiveMessages()
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.Log("Received message: " + message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationTokenSource.Token);
                    isConnected = false;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket Receive Error: " + e.Message);
        }
    }

    void Update()
    {
        // Check for Space key press to send a text message if connected
        if (Input.GetKeyDown(KeyCode.Space) && isConnected)
        {
            // Example: Send text input to server
            SendTextMessage("Hello from Unity!");
        }
        // Check for 'A' key press to send audio data if connected
        else if (Input.GetKeyDown(KeyCode.A) && isConnected)
        {
            // Example: Send audio input to server
            byte[] sampleAudio = GetSampleAudioBytes();
            AppendAudioToBuffer(sampleAudio);
        }
    }

    // Sends a text message to the WebSocket server
    private async void SendTextMessage(string message)
    {
        // Construct the JSON message to send
        var jsonMessage = new
        {
            type = "conversation.item.create",
            item = new
            {
                type = "message",
                role = "user",
                content = new[]
                {
                    new
                    {
                        type = "input_text",
                        text = message
                    }
                }
            }
        };

        string jsonString = JsonUtility.ToJson(jsonMessage);
        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

        try
        {
            await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket Send Error: " + e.Message);
        }
    }

    // Appends audio data to the buffer and sends it to the server
    private async void AppendAudioToBuffer(byte[] audioData)
    {
        // Base64 encode the audio data to send it as text
        string base64Audio = Convert.ToBase64String(audioData);

        var jsonMessage = new
        {
            type = "input_audio_buffer.append",
            audio = base64Audio
        };

        string jsonString = JsonUtility.ToJson(jsonMessage);
        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

        try
        {
            await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket Send Error: " + e.Message);
        }
    }

    // Commits the audio buffer to indicate that the audio input is complete
    private async void CommitAudioBuffer()
    {
        var jsonMessage = new
        {
            type = "input_audio_buffer.commit"
        };

        string jsonString = JsonUtility.ToJson(jsonMessage);
        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

        try
        {
            await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket Send Error: " + e.Message);
        }
    }

    // Generates sample audio bytes for testing purposes
    private byte[] GetSampleAudioBytes()
    {
        // Replace this method with real audio capturing logic
        // Here we just generate some dummy bytes for testing purposes
        byte[] audioBytes = new byte[1024];
        new System.Random().NextBytes(audioBytes);
        return audioBytes;
    }

    // Cleanup method to close the WebSocket connection when the object is destroyed
    private async void OnDestroy()
    {
        if (ws != null && ws.State == WebSocketState.Open)
        {
            try
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                Debug.LogError("WebSocket Close Error: " + e.Message);
            }
        }
    }
}