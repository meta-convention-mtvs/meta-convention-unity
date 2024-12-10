using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro; // DOTween 네임스페이스 추가

public class ButtonFadeIn : MonoBehaviour
{
    public CanvasGroup canvasGroup; // CanvasGroup 컴포넌트
    public float fadeInDuration = 1.0f; // 페이드 인 시간

    public TMP_Dropdown Dropdown;

    bool isFaded = false;

    private void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        Dropdown.onValueChanged.AddListener((n) => FadeIn());
    }
    public void FadeIn()
    {
        if (!isFaded)
        {
            canvasGroup.alpha = 0; // 초기 알파값을 0으로 설정하여 보이지 않게 함
            canvasGroup.DOFade(1, fadeInDuration).OnComplete(()=> canvasGroup.blocksRaycasts = true); // 알파값을 1로 페이드 인 시킴
            isFaded = true;
        }
    }
}
