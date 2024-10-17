using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public CustomizeTemplet maleCharacterPrefabs;
    public CustomizeTemplet femaleCharacterPrefabs;
    public CinemachineVirtualCamera cinemachine;
    public Avatar maleAvatar;
    public Avatar femaleAvatar;

    CharacterCustomzieData myData;
    private void Start()
    {
        Create();
    }
    public void Create()
    {
        // �÷��̾� prefab ����
        GameObject player = Instantiate(playerPrefab);
        Animator anim = player.GetComponent<Animator>();
        // data�� �о�´�.
        myData = DatabaseManager.Instance.GetData<CharacterCustomzieData>(typeof(CharacterCustomzieData).ToString());

        // data�� �����ִ� gender, idx �� ���� prefab�� ������ �÷��̾��� prefab�� �ڽ����� �����.
        // �÷��̾��� ���ϸ������� Avatar�� �ٲ��ش�.

        if(myData!= null)
        {
            if(myData.isMan)
            {
                anim.avatar = maleAvatar;
                Instantiate(maleCharacterPrefabs.gamePrefabs[myData.customizingIdx], player.transform);
            }
            else
            {
                anim.avatar = femaleAvatar;
                Instantiate(femaleCharacterPrefabs.gamePrefabs[myData.customizingIdx], player.transform);
            }
        }

        // animator �� rebind�Ѵ�
        anim.Rebind();


        // �÷��̾ ������ ��ġ�� �ű��.
        player.transform.position = Vector3.zero;

        // Cinemachine ī�޶� ĳ���͸� �ٶ� �� �ְ� �Ѵ�.
        cinemachine.Follow = player.transform.Find("PlayerCameraRoot") ;
    }
}
