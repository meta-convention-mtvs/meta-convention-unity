using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Photon.Pun;

public class InteractableAIEmployeeObject : MonoBehaviourPun, IKeyInteractableObject
{
    public string companyID = "cf79ea17-a487-4b27-a20d-bbd11ff885da";

    GameObject AISpeackUI;
    BusinessRoomReservator businessRoomReservator;
    VoiceManager voiceManager;
    AIWebSocket aiWebSocket;

    bool isInteracting = false;

    // ToDo: UUID가 초기화되었을 때 UID 값을 읽어와야 함.
    private UID ownerUID;

    public void ShowText()
    {
        UIManager.Instance.ShowPopupUI("(F)키를 눌러 AI 직원과 실시간 상담을 시작해 보세요!");
    }
    public void HideText()
    {
        
    }
    public void Interact()
    {
        isInteracting = true;
        //UI를 띄운다
        UIManager.Instance.ShowUI(AISpeackUI, UIType.Conversation);
        // Button을 활성화시킨다.
        Button button = AISpeackUI.GetComponentInChildren<Button>();
        if (button != null)
        {
            // ToDo: photonView Owner 가 아닌 내가 가진 기업의 uuid로 player 객체 만들어서 건네주어야 함
            button.onClick.AddListener(() => businessRoomReservator.MakeAppointmentWith(photonView.Owner));
        }
        // 카메라 움직임을 조정한다.
        MainHallVirtualCameraMovement.Instance.SetActiveVirtualCamera(MainHallVirtualCameraMovement.Instance.aiSpeackVirtualCamera);
        // ToDo: 카메라 위치를 ai 직원 위치로 맞춰주어야 함;
        aiWebSocket = GameObject.FindObjectOfType<AIWebSocket>();
        if (aiWebSocket != null)
        {
            aiWebSocket.Connect(ownerUID.uid, CashedDataFromDatabase.Instance.playerLanguage.language);
        }
    }
    public void InteractEnd()
    {
        isInteracting = false;
        // UI를 끈다
        AISpeackUI.GetComponent<GeneralUI>().Hide();
        aiWebSocket = GameObject.FindObjectOfType<AIWebSocket>();
        if (aiWebSocket != null)
        {
            aiWebSocket.WebSocketEnd();
        }
        MainHallVirtualCameraMovement.Instance.SetActiveVirtualCamera(MainHallVirtualCameraMovement.Instance.playerFollowCamera);
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
        if (businessRoomReservator == null)
            Debug.LogError("Can't find BusinessRoomReservator, set tag");
        ownerUID = GetComponent<UID>();
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
