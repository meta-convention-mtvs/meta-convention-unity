using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Firebase.Firestore;
using System.Threading.Tasks;
using Firebase.Storage;
using System.IO;
using Firebase.Extensions;
using UnityEngine.Networking;

public class DatabaseManager : Singleton<DatabaseManager>
{
    FirebaseFirestore store;
    FirebaseStorage storage;

    private void Awake()
    {
        store = FirebaseFirestore.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
    }

    public void SaveData<T>(T info) where T : class
    {
        StartCoroutine(CoSaveUserInfo<T>(info));
    }
    IEnumerator CoSaveUserInfo<T>(T info) where T : class
    {
        // 저장 경로 USER/ID/내정보
        string path = "USER/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + "Data/" + typeof(T).ToString();
        // 정보 저장 요청
        Task task = FirebaseFirestore.DefaultInstance.Document(path).SetAsync(info);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없으면 
        if (task.Exception == null)
        {
            print("유저 정보 저장 성공");
        }
        else
        {
            print("유저 정보 저장 실패 : " + task.Exception);
        }
    }



    public void GetData<T>(Action<T> OnComplete) where T : class
    {
        StartCoroutine(CoLoadUserInfo<T>(OnComplete));
    }
    IEnumerator CoLoadUserInfo<T>(Action<T> onComplete) where T : class
    {
        // 저장 경로 USER/ID/내 정보
        string path = "USER/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + "Data/" + typeof(T).ToString();
        // 정보 조회 요청
        Task<DocumentSnapshot> task = FirebaseFirestore.DefaultInstance.Document(path).GetSnapshotAsync();
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약 예외가 없다면
        if (task.Exception == null)
        {
            print("회원 정보 불러오기 성공!");
            // 불러온 정보를 UserInfo 변수에 저장
            T loadInfo = task.Result.ConvertTo<T>();
            // 불러온 정보를 전달
            if (onComplete != null)
            {
                onComplete(loadInfo);
            }
            else
            {
                onComplete(null);
            }
        }
        else
        {
            print("유저 정보 불러오기 실패 : " + task.Exception);
        }
    }

    public T GetData<T>(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string data = PlayerPrefs.GetString(key);
            print(key + " : " + data);
            return (T)JsonUtility.FromJson<T>(data);
        }

        return default(T);
    }



    public void UploadImage(string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("images/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + Path.GetFileName(localFilePath));

        // 파일 업로드
        fileRef.PutFileAsync(localFilePath).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("이미지 업로드 성공");
                fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask =>
                {
                    if (!urlTask.IsFaulted && !urlTask.IsCanceled)
                    {
                        string downloadUrl = urlTask.Result.ToString();
                        Debug.Log("다운로드 URL: " + downloadUrl);
                    }
                });
            }
            else
            {
                Debug.LogError("이미지 업로드 실패: " + task.Exception);
            }
        });
    }

    public void DownloadImage(string imageFileName, Action<Texture2D> OnTextureLoad)
    {
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("images/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + imageFileName);

        fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                string downloadUrl = task.Result.ToString();
                Debug.Log("이미지 다운로드 URL: " + downloadUrl);

                // URL을 통해 비디오 재생
                StartCoroutine(CoDownloadImage(downloadUrl, OnTextureLoad));
            }
            else
            {
                Debug.LogError("이미지 URL 가져오기 실패: " + task.Exception);
            }
        });
    }

    private IEnumerator CoDownloadImage(string url, Action<Texture2D> OnTextureLoad)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("이미지 다운로드 실패: " + www.error);
            }
            else
            {
                // 다운로드한 텍스처를 RawImage에 적용
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                OnTextureLoad?.Invoke(texture);
                Debug.Log("이미지 다운로드 성공");
            }
        }
    }

    public void UploadVideo(string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("videos/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + Path.GetFileName(localFilePath));

        // 파일 업로드
        fileRef.PutFileAsync(localFilePath).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("비디오 업로드 성공");
                fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask =>
                {
                    if (!urlTask.IsFaulted && !urlTask.IsCanceled)
                    {
                        string downloadUrl = urlTask.Result.ToString();
                        Debug.Log("다운로드 URL: " + downloadUrl);
                    }
                });
            }
            else
            {
                Debug.LogError("비디오 업로드 실패: " + task.Exception);
            }
        });
    }

    public void DownLoadVideo(string videoFileName, Action<string> OnVideoLoad)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("videos/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + videoFileName);


        fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                string downloadUrl = task.Result.ToString();
                Debug.Log("비디오 다운로드 URL: " + downloadUrl);
                OnVideoLoad?.Invoke(downloadUrl);
       
            }
            else
            {
                Debug.LogError("비디오 URL 가져오기 실패: " + task.Exception);
            }
        });
    }

    public void UploadObject(string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("objects/" + FireAuthManager.Instance.GetCurrentUser().UserId + "/" + Path.GetFileName(localFilePath));

        // 파일 업로드
        fileRef.PutFileAsync(localFilePath).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("오브젝트 업로드 성공");
                fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(urlTask =>
                {
                    if (!urlTask.IsFaulted && !urlTask.IsCanceled)
                    {
                        string downloadUrl = urlTask.Result.ToString();
                        Debug.Log("다운로드 URL: " + downloadUrl);
                    }
                });
            }
            else
            {
                Debug.LogError("오브젝트 업로드 실패: " + task.Exception);
            }
        });
    }

    public void DownloadObject(string obejctFileName, Action<string> OnObjectLoad)
    {

    }
}
