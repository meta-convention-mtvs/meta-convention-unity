using System;
using System.IO;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

public class VirtualAssistant : MonoBehaviour
{
    private WebSocket ws;
    public AudioSource audioSource; // ���� �Է� �� ����� ���� ����� �ҽ�
    private string apiKey = "APIKEY"; // OpenAI API Ű

    private bool isRecording = false; // ���� ���� ������ ����
    private const float silenceThreshold = 0.01f; // ���� Ȱ���� ������ ���ذ�

    void Start()
    {
        // WebSocket ���� ����
        string url = "wss://api.openai.com/v1/realtime?model=gpt-4o-realtime-preview-2024-10-01";
        ws = new WebSocket(url);

        ws.SetCredentials(apiKey, "", true);
        ws.OnOpen += (sender, e) => {
            Debug.Log("������ ����Ǿ����ϴ�.");
        };

        ws.OnMessage += (sender, e) => {
            HandleResponse(e.Data);
        };

        ws.OnError += (sender, e) => {
            Debug.LogError("WebSocket Error: " + e.Message);
        };

        ws.Connect();
    }

    void Update()
    {
        if (Microphone.IsRecording(null))
        {
            float[] samples = new float[128];
            audioSource.clip.GetData(samples, Microphone.GetPosition(null) - 128);
            float maxVolume = 0f;

            foreach (float sample in samples)
            {
                maxVolume = Mathf.Max(maxVolume, Mathf.Abs(sample));
            }

            // ����ڰ� ���� ���� ��� (���� Ȱ���� �������� ����)
            if (maxVolume < silenceThreshold && isRecording)
            {
                StopRecording();
            }
        }
        else
        {
            // ����ڰ� ���� ������ ��� (���� Ȱ�� ������)
            if (!isRecording && Input.GetAxis("Vertical") > 0.1)
            {
                StartRecording();
            }
        }
    }

    // AI ���� ó��
    private void HandleResponse(string data)
    {
        // ���⼭ AI ������ ó���ϰ�, �ʿ�� ���� ��� �� ĳ���� �ִϸ��̼� ����
        var responseObj = JsonConvert.DeserializeObject<AIResponse>(data);
        if (responseObj.item.content[0].type == "input_audio")
        {
            byte[] audioData = Convert.FromBase64String(responseObj.item.content[0].audio);
            PlayAudio(audioData);
        }
    }

    // ����ũ�� ���� �Է� ����
    private void StartRecording()
    {
        Debug.Log("���� ����");
        isRecording = true;
        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        audioSource.loop = false;
    }

    // ����ũ ���� �Է� ���� �� ����� ������ ó��
    private void StopRecording()
    {
        Debug.Log("���� ����");
        isRecording = false;
        Microphone.End(null);

        // ������ �����͸� WAV�� ��ȯ �� ����
        var audioData = WavUtility.FromAudioClip(audioSource.clip);
        SendAudioToAI(audioData);
    }

    // ����� �����͸� OpenAI API�� ����
    private void SendAudioToAI(byte[] audioData)
    {
        var base64Audio = Convert.ToBase64String(audioData);
        var eventObj = new
        {
            type = "conversation.item.create",
            item = new
            {
                type = "message",
                role = "user",
                content = new[]
                {
                    new { type = "input_audio", audio = base64Audio }
                }
            }
        };

        string jsonMessage = JsonConvert.SerializeObject(eventObj);
        ws.Send(jsonMessage);
    }

    // AI ���� ���� ���
    private void PlayAudio(byte[] audioData)
    {
        var audioClip = WavUtility.ToAudioClip(audioData);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }
}

public class AIResponse
{
    public Item item;

    public class Item
    {
        public Content[] content;
    }

    public class Content
    {
        public string type;
        public string audio;
    }
}
