using Dummiesman;
using Siccity.GLTFUtility;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    static GameObject loadedObject;

    public static void ImportObj(string objPath)
    {
        if (!File.Exists(objPath))
        {
            print("File doesn't exist.");
        }
        else
        {
            if (loadedObject != null)
                Destroy(loadedObject);
            loadedObject = new OBJLoader().Load(objPath);
            ChangeShaderToLit(loadedObject);
        }
    }

    static void ChangeShaderToLit(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                material.shader = Shader.Find("Universal Render Pipeline/Lit");
            }
        }
    }

    public static void ImportGLTFAsync(string filepath)
    {
        Importer.ImportGLTFAsync(filepath, new ImportSettings(), OnFinishAsync);
    }

    static void OnFinishAsync(GameObject result, AnimationClip[] animations)
    {
        Debug.Log("Finished importing " + result.name);
        if (loadedObject != null)
            Destroy(loadedObject);
        loadedObject = result;
    }
}
