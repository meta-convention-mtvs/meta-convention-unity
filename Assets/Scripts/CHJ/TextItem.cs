using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TextItem : MonoBehaviour
{
    //Text 
    Text chatText;

    RectTransform rt;

    public RectTransform viewport;

    private void Awake()
    {
        // Text 컴포넌트 가져오자
        chatText = GetComponent<Text>();
        rt = GetComponent<RectTransform>();
    }

    public void SetText(string s)
    {
        // 텍스트 갱신
        chatText.text += s;

        // 사이즈 조절 코루틴 실행
        StartCoroutine(UpdateSize());
    }

    IEnumerator UpdateSize()
    {
        yield return null;

        // 텍스트의 내용에 맞춰서 크기를 조절
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, chatText.preferredHeight);

        yield return null;

        if(viewport.sizeDelta.y < rt.sizeDelta.y)
        {
            rt.anchoredPosition = new Vector2(0, rt.sizeDelta.y - viewport.sizeDelta.y);
        }
    }
}
