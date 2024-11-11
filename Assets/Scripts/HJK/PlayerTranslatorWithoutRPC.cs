using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;  // Text 컴포넌트 사용을 위해 추가

/// <summary>
/// 개별 플레이어의 AI 통역 관련 기능을 처리하는 컴포넌트
/// - 음성 녹음 (M키)
/// - 통역된 음성 재생
/// - 발화 상태 UI 관리
/// - 다중 사용자 간 발화 제어
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class PlayerTranslatorWithoutRPC : MonoBehaviourPunCallbacks
{
    // 오디오 관련 컴포넌트
    private AudioSource translatedAudioSource;  // 통역된 음성을 재생할 AudioSource
    private AudioClip recordingClip;           // 현재 녹음 중인 AudioClip
    private bool isRecording = false;          // 현재 녹음 중인지 여부
    private float[] tempRecordingBuffer;       // 임시 녹음 버퍼
    private int recordingPosition = 0;         // 현재 녹음 위치

    // 설정값들
    [SerializeField] private KeyCode speakKey = KeyCode.M;      // 발언 시작/종료 키
    [SerializeField] private float maxRecordingTime = 60f;      // 최대 녹음 시간(초)
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape; // 발언 취소 키

    // 오디오 녹음 관련 상수
    private const int RECORDING_FREQUENCY = 24000;              // 녹음 주파수
    private readonly int RECORDING_BUFFER_SIZE = 24000 * 60;    // 녹음 버퍼 크기 (1분)

    // 스트리밍 재생 관련 변수들
    private List<float> audioBuffer = new List<float>();        // 오디오 재생 버퍼
    private const int BUFFER_THRESHOLD = 24000;                 // 버퍼 임계값 (1초)
    private bool isPlaying = false;                            // 현재 재생 중인지 여부
    private Coroutine playCoroutine;                          // 재생 코루틴
    private bool isAudioCancelled = false;                    // 오디오 재생 취소 여부

    // UI 요소들
    [SerializeField] private GameObject cantSpeakUI;            // 발언 불가 시 표시할 UI
    [SerializeField] private GameObject speakButton;            // 발언 가능 상태 표시 UI
    [SerializeField] private GameObject waitingText;            // 대기 상태 표시 UI
    [SerializeField] private Text errorMessageUI;               // 에러 메시지 UI (TMP -> Text)

    // 에러 메시지 관련
    private float errorMessageDisplayTime = 3f;                // 에러 메시지 표시 시간
    private Coroutine errorMessageCoroutine;                  // 에러 메시지 표시 코루틴

    /// <summary>
    /// 발언 가능 상태에 따라 UI를 업데이트
    /// </summary>
    public void UpdateSpeakUI(bool canSpeak)
    {
        if (speakButton != null)
        {
            speakButton.SetActive(canSpeak);
        }
        
        if (waitingText != null)
        {
            waitingText.SetActive(!canSpeak);
        }
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Start()
    {
        // 통역된 음성 재생을 위한 AudioSource 컴포넌트 추가
        translatedAudioSource = gameObject.AddComponent<AudioSource>();
        // 녹음 버퍼 초기화
        tempRecordingBuffer = new float[RECORDING_BUFFER_SIZE];

        // TranslationEventHandler의 Ready 상태 변경 이벤트 구독
        TranslationEventHandler.Instance.OnRoomReadyStateChanged += UpdateSpeakUI;
        TranslationEventHandler.Instance.OnSpeakerChanged += HandleSpeakerChanged;
    }

    /// <summary>
    /// 매 프레임마다 입력 체크
    /// </summary>
    private void Update()
    {
        // 발언 키(M) 눌렀을 때
        if (Input.GetKeyDown(speakKey))
        {
            TryStartRecording();
        }
        // 발언 키(M)에서 손을 뗐을 때
        else if (Input.GetKeyUp(speakKey))
        {
            StopRecording();
        }
        // 취소 키(ESC)를 눌렀을 때
        else if (Input.GetKeyDown(cancelKey))
        {
            CancelRecording();
        }
    }

    /// <summary>
    /// 특정 사용자가 발언할 수 있는지 확인
    /// </summary>
    /// <param name="userId">확인할 사용자 ID</param>
    /// <returns>발언 가능 여부</returns>
    public bool CanSpeak(string userId)
    {
        // TranslationEventHandler의 CurrentSpeakerId 사용
        return string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId) || 
               TranslationEventHandler.Instance.CurrentSpeakerId == userId;
    }

    /// <summary>
    /// 녹음 시작 시도
    /// </summary>
    private void TryStartRecording()
    {
        string userId = FireAuthManager.Instance.GetCurrentUser().UserId;
        if (!TranslationEventHandler.Instance.IsRoomReady)
        {
            ShowWaitingUI("통역을 시작하려면 다른 언어 사용자가 필요합니다");
            return;
        }

        // 발언 가능한지 확인
        if (!string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId) && TranslationEventHandler.Instance.CurrentSpeakerId != userId)
        {
            ShowWaitingUI("이미 상대방이 통역 중입니다");
            return;
        }

        ShowWaitingUI("통역 대기 중");
        TranslationManager.Instance.RequestSpeech();
    }

    /// <summary>
    /// 실제 녹음 시작
    /// </summary>
    private void StartRecording()
    {
        print("StartRecording(녹음 시작됨)");
        // currentSpeakerId 참조를 TranslationEventHandler로 변경
        print("CurrentSpeakerId: " + TranslationEventHandler.Instance.CurrentSpeakerId);
        // if (!string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId)) return;

        isRecording = true;
        recordingPosition = 0;
        
        // 사용 가능한 마이크 확인
        string[] devices = Microphone.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("사용 가능한 마이크가 없습니다!");
            return;
        }

        // 녹음 시작
        recordingClip = Microphone.Start(null, true, (int)maxRecordingTime, RECORDING_FREQUENCY);
    }

    /// <summary>
    /// 녹음 중지 및 녹음된 오디오 전송
    /// </summary>
    private void StopRecording()
    {
        print("StopRecording(일단 실행됨)");
        if (!isRecording) return;
        print("StopRecording(녹음 중지됨)");
        
        // 현재 position 저장
        int position = Microphone.GetPosition(null);
        Debug.Log($"[Debug] 녹음 중지 전 position: {position}");
        
        // position이 유효한 경우에만 처리
        if (position > 0)
        {
            // 녹음된 데이터를 임시 배열에 저장
            float[] samples = new float[position];
            recordingClip.GetData(samples, 0);
            
            // 마이크 녹음 중지
            Microphone.End(null);
            isRecording = false;
            
            // 저장된 샘플을 base64로 변환
            string audioData = ConvertAudioToBase64(samples);
            print("오디오 변환 성공 -> audioData: " + audioData);
            
            if (!string.IsNullOrEmpty(audioData))
            {
                // 오디오 데이터 전송
                TranslationManager.Instance.SendAudioData(audioData);
                // 발언 종료 신호 전송
                TranslationManager.Instance.DoneSpeech();
            }
        }
        else
        {
            Microphone.End(null);
            isRecording = false;
            Debug.LogWarning("녹음된 데이터가 없습니다.");
        }

        // 발언자 상태 초기화
        TranslationEventHandler.Instance.ResetSpeaker();
    }

    /// <summary>
    /// 녹음된 오디오 데이터를 base64 문자열로 변환
    /// </summary>
    private string ConvertAudioToBase64(float[] samples)
    {
        if (samples == null || samples.Length == 0)
        {
            Debug.LogWarning("[Debug] 샘플 데이터가 비어있습니다.");
            return string.Empty;
        }
        
        Debug.Log($"[Debug] 샘플 배열 크기: {samples.Length}");
        
        // float[] to byte[] 변환
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        float rescaleFactor = 32767f;

        int nonZeroSamples = 0;
        for (int i = 0; i < samples.Length; i++)
        {
            float sample = samples[i];
            if (Mathf.Abs(sample) >= 0.001f)
                nonZeroSamples++;
            
            // 클리핑 방지
            if (sample > 1f) sample = 1f;
            if (sample < -1f) sample = -1f;
            
            intData[i] = (short)(sample * rescaleFactor);
        }
        
        Debug.Log($"[Debug] 0이 아닌 샘플 수: {nonZeroSamples}");

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        string result = Convert.ToBase64String(bytesData);
        Debug.Log($"[Debug] 최종 base64 문자열 길이: {result.Length}");
        
        return result;
    }

    /// <summary>
    /// 서버로부터 받은 오디오 스트림 처리
    /// </summary>
    public void ProcessAudioStream(string base64AudioData)
    {
        if (isAudioCancelled) return;

        try
        {
            // base64 디코딩 과정 로깅
            Debug.Log($"[Audio] Received base64 length: {base64AudioData.Length}");
            
            byte[] audioData = Convert.FromBase64String(base64AudioData);
            Debug.Log($"[Audio] Converted to bytes length: {audioData.Length}");
            
            short[] shortArray = new short[audioData.Length / 2];
            Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
            Debug.Log($"[Audio] Converted to shorts length: {shortArray.Length}");
            
            float[] samples = new float[shortArray.Length];
            for (int i = 0; i < shortArray.Length; i++)
            {
                samples[i] = shortArray[i] / 32768f;
            }
            Debug.Log($"[Audio] Final samples length: {samples.Length}");

            // 유효한 오디오 데이터 체크
            bool hasValidAudio = samples.Any(s => Mathf.Abs(s) > 0.0001f);
            Debug.Log($"[Audio] Contains valid audio data: {hasValidAudio}");

            if (hasValidAudio)
            {
                audioBuffer.AddRange(samples);
                StartAudioBuffer();
            }
            else
            {
                Debug.LogWarning("[Audio] Skipping empty or invalid audio data");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Audio] Processing error: {e.GetType().Name} - {e.Message}\nStack: {e.StackTrace}");
        }
    }

    /// <summary>
    /// 오디오 버퍼 재생 시작
    /// </summary>
    private void StartAudioBuffer()
    {
        Debug.Log($"[Audio] Buffer status - Size: {audioBuffer.Count}, IsPlaying: {isPlaying}");
        
        // BUFFER_THRESHOLD 값을 더 작게 조정
        const int MIN_BUFFER_SIZE = 4800;  // 0.2초 분량
        
        if (audioBuffer.Count >= MIN_BUFFER_SIZE && !isPlaying)
        {
            if (playCoroutine != null)
            {
                Debug.Log("[Audio] Stopping previous coroutine");
                StopCoroutine(playCoroutine);
            }
            
            Debug.Log("[Audio] Starting new playback coroutine");
            playCoroutine = StartCoroutine(PlayBufferedAudio());
        }
    }

    /// <summary>
    /// 버퍼에 있는 오디오 데이터 재생
    /// </summary>
    private IEnumerator PlayBufferedAudio()
    {
        isPlaying = true;
        Debug.Log("[Audio] Starting audio playback");

        while (audioBuffer.Count > 0 && !isAudioCancelled)
        {
            // 버퍼 크기를 더 작게 조정
            int sampleCount = Mathf.Min(audioBuffer.Count, 4800);  // 0.2초 분량
            Debug.Log($"[Audio] Playing chunk of {sampleCount} samples");
            
            float[] playbackSamples = audioBuffer.GetRange(0, sampleCount).ToArray();
            audioBuffer.RemoveRange(0, sampleCount);

            // 재생할 AudioClip 생성
            AudioClip clip = AudioClip.Create("TranslatedAudio", 
                sampleCount, 1, RECORDING_FREQUENCY, false);
            clip.SetData(playbackSamples, 0);

            if (translatedAudioSource != null)
            {
                translatedAudioSource.clip = clip;
                translatedAudioSource.Play();
                Debug.Log($"[Audio] Playing clip of length: {clip.length}s");
                
                // 클립이 실제로 재생될 때까지 짧게 대기
                yield return new WaitForSeconds(clip.length + 0.05f);
            }
            else
            {
                Debug.LogError("[Audio] AudioSource is null!");
                break;
            }
        }

        Debug.Log("[Audio] Playback completed");
        isPlaying = false;
        playCoroutine = null;
    }

    /// <summary>
    /// 마지막 오디오 데이터 재생 완료
    /// </summary>
    public void FinalizeAudioPlayback()
    {
        if (audioBuffer.Count > 0)
        {
            StartAudioBuffer();
        }
    }

    /// <summary>
    /// 오디오 재생 취소
    /// </summary>
    public void CancelAudioPlayback()
    {
        isAudioCancelled = true;
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        audioBuffer.Clear();
        isPlaying = false;
    }

    /// <summary>
    /// 발언 불가 UI 표시
    /// </summary>
    private void ShowCantSpeakUI()
    {
        if (cantSpeakUI != null)
            cantSpeakUI.SetActive(true);
    }

    /// <summary>
    /// 발언자 변경 처리 메서드 추가
    /// </summary>
    private void HandleSpeakerChanged(string newSpeakerId)
    {
        if (string.IsNullOrEmpty(newSpeakerId))
        {
            UpdateSpeakUI(TranslationEventHandler.Instance.IsRoomReady);
        }
    }

    /// <summary>
    /// 녹음 취소 처리
    /// </summary>
    private void CancelRecording()
    {
        string userId = FireAuthManager.Instance.GetCurrentUser().UserId;
        if (TranslationEventHandler.Instance.CurrentSpeakerId != userId) return;

        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            TranslationManager.Instance.ClearAudioBuffer();
        }
    }

    /// <summary>
    /// 컴포넌트 제거 시 정리 작업
    /// </summary>
    private void OnDestroy()
    {
        // currentSpeakerId 참조를 TranslationEventHandler로 변경
        if (photonView.IsMine && !string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId))
        {
            TranslationManager.Instance.ClearAudioBuffer();
            TranslationManager.Instance.LeaveRoom();
        }

        // 이벤트 구독 해제
        var handler = TranslationEventHandler.Instance;
        if (handler != null)
        {
            handler.OnRoomReadyStateChanged -= UpdateSpeakUI;
            handler.OnSpeakerChanged -= HandleSpeakerChanged;
        }
    }

    /// <summary>
    /// 발언권 승인 처리
    /// </summary>
    public void OnSpeechApproved(string approvedUserId)
    {
        print("OnSpeechApproved(발언권 승인됨)");
        if (FireAuthManager.Instance.GetCurrentUser().UserId == approvedUserId)
        {
            print("if문 통과함");
            ShowCanSpeakUI();
            StartRecording();
        }
    }

    /// <summary>
    /// 에러 처리
    /// </summary>
    public void HandleError(string errorMessage)
    {
        print("HandleError(에러 발생됨)");
        // 현재 녹음 중이면 녹음 중지
        if (isRecording)
        {
            CancelRecording();
        }

        // 현재 재생 중이면 재생 중지
        CancelAudioPlayback();

        // UI 업데이트
        if (errorMessageUI != null)
        {
            // 이전 에러 메시지 코루틴이 있다면 중지
            if (errorMessageCoroutine != null)
            {
                StopCoroutine(errorMessageCoroutine);
            }
            
            // 새로운 에러 메시지 표시
            errorMessageCoroutine = StartCoroutine(ShowErrorMessage(errorMessage));
        }
        else
        {
            Debug.LogError($"Error occurred: {errorMessage}");
        }
    }

    /// <summary>
    /// 에러 메시지 표시 코루틴
    /// </summary>
    private IEnumerator ShowErrorMessage(string message)
    {
        // 에러 메시지 UI 표시
        errorMessageUI.text = message;
        errorMessageUI.gameObject.SetActive(true);
        
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(errorMessageDisplayTime);
        
        // UI 숨김 및 코루틴 정리
        errorMessageUI.gameObject.SetActive(false);
        errorMessageCoroutine = null;
    }

    private void ShowWaitingUI(string message)
    {
        if (waitingText != null && waitingText.GetComponent<Text>() != null)
        {
            waitingText.GetComponent<Text>().text = message;
            waitingText.SetActive(true);
        }
    }

    private void ShowCanSpeakUI()
    {
        if (waitingText != null)
            waitingText.SetActive(false);
        if (speakButton != null)
            speakButton.SetActive(true);
    }
}