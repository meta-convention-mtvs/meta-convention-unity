using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRuntimeCreate : MonoBehaviourPun
{
    public CustomizeTemplet maleCharacterPrefabs;
    public CustomizeTemplet femaleCharacterPrefabs;

    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    CharacterCustomzieData myData;
    Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        if (photonView.IsMine)
        {
            CreateAvatar();
        }
    }


    void CreateAvatar()
    {
        // data를 읽어온다.
        myData = DatabaseManager.Instance.GetData<CharacterCustomzieData>(typeof(CharacterCustomzieData).ToString());

        // data에 적혀있는 gender, idx 에 따라 prefab을 생성후 플레이어의 prefab의 자식으로 만든다.
        // 플레이어의 에니메이터의 Avatar를 바꿔준다.

        if (myData != null)
        {
            photonView.RPC(nameof(RpcCreateAvatar), RpcTarget.AllBuffered, myData.isMan, myData.customizingIdx);
        }


    }

    [PunRPC]
    void RpcCreateAvatar(bool isMan, int customizingIdx)
    {
        CustomizeTemplet templet = isMan ? maleCharacterPrefabs : femaleCharacterPrefabs;

        Instantiate(templet.gamePrefabs[customizingIdx], gameObject.transform);
        anim.avatar = isMan ? maleAvatar : femaleAvatar;

        anim.Rebind();
    }
}
