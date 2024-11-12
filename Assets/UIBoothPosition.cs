using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBoothPosition : MonoBehaviour
{
    public List<GameObject> boothPosition;

    public Camera renderTextureCamera; // RenderTexture를 렌더링하는 카메라
    public RawImage rawImage; // RawImage UI 요소
    private RectTransform rectTransform;
    private int currentIndex = -1;
    private SelectableParentObject selectedObject;
    private ChargedBoothPosition position;

    void Start()
    {
        rectTransform = rawImage.GetComponent<RectTransform>();
        DatabaseManager.Instance.GetPublicData<ChargedBoothPosition>(SetChargedBoothPosition);
    }

    void SetChargedBoothPosition(ChargedBoothPosition position)
    {
        this.position = position;
        for(int i = 0; i < position.BoothPositionList.Count; i++)
        {
            if(i < boothPosition.Count)
            {
                boothPosition[i].GetComponentInParent<SelectableParentObject>().isInteractable = !position.BoothPositionList[i];
            }
        }
    }
    void Update()
    {

        // 마우스가 RawImage 위에 있는지 확인
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Input.mousePosition))
        {
            Ray ray = renderTextureCamera.ViewportPointToRay(GetViewportPointInRecttTransform(rectTransform, Input.mousePosition));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 마우스를 눌렀으면
                if (Input.GetMouseButtonDown(0))
                {
                    if(selectedObject != null)
                        selectedObject.Deselect();
                    currentIndex = FindObjectIndexInList(boothPosition, hit.collider.gameObject);
                    selectedObject = hit.collider.GetComponentInParent<SelectableParentObject>();
                    if (selectedObject != null)
                    {
                        selectedObject.Select();
                    }
                }
                // 선택또는 선택 해제

                // 선택 해제 상황이면
                if(currentIndex == -1)
                {
                    // ray에 닿는 물체를 선택한다.
                    if (selectedObject != null)
                        selectedObject.Deselect();
                    selectedObject = hit.collider.GetComponentInParent<SelectableParentObject>();
                    if (selectedObject != null)
                    {
                        selectedObject.Select();
                    }
                }
            }

        }
    }

    public void SaveBoothPosition()
    {
        if(currentIndex != -1)
        {
            BoothPosition myPosition = new BoothPosition();
            myPosition.boothPositionIndex = currentIndex;
            DatabaseManager.Instance.SaveData<BoothPosition>(myPosition);
        }
    }

    public bool CanSaveData()
    {
        if (currentIndex == -1)
            return false;
        if (position != null && position.BoothPositionList[currentIndex] == true)
            return false;
        return true;
    }

    Vector2 GetViewportPointInRecttTransform(RectTransform rectTransform, Vector3 mousePosition)
    {
        // 마우스 좌표를 Screen Point로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePosition, null, out localPoint);


        Vector2 normalizedPoint = new Vector2((rectTransform.rect.width * 0.5f + localPoint.x) / rectTransform.rect.width, (rectTransform.rect.height * 0.5f + localPoint.y) / rectTransform.rect.height);

        return normalizedPoint;
    }

    int FindObjectIndexInList(List<GameObject> list, GameObject findObject)
    {
        int index = list.FindIndex(x => x == findObject);
        if (index < 0)
        {
            return -1;
        }

        return index;
    }

}

[FirestoreData]
public class BoothPosition
{
    [FirestoreProperty]
    public int boothPositionIndex { get; set; }
}

[FirestoreData]
public class ChargedBoothPosition
{
    [FirestoreProperty]
    public List<bool> BoothPositionList { get; set; }
}
