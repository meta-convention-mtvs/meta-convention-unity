using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterMaker : MonoBehaviour
{
    public Button btn_prev;
    public Button btn_next;
    public Button btn_save;
    public Button btn_gender;

    public Action OnPrevClick, OnNextClick, OnSaveClick, OnGenderClick;

    private void Start()
    {
        btn_prev.onClick.AddListener(_OnPrevClick);
        btn_next.onClick.AddListener(_OnNextClick);
        btn_save.onClick.AddListener(_OnSaveClick);
        btn_gender.onClick.AddListener(_OnGenderClick);
    }

    private void _OnPrevClick()
    {
        OnPrevClick?.Invoke();
    }
    private void _OnNextClick()
    {
        OnNextClick?.Invoke();
    }

    private void _OnSaveClick()
    {
        OnSaveClick?.Invoke();
    }

    private void _OnGenderClick()
    {
        OnGenderClick?.Invoke();
    }
}
