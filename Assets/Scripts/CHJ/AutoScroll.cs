using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;

    void Update()
    {
        // If content height exceeds the viewport height, auto-scroll to the bottom
        if (scrollRect.verticalNormalizedPosition <= 0.01f)
        {
            AutoScrollToBottom();
        }
    }

    public void AutoScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // Force the UI to update before setting the scroll position
        scrollRect.verticalNormalizedPosition = 0f; // 0 means bottom, 1 means top
    }

    // Call this method when content is added to ensure it scrolls automatically
    public void OnContentAdded()
    {
        AutoScrollToBottom();
    }
}
