using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    public Transform contentParent; // The parent GameObject (usually within a ScrollView) with a GridLayoutGroup
    public GameObject imagePanel;  // Prefab with an Image component for displaying each image
    public string folderPath;    // Path to the folder containing images

    void Start()
    {
        folderPath = Application.persistentDataPath + "/Screenshots";
        LoadImagesFromFolder();
    }

    void LoadImagesFromFolder()
    {
        if (Directory.Exists(folderPath))
        {
            string[] imageFiles = Directory.GetFiles(folderPath, "*.*").Where(s =>s.EndsWith(".png")).ToArray();

            foreach (string filePath in imageFiles)
            {
                // Load each image file as a Texture2D
                Texture2D texture = LoadTexture(filePath);
                if (texture != null)
                {
                    
                    // Instantiate a new Image prefab and set the sprite
                    GameObject newImageObject = Instantiate(imagePanel, contentParent);
                    newImageObject.GetComponent<ImagePanel>().SetImage(texture);
                    newImageObject.GetComponent<ImagePanel>().SetText(filePath.Split('\\').Last());
                }
            }
        }
        else
        {
            Debug.LogError("Folder does not exist: " + folderPath);
        }
    }

    Texture2D LoadTexture(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2); // Temporary size, will resize
        if (texture.LoadImage(fileData))
        {
            return texture;
        }
        else
        {
            Debug.LogError("Failed to load texture from: " + filePath);
            return null;
        }
    }
}
