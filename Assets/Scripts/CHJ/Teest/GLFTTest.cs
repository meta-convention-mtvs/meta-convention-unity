using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TriLibCore;
using System;

public class GLFTTest : MonoBehaviour
{
    public string filepath;

    GameObject instantiatedObject;

    private void Start()
    {
        FileUploadManager.Instance.ShowDialog(ImportGLTFAsync);
        instantiatedObject = null;
    }
    void ImportGLTFAsync(string[] filepaths)
    {
        string filepath = filepaths[0];
        AssetLoader.CreateDefaultLoaderOptions(false, true);
        AssetLoader.LoadModelFromFile(filepath, OnFinishAsync);
    }

    private void OnFinishAsync(AssetLoaderContext obj)
    {
        instantiatedObject = obj.RootGameObject;

    }


}
