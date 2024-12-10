using Ricimi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SceneTransition))]
public class CheckBoothPositionDataAvailable : MonoBehaviour
{
    public Text errorText;
    public UIBoothPosition boothPositionMgr;
    public BoothCustomizingManager boothMgr;

    SceneTransition myTransition;
    Button myButton;


    private void Start()
    {
        myTransition = GetComponent<SceneTransition>();
        myButton = GetComponent<Button>();
    }

    public void OnCheckDataAvailable()
    {
        if (boothPositionMgr.SaveBoothPosition(boothMgr.GetBoothCategory()))
        {
            boothMgr.SaveBoothData();
            boothMgr.SaveOwnerData();
            myButton.interactable = false;
            myTransition.PerformTransition();
            errorText.text = "";
        }
        else
        {
            if (LanguageSingleton.Instance.language == "ko")
                errorText.text = "부스 위치를 설정해주세요!";
            else if (LanguageSingleton.Instance.language == "en")
                errorText.text = "Please set the booth location";
        }
    }

}
