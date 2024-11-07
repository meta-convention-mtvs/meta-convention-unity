using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBusinessRoomQueueManager : MonoBehaviour
{
    public Button yesButton;
    public Button noButton;

    public Action OnYesButtonClick, OnNoButtonClick;

    private void Start()
    {
        yesButton.onClick.AddListener(_OnYesButtonClick);
        noButton.onClick.AddListener(_OnNoButtonClick);
    }

    void _OnYesButtonClick()
    {
        OnYesButtonClick?.Invoke();
    }

    void _OnNoButtonClick()
    {
        OnNoButtonClick?.Invoke();
    }
}
