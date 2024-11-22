using Firebase.Firestore;
using Firebase.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace CHJ
{
    public static class DatabasePath
    {
        public static string GetUserDataPath(string uid, string className)
        {
            return "USER/" + uid + "/Data/" + className;
        }

        public static string GetCompanyDataPath(string uuid, string className)
        {
            return "COMPANY/" + uuid + "/Data/" + className;
        }
    }

    public static class AsyncDatabase
    {
        public static async Task<Texture2D> GetTextureFromDatabaseWithUid(string uid, string fileName)
        {
            var url = await GetLogoDownloadUrl(uid, fileName);
            var texture = await GetTextureFromDatabaseWithUrl(url.ToString());
            return texture;
        }

        public static async Task<string> GetObjectFileLocalPathFromDatabaseWithUid(string uid, string fileName)
        {
            var url = await GetObjectDownloadUrl(uid, fileName);
            var localPath = await GetObjectFileLocalPathFromDatabaseWithUrl(url.ToString(), uid, fileName);
            return localPath;
        }

        public static async Task<T> GetDataFromDatabase<T>(string path) where T : class
        {
            Task<DocumentSnapshot> task = FirebaseFirestore.DefaultInstance.Document(path).GetSnapshotAsync();
            await task;

            if (task.Exception == null)
            {
                Debug.Log("회원 정보 불러오기 성공!");
                T loadInfo = task.Result.ConvertTo<T>();
                return loadInfo;
            }
            else
            {
                Debug.LogError("Exception in Database loading: " + typeof(T) + " " + task.Exception);
                return null;
            }
        }

        public static async Task<Uri> GetLogoDownloadUrl(string uid, string logoFileName)
        {
            return await GetDownloadUrl("logos", uid, logoFileName);
        }

        public static async Task<Uri> GetImageDownloadUrl(string uid, string imageFileName)
        {
            return await GetDownloadUrl("images", uid, imageFileName);
        }

        public static async Task<Uri> GetVideoDownloadUrl(string uid, string videoFileName)
        {
            return await GetDownloadUrl("videos", uid, videoFileName);
        }

        public static async Task<Uri> GetObjectDownloadUrl(string uid, string objectFileName)
        {
            return await GetDownloadUrl("objects", uid, objectFileName);
        }

        /// <summary>
        /// 이거를 직접 호출하면 안 됨!!
        /// </summary>
        /// <param name="type">images, videos, objects</param>
        /// <param name="uid"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        async static Task<Uri> GetDownloadUrl(string type, string uid, string fileName)
        {
            var storageRef = FirebaseStorage.DefaultInstance.GetReferenceFromUrl("gs://metaconvention.appspot.com");
            var fileRef = storageRef.Child(type + "/" + uid + "/" + fileName);

            Task<Uri> task = fileRef.GetDownloadUrlAsync();
            await task;

            if (task.Exception == null)
            {
                Debug.Log(type + " url 불러오기 성공");
                return task.Result;
            }
            else
            {
                Debug.LogError("Exception in Url loading: " + uid + ", " + fileName + " " + task.Exception);
                return null;
            }
        }

        public static async Task<Texture2D> GetTextureFromDatabaseWithUrl(string url)
        {
            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            // 비동기 요청 보내기
            var operation = request.SendWebRequest();

            // 요청이 완료될 때까지 대기
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // 요청 결과 확인
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
                return null;
            }

            return ((DownloadHandlerTexture)request.downloadHandler).texture;
        }

        public static async Task<string> GetObjectFileLocalPathFromDatabaseWithUrl(string url, string uid, string fileName)
        {
            // 로컬 파일 저장 경로 설정
            string localPath = Path.Combine(Application.persistentDataPath, uid, fileName);

            // UnityWebRequest를 통해 URL에서 파일을 다운로드
            using UnityWebRequest request = new UnityWebRequest(url);
            request.downloadHandler = new DownloadHandlerFile(localPath);

            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            // 요청 결과 확인
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}");
                return null;
            }

            return localPath;
        }
    }

}
