using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Threading.Tasks;

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
                    // 데이터 베이스에서 부스 오브젝트를 읽어온다 => 부스를 읽은 다음 저장 후 불러온다. 그런 다음 적절한 위치로 옮긴다.

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

    [PunRPC]
    async Task RPCInstantiateBlankBooth(string uuid, string objectFileName)
    {
        string localPath = await AsyncDatabase.GetObjectFileLocalPathFromDatabaseWithUid(uuid, objectFileName);
        ObjectLoader.ImportGLTFAsync(localPath, OnLoadFinish);
    }

    private void OnLoadFinish(GameObject obj, AnimationClip[] arg2)
    {
        obj.transform.position = BoothPosition[boothPositionIndex].position;
    }
}
