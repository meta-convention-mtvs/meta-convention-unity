using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WearClothesTest : MonoBehaviour
{
    public GameObject player;
    public SkinnedMeshRenderer originalClothes;
    public SkinnedMeshRenderer mesh;

    private void Start()
    {
        ChangeClothes(player,originalClothes, mesh);
    }

    void ChangeClothes(GameObject player,SkinnedMeshRenderer originalClothes, SkinnedMeshRenderer newClothes)
    {
        GameObject go = new GameObject();
        go.transform.SetParent(player.transform);

        SkinnedMeshRenderer mesh = go.AddComponent<SkinnedMeshRenderer>();
        mesh.rootBone = originalClothes.rootBone;
        mesh.bones = originalClothes.bones;
        mesh.localBounds = originalClothes.localBounds;
        mesh.sharedMesh = newClothes.sharedMesh;
        mesh.sharedMaterials = newClothes.sharedMaterials;

        originalClothes.gameObject.SetActive(false);
    }
}
