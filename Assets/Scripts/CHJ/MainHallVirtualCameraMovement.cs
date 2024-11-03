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
    public CinemachineTargetGroup targetGroup;

    public void SetAiSpeackUICamera()
    {
        // priority를 높게 바꿔준다
        aiSpeackVirtualCamera.Priority = 20;
        playerFollowCamera.Priority = 10;
    }

    public void SetPlayerFollowCamera()
    {
        aiSpeackVirtualCamera.Priority = 10;
        playerFollowCamera.Priority = 20;
    }
}
