using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBook : MonoBehaviour
{
    [SerializeField]
    List<Card> myCardBook = new List<Card>();


    public void addCard(Card newCard)
    {
        if (hasCard(newCard))
            return;
        myCardBook.Add(newCard);
    }

    bool hasCard(Card searchingCard)
    {
        foreach(Card card in myCardBook)
        {
            if(card.id == searchingCard.id)
            {
                return true;
            }
        }
        return false;
    }

    public Card GetCardBook(int idx)
    {
        return myCardBook[idx];
    }

    public int GetCount()
    {
        return myCardBook.Count;
    }
}
