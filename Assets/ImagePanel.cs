using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImagePanel : MonoBehaviour
{
    public Image image;
    public Text text;

    public void SetImage(Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite; // Set the Sprite to the Image UI component
    }

    public void SetText(string s)
    {
        text.text = s;
    }
}
