using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System;

public class UICard : MonoBehaviour
{
    public Text nameText;
    public Text instituteText;
    public Text majorText;
    public Text emailText;

    public void ShowCardUI(Card card)
    {
        nameText.text = card.nickname;
        instituteText.text = card.institute;
        majorText.text = card.major;
        emailText.text = card.email;
    }

}
