using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelingRuntimeCreate : MonoBehaviourPun
{
    public CustomizeTemplet maleCharacterPrefabs;
    public CustomizeTemplet femaleCharacterPrefabs;

    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    Animator anim;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        if (photonView.IsMine)
        {
            // data�� �о�´�.
            CharacterCustomizeData myData = DatabaseManager.Instance.GetData<CharacterCustomizeData>(typeof(CharacterCustomizeData).ToString());
            CreateAvatar(myData);
        }
    }


    void CreateAvatar(CharacterCustomizeData customzieData)
    {

        // data�� �����ִ� gender, idx �� ���� prefab�� ������ �÷��̾��� prefab�� �ڽ����� �����.
        // �÷��̾��� ���ϸ������� Avatar�� �ٲ��ش�.

        if (customzieData != null)
        {
            photonView.RPC(nameof(RpcCreateAvatar), RpcTarget.AllBuffered, customzieData.isMan, customzieData.customizingIdx);
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
