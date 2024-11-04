using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterTemplet", menuName = "CHJ/ScriptableObject/CharacterTemplet")]
public class CharacterTemplet : ScriptableObject
{

    [System.Serializable]
    public class Row
    {
        public GameObject[] column;
    }

    public List<Row> maleCharacterPrefabs;
    public List<Row> femaleCharacterPrefabs;

}
