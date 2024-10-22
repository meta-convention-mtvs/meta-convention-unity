using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Card
{ 
    public string id;
    public string nickname;
    public string institute;
    public string major;
    public string email;

    public Card(string id, string nickname, string institute, string major, string email)
    {
        this.id = id;
        this.nickname = nickname;
        this.institute = institute;
        this.major = major;
        this.email = email;
    }
    
}
