using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBrochureObject : MonoBehaviour,IKeyInteractableObject
{
    public Transform brochureCameraTransform;
    public Transform brochurePosition;

    public void HideText()
    {
        
    }

    public void Interact()
    {
        MainHallVirtualCameraMovement.Instance.SetBrochureCameraPosition(brochurePosition, brochureCameraTransform);
        // 시네머신 카메라가 브로슈어 앞으로 이동한다.
        MainHallVirtualCameraMovement.Instance.SetActiveVirtualCamera(MainHallVirtualCameraMovement.Instance.brocureCamera);
        // F키를 눌러서 화면 끄기 버튼 추가하기
    }

    public void InteractEnd()
    {
        // 시네머신 카메라가 다시 원래대로 돌아간다.
        MainHallVirtualCameraMovement.Instance.SetActiveVirtualCamera(MainHallVirtualCameraMovement.Instance.playerFollowCamera);
        
    }

    public void ShowText()
    {
        UIManager.Instance.ShowPopupUI("(F) 키 눌러 자세히 보기", "(F) Press the key to see details");
    }
}
