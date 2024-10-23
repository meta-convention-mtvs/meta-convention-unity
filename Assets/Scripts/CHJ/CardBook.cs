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

    public GameObject cardBookUI;
    public UICard myCard;

    public RectTransform content;
    public GameObject cardPrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowCardBookUI();
        }
    }
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

    public void ShowCardBookUI()
    {
        //cardBookUI.SetActive(true);
        UIManager.Instance.ShowUI(cardBookUI, UIType.Normal);

        myCard.ShowCardUI(CardReader.ReadCard(PhotonNetwork.LocalPlayer));
        for(int i = 0; i<myCardBook.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab, content);
            go.GetComponent<UICard>().ShowCardUI(myCardBook[i]);
        }
    }


}
