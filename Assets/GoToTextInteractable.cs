using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class GoToTextInteractable : MonoBehaviour
{
    [SerializeField] private int pos1;
    [SerializeField] private int pos2;
    HorizontalLayoutGroup horizontal;
    private TweenerCore<int, int, NoOptions> lastTween;

    private void Start()
    {
        horizontal = GetComponent<HorizontalLayoutGroup>();
    }
    public void Toggle(bool b)
    {
        Debug.Log(b);
        lastTween?.Kill();
        if (b)
        {
            lastTween = DOTween.To(() => horizontal.padding.right, delegate (int x)
            {
                horizontal.padding.right = x;
            }, pos1, pos2);
        }
        else
        {
            lastTween = DOTween.To(() => horizontal.padding.right, delegate (int x)
            {
                horizontal.padding.right = x;
            }, pos2, pos1);
        }
        lastTween.Play();
    }
}
