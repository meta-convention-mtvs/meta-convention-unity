using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractablePlayerObject : MonoBehaviourPun, IKeyInteractableObject
{
    public TMP_Text infoText;

    CardBook cardBook;
    CardReader cardReader;

    private void Start()
    {
        cardBook = GameObject.FindWithTag("CardBook").GetComponent<CardBook>();
        cardReader = GameObject.FindWithTag("CardReader").GetComponent<CardReader>();
    }
    public void HideText()
    {
        infoText.gameObject.SetActive(false);
    }

    public void Interact()
    {
        cardReader.ShowCardUI(photonView.Owner);
    }


    public void ShowText()
    {
        infoText.gameObject.SetActive(true);
    }
}
