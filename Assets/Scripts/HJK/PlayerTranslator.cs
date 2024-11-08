using UnityEngine;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;

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
    private AudioSource translatedAudioSource;
    private AudioClip recordingClip;
    private bool isRecording = false;
    private float[] tempRecordingBuffer;
    private int recordingPosition = 0;

    [SerializeField] private KeyCode speakKey = KeyCode.M;
    [SerializeField] private GameObject cantSpeakUI;
    [SerializeField] private float maxRecordingTime = 60f;
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;

    private const int RECORDING_FREQUENCY = 24000;
    private readonly int RECORDING_BUFFER_SIZE = 24000 * 60;

    // 스트리밍 재생을 위한 변수들
    private List<float> audioBuffer = new List<float>();
    private const int BUFFER_THRESHOLD = 24000;
    private bool isPlaying = false;
    private Coroutine playCoroutine;
    private bool isAudioCancelled = false;

    private static string currentSpeakerId = "";

    private void Start()
    {
        translatedAudioSource = gameObject.AddComponent<AudioSource>();
        tempRecordingBuffer = new float[RECORDING_BUFFER_SIZE];
    }

    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(speakKey))
        {
            TryStartRecording();
        }
        else if (Input.GetKeyUp(speakKey))
        {
            StopRecording();
        }
        else if (Input.GetKeyDown(cancelKey))
        {
            CancelRecording();
        }
    }

    public bool CanSpeak(string userId)
    {
        return string.IsNullOrEmpty(currentSpeakerId) || currentSpeakerId == userId;
    }

    private void TryStartRecording()
    {
        string userId = photonView.Owner.UserId;
        if (!CanSpeak(userId))
        {
            ShowCantSpeakUI();
            return;
        }

        TranslationManager.Instance.RequestSpeech();
        photonView.RPC("RPC_OnStartSpeaking", RpcTarget.All, userId);
    }

    private void StartRecording()
    {
        if (!string.IsNullOrEmpty(currentSpeakerId)) return;

        isRecording = true;
        recordingPosition = 0;
        
        string[] devices = Microphone.devices;
        if (devices.Length == 0)
        {
            Debug.LogError("사용 가능한 마이크가 없습니다!");
            return;
        }

        recordingClip = Microphone.Start(null, true, (int)maxRecordingTime, RECORDING_FREQUENCY);
    }

    private void StopRecording()
    {
        string userId = photonView.Owner.UserId;
        if (currentSpeakerId != userId) return;

        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            
            string audioData = ConvertAudioToBase64();
            if (!string.IsNullOrEmpty(audioData))
            {
                TranslationManager.Instance.SendAudioData(audioData);
                TranslationManager.Instance.DoneSpeech();
            }
        }
        
        photonView.RPC("RPC_OnStopSpeaking", RpcTarget.All, userId);
    }

    private string ConvertAudioToBase64()
    {
        int position = Microphone.GetPosition(null);
        if (position <= 0) position = recordingClip.samples;

        float[] samples = new float[position];
        recordingClip.GetData(samples, 0);

        // float[] to byte[] 변환
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        float rescaleFactor = 32767f;

        for (int i = 0; i < samples.Length; i++)
        {
            float sample = samples[i];
            if (Mathf.Abs(sample) < 0.001f)
                sample = 0f;
            
            if (sample > 1f) sample = 1f;
            if (sample < -1f) sample = -1f;
            
            intData[i] = (short)(sample * rescaleFactor);
        }

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        return Convert.ToBase64String(bytesData);
    }

    public void ProcessAudioStream(string base64AudioData)
    {
        if (isAudioCancelled) return;

        try
        {
            byte[] audioData = Convert.FromBase64String(base64AudioData);
            short[] shortArray = new short[audioData.Length / 2];
            Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
            
            float[] samples = new float[shortArray.Length];
            for (int i = 0; i < shortArray.Length; i++)
            {
                samples[i] = shortArray[i] / 32768f;
            }

            audioBuffer.AddRange(samples);
            StartAudioBuffer();
        }
        catch (Exception e)
        {
            Debug.LogError($"오디오 스트림 처리 중 오류: {e.Message}");
        }
    }

    private void StartAudioBuffer()
    {
        if (audioBuffer.Count >= BUFFER_THRESHOLD && !isPlaying)
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
            }
            playCoroutine = StartCoroutine(PlayBufferedAudio());
        }
    }

    private IEnumerator PlayBufferedAudio()
    {
        isPlaying = true;

        while (audioBuffer.Count > 0 && !isAudioCancelled)
        {
            int sampleCount = Mathf.Min(audioBuffer.Count, BUFFER_THRESHOLD);
            float[] playbackSamples = audioBuffer.GetRange(0, sampleCount).ToArray();
            audioBuffer.RemoveRange(0, sampleCount);

            AudioClip clip = AudioClip.Create("TranslatedAudio", 
                sampleCount, 1, RECORDING_FREQUENCY, false);
            clip.SetData(playbackSamples, 0);

            translatedAudioSource.clip = clip;
            translatedAudioSource.Play();

            yield return new WaitForSeconds(clip.length);
        }

        isPlaying = false;
        playCoroutine = null;
    }

    public void FinalizeAudioPlayback()
    {
        if (audioBuffer.Count > 0)
        {
            StartAudioBuffer();
        }
    }

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

    private void ShowCantSpeakUI()
    {
        if (cantSpeakUI != null)
            cantSpeakUI.SetActive(true);
    }

    [PunRPC]
    private void RPC_OnStartSpeaking(string speakerId)
    {
        if (!string.IsNullOrEmpty(currentSpeakerId) && currentSpeakerId != speakerId)
        {
            Debug.LogWarning("다른 사용자가 이미 말하고 있습니다.");
            return;
        }

        currentSpeakerId = speakerId;
        
        if (photonView.IsMine && speakerId == photonView.Owner.UserId)
        {
            StartRecording();
        }
    }

    [PunRPC]
    private void RPC_OnStopSpeaking(string speakerId)
    {
        if (currentSpeakerId == speakerId)
        {
            currentSpeakerId = "";
        }
    }

    private void CancelRecording()
    {
        string userId = photonView.Owner.UserId;
        if (currentSpeakerId != userId) return;

        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            TranslationManager.Instance.ClearAudioBuffer();
        }

        photonView.RPC("RPC_OnStopSpeaking", RpcTarget.All, userId);
    }

    private void OnDestroy()
    {
        if (photonView.IsMine && !string.IsNullOrEmpty(currentSpeakerId))
        {
            TranslationManager.Instance.ClearAudioBuffer();
            TranslationManager.Instance.LeaveRoom();
        }
    }

    public void OnSpeechApproved()
    {
        if (photonView.IsMine)
        {
            StartRecording();
            // 필요한 경우 UI 업데이트 등 추가 처리
        }
    }
}