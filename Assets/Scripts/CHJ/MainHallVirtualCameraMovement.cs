using Cinemachine;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainHallVirtualCameraMovement : MonoBehaviour
{
    public static MainHallVirtualCameraMovement Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public CinemachineVirtualCamera aiSpeackVirtualCamera;
    public CinemachineVirtualCamera playerFollowCamera;
    public CinemachineVirtualCamera brocureCamera;

    public List<CinemachineVirtualCamera> virtualCameras;

    CreatePlayer playerFactory;
    StarterAssetsInputs playerInputs;

    public enum MainHallCameraType
    {
        PlayerFollowCam,
        AiEmployeeCam,
        BrocureCam
    }

    private void Start()
    {
        virtualCameras = new List<CinemachineVirtualCamera>();
        virtualCameras.Add(playerFollowCamera);
        virtualCameras.Add(aiSpeackVirtualCamera);
        virtualCameras.Add(brocureCamera);

        playerFactory = GameObject.FindWithTag("PlayerFactory").GetComponent<CreatePlayer>();
        if (playerFactory == null)
            Debug.LogError("Player Factory is null... set tag");
        playerFactory.OnPlayerCreate += OnPlayerCreate;
    }

    void OnPlayerCreate(GameObject player)
    {
        playerInputs = player.GetComponent<StarterAssetsInputs>();
    }

    public void SetActiveVirtualCamera(CinemachineVirtualCamera activeCamera)
    {
        virtualCameras = ResetVirtualCameraPrioriy(virtualCameras);
        activeCamera.Priority = 20;
        if(activeCamera != playerFollowCamera)
        {
            playerInputs.cursorInputForLook = false;
        }
        else
        {
            playerInputs.cursorInputForLook = true;
        }
        
    }

    public void SetBrochureCameraPosition(Transform brochure, Transform brochureCamTransform)
    {
        brocureCamera.gameObject.transform.position = brochureCamTransform.position;
        brocureCamera.LookAt = brochure;
    }

    // ToDo: aiSpeackCamera의 position도 지정할 수 있어야 함
    public void SetAiSpeackCameraPosition(Transform lookAtPosition, Transform followPosition)
    {
        aiSpeackVirtualCamera.LookAt = lookAtPosition;
        aiSpeackVirtualCamera.Follow = followPosition;
    }
    List<CinemachineVirtualCamera> ResetVirtualCameraPrioriy(List<CinemachineVirtualCamera> cameras)
    {
        List<CinemachineVirtualCamera> newCameras = new List<CinemachineVirtualCamera>(cameras);
        foreach(CinemachineVirtualCamera camera in newCameras)
        {
            camera.Priority = 10;
        }
        return newCameras;
    }


}
