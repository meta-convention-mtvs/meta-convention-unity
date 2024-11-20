using UnityEngine;
using DG.Tweening;
using UnityEngine.UI; // DOTween 네임스페이스 추가

public class ButtonFadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup; // CanvasGroup 컴포넌트
    public float fadeInDuration = 1.0f; // 페이드 인 시간

    public Dropdown Dropdown;

    bool isFaded = false;
    public void FadeIn()
    {
        if (!isFaded)
        {
            canvasGroup.alpha = 0; // 초기 알파값을 0으로 설정하여 보이지 않게 함
            canvasGroup.DOFade(1, fadeInDuration); // 알파값을 1로 페이드 인 시킴
            isFaded = true;
        }
    }
}
