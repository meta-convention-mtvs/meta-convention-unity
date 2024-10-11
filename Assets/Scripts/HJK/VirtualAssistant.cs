using System;
using System.IO;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

public class VirtualAssistant : MonoBehaviour
{
    private WebSocket ws;
    public AudioSource audioSource; // 음성 입력 및 출력을 위한 오디오 소스
    private string apiKey = "APIKEY"; // OpenAI API 키

    private bool isRecording = false; // 현재 녹음 중인지 여부
    private const float silenceThreshold = 0.01f; // 음성 활동을 감지할 기준값

    void Start()
    {
        // WebSocket 연결 설정
        string url = "wss://api.openai.com/v1/realtime?model=gpt-4o-realtime-preview-2024-10-01";
        ws = new WebSocket(url);

        ws.SetCredentials(apiKey, "", true);
        ws.OnOpen += (sender, e) => {
            Debug.Log("서버에 연결되었습니다.");
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

            // 사용자가 말을 멈춘 경우 (음성 활동이 감지되지 않음)
            if (maxVolume < silenceThreshold && isRecording)
            {
                StopRecording();
            }
        }
        else
        {
            // 사용자가 말을 시작한 경우 (음성 활동 감지됨)
            if (!isRecording && Input.GetAxis("Vertical") > 0.1)
            {
                StartRecording();
            }
        }
    }

    // AI 응답 처리
    private void HandleResponse(string data)
    {
        // 여기서 AI 응답을 처리하고, 필요시 음성 재생 및 캐릭터 애니메이션 연동
        var responseObj = JsonConvert.DeserializeObject<AIResponse>(data);
        if (responseObj.item.content[0].type == "input_audio")
        {
            byte[] audioData = Convert.FromBase64String(responseObj.item.content[0].audio);
            PlayAudio(audioData);
        }
    }

    // 마이크로 음성 입력 시작
    private void StartRecording()
    {
        Debug.Log("녹음 시작");
        isRecording = true;
        audioSource.clip = Microphone.Start(null, true, 10, 44100);
        audioSource.loop = false;
    }

    // 마이크 음성 입력 중지 및 오디오 데이터 처리
    private void StopRecording()
    {
        Debug.Log("녹음 중지");
        isRecording = false;
        Microphone.End(null);

        // 녹음된 데이터를 WAV로 변환 후 전송
        var audioData = WavUtility.FromAudioClip(audioSource.clip);
        SendAudioToAI(audioData);
    }

    // 오디오 데이터를 OpenAI API로 전송
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

    // AI 응답 음성 재생
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
