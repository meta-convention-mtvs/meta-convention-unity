using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    public Animator animator;
    public VoiceManager voiceManager;

    void Start()
    {
        //voiceManager = GameObject.FindWithTag("VoiceManager")?.GetComponent<VoiceManager>();
        // AudioSource 컴포넌트가 없으면 자동으로 추가
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        // VoiceManager의 AudioSource 상태를 현재 AudioSource에 동기화
        if (voiceManager != null && voiceManager.audioSource != null)
        {
            audioSource.clip = voiceManager.audioSource.clip;
            animator.SetBool("isTalking", voiceManager.IsPlaying());
        }
    }
}
