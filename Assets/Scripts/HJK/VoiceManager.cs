using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text;

public class VoiceManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AIWebSocket aiWebSocket;

    private AudioClip recordedClip;
    private bool isRecording = false;
    private List<float> audioBuffer = new List<float>();
    private const int BUFFER_THRESHOLD = 24000;
    private bool isPlaying = false;
    private string lastRecordedAudioBase64;

    private Coroutine playCoroutine;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

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

    public void StartRecording()
    {
        if (!isRecording)
        {
            recordedClip = Microphone.Start(null, true, 10, 24000);
            isRecording = true;
            audioBuffer.Clear();
            Debug.Log("녹음 시작");
        }
    }

    public void StopRecordingAndSend()
    {
        if (isRecording)
        {
            int position = Microphone.GetPosition(null);
            Microphone.End(null);
            isRecording = false;
            Debug.Log("녹음 중지");

            float[] samples = new float[position];
            recordedClip.GetData(samples, 0);
            byte[] audioData = ConvertToByteArray(samples);
            lastRecordedAudioBase64 = Convert.ToBase64String(audioData);
            
            aiWebSocket.SendBufferAddAudio(lastRecordedAudioBase64);
            aiWebSocket.SendGenerateTextAudio();
        }
    }

    private void StopCurrentAudioPlayback()
    {
        Debug.Log("오디오 재생 중지 요청");
        
        audioSource.Stop();
        audioBuffer.Clear();
        isPlaying = false;
        
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        aiWebSocket.SendGenerateCancel();
    }

    public void HandleAudioDelta(string base64AudioDelta)
    {
        byte[] audioData = System.Convert.FromBase64String(base64AudioDelta);
        short[] shortArray = new short[audioData.Length / 2];
        System.Buffer.BlockCopy(audioData, 0, shortArray, 0, audioData.Length);
        
        float[] samples = new float[shortArray.Length];
        for (int i = 0; i < shortArray.Length; i++)
        {
            samples[i] = shortArray[i] / 32768f;
        }

        audioBuffer.AddRange(samples);

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

        while (audioBuffer.Count > 0)
        {
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
        playCoroutine = null;
    }

    private byte[] ConvertToByteArray(float[] samples)
    {
        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];
        float rescaleFactor = 32767f;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(Mathf.Clamp(samples[i], -1f, 1f) * rescaleFactor);
        }

        Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);
        return bytesData;
    }
}
