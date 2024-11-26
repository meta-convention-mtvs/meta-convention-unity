//using UnityEditor;
//using UnityEngine;

//public class PrefabGuidFinder : MonoBehaviour
//{
//    public string guid = "eac3418f84f5a6744930cea38d392693"; // YAML에서 가져온 GUID
//    void Start()
//    {
//        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

//        if (!string.IsNullOrEmpty(assetPath))
//        {
//            Debug.Log($"Prefab Path: {assetPath}");
//        }
//        else
//        {
//            Debug.LogError("GUID로 프리팹을 찾을 수 없습니다.");
//        }
//    }
//}
