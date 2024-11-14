using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBoothWithoutPhoton : MonoBehaviour
{
    public Transform[] BoothPosition;
    public Action<GameObject> OnBoothCreate;

    public GameObject cubicBooth;
    public GameObject roundBooth;

    BoothType boothType;
    int boothPositionIndex;

    bool isBoothDataLoaded;
    bool isBoothPositionLoaded;

    private void Start()
    {
        // 만약 내가 기업 유저이다
        // 부스를 만든다.
        // 부스 위치 정보를 읽어온다.
        // 부스 위치 정보에 맞는 위치에 부스를 생성한다.

        FireAuthManager.Instance.OnLogin += GetUserTypeData;

    }

    void GetUserTypeData()
    {
        DatabaseManager.Instance.GetData<UserTypeData>(OnLoadUserTypeData);
    }

    void OnLoadUserTypeData(UserTypeData data)
    {
        if (data != null)
        {
            if (data.userType == UserTypeData.UserType.company)
            {
                DatabaseManager.Instance.GetData<BoothCustomizeData>(OnLoadBoothData);
                DatabaseManager.Instance.GetData<BoothPosition>(OnLoadBoothPosition);
            }
        }
    }

    void OnLoadBoothData(BoothCustomizeData data)
    {
        boothType = data.boothType;
        isBoothDataLoaded = true;
        CheckAllDataLoaded();
    }

    void OnLoadBoothPosition(BoothPosition position)
    {
        boothPositionIndex = position.boothPositionIndex;
        isBoothPositionLoaded = true;
        CheckAllDataLoaded();
    }

    void CheckAllDataLoaded()
    {
        if (isBoothPositionLoaded && isBoothDataLoaded)
        {
            switch (boothType)
            {
                case BoothType.Blank:
                    //photonView.RPC(nameof(RPCInstantiateBlankBooth), )
                    break;
                case BoothType.Cubic:
                    Instantiate(cubicBooth);
                    break;
                case BoothType.Round:
                    Instantiate(roundBooth);
                    break;
            }
        }
    }
}
