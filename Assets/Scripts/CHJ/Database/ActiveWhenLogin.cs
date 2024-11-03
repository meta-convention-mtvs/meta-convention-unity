using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveWhenLogin : MonoBehaviour
{
    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        FireAuthManager.Instance.OnLogin += () => ActiveButton(button);
    }
 
    void ActiveButton(Button button)
    {
        button.interactable = true;
    }
}
