using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class SoundMgr : Singleton<SoundMgr>
{
    // 재생 할 오디오 파일
    public AudioSource audioSource;
    // 우리에게 필요한 오디오 파일들 담아놓기
    public AudioClip[] audios = new AudioClip[10];

    public int idx;
    public int curidx;
    void Start()
    {
        idx = 0;
        curidx = 10;
        audioSource.volume = 0.5f;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    idx++;
        //}


        // 씬 이동 이나 음악 변경이 필요할때 
        // idx 를 변경하면 idx가 변화를 감지하고 음악을 바꿔 줌
        if (idx != curidx) {
            curidx = idx;
            PlayAudio(audios, idx);
        }
    }    

    public void StopAudio()
    {
        if(audioSource != null)
            audioSource.Stop();
    }
    public void PlayAudio(AudioClip[] clips, int idx)
    {
        audioSource.Stop();

        audioSource.clip = clips[idx];
        audioSource.Play();
    }
    // 백그라운드 audio가 필요하면 따로 만들어서 계속 재생되게 하면 됨..

}
