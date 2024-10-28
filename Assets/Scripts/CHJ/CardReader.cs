using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardReader : MonoBehaviour
{
    public GameObject uiCardObject;
    public Button saveButton;

    CardBook cardBook;

    private void Start()
    {
        cardBook = GameObject.FindWithTag("CardBook").GetComponent<CardBook>();
    }

    public void ShowCardUI(Player player)
    {
        //uiCardPopup.SetActive(true);
        UIManager.Instance.ShowUI(uiCardObject, UIType.Normal);
        UICard uiCard = uiCardObject.GetComponentInChildren<UICard>();
        // 남의 카드를 읽어온다.
        Card opponentCard = ReadCard(player);
        // 그 카드를 UI에 띄운다.
        uiCard.ShowCardUI(opponentCard);
        // save 버튼이 눌려지면, 그 카드를 저장한다.
        saveButton.onClick.AddListener(() => OnAddCard(opponentCard));
    }

    public void OnAddCard(Card card)
    {
        cardBook.addCard(card);
        UIManager.Instance.ShowPopupUI("카드가 추가되었습니다.");
    }
    public static Card ReadCard(Player player)
    {
        Card newCard = new Card((string)player.CustomProperties["id"], player.NickName, (string)player.CustomProperties["institute"], (string)player.CustomProperties["major"], (string)player.CustomProperties["email"]);

        return newCard;
    }
}
