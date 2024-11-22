using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureTest : MonoBehaviour
{
    public GameObject boothImage;
    // Start is called before the first frame update
    void Start()
    {
        FileUploadManager.Instance.SetUpImageFileBrowser();
        FileUploadManager.Instance.ShowDialog(SetTexture);
    }

    void SetTexture(string[] filePaths)
    {
        string filePath = filePaths[0];
        boothImage.GetComponent<Renderer>().material.mainTexture = ImageUtility.LoadTexture(filePath);
    }
}
