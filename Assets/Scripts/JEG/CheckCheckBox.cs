using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckCheckBox : MonoBehaviour
{
    public GameObject togglePrefab;  // 체크박스 프리팹
    public Transform panel;  // 체크박스가 추가될 부모 오브젝트
    private List<Toggle> toggles = new List<Toggle>();  // 생성된 체크박스 저장 리스트
    public List<int> selectedIndices = new List<int>();  // 체크된 체크박스의 인덱스 저장 리스트
    private const int maxSelections = 5;  // 최대 선택 가능한 체크박스 개수

    void Start()
    {
        //GenerateToggles(AIConnectionMgr.fields.Length);  // 10개의 체크박스를 생성 (원하는 수로 변경 가능)
    }

    //void GenerateToggles(int count)
    //{
    //    // 원래 있던 토글들은 제거하고 생성해줘 (스크롤 바는 제거하지 않음)
    //    foreach (Transform child in panel)
    //    {
    //        if (child.GetComponent<Toggle>() != null)
    //        {
    //            Destroy(child.gameObject);
    //        }
    //    }
    //    toggles.Clear();

    //    for (int i = 0; i < count; i++)
    //    {
    //        // 프리팹 인스턴스화
    //        GameObject toggleInstance = Instantiate(togglePrefab, panel);
    //        Toggle toggle = toggleInstance.GetComponent<Toggle>();

    //        if (toggle != null)
    //        {
    //            // 인덱스 부여
    //            int index = i;
    //            toggles.Add(toggle);

    //            // 인덱스에 따라 라벨 텍스트 설정
    //            Text label = toggle.GetComponentInChildren<Text>();
    //            if (label != null)
    //            {
    //                //label.text = AIConnectionMgr.fields[i];
    //            }

    //            // 이벤트 리스너 설정
    //            toggle.onValueChanged.AddListener((isOn) => OnToggleValueChanged(index, isOn));
    //        }
    //    }
    //}

    void OnToggleValueChanged(int index, bool isOn)
    {
        if (isOn)
        {
            // 최대 개수를 초과하면 체크 해제
            if (selectedIndices.Count >= maxSelections)
            {
                toggles[index].isOn = false;
                Debug.Log("최대 " + maxSelections + "개까지만 선택할 수 있습니다.");
                return;
            }

            // 체크된 항목 리스트에 인덱스 추가
            selectedIndices.Add(index);
        }
        else
        {
            // 체크 해제 시 리스트에서 인덱스 제거
            selectedIndices.Remove(index);
        }

        Debug.Log("현재 체크된 항목 인덱스들: " + string.Join(", ", selectedIndices));
    }
}
