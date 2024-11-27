using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBannerObject : MonoBehaviour, IKeyInteractableObject
{
    public string homepageURL;

    public void HideText()
    {
        
    }

    public void Interact()
    {
        if(!string.IsNullOrEmpty(homepageURL))
            Application.OpenURL(homepageURL);
        else
            UIManager.Instance.ShowPopupUI("홈페이지 링크가 없어요...", "There is no homepage link.");
    }

    public void InteractEnd()
    {
        
    }

    public void ShowText()
    {
        UIManager.Instance.ShowPopupUI("(F)키를 눌러서 회사 홈페이지에 접속하세요!", "Press (F) to access our company homepage");
    }

}
