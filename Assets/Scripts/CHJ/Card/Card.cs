using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Firestore;

[FirestoreData]
public class Card
{
    [FirestoreProperty]
    public string id { get; set;}
    [FirestoreProperty]
    public string nickname { get; set; }
    [FirestoreProperty]
    public string institute { get; set; }
    [FirestoreProperty]
    public string major { get; set; }
    [FirestoreProperty]
    public string email { get; set; }
    [FirestoreProperty]
    public string phoneNumber { get; set; }

    public Card(string id, string nickname, string institute, string major, string email, string phoneNumber)
    {
        this.id = id;
        this.nickname = nickname;
        this.institute = institute;
        this.major = major;
        this.email = email;
        this.phoneNumber = phoneNumber;
    }

    public Card()
    {

    }
    
}
