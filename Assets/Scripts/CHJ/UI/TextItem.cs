using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class TextItem : MonoBehaviour
{
    //Text 
    Text chatText;

    public RectTransform Content;

    public RectTransform viewport;

    private void Awake()
    {
        // Text 컴포넌트 가져오자
        chatText = GetComponent<Text>();
    }

    public void AddText(string s)
    {
        // 텍스트 갱신
        chatText.text += s;

        // 사이즈 조절 코루틴 실행
        StartCoroutine(UpdateSize());
    }

    public void ResetText()
    {
        chatText.text = "";
        StartCoroutine (UpdateSize());
    }
    IEnumerator UpdateSize()
    {
        yield return null;

        // 텍스트의 내용에 맞춰서 크기를 조절
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, chatText.preferredHeight);

        yield return null;

        if(viewport.sizeDelta.y < Content.sizeDelta.y)
        {
            Content.anchoredPosition = new Vector2(0, Content.sizeDelta.y - viewport.sizeDelta.y);
        }
    }
}
