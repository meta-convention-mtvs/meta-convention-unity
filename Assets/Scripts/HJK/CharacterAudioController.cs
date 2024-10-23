using UnityEngine;

public class CharacterAudioController : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;

    void Update()
    {
        if (audioSource.isPlaying)
        {
            // 음성이 재생 중일 때 말하는 애니메이션 활성화
            animator.SetBool("isTalking", true);
        }
        else
        {
            // 음성이 끝났을 때 대기 애니메이션 활성화
            animator.SetBool("isTalking", false);
        }
    }
}
