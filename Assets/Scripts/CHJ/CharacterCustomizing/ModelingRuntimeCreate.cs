using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelingRuntimeCreate : MonoBehaviourPun
{
    public CharacterTemplet characterTemplet;

    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    public SkinnedMeshRenderer customTShirts;
    
    Animator anim;


    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        if (photonView.IsMine)
        {
            // data를 읽어온다.
            DatabaseManager.Instance.GetData<CharacterTopBottomCustomizeData>(CreateAvatar);
            
        }
    }


    void CreateAvatar(CharacterTopBottomCustomizeData customzieData)
    {

        // data에 적혀있는 gender, idx 에 따라 prefab을 생성후 플레이어의 prefab의 자식으로 만든다.
        // 플레이어의 에니메이터의 Avatar를 바꿔준다.

        if (customzieData != null)
        {
            photonView.RPC(nameof(RpcCreateAvatar), RpcTarget.AllBuffered, customzieData.isMan, customzieData.topIndex, customzieData.bottomIndex, customzieData.isCustomTop, customzieData.customImageFileName, FireAuthManager.Instance.GetCurrentUser().UserId);
        }
    }

    
    [PunRPC]
    void RpcCreateAvatar(bool isMan, int topIndex, int bottomIndex, bool isCustomTop, string customImageFileName, string OwnerUID)
    {
        GameObject character;
        if (isMan)
        {
            character = characterTemplet.maleCharacterPrefabs[topIndex].column[bottomIndex];
        }
        else
        {
            character = characterTemplet.femaleCharacterPrefabs[topIndex].column[bottomIndex];
        }

        Instantiate(character, gameObject.transform);
        if (isCustomTop)
        {
            DatabaseManager.Instance.DownloadImageFrom(OwnerUID, customImageFileName, OnLoadTexture);
        }
        anim.avatar = isMan ? maleAvatar : femaleAvatar;

        anim.Rebind();
    }

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
