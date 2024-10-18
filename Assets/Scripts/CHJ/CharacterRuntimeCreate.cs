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
        // data�� �о�´�.
        myData = DatabaseManager.Instance.GetData<CharacterCustomzieData>(typeof(CharacterCustomzieData).ToString());

        // data�� �����ִ� gender, idx �� ���� prefab�� ������ �÷��̾��� prefab�� �ڽ����� �����.
        // �÷��̾��� ���ϸ������� Avatar�� �ٲ��ش�.

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
