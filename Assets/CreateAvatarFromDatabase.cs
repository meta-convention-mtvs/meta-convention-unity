using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHJ;
using System;
using System.Threading.Tasks;

[RequireComponent(typeof(RenderAvatarData), typeof(UID))]
public class CreateAvatarFromDatabase : MonoBehaviour
{
    UID uidComponent;
    RenderAvatarData renderAvatarData;

    private void Awake()
    {
        uidComponent = GetComponent<UID>();
        renderAvatarData = GetComponent<RenderAvatarData>();
        uidComponent.OnUIDChanged += OnUIDChanged;
    }

    void OnUIDChanged(string uid)
    {
        GetAvatarDataFromDatabaseAndLoadData(uid);
    }

    async Task<bool> GetAvatarDataFromDatabaseAndLoadData(string id)
    {
        try
        {
            var avatarData = await AsyncDatabase.GetDataFromDatabase<CharacterTopBottomCustomizeData>(DatabasePath.GetUserDataPath(id, typeof(CharacterTopBottomCustomizeData).ToString()));
            renderAvatarData.CreateAvatar(avatarData);

            if (avatarData.isCustomTop)
            {
                var texture = await AsyncDatabase.GetLogoFromDatabaseWithUid(id, avatarData.customImageFileName);
                renderAvatarData.OnLoadTexture(texture);
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
