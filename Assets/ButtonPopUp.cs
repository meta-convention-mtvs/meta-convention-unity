using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPopUp : MonoBehaviour
{
    public void ShowPopup(string s)
    {
        UIManager.Instance.ShowPopupUI(s);
    }
}
