using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabTMPInputFieldGroup : MonoBehaviour
{
    public TMP_InputField[] inputFieldGroup;

    [SerializeField]
    int currentIdx = 0;

    private void Start()
    {
        SetInputFieldActive(inputFieldGroup, 0);
    }

    void Update()
    {
        currentIdx = GetCurrentActiveInputField(inputFieldGroup);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentIdx++;
            currentIdx %= inputFieldGroup.Length;
            SetInputFieldActive(inputFieldGroup, currentIdx);
        }
    }

    void SetInputFieldActive(TMP_InputField[] inputFields, int idx)
    {
        inputFields[idx].ActivateInputField();
    }

    int GetCurrentActiveInputField(TMP_InputField[] inputFields)
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i].isFocused)
            {
                return i;
            }
        }

        return -1;
    }
}
