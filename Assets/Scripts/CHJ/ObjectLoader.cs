using Dummiesman;
using TriLibCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{
    public static GameObject ImportObj(string objPath)
    {
        if (!File.Exists(objPath))
        {
            print("File doesn't exist.");
            return null;
        }
        else
        {
            GameObject loadedObject = new OBJLoader().Load(objPath);
            ChangeShaderToLit(loadedObject);
            return loadedObject;
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

    public static void ImportGLTFAsync(string filepath, Action<AssetLoaderContext> OnLoadFinish)
    {
        AssetLoader.LoadModelFromFile(filepath, OnLoadFinish);
    }

}
