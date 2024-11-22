using Dummiesman;
using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TriLibCore;
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

    public static void ImportGLTFAsync(string filepath, Action<GameObject, AnimationClip[]> OnLoadFinish)
    {
        Importer.ImportGLTFAsync(filepath, new ImportSettings(), OnLoadFinish);
    }

    public static void StartImporting(string filePath, Action<AssetLoaderContext> OnLoad)
    {
        AssetLoaderOptions _assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);

        AssetLoader.LoadModelFromFile(filePath, OnLoad, OnMaterialsLoad, OnProgress, OnError, null, _assetLoaderOptions);
    }

    private static void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    private static void OnProgress(AssetLoaderContext arg1, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }

    private static void OnMaterialsLoad(AssetLoaderContext obj)
    {
        Debug.Log("Materials loaded. Model fully loaded.");
    }
}
