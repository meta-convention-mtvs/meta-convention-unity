using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Dummiesman;
using System.Linq;

public class ObjTest : MonoBehaviour
{
    public string objPath;
    GameObject loadedObject;


    private void Start()
    {
        LoadObj(objPath);
    }

    string LoadObj(string objPath)
    {
        if (!File.Exists(objPath))
        {
             return "File doesn't exist.";
        }
        else
        {
            if (loadedObject != null)
                Destroy(loadedObject);
            loadedObject = new OBJLoader().Load(objPath);
            ChangeShaderToLit(loadedObject);
            return string.Empty;
        }
    }

    void ChangeShaderToLit(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
        {
            foreach(Material material in renderer.materials)
            {
                material.shader = Shader.Find("Universal Render Pipeline/Lit");
            }
        }
    }
}
