using CHJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRuntimeCreate : MonoBehaviour
{
    [SerializeField] UICard uiCard;
    [SerializeField] InteractableCardObject interactableCardObject;

    private void Start()
    {
        UID uid = GetComponent<UID>();
        if(uid != null)
        {
            uid.OnUIDChanged += CreateCard;
        }
        else
        {
            Debug.Log("Card를 만들려는데 uid component가 존재하지 않습니다. 이름: " + gameObject.name);
        }
    }

    async void CreateCard(string uid)
    {
        Card card = await AsyncDatabase.GetDataFromDatabase<Card>(DatabasePath.GetUserDataPath(uid, nameof(Card)));
        if (card != null)
        {
            uiCard.ShowCardUI(card);
            interactableCardObject.SetCard(card);
        }
    }
}
