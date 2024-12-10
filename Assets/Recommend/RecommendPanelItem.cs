using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using CHJ;
using System.Threading.Tasks;
using Ricimi;

namespace UI2.Recommend
{
    [RequireComponent(typeof(CanvasGroup))]
    public class RecommendPanelItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private FadeTextTyping desc;
        public string LoadingScene { get; set; }
        private RecommendItem item;

        private CanvasGroup group;

        [SerializeField] Image companyLogo;
        [SerializeField] RectTransform companyLogoParentRectTransform;
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
            SetSpriteUsingFirebase(item.company_uuid, item.sprite_name);
            text.text = item.name;
            desc.TypingText = item.desc;
        }
        
        public void OnGotoButtonClick()
        {
            UpdateNextCategoryRoom(item.category, LoadingScene, item.company_uuid);
            Transition.LoadLevel(LoadingScene, 1, Color.black);
        }

        void UpdateNextCategoryRoom(string companyCategory, string LoadingSceneName, string uuid)
        {
            BoothCategory category = EnumUtility.GetEnumValue<BoothCategory>(companyCategory).Value;
            MainHallData.Instance.SetMainHallLoadingData(category, LoadingSceneName);
            MainHallData.Instance.SetTargetCompanyUuid(uuid);
        }

        /// <summary>
        /// 주어진 경로의 로고 이미지를 로드하고 UI에 적절히 설정합니다.
        /// </summary>
        /// <param name="sprite_name">로고 파일 이름</param>
        async void SetSpriteUsingFirebase(string uuid, string sprite_name)
        {
            if (!string.IsNullOrEmpty(sprite_name))
            {
                Texture2D texture = await AsyncDatabase.GetLogoFromDatabaseWithUid(uuid, sprite_name + ".jpg");
                // Resources 폴더에서 스프라이트 로드
                if (texture != null)
                {
                    Sprite loadedSprite = SpriteUtility.ConvertTextureToSprite(texture);
                    image.sprite = loadedSprite;

                    // RectTransform의 크기를 원본 스프라이트 비율에 맞게 조정
                    AdjustImageSize(loadedSprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite not found at path: Logos/{sprite_name}");
                }
            }
            else
            {
                Debug.LogWarning("Logo file name is null or empty.");
            }
        }
        void AdjustImageSize(Sprite sprite)
        {
            if (companyLogo == null || companyLogoParentRectTransform == null)
            {
                Debug.LogError("Image or Parent RectTransform is not assigned.");
                return;
            }

            RectTransform rectTransform = companyLogo.GetComponent<RectTransform>();

            if (rectTransform == null || sprite.texture == null)
            {
                Debug.LogError("RectTransform or Sprite Texture is missing.");
                return;
            }

            // 스프라이트의 원본 크기와 비율 계산
            float spriteWidth = sprite.texture.width;
            float spriteHeight = sprite.texture.height;
            float spriteAspect = spriteWidth / spriteHeight;

            // 부모 RectTransform의 크기 가져오기
            float parentWidth = companyLogoParentRectTransform.rect.width;
            float parentHeight = companyLogoParentRectTransform.rect.height;
            float parentAspect = parentWidth / parentHeight;

            // 원본 비율을 유지하면서 부모 크기에 맞게 조정
            float newWidth, newHeight;

            if (spriteAspect > parentAspect)
            {
                // 스프라이트가 부모보다 넓을 때
                newWidth = parentWidth;
                newHeight = parentWidth / spriteAspect;
            }
            else
            {
                // 스프라이트가 부모보다 높을 때
                newHeight = parentHeight;
                newWidth = parentHeight * spriteAspect;
            }

            // RectTransform의 크기 설정
            rectTransform.sizeDelta = new Vector2(newWidth, newHeight);

            // 중앙 정렬 (필요 시)
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = Vector2.zero;
        }

    }

}