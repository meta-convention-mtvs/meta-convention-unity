using UnityEngine;

public class SelectableParentObject : MonoBehaviour
{
    public Color selectedColor = Color.blue;  // 선택 시 사용할 색상
    public Color uniteractableColor;          // 상호작용 불가 상태 색상
    public bool isInteractable = true;        // 상호작용 가능 여부
    private Color[][] originalColors;         // 각 자식의 원래 색상 저장
    private Material[][] materials;           // 각 자식의 메터리얼 저장

    void Start()
    {
        // 자식 오브젝트들의 Mesh Renderer를 가져오고, 원래 색상 저장
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        materials = new Material[childRenderers.Length][];
        originalColors = new Color[childRenderers.Length][];

        for (int i = 0; i < childRenderers.Length; i++)
        {
            materials[i] = childRenderers[i].materials; // 모든 메터리얼 배열 가져오기
            originalColors[i] = new Color[materials[i].Length];

            for (int j = 0; j < materials[i].Length; j++)
            {
                originalColors[i][j] = materials[i][j].color; // 각 메터리얼의 원래 색상 저장
            }
        }
    }

    public void Select()
    {
        if (isInteractable)
        {
            // 선택 시 모든 자식의 색상을 파란색으로 변경
            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < materials[i].Length; j++)
                {
                    materials[i][j].color = selectedColor;
                }
            }
        }
    }

    public void Deselect()
    {
        if (isInteractable)
        {
            // 선택 해제 시 모든 자식의 색상을 원래 색상으로 복원
            for (int i = 0; i < materials.Length; i++)
            {
                for (int j = 0; j < materials[i].Length; j++)
                {
                    materials[i][j].color = originalColors[i][j];
                }
            }
        }
    }

    public void SetNotInteractable()
    {
        isInteractable = false;

        // 상호작용 불가 상태 색상으로 변경
        for (int i = 0; i < materials.Length; i++)
        {
            for (int j = 0; j < materials[i].Length; j++)
            {
                materials[i][j].color = uniteractableColor;
            }
        }
    }
}
