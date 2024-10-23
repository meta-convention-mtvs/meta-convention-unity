using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAiSpeack : MonoBehaviour
{
    public TextItem textItem;

    // Content 의 Transform
    public RectTransform trContent;

    // ChatView 의 Transform
    public RectTransform trChatView;

    // 채팅이 추가되기 전의 Content 의 H(높이) 값을 가지고 있는 변수
    float prevContentH;

    private void Start()
    {
        textItem.onAutoScroll += AutoScrollBottom;
    }

    // 채팅 추가 되었을 때 맨밑으로 Content 위치를 옮기는 함수
    public void AutoScrollBottom()
    {
        // chatView 의 H 보다 content 의 H 값이 크다면 (스크롤이 가능한 상태라면)
        if (trContent.sizeDelta.y > trChatView.sizeDelta.y)
        {
            //trChatView.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;

            // 이전 바닥에 닿아있었다면
            if (prevContentH - trChatView.sizeDelta.y <= trContent.anchoredPosition.y)
            {
                // content 의 y 값을 재설정한다.
                trContent.anchoredPosition = new Vector2(0, trContent.sizeDelta.y - trChatView.sizeDelta.y);
            }
        }
    }
}
