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
        // 플레이어 prefab 생성
        GameObject player = Instantiate(playerPrefab);

        // data를 읽어온다.
        myData = DatabaseManager.Instance.GetData<CharacterCustomzieData>(typeof(CharacterCustomzieData).ToString());

        // data에 적혀있는 gender, idx 에 따라 prefab을 생성후 플레이어의 prefab의 자식으로 만든다.
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

        // 플레이어를 적절한 위치로 옮긴다.
        player.transform.position = Vector3.zero;

        // Cinemachine 카메라가 캐릭터를 바라볼 수 있게 한다.
        cinemachine.Follow = player.transform;
        cinemachine.LookAt = player.transform;
    }
}
