using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    public Slider slider;
    Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        slider.onValueChanged.AddListener(SetTextInComponent);
    }

    public void SetTextInComponent(float f)
    {
        text.text = f.ToString();
    }
}
