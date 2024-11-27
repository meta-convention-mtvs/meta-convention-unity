using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnterkeyInputWithoutInteractable : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (button.interactable && Input.GetKeyDown(KeyCode.Return))
        {
            button.onClick?.Invoke();
        }
    }
}
