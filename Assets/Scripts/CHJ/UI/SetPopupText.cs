using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPopupText : MonoBehaviour
{
    Text popupText;

    private void Awake()
    {
        popupText = GetComponentInChildren<Text>();
    }

    public void SetText(string s)
    {
        popupText.text = s;
    }


}
