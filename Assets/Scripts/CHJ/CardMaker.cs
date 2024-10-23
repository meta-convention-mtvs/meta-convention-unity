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

        Card myCard = new Card(PhotonNetwork.NickName = name, PhotonNetwork.NickName = name,institute, major, email);
        DatabaseManager.Instance.SaveData<Card>(myCard);
        print("Save data in database");
    }
}
