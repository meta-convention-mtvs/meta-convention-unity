using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroupTransition))]
public class CheckBoothDataAvailable : MonoBehaviour
{
    public Text errorText;
    public BoothCustomizingManager boothMgr;

    CanvasGroupTransition myTransition;

    private void Start()
    {
        myTransition = GetComponent<CanvasGroupTransition>();    
    }
    public void OnCheckDataAvailable()
    {
        BoothCustomizingManager.BoothCustomzieDataSaveError errorCode = new BoothCustomizingManager.BoothCustomzieDataSaveError();
        if (boothMgr.CanSaveData(ref errorCode))
        {
            errorText.text = "";
            myTransition.FadeOldCanvasGroup();
        }
        else
        {
            switch (errorCode)
            {
                case BoothCustomizingManager.BoothCustomzieDataSaveError.EmptyCompanyName:
                    errorText.text = "회사 이름을 적어주세요!";
                    break;
                case BoothCustomizingManager.BoothCustomzieDataSaveError.EmptyBoothObject:
                    errorText.text = "회사 부스를 설정해주세요!";
                    break;
            }
        }
    }
}
