using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System.Threading.Tasks;
using TriLibCore;

public class CreateBooth : MonoBehaviourPun
{
    public Transform[] BoothPosition;
    public Action<GameObject> OnBoothCreate;

    BoothType boothType;
    int boothPositionIndex;
    string boothFileName;

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
                DatabaseManager.Instance.GetCompanyData<BoothCustomizeData>(CashedDataFromDatabase.Instance.playerInfo.uuid, OnLoadBoothData);
                DatabaseManager.Instance.GetData<BoothPosition>(OnLoadBoothPosition);
            }
        }
    }

    void OnLoadBoothData(BoothCustomizeData data)
    {
        boothType = data.boothType;
        boothFileName = data.boothObjectPath;
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

            // TODO: 
            photonView.RPC(nameof(DeActivatePreviousBooth), RpcTarget.AllBuffered, boothPositionIndex);

            switch (boothType)
            {
                case BoothType.Blank:
                    photonView.RPC(nameof(RPCInstantiateBlankBooth), RpcTarget.All, CashedDataFromDatabase.Instance.playerInfo.uuid, boothFileName);

                    break;
                case BoothType.Cubic:
                    PhotonNetwork.Instantiate("New_Booth_BD", BoothPosition[boothPositionIndex].position, BoothPosition[boothPositionIndex].rotation);
                    break;
                case BoothType.Round:
                    PhotonNetwork.Instantiate("RoundBooth", BoothPosition[boothPositionIndex].position, BoothPosition[boothPositionIndex].rotation);
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

    private void OnLoadFinish(AssetLoaderContext context)
    {
        context.RootGameObject.transform.position = BoothPosition[boothPositionIndex].position;
        context.RootGameObject.transform.rotation = BoothPosition[boothPositionIndex].rotation;
    }

    [PunRPC]
    private void DeActivatePreviousBooth(int boothPositionIndex)
    {
        BoothPosition[boothPositionIndex].gameObject.SetActive(false);
    }
}
