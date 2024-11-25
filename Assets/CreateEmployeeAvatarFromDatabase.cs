using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(RenderAvatarData), typeof(UID))]
public class CreateEmployeeAvatarFromDatabase : MonoBehaviour
{
    UID uidComponent;
    RenderAvatarData renderAvatarData;
    InteractableAIEmployeeObject interactableAIEmployeeObject;

    private void Awake()
    {
        uidComponent = GetComponent<UID>();
        renderAvatarData = GetComponent<RenderAvatarData>();
        interactableAIEmployeeObject = GetComponent<InteractableAIEmployeeObject>();
        uidComponent.OnUUIDChanged += OnUUIDChanged;
    }

    void OnUUIDChanged(string uuid)
    {
        GetAvatarDataFromDatabaseAndLoadData(uuid);
    }

    async Task<bool> GetAvatarDataFromDatabaseAndLoadData(string id)
    {
        try
        {
            var avatarData = CharacterTopBottomCustomizeData.GetRandomCharacterData();
            renderAvatarData.CreateAvatar(avatarData);

            var boothCustomizeData = await AsyncDatabase.GetDataFromDatabase<BoothCustomizeData>(DatabasePath.GetCompanyDataPath(id, nameof(BoothCustomizeData)));

            if (boothCustomizeData.hasLogoImage)
            {
                var texture = await AsyncDatabase.GetLogoFromDatabaseWithUid(id, boothCustomizeData.logoImagePath);

                if (texture != null)
                {
                    renderAvatarData.OnLoadTexture(texture);

                    // ai employee 랑 상호작용할 때 texture도 미리 세팅해주자
                    interactableAIEmployeeObject.logoImage = texture;

                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"UID {id}에서 아바타 생성 중 오류 발생: {ex.Message}");
            return false;
        }
    }
}
