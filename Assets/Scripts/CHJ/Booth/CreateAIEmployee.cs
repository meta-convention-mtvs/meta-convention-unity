using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAIEmployee : MonoBehaviourPun
{
    public Transform AIEmployeePosition;
    public GameObject aiEmployeeFactory;

    GameObject aiEmployee;

    private void Start()
    {
        aiEmployee = Instantiate(aiEmployeeFactory, this.transform);
        aiEmployee.GetComponent<UID>().SetUUID(GetComponent<UID>().uuid);
    }

    public void RenderAiEmployee(Texture2D texture)
    {
        Debug.Log("Render AI Employee called");
        RenderAvatarData renderAvatarData = aiEmployee.GetComponent<RenderAvatarData>();
        renderAvatarData.CreateAvatar(CharacterTopBottomCustomizeData.GetRandomCharacterData());
        renderAvatarData.OnLoadTexture(texture);
    }
}
