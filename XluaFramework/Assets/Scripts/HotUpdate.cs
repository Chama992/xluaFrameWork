using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;


public class HotUpdate : MonoBehaviour
{
    internal struct FileDataInfo
    {
        public string url;
        public  string fileName;
        public DownloadHandler fileData;
    }

    IEnumerator DownLoadFile(FileDataInfo fileInfo, Action<FileDataInfo> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(fileInfo.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(webRequest.error);
            yield break;
        }
        fileInfo.fileData = webRequest.downloadHandler;
        callback?.Invoke(fileInfo);
        webRequest.Dispose();
    }

    IEnumerator DownLoadFiles(List<FileDataInfo> fileDataInfos, Action<FileDataInfo> callback = null,
        Action finishedCallback = null)
    {
        foreach (var fileData in fileDataInfos)
        {
            yield return DownLoadFile(fileData, callback);
        }
        finishedCallback?.Invoke();
    }

    private List<FileDataInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<FileDataInfo> fileDataInfos = new List<FileDataInfo>();
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            FileDataInfo file = new FileDataInfo();
            file.fileName = info[1];
            file.url = Path.Combine(path,info[1]);
            fileDataInfos.Add(file);
        }
        return fileDataInfos;
    }
}
