using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CompanyRecommendSelect : MonoBehaviour
{
    public TMP_Text example1;
    public TMP_Text example2;

    public TMP_InputField requestText;

    public void OnClickSetText1()
    {
        print(example1.text);
        requestText.text = example1.text;
    }

    public void OnClickSetText2() 
    { 
        requestText.text = example2.text;
    }


}
