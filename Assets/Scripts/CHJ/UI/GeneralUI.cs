using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GeneralUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    public KeyCode[] hideKeys;


    CanvasGroup canvas;
    bool isActivated = false;
    bool isMouseOutsideUI = true;

    public void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        canvas.alpha = 0;

        canvas.DOFade(1, 1).OnComplete(() => { isActivated = true; canvas.blocksRaycasts = true; }) ;
    }

    public void Hide()
    {
        isActivated = false;
        canvas.alpha = 1;
        canvas.DOFade(0, 1).OnComplete(() => { canvas.blocksRaycasts = false; });
    }

    private void Update()
    {
        if (isActivated)
        {
            if (isKeyInputIn(hideKeys))
            {
                Hide();
            }

            if(Input.GetMouseButtonDown(0) && isMouseOutsideUI)
            {
                Hide();
            }
        }
    }

    bool isKeyInputIn(KeyCode[] keys)
    {
        foreach(KeyCode key in keys)
        {
            if (Input.GetKeyDown(key))
                return true;
        }
        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOutsideUI = false;
        print(gameObject.name + "mouse Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOutsideUI = true;
        print(gameObject.name + "mouse Leave");
    }
}
