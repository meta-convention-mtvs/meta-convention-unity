using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICardBook : MonoBehaviour
{
    public UICard myCard;

    public RectTransform content;
    public GameObject cardPrefab;

    CardBook myCardBook;

    private void Start()
    {
        myCardBook = GameObject.FindWithTag("CardBook")?.GetComponent<CardBook>();
        if(myCardBook == null)
        {
            Debug.LogError("Card book is null... please set tag");
            return;
        }
        ShowCardBookUI(PhotonNetwork.LocalPlayer, myCardBook);

    }

    public void ShowCardBookUI(Player myPlayer, CardBook myCardBook )
    {
        myCard.ShowCardUI(CardReader.ReadCard(myPlayer));
        for (int i = 0; i < myCardBook.GetCount(); i++)
        {
            GameObject go = Instantiate(cardPrefab, content);
            go.GetComponent<UICard>().ShowCardUI(myCardBook.GetCardBook(i));
        }
    }
}
