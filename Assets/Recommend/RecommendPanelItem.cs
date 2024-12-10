using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI2.Recommend
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RecommendPanelItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private FadeTextTyping desc;
        private RecommendItem item;

        private CanvasGroup group;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        public void SetItem(RecommendItem item)
        {
            this.item = item;
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
            image.sprite = item.sprite;
            text.text = item.name;
            desc.TypingText = item.desc;
        }
    }
}