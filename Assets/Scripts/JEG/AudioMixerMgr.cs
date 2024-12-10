using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Audio;

public class AudioMixerMgr: MonoBehaviour
{
    [SerializeField] public AudioMixer audioMixer;
    [SerializeField] public Slider aiAnnounceSoundSlider;
    [SerializeField] public Slider bgmSlider;
    [SerializeField] public Slider effectSoundSlider;

    private void Awake()
    {
        // 슬라이더를 움직이면 해당 group의 볼륨을 조절한다.
        aiAnnounceSoundSlider.onValueChanged.AddListener(SetAnnounceSoundVolume);
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        effectSoundSlider.onValueChanged.AddListener(SetEffectSoundVolume);

    }

    public void SetAnnounceSoundVolume(float volume)
    {
        audioMixer.SetFloat("AiAnnounceSound", Mathf.Log10(volume) * 20);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }

    public void SetEffectSoundVolume(float volume)
    {
        audioMixer.SetFloat("EffectSound", Mathf.Log10(volume) * 20);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
