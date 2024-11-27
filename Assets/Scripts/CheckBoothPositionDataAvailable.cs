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
        if (boothPositionMgr.SaveBoothPosition())
        {
            boothMgr.SaveBoothData();
            myButton.interactable = false;
            myTransition.PerformTransition();
            errorText.text = "";
        }
        else
        {
            errorText.text = "부스 위치를 설정해주세요!";
        }
    }

}
