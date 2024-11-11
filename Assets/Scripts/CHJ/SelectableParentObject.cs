using UnityEngine;

public class SelectableParentObject : MonoBehaviour
{
    public Color selectedColor = Color.blue; // 선택 시 사용할 파란색
    private Color[] originalColors;          // 각 자식의 원래 색상 저장
    private Material[] materials;            // 각 자식의 메터리얼 저장

    void Start()
    {
        // 자식 오브젝트들의 Mesh Renderer를 가져오고, 원래 색상 저장
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        materials = new Material[childRenderers.Length];
        originalColors = new Color[childRenderers.Length];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            materials[i] = childRenderers[i].material;
            originalColors[i] = materials[i].color;
        }
    }

    public void Select()
    {
        // 선택 시 모든 자식의 색상을 파란색으로 변경
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = selectedColor;
        }
    }

    public void Deselect()
    {
        // 선택 해제 시 모든 자식의 색상을 원래 색상으로 복원
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
    }
}