using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;

public class UIAISummary : MonoBehaviour
{
    public Text summaryText;
    public Text originalText;

    public CanvasGroup canvas;

    public RectTransform summaryContentRectTransform;  // Scroll View의 Content RectTransform
    public RectTransform originalContentRectTransform;

    private void Start()
    {
        canvas.DOFade(1, 1);
        canvas.blocksRaycasts = true;
    }

    public void SetSummaryText(string summary)
    {
        summaryText.text = summary;
        UpdateContentHeight(summaryText, summaryContentRectTransform);
    }

    public void SetAllText(string text)
    {
        originalText.text = text;
        UpdateContentHeight(originalText, originalContentRectTransform);
    }
    public void Hide()
    {
        canvas.DOFade(0, 1);
        canvas.blocksRaycasts = false;
    }


    // 텍스트 내용이 변경될 때마다 호출
    public void UpdateContentHeight(Text text, RectTransform contentRectTransform)
    {
        // 텍스트가 변경된 후 레이아웃을 강제로 갱신
        LayoutRebuilder.ForceRebuildLayoutImmediate(text.rectTransform);

        // Text 컴포넌트의 preferredHeight를 사용하여 텍스트의 높이를 가져옴
        float preferredHeight = text.preferredHeight;

        // Content의 높이를 텍스트의 높이에 맞게 조정
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
        contentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
    }

}
