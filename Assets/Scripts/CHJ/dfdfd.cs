using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UICompanyRecommend))]
public class dfdfd : MonoBehaviour
{
    UICompanyRecommend ui_cr;

    private void Start()
    {
        ui_cr = GetComponent<UICompanyRecommend>();
        ui_cr.GetRecommendDataFromDatabase();
    }
}
