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

    private bool isAudioPlay;

    // 텍스트를 띄울 UI 도 만들어라
    public GameObject aiBubbleUI;
    // .. text 만 있으면 되는 걸까? 일단 만들어 둠
    public Text bubbleText;

    void Start()
    {
        isAudioPlay = false;

        idx = 0;
        currentIdx = 10;
        anounceText[0] = "안녕하세요 언어의 장벽 없이 전 세계 기업 부스를 손 쉽게 탐험하고, 글로벌 비지니스 미팅을 지원하는 메타 컨벤션에 오신 걸 환영해요!";
        anounceText[1] = "원하는 부스를 관람하실 수 이도록, 제가 부스를 추천 해 드릴게요";
        anounceText[2] = "관심사나 원하는 정보를 입력 해보세요!";

        anounceText[3] = "Hello and welcome to Meta Convention";
        anounceText[4] = "where you can effortlessly explore global corporate booths without language barriers and connect for business meetings!";
        anounceText[5] = "To help you find the right booths, I’ll recommend options based on your interests or needs";
        anounceText[6] = "Just let me know what you're looking for, and I'll guide you! ";

        anounceText[7] = "nǐ hǎo, huānyíng láidào META CONVENTION！ zài zhèlǐ, nǐ keyǐ qīngsōng tànsuǒ quánqiú qǐyè zhǎnwèi, cānjiā shāngwù huìyì qǐng ";
        anounceText[8] = "gàosu wo nǐ gǎn xìngqu de xìnxī，wǒ huì wèi nǐ tuījiàn héshì de zhǎnwèi";

        anounceText[9] = "안녕하세요, 당신의 맞춤형 비서입니다. ";
        anounceText[10] = "이제부터 하시는 모든 말씀을 제가 통역해드릴게요 !";

        anounceText[11] = "Hello! I’m your personal assistant.";
        anounceText[12] = "From now on, I’ll translate everything you say for you!";

        anounceText[13] = "nǐhao ! wǒ shì nǐ de zhùlǐ.";
        anounceText[14] = "cóng xiànzài kāishǐ, nǐ shuō de měi jù huà wǒ doū huì fānyì gei nǐ ! ";
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.M))
        {
            
            idx++;
            idx %= 15;
        }

        // 시점에 맞춰서 idx 지정하고 , 텍스트 노출 시키고, 사운드 재생하기
        if (idx != currentIdx)
        {
            currentIdx = idx;
            PlayAnounce(anounceVoice, idx);
        }

        if (audioSource.isPlaying)
        {
            isAudioPlay = true;
            aiBubbleUI.SetActive(true);
        }
        else if (!audioSource.isPlaying)
        {
            isAudioPlay = false;
            aiBubbleUI.SetActive(false);
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
