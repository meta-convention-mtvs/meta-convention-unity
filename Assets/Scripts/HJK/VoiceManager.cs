using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI; // UI 네임스페이스 추가

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

    [SerializeField] // Inspector에서 설정할 수 있도록 SerializeField 추가
    private string userId;  // userId를 Inspector에서 설정
    
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
        // 'M' 키를 눌러 녹음 시작
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!isRecording)
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
            isAudioCancelled = false;
            string[] devices = Microphone.devices;
            Debug.Log($"사용 가능한 마이크: {string.Join(", ", devices)}");
            
            if (devices.Length == 0)
            {
                Debug.LogError("사용 가능한 마이크가 없습니다!");
                return;
            }
            
            try
            {
                recordedClip = Microphone.Start(null, true, 10, 24000);
                if (recordedClip != null)
                {
                    isRecording = true;
                    audioBuffer.Clear();
                    Debug.Log("녹음 시작됨: AudioClip 생성 성공");
                }
                else
                {
                    Debug.LogError("녹음 시작 실패: AudioClip이 null입니다");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"녹음 시작 중 오류: {e.Message}");
            }
        }
    }

    // 녹음을 중지하고 서버로 전송하는 메서드
    public void StopRecordingAndSend()
    {
        if (isRecording)
        {
            int position = Microphone.GetPosition(null);
            Debug.Log($"녹음 종료. 녹음된 위치: {position}");
            
            Microphone.End(null);
            isRecording = false;

            if (recordedClip != null)
            {
                lastRecordedAudioBase64 = ConvertAudioDataToBytes(recordedClip);
                Debug.Log($"변환된 오디오 데이터 길이: {(lastRecordedAudioBase64?.Length ?? 0)}");
                
                if (string.IsNullOrEmpty(lastRecordedAudioBase64))
                {
                    Debug.LogError("오디오 데이터 변환 실패");
                    return;
                }
                
                aiWebSocket.SendBufferAddAudio(lastRecordedAudioBase64);
                aiWebSocket.SendGenerateTextAudio();
            }
            else
            {
                Debug.LogError("recordedClip이 null입니다");
            }
        }
    }

    string ConvertAudioDataToBytes(AudioClip recordedClip)
    {
        // 전체 클립 길이로 배열 생성
        float[] samples = new float[recordedClip.samples * recordedClip.channels];
        
        // 녹음된 실제 위치 확인
        int position = Microphone.GetPosition(null);
        if (position <= 0) position = recordedClip.samples;

        // 실제 녹음된 데이터만큼만 새 배열 생성
        float[] actualSamples = new float[position];
        
        // 전체 데이터 가져오기
        if (!recordedClip.GetData(samples, 0))
        {
            Debug.LogError("오디오 데이터를 가져오는데 실패했습니다.");
            return null;
        }

        // 실제 녹음된 부분만 복사
        Array.Copy(samples, actualSamples, position);
        
        // 변환 및 반환
        byte[] audioData = ConvertToByteArray(actualSamples);
        return Convert.ToBase64String(audioData);
    }


    // 현재 재생 중인 오디오를 중지하는 메서드
    private void StopCurrentAudioPlayback()
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
        float rescaleFactor = 32767f; // float를 short로 변환할 때 사용할 스케일 팩터

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * rescaleFactor);
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
            try
            {
                byte[] audioData = Convert.FromBase64String(lastRecordedAudioBase64);
                Debug.Log($"오디오 데이터 길이: {audioData.Length}");
                
                if (audioData.Length == 0)
                {
                    Debug.LogError("오디오 데이터가 비어있습니다.");
                    return;
                }

                short[] shortArray = new short[audioData.Length / 2];
                Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
                
                float[] samples = new float[shortArray.Length];
                for (int i = 0; i < shortArray.Length; i++)
                {
                    samples[i] = shortArray[i] / 32768f;
                }

                if (samples.Length == 0)
                {
                    Debug.LogError("변환 샘플 데이터가 비어있습니다.");
                    return;
                }

                AudioClip clip = AudioClip.Create("LastRecording", samples.Length, 1, 24000, false);
                clip.SetData(samples, 0);

                audioSource.clip = clip;
                audioSource.Play();
                
                Debug.Log($"오디오 재생 시작: 샘플 길이 = {samples.Length}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"오디오 재생 중 오류 발생: {e.Message}");
            }
        }
        else
        {
            Debug.Log($"재생 불가: lastRecordedAudioBase64 null? {lastRecordedAudioBase64 == null}, isPlaying? {isPlaying}");
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
