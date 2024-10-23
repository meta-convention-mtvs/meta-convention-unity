using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardReader : MonoBehaviour
{
    public GameObject uiCardObject;
    public Button saveButton;

    public CardBook cardBook;

    public void ShowCardUI(Player player)
    {
        //uiCardPopup.SetActive(true);
        UIManager.Instance.ShowUI(uiCardObject, UIType.Normal);
        UICard uiCard = uiCardObject.GetComponentInChildren<UICard>();
        // ���� ī�带 �о�´�.
        Card opponentCard = ReadCard(player);
        // �� ī�带 UI�� ����.
        uiCard.ShowCardUI(opponentCard);
        // save ��ư�� ��������, �� ī�带 �����Ѵ�.
        saveButton.onClick.AddListener(() => OnAddCard(opponentCard));
    }

    public void OnAddCard(Card card)
    {
        cardBook.addCard(card);
    }

    public static Card ReadCard(Player player)
    {
        Card newCard = new Card((string)player.CustomProperties["id"], player.NickName, (string)player.CustomProperties["institute"], (string)player.CustomProperties["major"], (string)player.CustomProperties["email"]);

        return newCard;
    }
}
