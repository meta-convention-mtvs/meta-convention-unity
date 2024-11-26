using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChargedBoothPosition : MonoBehaviour
{
    public ChargedBoothPosition newData;

    private void Start()
    {
        newData = new ChargedBoothPosition();

        newData.BoothPositionList = new List<ChargedBoothData>();
        newData.BoothPositionList.Add(new ChargedBoothData(true, ""));
        newData.BoothPositionList.Add(new ChargedBoothData(false, ""));
        newData.BoothPositionList.Add(new ChargedBoothData(true, ""));
        newData.BoothPositionList.Add(new ChargedBoothData(false, ""));

        FireAuthManager.Instance.OnLogin += ResetChargedBoothPosition;
    }

    void ResetChargedBoothPosition()
    {
        DatabaseManager.Instance.SavePublicData<ChargedBoothPosition>(newData);
    }
}
