using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;


public class HotUpdate : MonoBehaviour
{
    private byte[] ReadPathFileListData;//存入读取的本地filelist信息
    private byte[] ServerFileListData;//存入下载的服务器filelist信息
    internal class DownFileInfo
    {
        public string url;
        public  string fileName;
        public DownloadHandler fileData;
    }

    private void Start()
    {
        if (IsFirstInstall())//判断是否是第一次下载
        {
            ReleaseResources();//将只读文件放到可读写文件
        }
        else
        {
            CheckUpdate();//检查更新
        }
    }
    /// <summary>
    /// 判断是否是第一次下载，原理是如果是第一次下载，那么readpath中会有filelist文件 readwrite中会没有 
    /// </summary>
    /// <returns></returns>
    private bool IsFirstInstall()
    {
        bool isExistReadPath = FileUtil.FileExists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));
        bool isExistWritePath = FileUtil.FileExists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));
        return isExistReadPath && !isExistWritePath;
    }
    
    /// <summary>
    /// 将只读文件中的资源释放到可读写文件中
    /// </summary>
    private void ReleaseResources()
    {
        string url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);//filelist路径
        DownFileInfo info = new DownFileInfo(); 
        info.url = PathUtil.GetStandardPath(url);
        
        StartCoroutine(DownLoadFile(info, OnDownLoadReadPathFileListComplete));
    }

    /// <summary>
    /// 下载完成文件列表后
    /// </summary>
    /// <param name="downFile"></param>
    private void OnDownLoadReadPathFileListComplete(DownFileInfo downFile)
    {
        ReadPathFileListData = downFile.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(downFile.fileData.text, PathUtil.ReadPath);//获取文件列表指示的所有文件内容
        StartCoroutine(DownLoadFiles(fileInfos, OnReleaseFileComplete, OnReleaseAllFileComplete));//释放所有文件
    }

    private void OnReleaseAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath,AppConst.FileListName),ReadPathFileListData);
        CheckUpdate();
    }

    private void OnReleaseFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("OnReleaseFileComplete" + fileInfo.url );
        string writeFile = Path.Combine(PathUtil.ReadWritePath,fileInfo.fileName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);
    }
    /// <summary>
    /// 检查更新
    /// </summary>
    private void CheckUpdate()
    {
        string url = Path.Combine(AppConst.ResourceUrl, AppConst.FileListName);
        DownFileInfo info = new DownFileInfo();
        info.url = PathUtil.GetStandardPath(url);
        StartCoroutine(DownLoadFile(info, OnDownLoadServerFileListComplete));//获取服务器filelist
    }

    private void OnDownLoadServerFileListComplete(DownFileInfo file)
    {
        ServerFileListData = file.fileData.data;
        List<DownFileInfo> fileInfos = GetFileList(file.fileData.text,AppConst.FileListName);//获取服务器filelist指示的所有文件
        List<DownFileInfo> downFileInfos = new List<DownFileInfo>();
        for (int i = 0; i < fileInfos.Count; i++)
        {
            string localFile = Path.Combine(PathUtil.ReadWritePath,fileInfos[i].fileName);
            if (!FileUtil.FileExists(localFile)) //判断本地是否有
            {
                fileInfos[i].url = Path.Combine(AppConst.ResourceUrl,fileInfos[i].fileName);
                downFileInfos.Add(fileInfos[i]);//没有的话放到列表里
            }
        }

        if (downFileInfos.Count > 0)
        {
            StartCoroutine(DownLoadFiles(downFileInfos, OnUpdateFileComplete, OnUpdateAllFileComplete));//下载
        }
        else
        {
            EnterGame();
        }
    }
    private void OnUpdateAllFileComplete()
    {
        FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath,AppConst.FileListName),ServerFileListData);//把服务器端filelist更新到本地
        EnterGame();
    }

    private void OnUpdateFileComplete(DownFileInfo fileInfo)
    {
        Debug.Log("OnUpdateFileComplete" + fileInfo.url );
        string writeFile = Path.Combine(PathUtil.ReadWritePath,fileInfo.fileName);
        FileUtil.WriteFile(writeFile, fileInfo.fileData.data);
    }
    /// <summary>
    /// 游戏进入
    /// </summary>
    private void EnterGame()
    {
        Manager.ResourcesManager.ParseVersionFile();
        Manager.ResourcesManager.LoadUI("Canvas",OnLoadComplete);
    }
    
    private void OnLoadComplete(Object obj)
    {
        GameObject GGO = (GameObject)GameObject.Instantiate(obj);
            
    }
    IEnumerator DownLoadFile(DownFileInfo downFileInfo, Action<DownFileInfo> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get(downFileInfo.url);
        yield return webRequest.SendWebRequest();
        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(webRequest.error);
            yield break;
        }
        downFileInfo.fileData = webRequest.downloadHandler;
        callback?.Invoke(downFileInfo);
        webRequest.Dispose();
    }

    IEnumerator DownLoadFiles(List<DownFileInfo> fileDataInfos, Action<DownFileInfo> callback = null,
        Action finishedCallback = null)
    {
        foreach (var fileData in fileDataInfos)
        {
            yield return DownLoadFile(fileData, callback);
        }
        finishedCallback?.Invoke();
    }

    private List<DownFileInfo> GetFileList(string fileData, string path)
    {
        string content = fileData.Trim().Replace("\r", "");
        string[] files = content.Split('\n');
        List<DownFileInfo> fileDataInfos = new List<DownFileInfo>();
        for (int i = 0; i < files.Length; i++)
        {
            string[] info = files[i].Split('|');
            DownFileInfo downFile = new DownFileInfo();
            downFile.fileName = info[1];
            downFile.url =PathUtil.GetStandardPath(Path.Combine(path,info[1]));
            fileDataInfos.Add(downFile);
        }
        return fileDataInfos;
    }
}
