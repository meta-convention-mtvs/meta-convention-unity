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

    public void GetPublicData<T>(Action<T> OnComplete) where T : class
    {
        StartCoroutine(CoLoadPublicData<T>(OnComplete));
    }

    IEnumerator CoLoadPublicData<T>(Action<T> onComplete) where T : class
    {
        // 저장 경로 USER/ID/내 정보
        string path = "PUBLIC/" +  typeof(T).ToString();
        // 정보 조회 요청
        Task<DocumentSnapshot> task = FirebaseFirestore.DefaultInstance.Document(path).GetSnapshotAsync();
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약 예외가 없다면
        if (task.Exception == null)
        {
            print("정보 불러오기 성공!");
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
            print("정보 불러오기 실패 : " + task.Exception);
        }
    }
    public void SaveDataTo<T>(string uid, T info) where T: class
    {
        StartCoroutine(CoSaveUserInfo<T>(uid, info));
    }
    public void SaveData<T>(T info) where T : class
    {
        StartCoroutine(CoSaveUserInfo<T>(FireAuthManager.Instance.GetCurrentUser().UserId,info));
    }
    IEnumerator CoSaveUserInfo<T>(string uid, T info) where T : class
    {
        // 저장 경로 USER/ID/내정보
        string path = "USER/" + uid + "/" + "Data/" + typeof(T).ToString();
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

    public void SaveCompanyDataTo<T>(string uuid, T info) where T: class
    {
        StartCoroutine(CoSaveCompanyInfo<T>(uuid, info));
    }

    IEnumerator CoSaveCompanyInfo<T>(string uid, T info) where T : class
    {
        // 저장 경로 USER/ID/내정보
        string path = "COMPANY/" + uid + "/" + "Data/" + typeof(T).ToString();
        // 정보 저장 요청
        Task task = FirebaseFirestore.DefaultInstance.Document(path).SetAsync(info);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없으면 
        if (task.Exception == null)
        {
            print("회사 정보 저장 성공");
        }
        else
        {
            print("회사 정보 저장 실패 : " + task.Exception);
        }
    }

    public void GetDataFrom<T>(string uid, Action<T> OnComplete) where T : class
    {
        StartCoroutine(CoLoadUserInfo<T>(uid, OnComplete));
    }

    public void GetData<T>(Action<T> OnComplete) where T : class
    {
        StartCoroutine(CoLoadUserInfo<T>(FireAuthManager.Instance.GetCurrentUser().UserId, OnComplete));
    }
    IEnumerator CoLoadUserInfo<T>(string uid, Action<T> onComplete) where T : class
    {
        // 저장 경로 USER/ID/내 정보
        string path = "USER/" + uid + "/" + "Data/" + typeof(T).ToString();
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

    public void GetCompanyData<T>(string uid, Action<T> OnComplete) where T : class
    {
        StartCoroutine(CoLoadCompanyInfo<T>(uid, OnComplete));
    }

    IEnumerator CoLoadCompanyInfo<T>(string uid, Action<T> onComplete) where T: class
    {
        // 저장 경로 USER/ID/내 정보
        string path = "COMPANY/" + uid + "/" + "Data/" + typeof(T).ToString();
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
    public void UploadImage(string localFilePath)
    {
        UploadImageTo(FireAuthManager.Instance.GetCurrentUser().UserId, localFilePath);
    }

    public void UploadImageTo(string uid, string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("images/" + uid + "/" + Path.GetFileName(localFilePath));

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

    public void UploadLogoTo(string uid, string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("logos/" + uid + "/" + Path.GetFileName(localFilePath));

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

    public void UploadBannerTo(string uid, string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("banners/" + uid + "/" + Path.GetFileName(localFilePath));

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

    public void UploadBrochure(string uid, string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("brochures/" + uid + "/" + Path.GetFileName(localFilePath));

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
        DownloadImageFrom(FireAuthManager.Instance.GetCurrentUser().UserId, imageFileName, OnTextureLoad);
    }

    public void DownloadImageFrom(string uid, string imageFileName, Action<Texture2D> OnTextureLoad)
    {
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("images/" + uid + "/" + imageFileName);

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

    public void DownloadLogoFrom(string uid, string imageFileName, Action<Texture2D> OnTextureLoad)
    {
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("logos/" + uid + "/" + imageFileName);

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

    public void DownloadBannerFrom(string uid, string imageFileName, Action<Texture2D> OnTextureLoad)
    {
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("banners/" + uid + "/" + imageFileName);

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

    public void DownloadBrochureFrom(string uid, string imageFileName, Action<Texture2D> OnTextureLoad)
    {
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("brochures/" + uid + "/" + imageFileName);

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
        UploadVideoTo(FireAuthManager.Instance.GetCurrentUser().UserId, localFilePath);
    }
    public void UploadVideoTo(string uid, string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("videos/" + uid + "/" + Path.GetFileName(localFilePath));

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
        DownLoadVideoFrom(FireAuthManager.Instance.GetCurrentUser().UserId, videoFileName, OnVideoLoad);
    }

    public void DownLoadVideoFrom(string uid, string videoFileName, Action<string> OnVideoLoad)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("videos/" + uid + "/" + videoFileName);


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
        UploadObjectTo(FireAuthManager.Instance.GetCurrentUser().UserId, localFilePath);
    }
    public void UploadObjectTo(string uid, string localFilePath)
    {
        // Storage 참조 설정
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("objects/" + uid + "/" + Path.GetFileName(localFilePath));

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
    public void DownloadObject(string objFileName, Action<string> OnObjDownload)
    {
        DownloadObjectFrom(FireAuthManager.Instance.GetCurrentUser().UserId, objFileName, OnObjDownload);
    }

    // .obj 파일 다운로드 메서드
    public void DownloadObjectFrom(string uid, string objFileName, Action<string> OnObjDownload)
    {
        // Firebase Storage에서 객체 참조
        var storageRef = storage.GetReferenceFromUrl("gs://metaconvention.appspot.com");
        var fileRef = storageRef.Child("objects/" + uid + "/" + objFileName);  // models 폴더에 있는 .obj 파일

        // 비디오 다운로드 URL을 가져오는 방식과 동일
        fileRef.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                string downloadUrl = task.Result.ToString();
                Debug.Log("파일 다운로드 URL: " + downloadUrl);

                // 다운로드한 URL을 사용하여 파일을 로컬에 저장
                StartCoroutine(DownloadFileToLocal(downloadUrl, uid, objFileName, OnObjDownload));
            }
            else
            {
                Debug.LogError("파일 URL 가져오기 실패: " + task.Exception);
            }
        });
    }

    // 파일 다운로드를 처리하는 코루틴
    private IEnumerator DownloadFileToLocal(string url, string uid, string fileName, Action<string> OnObjDownload)
    {
        // 로컬 파일 저장 경로 설정
        string localPath = Path.Combine(Application.persistentDataPath, uid, fileName);

        // UnityWebRequest를 통해 URL에서 파일을 다운로드
        using (var www = new UnityWebRequest(url))
        {
            www.downloadHandler = new DownloadHandlerFile(localPath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("파일 다운로드 완료: " + localPath);
                OnObjDownload?.Invoke(localPath);
            }
            else
            {
                Debug.LogError("파일 다운로드 실패: " + www.error);
            }
        }
    }
}
