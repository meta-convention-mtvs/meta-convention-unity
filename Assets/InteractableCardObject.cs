using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCardObject : MonoBehaviour, IKeyInteractableObject
{
    Card card;
    CardBook cardBook;

    public void SetCard(Card card)
    {
        this.card = card;
    }

    private void Start()
    {
        cardBook = GameObject.FindWithTag("CardBook").GetComponent<CardBook>();
    }

    public void HideText()
    {

    }

    public void Interact()
    {
        if(card != null)
        {
            cardBook.addCard(card);
            UIManager.Instance.ShowPopupUI("명함이 명함첩에 저장되었습니다!", "Business card is saved in Cardbook");
        }
    }

    public void InteractEnd()
    {
    }

    public void ShowText()
    {
        UIManager.Instance.ShowPopupUI("(F)키를 눌러 명함을 저장해보세요!", "(F)Press the key to save business card!");
    }

}
