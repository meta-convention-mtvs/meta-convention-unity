using UnityEngine;
using System;
using System.Linq;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource;  // 음성 재생을 위한 AudioSource
    private AIWebSocket aiWebSocket; // WebSocket 스크립트 참조

    private AudioClip recordedClip;  // 녹음된 오디오 클립
    private bool isRecording = false;

    void Start()
    {
        aiWebSocket = GetComponent<AIWebSocket>();
        if (aiWebSocket == null)
        {
            Debug.LogError("AIWebSocket 컴포넌트를 찾을 수 없습니다.");
        }
    }

    void Update()
    {
        // 스페이스바를 눌렀을 때 녹음 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRecording();
        }

        // 스페이스바에서 손을 뗐을 때 녹음 중지
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopRecordingAndSend();
        }
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            recordedClip = Microphone.Start(null, false, 10, 44100);  // 마이크 녹음 시작
            isRecording = true;
            Debug.Log("녹음 시작");
        }
    }

    public void StopRecordingAndSend()
    {
        if (isRecording)
        {
            Microphone.End(null);  // 녹음 중단
            isRecording = false;
            Debug.Log("녹음 중지");

            // 녹음된 데이터를 서버로 전송
            float[] samples = new float[recordedClip.samples];
            recordedClip.GetData(samples, 0);
            byte[] audioData = ConvertToByteArray(samples);
            string base64AudioData = System.Convert.ToBase64String(audioData);
            aiWebSocket.SendBufferAddAudio(base64AudioData);  // buffer.add_audio 메시지 전송
        }
    }

    private byte[] ConvertToByteArray(float[] samples)
    {
        byte[] byteArray = new byte[samples.Length * 4];
        System.Buffer.BlockCopy(samples, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    // 서버로부터 받은 오디오 데이터를 재생
    public void PlayAIResponseAudio(byte[] audioData)
    {
        float[] samples = new float[audioData.Length / 4];
        System.Buffer.BlockCopy(audioData, 0, samples, 0, audioData.Length);

        AudioClip clip = AudioClip.Create("AI_Response", samples.Length, 1, 44100, false);
        clip.SetData(samples, 0);
        
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void HandleAudioDelta(string base64AudioDelta)
    {
        // Base64로 인코딩된 오디오 델타 데이터를 디코딩하고 재생
        byte[] audioBytes = System.Convert.FromBase64String(base64AudioDelta);
        PlayAIResponseAudio(audioBytes);
    }
}
