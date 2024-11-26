using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChargedBoothPosition : MonoBehaviour
{
    public ChargedBoothPosition newData;
    public bool[] isChargedList;

    private void Start()
    {
        newData = new ChargedBoothPosition();

        newData.BoothPositionList = new List<ChargedBoothData>();
        for(int i = 0; i < isChargedList.Length; i++)
        {
            newData.BoothPositionList.Add(new ChargedBoothData(isChargedList[i], ""));
        }

        FireAuthManager.Instance.OnLogin += ResetChargedBoothPosition;
    }

    void ResetChargedBoothPosition()
    {
        DatabaseManager.Instance.SavePublicData<ChargedBoothPosition>(newData);
    }
}
