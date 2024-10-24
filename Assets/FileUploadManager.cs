using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using System;

public class FileUploadManager : Singleton<FileUploadManager>
{
    private void Start()
    {
        SetUpFileBrowser();
    }

    void SetUpFileBrowser()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Images", ".jpg", ".png"), new FileBrowser.Filter("Text Files", ".txt"), new FileBrowser.Filter("All Files", "*"));
        FileBrowser.SetDefaultFilter(".png");
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
