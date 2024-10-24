﻿using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Threading.Tasks; // UI 네임스페이스 추가

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

    public Text playingStatusText; // UI Text 컴포넌트를 위한 변수 추가
    public Button replayButton; // 재생 버튼을 위한 변수 추가

    private string userId;           // 사용자 식별자 추가
    private AIWebSocket currentAI;   // 현재 연결된 AI 참조 추가

    // 시작 시 실행되는 메서드
    void Start()
    {
        NetworkManager.Instance.RegisterUser(this, userId);
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSource 컴포넌트 추가
        UpdatePlayingStatusUI();
        
        // 버튼에 클릭 이벤트 리스너 추가
        if (replayButton != null)
        {
            replayButton.onClick.AddListener(PlayLastRecordedAudio);
        }
    }

    // 매 프레임마다 실행되는 메서드
    void Update()
    {

    }

    // 녹음을 시작하는 메서드
    public void StartRecording()
    {
        if (!isRecording)
        {
            Debug.Log("[시간] StartRecording 시작");
            string[] devices = Microphone.devices;
            Debug.Log($"[시간] 사용 가능한 마이크: {string.Join(", ", devices)}");
            
            if (devices.Length == 0)
            {
                Debug.LogError("사용 가능한 마이크가 없습니다!");
                return;
            }
            
            Debug.Log("[시간] Microphone.Start 호출 직전");
            recordedClip = Microphone.Start(null, true, 10, 24000);
            Debug.Log("[시간] Microphone.Start 호출 완료");
            
            isRecording = true;
            audioBuffer.Clear();
            Debug.Log("[시간] StartRecording 완료");
        }
    }

    // 녹음을 중지하고 서버로 전송하는 메서드
    //public void StopRecordingAndSend()
    //{
    //    if (isRecording)
    //    {
    //        Microphone.End(null); // 녹음 중지
    //        isRecording = false;
    //        Debug.Log("녹음 중지");

    //        //int position = Microphone.GetPosition(null); // 현재 녹음 위치 가져오기
    //        //float[] samples = new float[position];
    //        //recordedClip.GetData(samples, 0); // 녹음된 데이터 가져오기
    //        //byte[] audioData = ConvertToByteArray(samples); // float 배열을 byte 배열로 변환
    //        lastRecordedAudioBase64 = ConvertAudioDataToBytes(recordedClip); // byte 배열을 Base64 문자열로 변환
    //        if(lastRecordedAudioBase64 == null)
    //        {
    //            Debug.Log("녹음된 오디오 데이터가 없습니다.");
    //            return;
    //        }
    //        aiWebSocket.SendBufferAddAudio(lastRecordedAudioBase64); // 녹음된 오디오 데이터를 서버로 전송
    //        aiWebSocket.SendGenerateTextAudio(); // 텍스트 및 오디오 생성 요청
    //    }
    //}

    public async Task StopRecordingAndSend()
    {
        if (isRecording)
        {
            Debug.Log("[시간] StopRecordingAndSend 시작");
            int position = Microphone.GetPosition(null);
            Debug.Log($"[시간] 녹음 종료. 녹음된 위치: {position}, 전체 샘플 수: {recordedClip.samples}");
            
            Debug.Log("[시간] Microphone.End 호출 직전");
            Microphone.End(null);
            Debug.Log("[시간] Microphone.End 호출 완료");
            
            isRecording = false;

            if (recordedClip != null)
            {
                Debug.Log("[시간] ConvertAudioDataToBytes 시작");
                lastRecordedAudioBase64 = ConvertAudioDataToBytes(recordedClip);
                Debug.Log($"[시간] 변환된 오디오 데이터 길이: {(lastRecordedAudioBase64?.Length ?? 0)}");

                if (string.IsNullOrEmpty(lastRecordedAudioBase64))
                {
                    Debug.LogError("오디오 데이터 변환 실패");
                    return;
                }
                
                Debug.Log("[시간] SendBufferAddAudio 호출 직전");
                await aiWebSocket.SendBufferAddAudio(lastRecordedAudioBase64);
                Debug.Log("[시간] SendGenerateTextAudio 호출 직전");
                await aiWebSocket.SendGenerateTextAudio();
                Debug.Log("[시간] StopRecordingAndSend 완료");
            }
            else
            {
                Debug.LogError("recordedClip이 null입니다");
            }
        }
        
        // Task.CompletedTask를 반환하는 대신 메소드를 여기서 종료합니다.
    }

    string ConvertAudioDataToBytes(AudioClip recordedClip)
    {
        // 녹음된 실제 위치 확인
        int position = Microphone.GetPosition(null);
        if (position <= 0) position = recordedClip.samples;

        float[] samples = new float[position * recordedClip.channels];

        // 원본 데이터 가져오기
        if (!recordedClip.GetData(samples, 0))
        {
            Debug.LogError("오디오 데이터를 가져오는데 실패했습니다.");
            return null;
        }

        // 변환 및 반환
        byte[] audioData = ConvertToByteArray(samples);
        return Convert.ToBase64String(audioData);
    }


    // 현재 재생 중인 오디오를 중지하는 메서드
    public void StopCurrentAudioPlayback()
    {
        Debug.Log("오디오 재생 중지 요청");
        
        if (isPlaying)
        {
            // 코루틴 중지
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                playCoroutine = null;
            }

            // 현재 재생 중인 오디오 중지
            audioSource.Stop();
            audioSource.clip = null;  // 현재 클립도 제거

            // 버퍼와 상태 초기화
            audioBuffer.Clear();
            isPlaying = false;
            isAudioCancelled = true;
            
            // 서버에 취소 요청
            aiWebSocket.SendGenerateCancel();
            
            UpdatePlayingStatusUI();
        }
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
        StartAudioBuffer(audioBuffer);
    }

    void StartAudioBuffer(List<float> audioBuffer)
    {
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
        UpdatePlayingStatusUI();

        while (audioBuffer.Count > 0 && !isAudioCancelled)  // 취소 상태 확인 추가
        {
            if (isAudioCancelled)  // 추가 체크
            {
                break;
            }

            int sampleCount = Mathf.Min(audioBuffer.Count, BUFFER_THRESHOLD);
            float[] playbackSamples = audioBuffer.GetRange(0, sampleCount).ToArray();
            audioBuffer.RemoveRange(0, sampleCount);

            AudioClip clip = AudioClip.Create("AI_Response", sampleCount, 1, 24000, false);
            clip.SetData(playbackSamples, 0);

            audioSource.clip = clip;
            audioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }

        isPlaying = false;
        UpdatePlayingStatusUI();
        playCoroutine = null;
    }

    // float 배열을 byte 배열로 변환하는 메서드
    private byte[] ConvertToByteArray(float[] samples)
    {
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        float rescaleFactor = 32767f;

        for (int i = 0; i < samples.Length; i++)
        {
            // 노이즈 게이트 적용 (작은 소리는 제거)
            float sample = samples[i];
            if (Mathf.Abs(sample) < 0.01f)
                sample = 0f;
                
            intData[i] = (short)(Mathf.Clamp(sample, -1f, 1f) * rescaleFactor);
        }

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        return bytesData;
    }

    // UI 업데이트를 위한 새로운 메서드
    private void UpdatePlayingStatusUI()
    {
        if (playingStatusText != null)
        {
            playingStatusText.text = "isPlaying: " + (isPlaying ? "true" : "false");
        }
    }

    // isPlaying 상태를 외부에서 확인할 수 있는 메서드 추가
    public bool IsPlaying()
    {
        return isPlaying;
    }

    // 마지막으로 녹음된 오디오를 재생하는 메서드
    public void PlayLastRecordedAudio()
    {
        Debug.Log("마지막 녹음된 오디오 재생 요청 버튼 클릭");
        if (lastRecordedAudioBase64 != null && !isPlaying)
        {
            byte[] audioData = Convert.FromBase64String(lastRecordedAudioBase64);
            short[] shortArray = new short[audioData.Length / 2];
            Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
            
            float[] samples = new float[shortArray.Length];
            for (int i = 0; i < shortArray.Length; i++)
            {
                samples[i] = shortArray[i] / 32768f;
            }

            AudioClip clip = AudioClip.Create("LastRecording", samples.Length, 1, 24000, false);
            clip.SetData(samples, 0);

            audioSource.clip = clip;
            audioSource.Play();
            
            Debug.Log("마지막 녹음된 오디오 재생 중");
        }
        else
        {
            Debug.Log("재생할 녹음된 오디오가 없거나 현재 다른 오디오가 재생 중입니다.");
        }
    }

    // AI 연결 설정 메서드 추가
    public void SetCurrentAI(AIWebSocket aiWebSocket)
    {
        currentAI = aiWebSocket;
        Debug.Log($"사용자 {userId}가 AI {aiWebSocket.AiId}와 연결됨");
    }

    // 사용자 ID 초기화 메서드 추가
    public void Initialize(string newUserId)
    {
        userId = newUserId;
        Debug.Log($"VoiceManager가 사용자 {userId}로 초기화됨");
    }

    // 연결 해제 메서드 추가
    public void DisconnectFromAI()
    {
        if (currentAI != null)
        {
            Debug.Log($"사용자 {userId}가 AI {currentAI.AiId}와 연결 해제됨");
            currentAI = null;
        }
    }
}
