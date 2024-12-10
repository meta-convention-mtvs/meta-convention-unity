using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneFadeIn : MonoBehaviour
{
    [SerializeField] Image blackScreen;
    [SerializeField] float duration = 2.0f;

    void Start()
    {
        FadeIn(duration);
    }

    // Fade In 애니메이션
    void FadeIn(float seconds)
    {
        // 검정색 화면을 서서히 투명하게 만드는 애니메이션
        blackScreen.DOFade(0f, seconds)
            .OnStart(() => {
                blackScreen.color = new Color(1, 1, 1, 1);
            }).OnComplete(() => Destroy(blackScreen.gameObject));
    }
}
