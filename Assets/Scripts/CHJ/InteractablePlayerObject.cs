using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractablePlayerObject : MonoBehaviourPun, IKeyInteractableObject
{
    CardBook cardBook;
    CardReader cardReader;

    private void Start()
    {
        cardBook = GameObject.FindWithTag("CardBook")?.GetComponent<CardBook>();
        cardReader = GameObject.FindWithTag("CardReader")?.GetComponent<CardReader>();
    }
    public void HideText()
    {
        
    }

    public void Interact()
    {
        cardReader.ShowCardUI(photonView.Owner);
    }


    public void ShowText()
    {
        UIManager.Instance.ShowPopupUI("(F)키를 눌러 명함을 주고 받으세요!");
    }

    public void InteractEnd()
    {
        
    }
}
