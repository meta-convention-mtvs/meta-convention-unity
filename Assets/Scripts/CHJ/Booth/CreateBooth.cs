using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateBooth : MonoBehaviourPun
{
    public Transform[] BoothPosition;
    public Action<GameObject> OnBoothCreate;

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
                    PhotonNetwork.Instantiate("CubicBooth", BoothPosition[boothPositionIndex].position, Quaternion.identity);
                    break;
                case BoothType.Round:
                    PhotonNetwork.Instantiate("RoundBooth", BoothPosition[boothPositionIndex].position, Quaternion.identity);
                    break;
            }
        }
    }
}
