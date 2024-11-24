using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;

public class CardMaker : MonoBehaviour
{
    public UICardMaker uiCardMaker;

    private void Start()
    {
        uiCardMaker.OnSaveClick += SaveCardInDatabase;
    }


    public void SaveCardInDatabase(string name, string institute, string major, string phoneNumber, string uuid) {
        if (name == "" || institute == "" || major == "" || phoneNumber == "")
            return;
        FirebaseUser user = FireAuthManager.Instance.GetCurrentUser();
        if (user == null)
            return;
        Card myCard = new Card(user.UserId, name,institute, major, user.Email, phoneNumber);

        // TODO: 만약 회사 유저이면 UUID를 받아야 한다. Where is uuid?

        myCard.uuid = uuid;
        print("company saved uuid: " + uuid);

        DatabaseManager.Instance.SaveData<Card>(myCard);
        print("Save data in database");
    }
}
