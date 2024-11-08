using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPaletteTest : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image palette;
    RectTransform rectTransform;
    public Image currentColorImage;
    public Image blackDot;
    RectTransform blackDotRt;

    void Start()
    {
        rectTransform = palette.GetComponent<RectTransform>();
        blackDotRt = blackDot.GetComponent<RectTransform>();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        MousePositionColorPicker(eventData);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        MousePositionColorPicker(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        MousePositionColorPicker(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        MousePositionColorPicker(eventData);
    }

    void MousePositionColorPicker(PointerEventData eventData)
    {
        Vector2 colorUV = GetNormalizedPointIn(eventData, rectTransform);
        SetColor(currentColorImage, colorUV);
        blackDot.gameObject.SetActive(true);
        blackDotRt.transform.position = eventData.position;

    }
    Vector2 GetNormalizedPointIn(PointerEventData eventData, RectTransform rectTransform)
    {
        // 클릭한 위치가 이미지 영역 내에 있는지 확인하고 로컬 좌표 얻기
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // 이미지의 좌표를 0~1 범위로 정규화 (왼쪽 하단이 (0,0)이고, 오른쪽 상단이 (1,1))
            Vector2 normalizedPoint = new Vector2(
                (rectTransform.rect.width * 0.5f + localPoint.x) / rectTransform.rect.width,
                (rectTransform.rect.height * 0.5f + localPoint.y) / rectTransform.rect.height
            );
            return normalizedPoint;
        }
        return Vector2.zero;
    }

    void SetColor(Image image, Vector2 uv)
    {
        image.color = Color.HSVToRGB(0, uv.x, uv.y);
    }

}
