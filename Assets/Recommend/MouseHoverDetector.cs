using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class MouseHoverDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool hovered;
    [SerializeField] private UnityEvent<bool> onHovered;

    private void Start()
    {
        hovered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
        onHovered?.Invoke(hovered);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
        onHovered?.Invoke(hovered);
    }
}
