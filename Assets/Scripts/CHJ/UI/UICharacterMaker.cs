using Ricimi;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacterMaker : MonoBehaviour
{
    public Button[] btn_top_prev;
    public Button[] btn_top_next;
    public Button[] btn_bottom_prev;
    public Button[] btn_bottom_next;
    public Button btn_save;
    public Button[] btn_gender;

    public Action OnTopPrevClick, OnTopNextClick, OnSaveClick, OnGenderClick, OnBottomPrevClick, OnBottomNextClick;

    private void Start()
    {
        foreach(Button button in btn_top_prev)
            button.onClick.AddListener(_OnTopPrevClick);
        foreach(Button button in btn_top_next)
            button.onClick.AddListener(_OnTopNextClick);
        foreach (Button button in btn_bottom_prev)
            button.onClick.AddListener(_OnBottomPrevClick);
        foreach (Button button in btn_bottom_next)
            button.onClick.AddListener(_OnBottonNextClick);
        btn_save.onClick.AddListener(_OnSaveClick);
        foreach(Button button in btn_gender)    
            button.onClick.AddListener(_OnGenderClick);
    }

    private void _OnTopPrevClick()
    {
        OnTopPrevClick?.Invoke();
    }
    private void _OnTopNextClick()
    {
        OnTopNextClick?.Invoke();
    }

    public void _OnBottomPrevClick()
    {
        OnBottomPrevClick?.Invoke();
    }

    public void _OnBottonNextClick()
    {
        OnBottomNextClick?.Invoke();
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
