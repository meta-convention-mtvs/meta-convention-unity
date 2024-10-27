using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Photon.Pun;

public class InteractableAIEmployeeObject : MonoBehaviourPun, IKeyInteractableObject
{
    public Text text_1;

    GameObject AISpeackUI;
    BusinessRoomReservator businessRoomReservator;
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
        //UI를 띄운다
        UIManager.Instance.ShowUI(AISpeackUI, UIType.Conversation);
        // Button을 활성화시킨다.
        AISpeackUI.GetComponentInChildren<Button>()?.onClick.AddListener(() => businessRoomReservator.MakeAppointmentWith(photonView.Owner));
    }

    void Start()
    {
        voiceManager = GameObject.FindWithTag("VoiceManager").GetComponent<VoiceManager>();
        AISpeackUI = GameObject.FindWithTag("AISpeackUI");
        businessRoomReservator = GameObject.FindWithTag("BusinessRoomReservator").GetComponent<BusinessRoomReservator>();
        if (voiceManager == null)
            Debug.LogError("Can't find voiceManager, set tag");
        if (AISpeackUI == null)
            Debug.LogError("Can't find AISpeackUI, set tag");
    }

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
