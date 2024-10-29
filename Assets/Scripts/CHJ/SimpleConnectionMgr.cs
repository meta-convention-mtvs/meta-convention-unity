using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConnectionMgr : MonoBehaviourPunCallbacks
{
    public GameObject blackUi;
    public float sceneTransitionTime = 3.0f;
    public float fadeDuration = 2.0f;

    #region 메인서버에 연결
    private void Start()
    {
        PhotonNetwork.SendRate = 120;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.ConnectUsingSettings();

        GameObject go = Instantiate(blackUi);
        CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(NextScene(canvasGroup));
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to Master");
        JoinOrCreateRoom("MainHall");
    }
    #endregion

    #region 룸에 입장, 나가기
    public void JoinOrCreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 20;
            roomOptions.PublishUserId = true;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    #endregion

    #region 룸 콜백함수
    public override void OnJoinedRoom()
    {
        print("Entered the Room");

    }

    IEnumerator NextScene(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(sceneTransitionTime);
        while (!PhotonNetwork.InRoom)
        {
           yield return null;
        }
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

        PhotonNetwork.LoadLevel("MainScene_CHJ");
    }

    #endregion
}
