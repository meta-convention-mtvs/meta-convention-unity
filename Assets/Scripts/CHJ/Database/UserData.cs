using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public Card card { get; private set; }
    public CharacterCustomizeData characterCustomize { get; private set; }


    public void SetCard(string id, string nickname, string institute, string major, string email, string phoneNumber)
    {
        card = new Card(id, nickname, institute, major, email, phoneNumber);
    }

    public void SetCustomizeData(bool isMan, int customizingIdx)
    {
        characterCustomize = new CharacterCustomizeData(isMan, customizingIdx);
    }
}
