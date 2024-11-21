using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
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

    private const float CHUNK_DURATION = 0.1f;                  // 청크 단위 (100ms)
    private const int SAMPLES_PER_CHUNK = (int)(RECORDING_FREQUENCY * CHUNK_DURATION);  // 청크당 샘플 수
    private Coroutine recordingCoroutine;                      // 녹음 코루틴

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
    [SerializeField] private Text errorMessageUI;              // 에러 메시지 UI (TMP -> Text)
    [SerializeField] private ScrollRect translationScrollView;              // Scroll View
    //[SerializeField] private GameObject translationTextPrefab;              // 프리팹
    [SerializeField] private GameObject MessageBubble_Original_Mine;   // 내가 말한 메시지 프리팹 (흰색)
    [SerializeField] private GameObject MessageBubble_Original_Yours;  // 상대방이 말한 메시지 프리팹 (파란색)
    [SerializeField] private GameObject MessageBubble_Translated;      // 번역된 메시지 프리팹
    private Dictionary<int, TextMeshProUGUI> translationTexts = new Dictionary<int, TextMeshProUGUI>();                 // order별로 저장
    private bool isRecording = false;  // 현재 녹음 중인지 여부를 나타내는 변수 추가
    private Queue<string> pendingAudioBuffer = new Queue<string>();  // 녹음 중에 받은 오디오를 저장할 큐

    // 현재 누군가 말하고 있는지 확인하는 프로퍼티
    private bool IsSomeoneSpeaking => !string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId);

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

    /// <summary>
    /// 발언 가능 상태에 따라 UI를 업데이트
    /// </summary>
    public void UpdateSpeakUI(bool canSpeak)
    {
        float currentTime = Time.time;
        Debug.Log($"이벤트 발생 간경: {currentTime - lastEventTime}");
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
            Debug.Log("M키가 눌렸습니다.");
            TryStartRecording();
        }
        // 발언 키(M)에서 손을 뗐을 때
        else if (Input.GetKeyUp(speakKey))
        {
            Debug.Log("M키에서 손을 뗐습니다.");
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

    public void OnApprovedSpeech(int order, string userid, string lang)
    {
        currentOrder = order;

        if (userid == FireAuthManager.Instance.GetCurrentUser().UserId)
        {
            // 내가 발언권을 얻은 경우
            ShowCanSpeakUI();
            Debug.Log("UI 표시됨: ShowCanSpeakUI() 실행 in OnApprovedSpeech()");
            StartRecording();
            Debug.Log("녹음 시작됨: StartRecording() 실행 in OnApprovedSpeech()");
        }
        else
        {
            // 다른 사용자가 발언권을 얻은 경우
            // 필요에 따라 처리
        }
    }

    /// <summary>
    /// 실제 녹음 시작
    /// </summary>
    private void StartRecording()
    {
        if (isRecording) return;
        
        isRecording = true;
        pendingAudioBuffer.Clear();  // 녹음 시작 시 버퍼 초기화
        
        print("StartRecording(녹음 시작되는 함수에 진입)");
        // currentSpeakerId 참조를 TranslationEventHandler로 변경
        print("CurrentSpeakerId: " + TranslationEventHandler.Instance.CurrentSpeakerId);
        // if (!string.IsNullOrEmpty(TranslationEventHandler.Instance.CurrentSpeakerId)) return;

        // MessageData 생성
        MessageData messageData = new MessageData();
        messageData.isMine = true;
        messageData.order = currentOrder;
        messageData.userid = FireAuthManager.Instance.GetCurrentUser().UserId;

        // 내 메시지 프리팹 생성
        messageData.userMessagePrefab = Instantiate(MessageBubble_Original_Mine, translationScrollView.content);
        messages.Add(messageData);

        // 사용 가능한 마이크 확인
        string[] devices = Microphone.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("사용 가능한 마이크가 없습니다!");
            return;
        }

        // 녹음 시작
        Debug.Log("녹음을 진짜 시작하는 부분(recordingClip)");
        recordingClip = Microphone.Start(null, true, (int)maxRecordingTime, RECORDING_FREQUENCY);

        // 실시간 스트리밍 코루틴 시작
        if (recordingCoroutine != null)
        {
            StopCoroutine(recordingCoroutine);
        }
        recordingCoroutine = StartCoroutine(StreamAudioData());
    }

    private IEnumerator StreamAudioData()
    {
        int lastPosition = 0;
        float[] tempBuffer = new float[SAMPLES_PER_CHUNK];
    
        while (isRecording)
        {
            int currentPosition = Microphone.GetPosition(null);
            if (currentPosition < 0 || lastPosition == currentPosition)
            {
                yield return new WaitForSeconds(CHUNK_DURATION / 2);
                continue;
            }
    
            // 청크 크기만큼의 데이터가 있는지 확인
            int availableSamples = 0;
            if (currentPosition < lastPosition)
            {
                // 버퍼가 순환된 경우
                availableSamples = (recordingClip.samples - lastPosition) + currentPosition;
            }
            else
            {
                availableSamples = currentPosition - lastPosition;
            }
    
            if (availableSamples >= SAMPLES_PER_CHUNK)
            {
                // 청크 데이터 추출
                recordingClip.GetData(tempBuffer, lastPosition);
                
                // Base64로 변환하여 전송
                string audioData = ConvertAudioToBase64(tempBuffer);
                if (!string.IsNullOrEmpty(audioData))
                {
                    TranslationManager.Instance.SendAudioData(audioData);
                }
    
                // 다음 청크를 위한 위치 업데이트
                lastPosition = (lastPosition + SAMPLES_PER_CHUNK) % recordingClip.samples;
            }
    
            yield return new WaitForSeconds(CHUNK_DURATION / 2);
        }
    }

    /// <summary>
    /// 녹음 중지 및 녹음된 오디오 전송
    /// </summary>
    private void StopRecording()
    {
        if (!isRecording) return;
        
        isRecording = false;
        
        // 발언자 상태 초기화 (이 부분이 currentSpeakerId를 비움)
        TranslationEventHandler.Instance.ResetSpeaker();
        
        // currentSpeakerId가 비워진 후에 대기 중인 오디오를 재생 큐로 이동
        while (pendingAudioBuffer.Count > 0)
        {
            string audioData = pendingAudioBuffer.Dequeue();
            audioBuffer.Enqueue(audioData);
        }
        
        if (audioBuffer.Count > 0)
        {
            StartAudioBuffer();  // 재생 시작
        }

        if (recordingCoroutine != null)
        {
            StopCoroutine(recordingCoroutine);
            recordingCoroutine = null;
        }
        
        Microphone.End(null);
    }

    public void OnInputAudioDone(int order, string text)
    {
        // order로 메시지 데이터를 찾음
        MessageData messageData = messages.FirstOrDefault(m => m.order == order);

        if (messageData != null)
        {
            // 발화자가 나인지 상대방인지에 따라 적절한 컴포넌트를 찾습니다.
            string contentName = messageData.isMine ? "Content_Mine" : "Content_Yours";
            TextMeshProUGUI contentText = messageData.userMessagePrefab.transform.Find(contentName)?.GetComponent<TextMeshProUGUI>();
            if (contentText != null)
            {
                contentText.text = text;
                Debug.Log($"Updated original text for order: {order}");
            }
            else
            {
                Debug.LogError($"{contentName} TMP component not found");
            }
        }
        else
        {
            Debug.LogError($"MessageData not found for order: {order}");
        }
    }


    public void OnOtherInputAudioDone(int order, string text, string speakerId)
    {
        // 상대방의 메시지 처리
        MessageData messageData = new MessageData();
        messageData.isMine = (speakerId == FireAuthManager.Instance.GetCurrentUser().UserId);
        messageData.order = order;
        messageData.userid = speakerId;

        // 발화자에 따라 적절한 프리팹 선택
        if (messageData.isMine)
        {
            messageData.userMessagePrefab = Instantiate(MessageBubble_Original_Mine, translationScrollView.content);
        }
        else
        {
            messageData.userMessagePrefab = Instantiate(MessageBubble_Original_Yours, translationScrollView.content);
        }

        // 텍스트 업데이트
        string contentName = messageData.isMine ? "Content_Mine" : "Content_Yours";
        TextMeshProUGUI contentText = messageData.userMessagePrefab.transform.Find(contentName)?.GetComponent<TextMeshProUGUI>();
        if (contentText != null)
        {
            contentText.text = text;
        }
        else
        {
            Debug.LogError($"{contentName} TMP component not found");
        }

        // 번역 메시지 프리팹 생성
        messageData.translationPrefab = Instantiate(MessageBubble_Translated, translationScrollView.content);

        // translationPrefab을 userMessagePrefab 아래에 위치시킴
        int index = messageData.userMessagePrefab.transform.GetSiblingIndex();
        messageData.translationPrefab.transform.SetSiblingIndex(index + 1);

        messages.Add(messageData);
    }


    public void UpdatePartialTranslatedText(int order, string partialText, string speakerId)
    {
        Debug.Log($"[Translation Debug] Starting UpdatePartialTranslatedText for order: {order}");

        // order로 메시지 데이터를 찾음
        MessageData messageData = messages.FirstOrDefault(m => m.order == order);

        // 메시지 데이터가 없는 경우 새로 생성
        if (messageData == null)
        {
            bool isMine = (speakerId == FireAuthManager.Instance.GetCurrentUser().UserId);

            messageData = new MessageData();
            messageData.order = order;
            messageData.isMine = isMine;
            messageData.userid = speakerId;

            // 발화자에 따라 적절한 프리팹 선택
            if (isMine)
            {
                messageData.userMessagePrefab = Instantiate(MessageBubble_Original_Mine, translationScrollView.content);
            }
            else
            {
                messageData.userMessagePrefab = Instantiate(MessageBubble_Original_Yours, translationScrollView.content);
            }

            messages.Add(messageData);
            Debug.Log($"Created new MessageData for order: {order}, isMine: {isMine}");
        }

        // 번역 프리팹이 없는 경우에만 생성
        if (messageData.translationPrefab == null)
        {
            Debug.Log("[Translation Debug] Creating new translation prefab");
            messageData.translationPrefab = Instantiate(MessageBubble_Translated, translationScrollView.content);
            int index = messageData.userMessagePrefab.transform.GetSiblingIndex();
            messageData.translationPrefab.transform.SetSiblingIndex(index + 1);
            Debug.Log($"[Translation Debug] Translation prefab created and positioned at index: {index + 1}");
        }

        // 번역된 내용을 표시하는 TextMeshProUGUI 컴포넌트를 찾습니다.
        TextMeshProUGUI textComponent = messageData.translationPrefab.transform.Find("TranslatedContent")?.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            // 기존 텍스트에 partialText를 추가합니다.
            textComponent.text += partialText;

            Debug.Log($"[Translation Debug] Updated accumulated text: {textComponent.text}");

            if (translationScrollView != null)
            {
                StartCoroutine(ScrollToBottomNextFrame());
            }
        }
        else
        {
            Debug.LogError("[Translation Debug] Failed to find TranslatedContent component");
        }
    }


    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        translationScrollView.verticalNormalizedPosition = 0f;
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
            if (IsSomeoneSpeaking)
            {
                // 누군가 말하고 있으면 pendingAudioBuffer에 저장
                pendingAudioBuffer.Enqueue(base64AudioData);
                Debug.Log($"[Audio] Someone is speaking ({TranslationEventHandler.Instance.CurrentSpeakerId}), buffering audio");
            }
            else
            {
                // 아무도 말하고 있지 않으면 바로 재생 큐에 추가
                audioBuffer.Enqueue(base64AudioData);
                if (!isPlaying)
                {
                    StartAudioBuffer();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[Audio] Error processing audio stream: {e.Message}");
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
            // 현재 버퍼의 전체 길이를 초 단위로 계산
            float bufferDuration = (float)audioBuffer.Count / RECORDING_FREQUENCY;
            // Debug.Log($"[Audio] Current buffer duration: {bufferDuration:F2} seconds");
            
            // 버퍼의 모든 데이터를 한 번에 재생
            int sampleCount = audioBuffer.Count;
            Debug.Log($"[Audio] Playing buffer of {sampleCount} samples");
            
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
    // public void OnSpeechApproved(string approvedUserId)
    // {
    //     print("OnSpeechApproved(발언권 승인됨)");
    //     if (FireAuthManager.Instance.GetCurrentUser().UserId == approvedUserId)
    //     {
    //         ShowCanSpeakUI();
    //         Debug.Log("UI 표시됨: ShowCanSpeakUI() 실행");
    //         StartRecording();
    //         Debug.Log("녹음 시작됨: StartRecording() 실행");

    //     }
    // }

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

    // 부분 번역된 텍스트를 업데이트하는 메서드
    // 이전의 것
    /*
    public void UpdatePartialTranslatedText(int order, string partialText)
    {
        if (translationScrollView == null || translationTextPrefab == null)
        {
            Debug.LogError("필수 UI 컴포넌트가 할당되지 않았습니다!");
            return;
        }

        try
        {
            if (!translationTexts.ContainsKey(order))
            {
                // 새로운 order이므로 프리팹 생성
                CreateNewTranslationText(order);
            }

            if (translationTexts.TryGetValue(order, out TextMeshProUGUI textComponent))
            {
                // 텍스트 업데이트
                textComponent.text = partialText;
                
                // UI 업데이트를 다음 프레임에서 실행
                StartCoroutine(ScrollToBottomNextFrame());
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"텍스트 업데이트 중 오류 발생: {e.Message}");
        }
    }
    */

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

    
}