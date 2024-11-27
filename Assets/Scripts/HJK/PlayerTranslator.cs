using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Linq;  // Text 컴포넌트 사용을 위해 추가
using Photon.Voice.Unity;  // Recorder를 위해 추가

/// <summary>
/// 개별 플레이어의 AI 통역 관련 기능을 처리하는 컴포넌트
/// - 음성 녹음 (M키)
/// - 통역된 음성 재생
/// - 발화 상태 UI 관리
/// - 다중 사용자 간 발화 제어
/// </summary>

[RequireComponent(typeof(PhotonView))]
public class PlayerTranslator : MonoBehaviourPunCallbacks
{
    // 오디오 관련 컴포넌트
    private AudioSource translatedAudioSource;  // 통역된 음성을 재생할 AudioSource
    private bool isTranslating = false;
    
    [SerializeField] private Recorder voiceRecorder;  // Inspector에서 할당
    private const int RECORDING_FREQUENCY = 24000;    // 녹음 주파수는 유지
    
    // 설정값들
    [SerializeField] private KeyCode speakKey = KeyCode.M;      // 발언 시작/종료 키
    [SerializeField] private float maxRecordingTime = 60f;      // 최대 녹음 시간(초)
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape; // 발언 취소 키
    [SerializeField] private KeyCode resetKey = KeyCode.R;      // 리셋 키 추가
    
    // 스크롤 관련
    [Header("Scroll Animation Settings")]
    public float scrollAnimationDuration = 0.5f;  // 스크롤 애니메이션 지속 시간


    // 오디오 녹음 관련 상수
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
    [SerializeField] private Text errorMessageUI;              // 에러 메시지 UI (TMP -> Text)
    [SerializeField] private ScrollRect translationScrollView;              // Scroll View
    //[SerializeField] private GameObject translationTextPrefab;              // 프리팹
    [SerializeField] private GameObject MessageBubble_Original_Mine;   // 내가 말한 메시지 프리팹 (흰색)
    [SerializeField] private GameObject MessageBubble_Original_Yours;  // 상대방이 말한 메시지 프리팹 (파란색)
    [SerializeField] private GameObject MessageBubble_Translated;      // 번역된 메시지 프리팹
    private Dictionary<int, TextMeshProUGUI> translationTexts = new Dictionary<int, TextMeshProUGUI>();                 // order별로 저장

    // 에러 메시지 관련
    private float errorMessageDisplayTime = 3f;                // 에러 메시지 표시 시간
    private Coroutine errorMessageCoroutine;                  // 에러 메시지 표시 코루틴
    private float lastEventTime = 0f;

    private class MessageData
    {
        public GameObject userMessagePrefab;
        public GameObject translationPrefab;
        public int order;
        public string userid;
        public bool isMine;
    }

    private List<MessageData> messages = new List<MessageData>();

    private int currentOrder = -1; // 현재 발언 순번

    [SerializeField]
    private TranslationRoomIDSynchronizer translationRoomIDSynchronizer;  // Inspector에서 할당

    /// <summary>
    /// 발언 가능 상태에 따라 UI를 업데이트
    /// </summary>
    public void UpdateSpeakUI(bool canSpeak)
    {
        float currentTime = Time.time;
        //Debug.Log($"이벤트 발생 간격: {currentTime - lastEventTime}");
        lastEventTime = currentTime;
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
        
        // Recorder 이벤트 구독
        if (voiceRecorder == null)
        {
            voiceRecorder = FindObjectOfType<Recorder>();
            if (voiceRecorder == null)
            {
                Debug.LogError("[PlayerTranslator] Recorder를 찾을 수 없습니다!");
                return;
            }
        }
        
        voiceRecorder.AudioFrame += OnAudioFrameReceived;
        
        // TranslationEventHandler 이벤트 구독 유지
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
            Debug.Log("M키가 눌렸습니다.");
            TryStartRecording();
        }
        // 발언 키(M)에서 손을 뗐을 때
        else if (Input.GetKeyUp(speakKey))
        {
            Debug.Log("M키에서 손을 뗐습니다.");
            StopTranslating();
        }
        // 취소 키(ESC)를 눌렀을 때
        else if (Input.GetKeyDown(cancelKey))
        {
            CancelTranslating();
        }
        // 리셋 키(R) 눌렀을 때
        if (Input.GetKeyDown(resetKey))
        {
            string userId = FireAuthManager.Instance.GetCurrentUser().UserId;
            // Debug.Log("[PlayerTranslator] R키 입력 감지");
            Debug.Log($"[PlayerTranslator] Reset 요청 전송 - UserId: {userId}");
            
            var synchronizer = FindObjectOfType<TranslationRoomIDSynchronizer>();
            if (synchronizer != null)
            {
                // Debug.Log("[PlayerTranslator] TranslationRoomIDSynchronizer 찾음");
                // RpcTarget.All 대신 RpcTarget.MasterClient 사용
                synchronizer.photonView.RPC("RequestReset", RpcTarget.MasterClient, userId);
            }
            else
            {
                Debug.LogError("[PlayerTranslator] TranslationRoomIDSynchronizer를 찾을 수 없습니다!");
            }
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

        if (!string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId) && 
            TranslationEventHandler.Instance.CurrentSpeakerId != userId)
        {
            ShowWaitingUI("이미 상대방이 통역 중입니다");
            return;
        }

        ShowWaitingUI("통역 대기 중");
        TranslationManager.Instance.RequestSpeech();
    }

    public void OnApprovedSpeech(int order, string userid, string lang)
    {
        currentOrder = order;

        if (userid == FireAuthManager.Instance.GetCurrentUser().UserId)
        {
            ShowCanSpeakUI();
            StartTranslating();
        }
    }

    private void StartTranslating()
    {
        isTranslating = true;
        
        MessageData messageData = new MessageData
        {
            isMine = true,
            order = currentOrder,
            userid = FireAuthManager.Instance.GetCurrentUser().UserId
        };

        messageData.userMessagePrefab = Instantiate(MessageBubble_Original_Mine, translationScrollView.content);
        messages.Add(messageData);
        StartCoroutine(ScrollToBottomNextFrame());
    }

    private void StopTranslating()
    {
        if (!isTranslating) return;
        
        isTranslating = false;
        TranslationManager.Instance.DoneSpeech();
        TranslationEventHandler.Instance.ResetSpeaker();
    }

    private void OnDestroy()
    {
        if (voiceRecorder != null)
        {
            voiceRecorder.AudioFrame -= OnAudioFrameReceived;
        }
        
        var handler = TranslationEventHandler.Instance;
        if (handler != null)
        {
            handler.OnRoomReadyStateChanged -= UpdateSpeakUI;
            handler.OnSpeakerChanged -= HandleSpeakerChanged;
        }
    }

    private void OnAudioFrameReceived(float[] frame)
    {
        if (isTranslating)
        {
            string audioData = ConvertAudioToBase64(frame);
            if (!string.IsNullOrEmpty(audioData))
            {
                TranslationManager.Instance.SendAudioData(audioData);
            }
        }
    }

    private void CancelTranslating()
    {
        string userId = FireAuthManager.Instance.GetCurrentUser().UserId;
        if (TranslationEventHandler.Instance.CurrentSpeakerId != userId) return;

        if (isTranslating)
        {
            isTranslating = false;
            TranslationManager.Instance.ClearAudioBuffer();
        }
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
        
        // Debug.Log($"[Debug] 샘플 배열 크기: {samples.Length}");
        
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
        
        // Debug.Log($"[Debug] 0이 아닌 샘플 수: {nonZeroSamples}");

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        string result = Convert.ToBase64String(bytesData);
        // Debug.Log($"[Debug] 최종 base64 문자열 길이: {result.Length}");
        
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
            // Debug.Log($"[Audio] Received base64 length: {base64AudioData.Length}");
            
            byte[] audioData = Convert.FromBase64String(base64AudioData);
            // Debug.Log($"[Audio] Converted to bytes length: {audioData.Length}");
            
            short[] shortArray = new short[audioData.Length / 2];
            Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
            // Debug.Log($"[Audio] Converted to shorts length: {shortArray.Length}");
            
            float[] samples = new float[shortArray.Length];
            for (int i = 0; i < shortArray.Length; i++)
            {
                samples[i] = shortArray[i] / 32768f;
            }
            // Debug.Log($"[Audio] Final samples length: {samples.Length}");

            // 현재 청크의 길이를 초 단위로 계산
            float currentChunkSeconds = (float)samples.Length / RECORDING_FREQUENCY;
            // Debug.Log($"[Audio] 현재 청크 duration: {currentChunkSeconds:F2} seconds");

            // 유효한 오디오 데이터 체크
            bool hasValidAudio = samples.Any(s => Mathf.Abs(s) > 0.0001f);
            // Debug.Log($"[Audio] Contains valid audio data: {hasValidAudio}");

            if (hasValidAudio)
            {
                audioBuffer.AddRange(samples);
                // 전체 버퍼의 길이를 초 단위로 계산
                float totalBufferSeconds = (float)audioBuffer.Count / RECORDING_FREQUENCY;
                // Debug.Log($"[Audio] 전체 버퍼 duration: {totalBufferSeconds:F2} seconds");
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
        // Debug.Log($"[Audio] Buffer status - Size: {audioBuffer.Count}, IsPlaying: {isPlaying}");
        float bufferDuration = (float)audioBuffer.Count / RECORDING_FREQUENCY;
        // Debug.Log($"[Audio] Buffer duration: {bufferDuration:F2} seconds");
        
        // 최소 버퍼 크기를 0.1초로 설정 (2400 샘플)
        const int MIN_BUFFER_SIZE = RECORDING_FREQUENCY / 10;  // 2400 = 0.1초
        
        if ((audioBuffer.Count >= MIN_BUFFER_SIZE || bufferDuration >= 0.1f) && !isPlaying)
        {
            if (playCoroutine != null)
            {
                // Debug.Log("[Audio] Stopping previous coroutine");
                StopCoroutine(playCoroutine);
            }
            
            // Debug.Log("[Audio] Starting new playback coroutine");
            playCoroutine = StartCoroutine(PlayBufferedAudio());
        }
    }

    /// <summary>
    /// 버퍼에 있는 오디오 데이터 재생
    /// </summary>
    private IEnumerator PlayBufferedAudio()
    {
        isPlaying = true;
        // Debug.Log("[Audio] Starting audio playback");

        while (audioBuffer.Count > 0 && !isAudioCancelled)
        {
            // 현재 버퍼의 전체 길이를 초 단위로 계산
            float bufferDuration = (float)audioBuffer.Count / RECORDING_FREQUENCY;
            // Debug.Log($"[Audio] Current buffer duration: {bufferDuration:F2} seconds");
            
            // 버퍼의 모든 데이터를 한 번에 재생
            int sampleCount = audioBuffer.Count;
            // Debug.Log($"[Audio] Playing buffer of {sampleCount} samples");
            
            float[] playbackSamples = audioBuffer.ToArray();
            audioBuffer.Clear();

            AudioClip clip = AudioClip.Create("TranslatedAudio", 
                sampleCount, 1, RECORDING_FREQUENCY, false);
            clip.SetData(playbackSamples, 0);

            if (translatedAudioSource != null)
            {
                translatedAudioSource.clip = clip;
                translatedAudioSource.Play();
                float duration = (float)sampleCount / RECORDING_FREQUENCY;
                // Debug.Log($"[Audio] Playing clip of length: {duration:F2} seconds");
                
                // 클립이 완전히 재생될 때까지 대기
                yield return new WaitForSeconds(duration);
            }
            else
            {
                Debug.LogError("[Audio] AudioSource is null!");
                break;
            }
        }

        // Debug.Log("[Audio] Playback completed");
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
    /// 에러 처리
    /// </summary>
    public void HandleError(string errorMessage)
    {
        print("HandleError(에러 발생됨)");
        // 현재 녹음 중이면 녹음 중지
        if (isTranslating)
        {
            StopTranslating();
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

    //private void CreateNewTranslationText(int order)
    //{
    //    if (translationScrollView.content == null)
    //    {
    //        Debug.LogError("ScrollView의 content가 없습니다!");
    //        return;
    //    }

    //    // 프리팹 인스턴스 생성
    //    GameObject newTextObj = Instantiate(translationTextPrefab, translationScrollView.content);
    //    TextMeshProUGUI newText = newTextObj.GetComponent<TextMeshProUGUI>();

    //    if (newText != null)
    //    {
    //        translationTexts.Add(order, newText);
    //    }
    //    else
    //    {
    //        Debug.LogError("프리팹에 TextMeshProUGUI 컴포넌트가 없습니다!");
    //    }
    //}

    // 발화가 완료되었을 때 호출되는 메서드
    public void OnCompleteAudioReceived()
    {
        // ... 기존 코드 ...

        // 발화 완료 시 처리할 내용이 있다면 여기에 추가
        // 예를 들어 오래된 텍스트를 정리하는 등
    }

    public void ClearMessages()
    {
        Debug.Log("[PlayerTranslator] 메시지 초기화 시작");
        
        // 1. UI 요소 정리
        if (translationScrollView != null && translationScrollView.content != null)
        {
            foreach (var messageData in messages)
            {
                if (messageData.userMessagePrefab != null)
                    Destroy(messageData.userMessagePrefab);
                if (messageData.translationPrefab != null)
                    Destroy(messageData.translationPrefab);
            }
        }
        
        // 2. 메시지 리스트 초기화
        messages.Clear();
        
        // 3. 현재 진행중인 번역/음성 처리 중단
        if (isTranslating)
        {
            StopTranslating();
        }
        CancelAudioPlayback();
        
        // 4. 현재 순번 초기화
        currentOrder = -1;
        
        Debug.Log("[PlayerTranslator] 메시지 초기화 완료");
    }

    
}