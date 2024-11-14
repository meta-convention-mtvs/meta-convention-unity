using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AutoFillInput : MonoBehaviour
{
    [SerializeField] private List<InputField> inputField;
    [SerializeField] private List<string> texts;

    public void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            StopAllCoroutines();
            StartCoroutine(Fill());

        }
    }
    public IEnumerator Fill()
    {
        for (var i = 0; i < inputField.Count; i++)
        {
            var text = texts[i];
            inputField[i].text = "";
            for (var k = 0; k < text.Length; k++)
            {
                inputField[i].text += text[k];
                yield return new WaitForSeconds(0.05f);
            }

            //DOTween.To(() => inputField[i].text, x => inputField[i].text = x, text, 0.05f * text.Length)
            //.SetEase(Ease.Linear);
            //yield return new WaitForSeconds(0.05f * text.Length);
        }
    }
}
