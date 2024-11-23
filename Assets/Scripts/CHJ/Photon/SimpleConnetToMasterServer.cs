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

    void Start()
    {
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.ConnectUsingSettings();

        GameObject go = Instantiate(blackUi);
        CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(NextScene(canvasGroup));
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

        // 기존 코드 - 마스터 서버 연결 전 방 생성 시도 (문제 발생)
        /*
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.PublishUserId = true;
        PhotonNetwork.JoinOrCreateRoom("MainHall", roomOptions, TypedLobby.Default);
        */
    }
    #region 메인서버에 연결

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");

        // 1123 추가 - 마스터 서버 연결 완료 후 방 생성 시도
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        roomOptions.PublishUserId = true;
        PhotonNetwork.JoinOrCreateRoom("MainHall", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MainScene_CHJ");
    }
    #endregion
}
