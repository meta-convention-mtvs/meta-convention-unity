using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLogoImageInSpeackCanvas : MonoBehaviour
{
    public Image logo;

    public void SetLogoImage(Texture2D image)
    {
        if(image == null)
        {
            logo.sprite = null;
            return;
        }

        Sprite sprite = Sprite.Create(
                image,                                   
                new Rect(0, 0, image.width, image.height), 
                new Vector2(0.5f, 0.5f)                    
            );

        // Image 컴포넌트의 Source Image 설정
        logo.sprite = sprite;
    }
}
