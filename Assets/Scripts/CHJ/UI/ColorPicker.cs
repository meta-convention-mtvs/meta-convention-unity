using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image palette;
    public Image currentColorImage;
    public Image blackDot;
    public Slider hueSlider;

    RectTransform rectTransform;
    RectTransform blackDotRt;

    [SerializeField]
    float h = 0;
    [SerializeField]
    float s = 0;
    [SerializeField]
    float v = 0;

    Action<Vector3> OnColorChange;
    void Start()
    {
        rectTransform = palette.GetComponent<RectTransform>();
        blackDotRt = blackDot.GetComponent<RectTransform>();

        Sprite sprite = Sprite.Create(GetGradientTexture(h), new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
        palette.sprite = sprite; // Set the Sprite to the Image UI component
        hueSlider.onValueChanged.AddListener(OnValueChanged);
        gameObject.SetActive(false);
    }

    public void ShowColorPicker(Action<Vector3> GetHSVColor)
    {
        gameObject.SetActive(true);
        OnColorChange += GetHSVColor;
    }

    public void HideColorPicker()
    {
        gameObject.SetActive(false);
        OnColorChange = null;
    }
    #region 마우스 이벤트 처리
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
    #endregion

    void MousePositionColorPicker(PointerEventData eventData)
    {
        // Check if the pointer is within the rectTransform bounds
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position))
        {
            Vector2 colorUV = GetNormalizedPointIn(eventData, rectTransform);
            s = colorUV.x;
            v = colorUV.y;

            SetColor(currentColorImage, h, s, v);
            blackDot.gameObject.SetActive(true);
            blackDotRt.transform.position = eventData.position;
        }
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

    void SetColor(Image image, float h, float s, float v)
    {
        image.color = Color.HSVToRGB(h, s, v);
        OnColorChange(new Vector3(h, s, v));
    }

    Texture2D GetGradientTexture(float hue)
    {
        Texture2D texture = new Texture2D(256, 256);

        for(int x = 0; x < texture.width; x++)
        {
            float saturate = Mathf.Lerp(0, 1, (float)x / (texture.width - 1));
            for(int y = 0; y < texture.height; y++)
            {
                float value = Mathf.Lerp(0, 1, (float)y / (texture.height - 1));
                texture.SetPixel(x, y, Color.HSVToRGB(hue, saturate, value));
            }
        }
        texture.Apply();
        return texture;
    }

    void OnValueChanged(float value)
    {
        h = value;
        Sprite sprite = Sprite.Create(GetGradientTexture(h), new Rect(0, 0, 256, 256), new Vector2(0.5f, 0.5f));
        palette.sprite = sprite; // Set the Sprite to the Image UI component
        SetColor(currentColorImage, h, s, v);
    }

}
