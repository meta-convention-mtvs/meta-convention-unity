using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIAnnounceMgr : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip[] announceVoice = new AudioClip[15];
    // 오디오 clip 담아두기 
    public AudioClip[] announceVoiceKR = new AudioClip[15];
    public AudioClip[] announceVoiceEN = new AudioClip[15];
    public AudioClip[] announceVoiceZH = new AudioClip[15];

    public string[] announceText = new string[15];
    // 사용 할 텍스트 담아두기
    public string[] announceTextKR = new string[15];
    public string[] announceTextEN = new string[15];
    public string[] announceTextZH = new string[15];

    public int idx;
    public int currentIdx;

    public int endIdx;

    private bool isAudioPlay;

    // 텍스트를 띄울 UI 도 만들어라
    public GameObject aiBubbleUI;
    // .. text 만 있으면 되는 걸까? 일단 만들어 둠
    public Text bubbleText;

    public int startNum;
    public int endNum;

    void Start()
    {
        print(LanguageSingleton.Instance.language);
        print(UuidMgr.Instance.currentUserInfo.language);

        isAudioPlay = false;
        currentIdx = 15;

        #region KR text
        announceTextKR[0] = "안녕하세요, 저는 당신의 AI 개인비서입니다";
        announceTextKR[1] = "편안한 관람을 위해 제가 부스를 추천해드릴게요";
        announceTextKR[2] = "관심사나 원하는 정보를 입력해보세요!";
        announceTextKR[3] = "";
        announceTextKR[4] = "";
        
        announceTextKR[5] = "언어의 장벽 없이 전 세계 기업 부스를 손쉽게 탐험하고,";
        announceTextKR[6] = "글로벌 비즈니스 미팅을 지원하는 메타컨벤션에 오신 걸 환영해요!";
        announceTextKR[7] = "";
        announceTextKR[8] = "";
        announceTextKR[9] = "";
        
        announceTextKR[10] = "성공적인 미팅을 위해 AI 직원이 응대한 내용을 요약하고 비즈니스 미팅 전략을 제안해드릴게요!";
        announceTextKR[11] = "";
        announceTextKR[12] = "";
        
        announceTextKR[13] = "원활한 소통을 위해, 이제부터 하시는 모든 말씀을 제가 책임지고 통역해드릴게요!";
        announceTextKR[14] = "";
        #endregion

        #region EN text
        announceTextEN[0] = "Hi there, I’m your AI personal assistant!";
        announceTextEN[1] = "To ensure a comfortable visit, I'll recommend some booths for you.";
        announceTextEN[2] = "Please enter your interests or the information you’re looking for!";
        announceTextEN[3] = "";
        announceTextEN[4] = "";
        
        announceTextEN[5] = "Welcome to MetaConvention,";
        announceTextEN[6] = "where you can effortlessly explore booths from companies around the world";
        announceTextEN[7] = "and support global business meetings without language barriers!";
        announceTextEN[8] = "";
        announceTextEN[9] = "";
        
        announceTextEN[10] = "For a successful meeting, I’ll provide a summary of the AI staff’s responses";
        announceTextEN[11] = "and suggest a tailored business meeting strategy!";
        announceTextEN[12] = "";

        announceTextEN[13] = "From now on, I’ll translate everything you say for you!";
        announceTextEN[14] = "";
        #endregion

        #region ZH text
        announceTextZH[0] = "您好，为了让您有一个舒适的参观体验，我会为您推荐一些展位。";
        announceTextZH[1] = "请输入您的兴趣或想了解的信息！";
        announceTextZH[2] = "";
        announceTextZH[3] = "";
        announceTextZH[4] = "";
        
        announceTextZH[5] = "欢迎来到MetaConvention，在这里您可以轻松探索来自世界各地的企业展台，";
        announceTextZH[6] = "并支持全球商务会议，无需语言障碍!";
        announceTextZH[7] = "";
        announceTextZH[8] = "";
        announceTextZH[9] = "";
        
        announceTextZH[10] = "为了达成一次成功的会议，我将总结AI员工的回复，";
        announceTextZH[11] = "并为您提供定制的商务会议策略！";
        announceTextZH[12] = "";

        announceTextZH[13] = "你好，我是你的AI个人助理！从现在开始，我会为你翻译你说的每一句话！";
        announceTextZH[14] = "";

        #endregion

        if(LanguageSingleton.Instance.language == "ko")
        {
            announceVoice = announceVoiceKR;
            announceText = announceTextKR;
        } else if(LanguageSingleton.Instance.language == "en")
        {
            announceVoice = announceVoiceEN;
            announceText = announceTextEN;
        } else if(LanguageSingleton.Instance.language == "zh")
        {
            announceVoice = announceVoiceZH;
            announceText = announceTextZH;
        }

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

        if (!audioSource.isPlaying && idx < endIdx)
        {
            idx++;
            if (idx < endIdx)
            {
                PlayAnounce(idx);
            }
        }

        if (audioSource.isPlaying)
        {
            isAudioPlay = true;
            if(aiBubbleUI != null)
                aiBubbleUI.SetActive(true);
        }
        else if (!audioSource.isPlaying)
        {
            isAudioPlay = false;
            if(aiBubbleUI != null)
                aiBubbleUI.SetActive(false);
        }

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

    public void PlayAnounce(int idx)
    {
        audioSource.Stop();
        audioSource.clip = announceVoice[idx];
        if(bubbleText != null)
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