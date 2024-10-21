using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource;  // 음성 재생을 위한 AudioSource
    private AIWebSocket aiWebSocket; // WebSocket 스크립트 참조

    private AudioClip recordedClip;  // 녹음된 오디오 클립
    private bool isRecording = false;
    private System.Collections.Generic.List<float> audioBuffer = new System.Collections.Generic.List<float>();
    private const int BUFFER_THRESHOLD = 24000; // 약 1초 분량의 오디오 (24kHz 샘플레이트 기준)
    private bool isPlaying = false;
    private string lastRecordedAudioBase64;

    void Start()
    {
        aiWebSocket = GetComponent<AIWebSocket>();
        if (aiWebSocket == null)
        {
            Debug.LogError("AIWebSocket 컴포넌트를 찾을 수 없습니다.");
        }
        audioSource = gameObject.AddComponent<AudioSource>();
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

            float[] samples = new float[recordedClip.samples];
            recordedClip.GetData(samples, 0);
            byte[] audioData = ConvertToByteArray(samples);
            lastRecordedAudioBase64 = Convert.ToBase64String(audioData);
            
            aiWebSocket.SendGenerateTextAudio(lastRecordedAudioBase64);
            
            // 재생 버튼 활성화
            EnablePlaybackButton(true);
        }
    }

    private byte[] ConvertToByteArray(float[] samples)
    {
        byte[] byteArray = new byte[samples.Length * 4];
        System.Buffer.BlockCopy(samples, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    public void HandleAudioDelta(string base64AudioDelta)
    {
        byte[] audioData = System.Convert.FromBase64String(base64AudioDelta);
        
        // PCM 16비트, 24kHz, 1채널, 리틀 엔디안 형식으로 가정
        short[] shortArray = new short[audioData.Length / 2];
        System.Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
        
        // 16비트 정수 배열을 float 배열로 변환 (-1.0f ~ 1.0f 범위)
        float[] samples = new float[shortArray.Length];
        for (int i = 0; i < shortArray.Length; i++)
        {
            samples[i] = shortArray[i] / 32768f;
        }

        // 버퍼에 새 샘플 추가
        audioBuffer.AddRange(samples);

        // 버퍼가 임계값을 초과하고 재생 중이 아니면 재생 시작
        if (audioBuffer.Count >= BUFFER_THRESHOLD && !isPlaying)
        {
            StartCoroutine(PlayBufferedAudio());
        }
    }

    private IEnumerator PlayBufferedAudio()
    {
        isPlaying = true;

        while (audioBuffer.Count > 0)
        {
            int sampleCount = Mathf.Min(audioBuffer.Count, BUFFER_THRESHOLD);
            float[] playbackSamples = audioBuffer.GetRange(0, sampleCount).ToArray();
            audioBuffer.RemoveRange(0, sampleCount);

            AudioClip clip = AudioClip.Create("AI_Response", sampleCount, 1, 24000, false);
            clip.SetData(playbackSamples, 0);

            audioSource.clip = clip;
            audioSource.Play();

            // 클립의 재생이 끝날 때까지 대기
            yield return new WaitForSeconds(clip.length);
        }

        isPlaying = false;
    }

    public void PlayLastRecordedAudio()
    {
        if (!string.IsNullOrEmpty(lastRecordedAudioBase64))
        {
            byte[] audioData = Convert.FromBase64String(lastRecordedAudioBase64);
            float[] samples = ConvertToFloatArray(audioData);
            
            AudioClip clip = AudioClip.Create("RecordedAudio", samples.Length, 1, 44100, false);
            clip.SetData(samples, 0);
            
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.Log("재생할 녹음된 오디오가 없습니다.");
        }
    }

    private float[] ConvertToFloatArray(byte[] byteArray)
    {
        float[] floatArray = new float[byteArray.Length / 4];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    private void EnablePlaybackButton(bool enable)
    {
        // UI에서 재생 버튼을 활성화/비활성화하는 로직
    }
}
