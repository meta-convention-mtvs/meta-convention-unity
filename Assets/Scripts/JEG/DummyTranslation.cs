using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyTranslation : MonoBehaviour
{
    public AudioSource translationAudio;

    public AudioClip[] dTranslations = new AudioClip[12];

    public int idx;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { idx = 0; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F2)) { idx = 1; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F3)) { idx = 2; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F4)) { idx = 3; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F5)) { idx = 4; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F6)) { idx = 5; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F7)) { idx = 6; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F8)) { idx = 7; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F9)) { idx = 8; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F10)) { idx = 9; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F11)) { idx = 10; TranslationPlay(idx);}
        if (Input.GetKeyDown(KeyCode.F12)) { idx = 11; TranslationPlay(idx);}
    }

    void TranslationPlay(int idx)
    {
        translationAudio.clip =  dTranslations[idx];
        translationAudio.Play();
    }
}
