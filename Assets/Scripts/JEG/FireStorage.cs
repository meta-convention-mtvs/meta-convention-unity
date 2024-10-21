using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;
using UnityEngine;

public class FireStorage : MonoBehaviour
{
    public static FireStorage instance;

    FirebaseStorage storage;

    private void Awake()
    {
        instance = this;    
    
    }
    void Start()
    {
        storage = FirebaseStorage.DefaultInstance;   
    }

    public void Upload(byte[] data, string path)
    {
        StartCoroutine(CoUpload(data, path)); 
    }

    IEnumerator CoUpload(byte[] data, string path)
    {
        // 경로 설정 및 파일 이름 설정
        StorageReference storageReference = storage.GetReference(path);
        // 파일 업로드 요청
        Task<StorageMetadata> task = storageReference.PutBytesAsync(data);
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약에 예외가 없다면
        if(task.Exception == null){
            print(" 파일 업로드 성공 ");
        }
        else
        {
            print("파일 업로드 실패 : " + task.Exception);
        }
    }

    public void DownLoad(string path, Action<byte[]> onComplete)
    {
        StartCoroutine(CoDownload(path, onComplete));
    }

    IEnumerator CoDownload(string path, Action<byte[]> onComplete)
    {
        // 경로 설정 및 하일 이름 설정
        StorageReference storageReference = storage.GetReference(path);
        // 파일 업로드 요청
        Task<byte[]> task = storageReference.GetBytesAsync(long.MaxValue);
        // 통신이 완료 될 때 까지 기다린다.
        yield return new WaitUntil(() => task.IsCompleted);
        // 만약 예외가 없으면
        if(task.Exception == null)
        {
            print(" 파일 다운 로드 성공");
            if(onComplete != null)
            {
                onComplete(task.Result);
            }
        }
        else
        {
            print("파일 다운로드 실패 : " + task.Exception);
        }
    }
}
