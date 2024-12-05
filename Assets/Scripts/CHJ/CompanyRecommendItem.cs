using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CHJ;
using System.Threading.Tasks;
using Ricimi;

[RequireComponent(typeof(SceneTransition))]
public class CompanyRecommendItem : MonoBehaviour
{
    public Text companyName;
    public Text companyItem;
    public Text companyLink;
    public Text companyMission;
    public Image companyLogo;
    public Button sceneTransitionButton;

    public RectTransform companyLogoParentRectTransform;

    public void SetItemText(TestRecommendedCompany companyInfo)
    {
        if(companyName != null)
            companyName.text = companyInfo.company_name;
        if(companyMission != null)
            companyMission.text = companyInfo.company_mission;
        if(companyItem != null)
            companyItem.text = companyInfo.items;
        if(companyLink != null)
            companyLink.text = companyInfo.link;

        SetLogo(companyInfo.uuid, companyInfo.logo_file_name);
    }

    public void SetButtonTransition(TestRecommendedCompany companyInfo)
    {
        if(sceneTransitionButton != null)
        {
            sceneTransitionButton.onClick.AddListener(() =>
            {
                string categoryString = companyInfo.category.Replace("_", " ");
                BoothCategory category = EnumUtility.GetEnumValue<BoothCategory>(categoryString).Value;
                MainHallData.Instance.SetMainHallLoadingData(category, "Start_Universe");
            });
        }
    }
    /// <summary>
    /// 주어진 경로의 로고 이미지를 로드하고 UI에 적절히 설정합니다.
    /// </summary>
    /// <param name="logoFileName">로고 파일 이름</param>
    async Task SetLogo(string uuid, string logoFileName)
    {
        if (!string.IsNullOrEmpty(logoFileName))
        {
            Texture2D texture = await AsyncDatabase.GetLogoFromDatabaseWithUid(uuid, logoFileName + ".jpg");
            // Resources 폴더에서 스프라이트 로드
            Sprite loadedSprite = SpriteUtility.ConvertTextureToSprite(texture);
            if (loadedSprite != null)
            {
                companyLogo.sprite = loadedSprite;

                // RectTransform의 크기를 원본 스프라이트 비율에 맞게 조정
                AdjustImageSize(loadedSprite);
            }
            else
            {
                Debug.LogWarning($"Sprite not found at path: Logos/{logoFileName}");
            }
        }
        else
        {
            Debug.LogWarning("Logo file name is null or empty.");
        }
    }

    /// <summary>
    /// Image의 RectTransform 크기를 원본 스프라이트 비율에 맞게 조정합니다.
    /// </summary>
    /// <param name="sprite">적용할 스프라이트</param>
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
