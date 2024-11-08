using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;

public class GLFTTest : MonoBehaviour
{
    public string filepath;

    GameObject instantiatedObject;

    private void Start()
    {
        ImportGLTFAsync(filepath);
        instantiatedObject = null;
    }
    void ImportGLTFAsync(string filepath)
    {
        Importer.ImportGLTFAsync(filepath, new ImportSettings(), OnFinishAsync);
    }

    void OnFinishAsync(GameObject result, AnimationClip[] animations)
    {
        Debug.Log("Finished importing " + result.name);
        instantiatedObject = result;
        result.transform.position = new Vector3(0, -20, 0);
    }

}
