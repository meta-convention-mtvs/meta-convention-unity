using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace CHJ
{
    public class Popup : MonoBehaviour
    {
        RectTransform rectTransform;
        public float DisplayTime = 3.0f;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Show();
        }
        private void Start()
        {
            StartCoroutine(HideCoroutine(DisplayTime));
        }
        public void Show()
        {
            rectTransform.localScale = Vector3.zero;
            rectTransform.DOScale(1, 1).SetEase(Ease.OutBounce);
        }

        public void Hide()
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.DOScale(0, 1).SetEase(Ease.InOutFlash).OnComplete(() => Destroy(gameObject));
        }

        IEnumerator HideCoroutine(float duration)
        {
            yield return new WaitForSeconds(duration);
            Hide();
        }
    }

}