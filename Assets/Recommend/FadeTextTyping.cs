using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI2.Recommend
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeTextTyping : MonoBehaviour
    {
        private CanvasGroup group;
        [SerializeField] private TextMeshProUGUI text;
        public string TypingText { get; set; }
        private readonly float FLOAT_DELTA = 0.00005f;
        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            group.alpha = 0;
        }

        public void Toggle(bool b)
        {
            if (b)
            {
                StopAllCoroutines();
                group.interactable = true;
                group.blocksRaycasts = true;
                StartCoroutine(SetAlpha(1));
                StartCoroutine(Typing(TypingText));
            }
            else
            {
                StopAllCoroutines();
                group.interactable = false;
                group.blocksRaycasts = false;
                StartCoroutine(SetAlpha(0));
                StartCoroutine(Typing(""));
            }
        }

        private IEnumerator SetAlpha(float alpha)
        {
            for (var a = group.alpha; !IsEqual(a, alpha); a = Mathf.Lerp(group.alpha, alpha, Time.deltaTime * 10))
            {
                group.alpha = a;
                yield return new WaitForEndOfFrame();
            }
            group.alpha = alpha;
        }

        private IEnumerator Typing(string str)
        {
            text.text = "";
            for (var i = 1; i < str.Length; i++)
            {
                text.text = str[..i];
                yield return new WaitForSeconds(0.02f);
            }
            text.text = str;
        }

        private bool IsEqual(float f1, float f2)
        {
            return Mathf.Abs(f1 - f2) < FLOAT_DELTA;
        }
    }
}