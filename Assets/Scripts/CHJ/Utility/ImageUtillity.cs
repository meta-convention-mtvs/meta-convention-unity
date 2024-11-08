using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ImageUtility : MonoBehaviour
{
    public static Texture2D LoadTexture(string filePath)
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
