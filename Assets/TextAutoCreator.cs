using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TextAutoCreator : MonoBehaviour
{
    public Text text;
    public string myText;

    private void Start()
    {
        text.DOText(myText, 5);
    }
}
