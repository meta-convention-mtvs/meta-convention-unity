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
                    if (LanguageSingleton.Instance.language == "ko")
                        errorText.text = "회사 이름을 적어주세요!";
                    else if (LanguageSingleton.Instance.language == "en")
                        errorText.text = "Please write your company name";
                    break;
                case BoothCustomizingManager.BoothCustomzieDataSaveError.EmptyBoothObject:
                    if (LanguageSingleton.Instance.language == "ko")
                        errorText.text = "회사 부스를 설정해주세요!";
                    else if (LanguageSingleton.Instance.language == "en")
                        errorText.text = "Please set up a company booth";
                    break;
            }
        }
    }
}
