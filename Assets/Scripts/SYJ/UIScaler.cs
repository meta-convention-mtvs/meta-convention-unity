using System.Collections;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    [SerializeField] private RectTransform target;
    [SerializeField] private float multiplier = 1.5f;
    [SerializeField] private Vector2 originSize;
    private readonly float FLOAT_DELTA = 0.00005f;
    private float currentMultiplier = 1f;

    private void Awake()
    {
        if (target == null)
        {
            target = GetComponent<RectTransform>();
        }
    }

    public void ScaleUp(bool b)
    {
        if (target == null)
        {
            Debug.LogWarning("target is null");
            return;
        }
        if (b)
        {
            if (originSize == default)
            {
                originSize = target.rect.size;
            }
            StopAllCoroutines();
            StartCoroutine(SizeTo(multiplier));
        }
        else
        {
            if (originSize == default)
            {
                originSize = target.rect.size;
            }
            StopAllCoroutines();
            StartCoroutine(SizeTo(1));
        }
    }

    private IEnumerator SizeTo(float multiplier)
    {
        var t = Time.deltaTime * 10;
        for (var m = currentMultiplier; !IsEqual(m, multiplier); m = Mathf.Lerp(m, multiplier, t))
        {
            currentMultiplier = m;
            target.sizeDelta = originSize * m;
            yield return new WaitForEndOfFrame();
        }
        target.sizeDelta = originSize * multiplier;
    }

    private bool IsEqual(float f1, float f2)
    {
        return Mathf.Abs(f1 - f2) < FLOAT_DELTA;
    }
}
