using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CHJ;
using System.Threading.Tasks;

public class UIBoothPosition : MonoBehaviour
{
    public List<GameObject> boothPosition;

    public Camera renderTextureCamera; // RenderTexture를 렌더링하는 카메라
    public RawImage rawImage; // RawImage UI 요소
    private RectTransform rectTransform;
    [SerializeField]
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
                if (position.BoothPositionList[i].isCharged)
                {
                    boothPosition[i].GetComponent<SelectableParentObject>().SetNotInteractable();
                }
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
                    selectedObject = hit.collider.GetComponent<SelectableParentObject>();
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
                    selectedObject = hit.collider.GetComponent<SelectableParentObject>();
                    if (selectedObject != null)
                    {
                        selectedObject.Select();
                    }
                }
            }

        }
    }

    public bool SaveBoothPosition()
    {
        if(currentIndex != -1 && CanSaveData(position, currentIndex))
        {
            // 내 꺼에 저장
            BoothPosition myPosition = new BoothPosition();
            myPosition.boothPositionIndex = currentIndex;
            DatabaseManager.Instance.SaveData<BoothPosition>(myPosition);

            // 서버에 저장
            SaveChargedBoothPosition(currentIndex, UuidMgr.Instance.currentUserInfo.companyUuid);

            // BoothPositionReseter 설정하기
            BoothPositionReseter.Instance.SetValue(currentIndex, UuidMgr.Instance.currentUserInfo.companyUuid);
            return true;
        }
        return false;
    }

    public async Task SaveChargedBoothPosition(int index, string uuid)
    {
        // 서버에 저장
        ChargedBoothPosition position = await AsyncDatabase.GetDataFromDatabase<ChargedBoothPosition>(DatabasePath.GetPublicDataPath(nameof(ChargedBoothPosition)));
        position.BoothPositionList[index] = new ChargedBoothData(true, uuid);
        DatabaseManager.Instance.SavePublicData<ChargedBoothPosition>(position);
    }
    public bool CanSaveData(ChargedBoothPosition currentChargedBoothPosition, int index)
    {
        if (index == -1)
            return false;
        if (currentChargedBoothPosition != null && currentChargedBoothPosition.BoothPositionList[index].isCharged == true)
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
    public List<ChargedBoothData> BoothPositionList { get; set; }

    public List<string> GetUUIDList()
    {
        List<string> uuidList = new List<string>();

        if (BoothPositionList == null)
            return null;

        for(int i = 0; i < BoothPositionList.Count; i++)
        {
            if (BoothPositionList[i].isCharged)
            {
                uuidList.Add(BoothPositionList[i].ownerUUID);
            }
            else
            {
                uuidList.Add("");
            }
        }

        return uuidList;
    }
}

[FirestoreData]
public class ChargedBoothData
{
    [FirestoreProperty]
    public bool isCharged { get; set; }
    [FirestoreProperty]
    public string ownerUUID { get; set; }

    public ChargedBoothData()
    {

    }

    public ChargedBoothData(bool isCharged, string ownerUUID)
    {
        this.isCharged = isCharged;
        this.ownerUUID = ownerUUID;
    }
}