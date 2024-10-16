using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public GameObject playerPrefab;
    public CustomizeTemplet maleCharacterPrefabs;
    public CustomizeTemplet femaleCharacterPrefabs;
    public CinemachineFreeLook cinemachine;
    CharacterCustomzieData myData;
    private void Start()
    {
        Create();
    }
    public void Create()
    {
        // �÷��̾� prefab ����
        GameObject player = Instantiate(playerPrefab);

        // data�� �о�´�.
        myData = DatabaseManager.Instance.GetData<CharacterCustomzieData>(typeof(CharacterCustomzieData).ToString());

        // data�� �����ִ� gender, idx �� ���� prefab�� ������ �÷��̾��� prefab�� �ڽ����� �����.
        if(myData!= null)
        {
            if(myData.isMan)
            {
                Instantiate(maleCharacterPrefabs.gamePrefabs[myData.customizingIdx], player.transform);
            }
            else
            {
                Instantiate(femaleCharacterPrefabs.gamePrefabs[myData.customizingIdx], player.transform);
            }
        }

        // �÷��̾ ������ ��ġ�� �ű��.
        player.transform.position = Vector3.zero;

        // Cinemachine ī�޶� ĳ���͸� �ٶ� �� �ְ� �Ѵ�.
        cinemachine.Follow = player.transform;
        cinemachine.LookAt = player.transform;
    }
}
