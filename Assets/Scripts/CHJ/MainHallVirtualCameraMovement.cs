using Cinemachine;
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
    }

    public void SetActiveVirtualCamera(CinemachineVirtualCamera activeCamera)
    {
        virtualCameras = ResetVirtualCameraPrioriy(virtualCameras);
        activeCamera.Priority = 20;
    }

    public void SetBrochureCameraPosition(Transform brochure, Transform brochureCamTransform)
    {
        brocureCamera.gameObject.transform.position = brochureCamTransform.position;
        brocureCamera.LookAt = brochure;
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
