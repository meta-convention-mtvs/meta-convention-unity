using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class RenderAvatarData : MonoBehaviourPun
{
    public CharacterTemplet characterTemplet;

    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    public SkinnedMeshRenderer customTShirts;
    
    Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void CreateAvatar(CharacterTopBottomCustomizeData customizeData)
    {
        if (customizeData != null)
        {
            GameObject character;
            if (customizeData.isMan)
            {
                character = characterTemplet.maleCharacterPrefabs[customizeData.topIndex].column[customizeData.bottomIndex];
            }
            else
            {
                character = characterTemplet.femaleCharacterPrefabs[customizeData.topIndex].column[customizeData.bottomIndex];
            }

            Instantiate(character, gameObject.transform);

            anim.avatar = customizeData.isMan ? maleAvatar : femaleAvatar;

            anim.Rebind();
        }
    }

    public void OnLoadTexture(Texture2D texture)
    {
        // texture 불러옴
        texture.wrapMode = TextureWrapMode.Clamp;

        SkinnedMeshRenderer skinned = Instantiate(customTShirts);
        skinned.gameObject.SetActive(false);
        skinned.materials[0].mainTexture = texture;

        ChangeClothes(this.gameObject, skinned);
    }

    void ChangeClothes(GameObject player, SkinnedMeshRenderer newClothes)
    {
        var originalClothes = player.GetComponentsInChildren<SkinnedMeshRenderer>().Where(go => go != null && go.gameObject.name.Contains("top"));

        GameObject go = new GameObject();
        go.transform.SetParent(player.transform);
        SkinnedMeshRenderer mesh = go.AddComponent<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer clothes in originalClothes)
        {
            mesh.rootBone = clothes.rootBone;
            mesh.bones = clothes.bones;
            mesh.localBounds = clothes.localBounds;
            mesh.sharedMesh = newClothes.sharedMesh;
            mesh.sharedMaterials = newClothes.sharedMaterials;

            clothes.gameObject.SetActive(false);
        }
    }

}
