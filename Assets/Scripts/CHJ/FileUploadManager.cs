using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using System;

public class FileUploadManager : Singleton<FileUploadManager>
{

    public void SetUpImageFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt"), new FileBrowser.Filter("All Files", "*"));
        //FileBrowser.SetDefaultFilter("Images");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
    }

    public void SetUpObjFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("ObjectFiles", ".obj", ".gltf"));
        //FileBrowser.SetDefaultFilter("ObjectFiles"); 
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
    }

    public void SetUpVideoFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Videos", ".mp4", ".mov", ".webm", ".wmv"));
        //FileBrowser.SetDefaultFilter("Videos");
        FileBrowser.AddQuickLink("Users", "C:\\Users", null);
    }
    public void ShowDialog(Action<string[]> onLoadSuccess)
    {
        StartCoroutine(ShowLoadDialogCoroutine(onLoadSuccess));
    }

    IEnumerator ShowLoadDialogCoroutine(Action<string[]> onLoadSuccess)
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Load files", "Select");

        if (FileBrowser.Success)
        {
            Debug.Log("File upload success");
            onLoadSuccess?.Invoke(FileBrowser.Result);
        }
        else
        {
            Debug.Log("File upload failed");
        }
    }

}
