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
        print("ImportGLTFAsync called");
        AssetLoader.LoadModelFromFile(filepath, OnLoadFinish);

        AssetLoaderOptions _assetLoaderOptions = AssetLoader.CreateDefaultLoaderOptions(false, true);
        AssetLoader.LoadModelFromFile(filepath, OnLoadFinish, OnMaterialsLoad, OnProgress, OnError, null, _assetLoaderOptions);
    }
    /// <summary>
    /// Called when any error occurs.
    /// </summary>
    /// <param name="obj">The contextualized error, containing the original exception and the context passed to the method where the error was thrown.</param>
    private static void OnError(IContextualizedError obj)
    {
        Debug.LogError($"An error occurred while loading your Model: {obj.GetInnerException()}");
    }

    /// <summary>
    /// Called when the Model loading progress changes.
    /// </summary>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    /// <param name="progress">The loading progress.</param>
    private static void OnProgress(AssetLoaderContext assetLoaderContext, float progress)
    {
        Debug.Log($"Loading Model. Progress: {progress:P}");
    }

    /// <summary>
    /// Called when the Model (including Textures and Materials) has been fully loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private static void OnMaterialsLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Materials loaded. Model fully loaded.");
    }

    /// <summary>
    /// Called when the Model Meshes and hierarchy are loaded.
    /// </summary>
    /// <remarks>The loaded GameObject is available on the assetLoaderContext.RootGameObject field.</remarks>
    /// <param name="assetLoaderContext">The context used to load the Model.</param>
    private void OnLoad(AssetLoaderContext assetLoaderContext)
    {
        Debug.Log("Model loaded. Loading materials.");
    }
}
