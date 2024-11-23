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


    public void SaveCardInDatabase(string name, string institute, string major, string phoneNumber) {
        if (name == "" || institute == "" || major == "" || phoneNumber == "")
            return;
        FirebaseUser user = FireAuthManager.Instance.GetCurrentUser();
        if (user == null)
            return;
        Card myCard = new Card(user.UserId, name,institute, major, user.Email, phoneNumber);;
        DatabaseManager.Instance.SaveData<Card>(myCard);
        print("Save data in database");
    }
}
