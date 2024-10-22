using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

// VoiceManager 클래스: 음성 녹음, 재생 및 AI와의 통신을 관리합니다.
public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource; // 오디오 재생을 위한 AudioSource 컴포넌트
    public AIWebSocket aiWebSocket; // AI 서버와의 WebSocket 통신을 위한 컴포넌트

    private AudioClip recordedClip; // 녹음된 오디오 클립을 저장
    private bool isRecording = false; // 현재 녹음 중인지 여부
    private List<float> audioBuffer = new List<float>(); // 수신된 오디오 데이터를 버퍼링하기 위한 리스트
    private const int BUFFER_THRESHOLD = 24000; // 오디오 재생을 시작할 버퍼 크기 임계값
    private bool isPlaying = false; // 현재 오디오 재생 중인지 여부
    private string lastRecordedAudioBase64; // 마지막으로 녹음된 오디오의 Base64 인코딩 문자열
    private bool isAudioCancelled = false; // 오디오 생성이 취소되었는지 여부를 나타내는 새로운 변수

    private Coroutine playCoroutine; // 오디오 재생을 위한 코루틴

    // 시작 시 실행되는 메서드
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 컴포넌트 추가
    }

    // 매 프레임마다 실행되는 메서드
    void Update()
    {
        // 'M' 키를 눌러 녹음 시작
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isRecording && !aiWebSocket.IsGenerating())
            {
                StartRecording();
            }
        }
        
        // 'M' 키를 떼면 녹음 중지 및 전송
        if (Input.GetKeyUp(KeyCode.M))
        {
            if (isRecording)
            {
                StopRecordingAndSend();
            }
        }

        // 'P' 키를 눌러 현재 재생 중인 오디오 중지
        if (Input.GetKeyDown(KeyCode.P))
        {
            StopCurrentAudioPlayback();
        }
    }

    // 녹음을 시작하는 메서드
    public void StartRecording()
    {
        if (!isRecording)
        {
            isAudioCancelled = false; // 새로운 녹음 시작 시 취소 상태 초기화
            recordedClip = Microphone.Start(null, true, 10, 24000); // 마이크로 10초 동안 24kHz로 녹음 시작
            isRecording = true;
            audioBuffer.Clear(); // 오디오 버퍼 초기화
            Debug.Log("녹음 시작");
        }
    }

    // 녹음을 중지하고 서버로 전송하는 메서드
    public void StopRecordingAndSend()
    {
        if (isRecording)
        {
            int position = Microphone.GetPosition(null); // 현재 녹음 위치 가져오기
            Microphone.End(null); // 녹음 중지
            isRecording = false;
            Debug.Log("녹음 중지");

            float[] samples = new float[position];
            recordedClip.GetData(samples, 0); // 녹음된 데이터 가져오기
            byte[] audioData = ConvertToByteArray(samples); // float 배열을 byte 배열로 변환
            lastRecordedAudioBase64 = Convert.ToBase64String(audioData); // byte 배열을 Base64 문자열로 변환
            
            aiWebSocket.SendBufferAddAudio(lastRecordedAudioBase64); // 녹음된 오디오 데이터를 서버로 전송
            aiWebSocket.SendGenerateTextAudio(); // 텍스트 및 오디오 생성 요청
        }
    }

    // 현재 재생 중인 오디오를 중지하는 메서드
    private void StopCurrentAudioPlayback()
    {
        Debug.Log("오디오 재생 중지 요청");
        
        audioSource.Stop(); // 오디오 재생 중지
        audioBuffer.Clear(); // 오디오 버퍼 초기화
        isPlaying = false;
        isAudioCancelled = true; // 오디오 취소 상태를 true로 설정
        
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine); // 재생 코루틴 중지
            playCoroutine = null;
        }

        aiWebSocket.SendGenerateCancel(); // 서버에 생성 취소 요청 전송
    }

    // 서버로부터 받은 오디오 데이터를 처리하는 메서드
    public void HandleAudioDelta(string base64AudioDelta)
    {
        if (isAudioCancelled) // 오디오가 취소된 상태라면 새로운 데이터를 무시
        {
            return;
        }

        byte[] audioData = System.Convert.FromBase64String(base64AudioDelta); // Base64 문자열을 byte 배열로 변환
        short[] shortArray = new short[audioData.Length / 2];
        System.Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length); // byte 배열을 short 배열로 변환
        
        float[] samples = new float[shortArray.Length];
        for (int i = 0; i < shortArray.Length; i++)
        {
            samples[i] = shortArray[i] / 32768f; // short 값을 -1 ~ 1 범위의 float로 변환
        }

        audioBuffer.AddRange(samples); // 변환된 샘플을 오디오 버퍼에 추가

        // 버퍼가 임계값에 도달하고 현재 재생 중이 아니면 재생 시작
        if (audioBuffer.Count >= BUFFER_THRESHOLD && !isPlaying)
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
            }
            playCoroutine = StartCoroutine(PlayBufferedAudio());
        }
    }

    // 버퍼링된 오디오를 재생하는 코루틴
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

            yield return new WaitForSeconds(clip.length); // 클립 재생이 끝날 때까지 대기
        }

        isPlaying = false;
        playCoroutine = null;
    }

    // float 배열을 byte 배열로 변환하는 메서드
    private byte[] ConvertToByteArray(float[] samples)
    {
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        float rescaleFactor = 32767f; // float를 short로 변환할 때 사용할 스케일 팩터

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * rescaleFactor);
        }

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        return bytesData;
    }
}
