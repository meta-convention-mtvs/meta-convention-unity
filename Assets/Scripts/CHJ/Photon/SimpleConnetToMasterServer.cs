using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConnetToMasterServer : MonoBehaviourPunCallbacks
{
    public GameObject blackUi;
    public float sceneTransitionTime = 3.0f;
    public float fadeDuration = 2.0f;
    public BoothCategory boothCategory = BoothCategory.Mobility;

    float currentTime = 0;
    bool isPhotonConnecting = false;

    private void Awake()
    {
        ShowBlackUi();
    }

    private void Update()
    {
        if (!isPhotonConnecting)
        {
            // sceneTransitionTime + fadeDuration 이 지나고, CashedDataFromDatabase가 다 읽어왔으면 실행
            if (currentTime > sceneTransitionTime + fadeDuration && CashedDataFromDatabase.Instance.allDataCashed)
            {
                PhotonConnect();
                isPhotonConnecting = true;
            }

            currentTime += Time.deltaTime;
        }
    }
    void PhotonConnect()
    {
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.ConnectUsingSettings();
    }
    IEnumerator NextScene(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(sceneTransitionTime);
        canvasGroup.alpha = 0f; // 알파 값을 투명으로 설정

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        // 최종 알파 값 설정 (완전 불투명)
        canvasGroup.alpha = 1f;
        yield return null;
    }
    #region 메인서버에 연결

    void ShowBlackUi()
    {
        GameObject go = Instantiate(blackUi);
        CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(NextScene(canvasGroup));
    }
    public override void OnConnectedToMaster()
    {
        print("Connected to Master");

        // 1123 추가 - 마스터 서버 연결 완료 후 방 생성 시도
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.PublishUserId = true;
        //ToDo: Main Hall이 아니라 카테고리 이름으로 읽어와야 한다.
        PhotonNetwork.JoinOrCreateRoom(boothCategory.ToString(), roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainScene_CHJ");
    }
    #endregion
}
