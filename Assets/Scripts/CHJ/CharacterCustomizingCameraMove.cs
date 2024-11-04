using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomizingCameraMove : MonoBehaviour
{
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera topCamera;
    public CinemachineVirtualCamera bottomCamera;

    public void SetMainCamera()
    {
        mainCamera.Priority = 20;
        topCamera.Priority = 10;
        bottomCamera.Priority = 10;
    }

    public void SetTopCamera()
    {
        mainCamera.Priority = 10;
        topCamera.Priority = 20;
        bottomCamera.Priority = 10;
    }

    public void SetBottomCamera()
    {
        mainCamera.Priority = 10;
        topCamera.Priority = 10;
        bottomCamera.Priority = 20;
    }
}
