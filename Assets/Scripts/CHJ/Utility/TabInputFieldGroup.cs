using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabInputFieldGroup : MonoBehaviour
{
    public InputField[] inputFieldGroup;

    [SerializeField]
    int currentIdx = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentIdx++;
            currentIdx %= inputFieldGroup.Length;
            SetInputFieldActive(inputFieldGroup, currentIdx);
        }
    }

    void SetInputFieldActive(InputField[] inputFields, int idx)
    {
        inputFields[idx].ActivateInputField();
    }
}
