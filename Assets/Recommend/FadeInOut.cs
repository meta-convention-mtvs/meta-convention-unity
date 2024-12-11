using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI2.Recommend
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeInOut : MonoBehaviour
    {
        private CanvasGroup group;
        private readonly float FLOAT_DELTA = 0.00005f;
        [SerializeField] private bool inverse = false;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        public void Toggle(bool b)
        {
            if (b | inverse)
            {
                StopAllCoroutines();
                StartCoroutine(SetAlpha(0));
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(SetAlpha(1));
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

        private bool IsEqual(float f1, float f2)
        {
            return Mathf.Abs(f1 - f2) < FLOAT_DELTA;
        }
    }
}