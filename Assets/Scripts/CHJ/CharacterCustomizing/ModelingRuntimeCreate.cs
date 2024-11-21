using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ModelingRuntimeCreate : MonoBehaviourPun
{
    public CharacterTemplet characterTemplet;

    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    public SkinnedMeshRenderer customTShirts;

    private UID ownerUID;
    
    Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();

    }

    private void Start()
    {
        ownerUID = GetComponent<UID>();
        LoadPlayerCustomizeData(ownerUID.uid);
    }

    void LoadPlayerCustomizeData(string uid)
    {
        // data를 읽어온다.
        DatabaseManager.Instance.GetDataFrom<CharacterTopBottomCustomizeData>(uid, CreateAvatar);
    }

    void CreateAvatar(CharacterTopBottomCustomizeData customizeData)
    {

        // data에 적혀있는 gender, idx 에 따라 prefab을 생성후 플레이어의 prefab의 자식으로 만든다.
        // 플레이어의 에니메이터의 Avatar를 바꿔준다.

        if (customizeData != null)
        {
            //photonView.RPC(nameof(RpcCreateAvatar), RpcTarget.AllBuffered, customzieData.isMan, customzieData.topIndex, customzieData.bottomIndex, customzieData.isCustomTop, customzieData.customImageFileName, FireAuthManager.Instance.GetCurrentUser().UserId);
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
            if (customizeData.isCustomTop)
            {
                DatabaseManager.Instance.DownloadImageFrom(ownerUID.uid, customizeData.customImageFileName, OnLoadTexture);
            }
            anim.avatar = customizeData.isMan ? maleAvatar : femaleAvatar;

            anim.Rebind();
        }
    }

    
    //[PunRPC]
    //void RpcCreateAvatar(bool isMan, int topIndex, int bottomIndex, bool isCustomTop, string customImageFileName, string OwnerUID)
    //{
    //    GameObject character;
    //    if (isMan)
    //    {
    //        character = characterTemplet.maleCharacterPrefabs[topIndex].column[bottomIndex];
    //    }
    //    else
    //    {
    //        character = characterTemplet.femaleCharacterPrefabs[topIndex].column[bottomIndex];
    //    }

    //    Instantiate(character, gameObject.transform);
    //    if (isCustomTop)
    //    {
    //        DatabaseManager.Instance.DownloadImageFrom(OwnerUID, customImageFileName, OnLoadTexture);
    //    }
    //    anim.avatar = isMan ? maleAvatar : femaleAvatar;

    //    anim.Rebind();
    //}

    void OnLoadTexture(Texture2D texture)
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
