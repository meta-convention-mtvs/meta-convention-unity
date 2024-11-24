using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIAnounceMgr : MonoBehaviour
{
    public AudioSource audioSource;

    // 오디오 clip 담아두기 
    public AudioClip[] anounceVoice = new AudioClip[10];
    // 사용 할 텍스트 담아두기
    public string[] anounceText = new string[10];

    public int idx;
    public int currentIdx;

    // 텍스트를 띄울 UI 도 만들어라
    public GameObject aiBubbleUI;
    // .. text 만 있으면 되는 걸까? 일단 만들어 둠
    public Text bubbleText;

    void Start()
    {
        idx = 0;
        currentIdx = 10;
        anounceText[0] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨셈변에 오신 걸 환영해요!";
        anounceText[1] = "원하는 부스를 관람하실 수 이도록, 제가 부스를 추천 해 드릴게요";
        anounceText[2] = "관심사나 원하는 정보를 입력 해보세요!";

        anounceText[3] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨셈변에 오신 걸 환영해요!";
        anounceText[4] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨셈변에 오신 걸 환영해요!";
        anounceText[5] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨셈변에 오신 걸 환영해요!";
        anounceText[6] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨셈변에 오신 걸 환영해요!";
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            idx++;
        }

        // 시점에 맞춰서 idx 지정하고 , 텍스트 노출 시키고, 사운드 재생하기
        if (idx != currentIdx)
        {
            currentIdx = idx;
            PlayAnounce(anounceVoice, idx);
        }
    }

    public void PlayAnounce(AudioClip[] clips, int idx)
    {
        audioSource.Stop();

        audioSource.clip = anounceVoice[idx];
        bubbleText.text = anounceText[idx];

        audioSource.Play();
    }
}
