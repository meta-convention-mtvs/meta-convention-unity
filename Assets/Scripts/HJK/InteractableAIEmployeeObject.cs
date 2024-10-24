using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class InteractableAIEmployeeObject : MonoBehaviour, IKeyInteractableObject
{
    public Text text_1;

    VoiceManager voiceManager;

    bool isInteracting = false;
    public void ShowText()
    {
        text_1.gameObject.SetActive(true);
    }
    public void HideText()
    {
        text_1.gameObject.SetActive(false);
    }
    public void Interact()
    {
        isInteracting = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        voiceManager = GameObject.FindWithTag("VoiceManager").GetComponent<VoiceManager>();
        if (voiceManager == null)
            Debug.LogError("Can't find voiceManager, set tag");
    }

    // Update is called once per frame
    async void Update()
    {
        if (isInteracting)
        {
            // 'M' 키를 눌러 녹음 시작
            if (Input.GetKeyDown(KeyCode.M))
            {
                voiceManager.StartRecording();
            }

            // 'M' 키를 떼면 녹음 중지 및 전송
            if (Input.GetKeyUp(KeyCode.M))
            {
                await voiceManager.StopRecordingAndSend();
            }

            // 'P' 키를 눌러 현재 재생 중인 오디오 중지
            if (Input.GetKeyDown(KeyCode.P))
            {
                voiceManager.StopCurrentAudioPlayback();
            }
        }
    }
}
