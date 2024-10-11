//using UnityEngine;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEngine.Windows;
//using UnityEngine.Audio;

//public class RealtimeAudioChat : MonoBehaviour
//{
//    // ClientWebSocket instance to manage connection
//    private ClientWebSocket ws;
//    // API URL for connecting to the realtime WebSocket server
//    private Uri apiUrl = new Uri("wss://api.openai.com/v1/realtime?model=gpt-4o-realtime-preview-2024-10-01");
//    private string apiKey = "YOUR_API_KEY"; // Replace with your API Key

//    // Queue to store audio data before sending it to the server
//    private Queue<byte[]> audioBufferQueue = new Queue<byte[]>();
//    private bool isConnected = false;
//    private CancellationTokenSource cancellationTokenSource;
//    private AudioSource audioSource;
//    private bool isRecording = false;
//    private bool isUserSpeaking = false;
//    private float silenceThreshold = 0.01f;
//    private int sampleRate = 44100;
//    private int recordingLength = 10;

//    void Start()
//    {
//        // Initialize WebSocket connection
//        ws = new ClientWebSocket();
//        cancellationTokenSource = new CancellationTokenSource();

//        // Set headers for authentication
//        ws.Options.SetRequestHeader("Authorization", "Bearer " + apiKey);
//        ws.Options.SetRequestHeader("OpenAI-Beta", "realtime=v1");

//        ConnectWebSocket();

//        // Initialize audio source for playing received audio
//        audioSource = gameObject.AddComponent<AudioSource>();
//    }

//    // Connect to the WebSocket server asynchronously
//    private async void ConnectWebSocket()
//    {
//        try
//        {
//            await ws.ConnectAsync(apiUrl, cancellationTokenSource.Token);
//            Debug.Log("Connected to server.");
//            isConnected = true;
//            ReceiveMessages(); // Start listening for messages from the server
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("WebSocket Error: " + e.Message);
//        }
//    }

//    // Listen for messages from the server
//    private async void ReceiveMessages()
//    {
//        var buffer = new byte[1024 * 4];

//        try
//        {
//            while (ws.State == WebSocketState.Open)
//            {
//                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
//                if (result.MessageType == WebSocketMessageType.Text)
//                {
//                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
//                    Debug.Log("Received message: " + message);
//                }
//                else if (result.MessageType == WebSocketMessageType.Binary)
//                {
//                    // Handle received audio data
//                    byte[] audioData = new byte[result.Count];
//                    Array.Copy(buffer, audioData, result.Count);
//                    PlayReceivedAudio(audioData);
//                }
//                else if (result.MessageType == WebSocketMessageType.Close)
//                {
//                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationTokenSource.Token);
//                    isConnected = false;
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("WebSocket Receive Error: " + e.Message);
//        }
//    }

//    void Update()
//    {
//        if (isConnected)
//        {
//            if (!isRecording && !Microphone.IsRecording(null))
//            {
//                StartRecording();
//            }
//            else if (isRecording)
//            {
//                MonitorAudioInput();
//            }
//        }

//        // Check for 'D' key press to disconnect from the server
//        if (Input.GetKeyDown(KeyCode.D) && isConnected)
//        {
//            DisconnectWebSocket();
//        }
//    }

//    // Start recording audio from the microphone continuously
//    private void StartRecording()
//    {
//        if (Microphone.devices.Length > 0)
//        {
//            isRecording = true;
//            recordedClip = Microphone.Start(null, true, recordingLength, sampleRate);
//            Debug.Log("Recording started.");
//        }
//        else
//        {
//            Debug.LogError("No microphone devices found.");
//        }
//    }

//    // Monitor audio input and handle user speaking detection
//    private void MonitorAudioInput()
//    {
//        if (recordedClip == null)
//            return;

//        float[] samples = new float[recordedClip.samples * recordedClip.channels];
//        recordedClip.GetData(samples, 0);

//        float maxVolume = 0f;
//        foreach (float sample in samples)
//        {
//            maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample));
//        }

//        // Detect if user starts speaking
//        if (maxVolume > silenceThreshold && !isUserSpeaking)
//        {
//            isUserSpeaking = true;
//            Debug.Log("User started speaking.");
//            SendAudioBuffer(samples);
//        }
//        // Detect if user stops speaking
//        else if (maxVolume <= silenceThreshold && isUserSpeaking)
//        {
//            isUserSpeaking = false;
//            Debug.Log("User stopped speaking.");
//            CommitAudioBuffer();
//        }
//    }

//    // Convert AudioClip data to PCM byte array and send to server
//    private void SendAudioBuffer(float[] samples)
//    {
//        byte[] audioData = ConvertAudioClipToPCM(samples);
//        AppendAudioToBuffer(audioData);
//    }

//    // Convert AudioClip data to PCM byte array
//    private byte[] ConvertAudioClipToPCM(float[] samples)
//    {
//        byte[] pcmData = new byte[samples.Length * 2];
//        int offset = 0;
//        foreach (float sample in samples)
//        {
//            short intSample = (short)(sample * short.MaxValue);
//            byte[] bytes = BitConverter.GetBytes(intSample);
//            pcmData[offset++] = bytes[0];
//            pcmData[offset++] = bytes[1];
//        }
//        return pcmData;
//    }

//    // Sends a text message to the WebSocket server
//    private async void SendTextMessage(string message)
//    {
//        // Construct the JSON message to send
//        var jsonMessage = new
//        {
//            type = "conversation.item.create",
//            item = new
//            {
//                type = "message",
//                role = "user",
//                content = new[]
//                {
//                    new
//                    {
//                        type = "input_text",
//                        text = message
//                    }
//                }
//            }
//        };

//        string jsonString = JsonUtility.ToJson(jsonMessage);
//        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

//        try
//        {
//            await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("WebSocket Send Error: " + e.Message);
//        }
//    }

//    // Appends audio data to the buffer and sends it to the server
//    private async void AppendAudioToBuffer(byte[] audioData)
//    {
//        // Base64 encode the audio data to send it as text
//        string base64Audio = Convert.ToBase64String(audioData);

//        var jsonMessage = new
//        {
//            type = "input_audio_buffer.append",
//            audio = base64Audio
//        };

//        string jsonString = JsonUtility.ToJson(jsonMessage);
//        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

//        try
//        {
//            await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("WebSocket Send Error: " + e.Message);
//        }
//    }

//    // Commits the audio buffer to indicate that the audio input is complete
//    private async void CommitAudioBuffer()
//    {
//        var jsonMessage = new
//        {
//            type = "input_audio_buffer.commit"
//        };

//        string jsonString = JsonUtility.ToJson(jsonMessage);
//        byte[] bytesToSend = Encoding.UTF8.GetBytes(jsonString);

//        try
//        {
//            await ws.SendAsync(new ArraySegment<byte>(bytesToSend), WebSocketMessageType.Text, true, cancellationTokenSource.Token);
//        }
//        catch (Exception e)
//        {
//            Debug.LogError("WebSocket Send Error: " + e.Message);
//        }
//    }

//    // Play received audio data
//    private void PlayReceivedAudio(byte[] audioData)
//    {
//        AudioClip clip = AudioClip.Create("ReceivedAudio", audioData.Length / 2, 1, sampleRate, false);
//        float[] samples = new float[audioData.Length / 2];

//        for (int i = 0; i < samples.Length; i++)
//        {
//            samples[i] = BitConverter.ToInt16(audioData, i * 2) / (float)short.MaxValue;
//        }

//        clip.SetData(samples, 0);
//        audioSource.clip = clip;
//        audioSource.Play();
//    }

//    // Cleanup method to close the WebSocket connection when the object is destroyed
//    private async void OnDestroy()
//    {
//        await DisconnectWebSocket();
//    }

//    // Disconnect the WebSocket connection
//    private async void DisconnectWebSocket()
//    {
//        if (ws != null && ws.State == WebSocketState.Open)
//        {
//            try
//            {
//                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", cancellationTokenSource.Token);
//                Debug.Log("Disconnected from server.");
//                isConnected = false;
//            }
//            catch (Exception e)
//            {
//                Debug.LogError("WebSocket Close Error: " + e.Message);
//            }
//        }
//    }
//}