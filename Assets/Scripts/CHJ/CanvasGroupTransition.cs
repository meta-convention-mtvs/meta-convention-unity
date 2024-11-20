using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CanvasGroupTransition : MonoBehaviour
{
    public CanvasGroup oldCanvasGroup;
    public CanvasGroup newCanvasGroup;

    public void FadeOldCanvasGroup()
    {
        oldCanvasGroup.DOFade(0, 0.5f).OnComplete(() => { oldCanvasGroup.blocksRaycasts = false; });
        oldCanvasGroup.gameObject.GetComponent<RectTransform>().DOScale(0.5f, 2).OnComplete(FadeNewCanvasGroup);
    }

    void FadeNewCanvasGroup()
    {
        newCanvasGroup.DOFade(1, 1).OnComplete(() => { newCanvasGroup.blocksRaycasts = true; });
        newCanvasGroup.gameObject.GetComponent<RectTransform>().transform.localScale = new Vector2(0.5f, 0.5f);
        newCanvasGroup.gameObject.GetComponent<RectTransform>().DOScale(1, 1);
    }
}
