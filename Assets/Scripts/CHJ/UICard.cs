using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System;

public class UICard : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text instituteText;
    public TMP_Text majorText;
    public TMP_Text emailText;

    // todo: Player °´Ã¼¸¦


    public void ShowCardUI(Card card)
    {
        nameText.text = card.nickname;
        instituteText.text = card.institute;
        majorText.text = card.major;
        emailText.text = card.email;
    }

}
