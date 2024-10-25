using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CardMaker : MonoBehaviour
{
    public UICardMaker uiCardMaker;


    private void Start()
    {
        uiCardMaker.OnSaveClick += SaveCardInDatabase;
    }


    public void SaveCardInDatabase(string name, string institute, string major, string email) {
        if (name == "" || institute == "" || major == "" || email == "")
            return;
        Card myCard = new Card(FireAuthManager.Instance.GetCurrentUser().UserId, name,institute, major, email);
        DatabaseManager.Instance.SaveData<Card>(myCard);
        print("Save data in database");

    }
}
