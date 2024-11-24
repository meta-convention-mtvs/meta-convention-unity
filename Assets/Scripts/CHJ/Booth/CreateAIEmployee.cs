using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAIEmployee : MonoBehaviourPun
{
    public Transform AIEmployeePosition;
    public GameObject aiEmployeeFactory;

    GameObject aiEmployee;
    UID companyUID;

    void Awake()
    {
        companyUID = GetComponent<UID>();
        companyUID.OnUUIDChanged += Create;
    }    
    private void Create(string uuid)
    {
        aiEmployee = Instantiate(aiEmployeeFactory, this.transform);
        aiEmployee.GetComponent<UID>().SetUUID(uuid);
    }

    public void RenderAiEmployee(Texture2D texture)
    {
        Debug.Log("Render AI Employee called");
        RenderAvatarData renderAvatarData = aiEmployee.GetComponent<RenderAvatarData>();
        renderAvatarData.CreateAvatar(CharacterTopBottomCustomizeData.GetRandomCharacterData());
        renderAvatarData.OnLoadTexture(texture);
    }
}
