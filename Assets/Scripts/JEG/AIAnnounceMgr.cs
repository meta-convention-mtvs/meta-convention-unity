using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIAnnounceMgr : MonoBehaviour
{
    public AudioSource audioSource;

    // 오디오 clip 담아두기 
    public AudioClip[] announceVoice = new AudioClip[15];
    // 사용 할 텍스트 담아두기
    public string[] announceText = new string[15];

    public int idx;
    public int currentIdx;

    private bool isAudioPlay;

    // 텍스트를 띄울 UI 도 만들어라
    public GameObject aiBubbleUI;
    // .. text 만 있으면 되는 걸까? 일단 만들어 둠
    public Text bubbleText;

    public int startNum;
    public int endNum;

    void Start()
    {
        isAudioPlay = false;
        idx = 15;
        currentIdx = 15;
        announceText[0] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨벤션에 오신 걸 환영해요!";
        announceText[1] = "원하는 부스를 관람하실 수 있도록, 제가 부스를 추천 해 드릴게요.";
        announceText[2] = "관심사나 원하는 정보를 입력 해보세요!";

        announceText[3] = "Hello and welcome to Meta Convention";
        announceText[4] = "where you can effortlessly explore global corporate booths without language barriers and connect for business meetings!";
        announceText[5] = "To help you find the right booths, I’ll recommend options based on your interests or needs";
        announceText[6] = "Just let me know what you're looking for, and I'll guide you! ";

        announceText[7] = "nǐ hǎo, huānyíng láidào META CONVENTION！ zài zhèlǐ, nǐ keyǐ qīngsōng tànsuǒ quánqiú qǐyè zhǎnwèi, cānjiā shāngwù huìyì qǐng ";
        announceText[8] = "gàosu wo nǐ gǎn xìngqu de xìnxī，wǒ huì wèi nǐ tuījiàn héshì de zhǎnwèi";

        announceText[9] = "안녕하세요, 당신의 맞춤형 비서입니다. ";
        announceText[10] = "이제부터 하시는 모든 말씀을 제가 통역해드릴게요 !";

        announceText[11] = "Hello! I’m your personal assistant.";
        announceText[12] = "From now on, I’ll translate everything you say for you!";

        announceText[13] = "nǐhao ! wǒ shì nǐ de zhùlǐ.";
        announceText[14] = "cóng xiànzài kāishǐ, nǐ shuō de měi jù huà wǒ doū huì fānyì gei nǐ ! ";

        // 유저의 언어 정보, 상황
        // 언어 ko, en, zh
        // 상황 입장, 번역
        // 3*2 case

        //LanguageSingleton.Instance.language;
        //UuidMgr
        //CashedDataFromDatabase.Instance.playerLanguage;

        // 입장하면 
        // 한글 이면 0-2
        // 영어면 3-6
        // 중국어면 7-8


        // 미팅룸에 입장하면 
        // 한글 이면 9-10
        // 영어면 11-12
        // 중국어면 13-14

    }

    // Update is called once per frame
    void Update()
    {
        // 시점에 맞춰서 idx 지정하고 , 텍스트 노출 시키고, 사운드 재생하기
        if (idx != currentIdx)
        {
            currentIdx = idx;
            PlayAnounce(idx);
        }

        // 오디오가 재생 중이 아니고, idx가 endNum보다 작으면 다음 오디오 재생
        if (!audioSource.isPlaying && idx < endNum)
        {
            idx++;
            if (idx < endNum)
            {
                PlayAnounce(idx);
            }
        }

        // 오디오 재생 상태에 따라 UI 업데이트
        if (audioSource.isPlaying)
        {
            isAudioPlay = true;
            aiBubbleUI.SetActive(true);
        }
        else
        {
            isAudioPlay = false;
            aiBubbleUI.SetActive(false);
        }

        if(LanguageSingleton.Instance.language == "KO")
        {
            AnnounceSetter(0, 2);
        }else if(LanguageSingleton.Instance.language =="EN")
        {
            AnnounceSetter(3, 6);
        }else if(LanguageSingleton.Instance.language == "ZH")
        {
            AnnounceSetter(7, 8);
        }
    }

    public void PlayAnounce(int idx)
    {
        audioSource.Stop();
        audioSource.clip = announceVoice[idx];
        bubbleText.text = announceText[idx];
        audioSource.Play();
    }

    public void AnnounceSetter(int startNum, int endNum)
    {
        this.startNum = startNum;
        this.endNum = endNum;
        idx = startNum;
        if (!audioSource.isPlaying)
        {
            PlayAnounce(idx);
        }
    }


}