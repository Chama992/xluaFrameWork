using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourcesManager : MonoBehaviour
{
    internal class BundleInfo
    {
        public string AssetName; //unity 相对路径
        public string BundleName;//名称 
        public List<string> Dependencies;//依赖组
    
    }
    private Dictionary<string, BundleInfo> Bundles = new Dictionary<string, BundleInfo>();


    private void Awake()
    {
        
    }
    

    /// <summary>
    /// 获取filelist中定义的所有资源，将其存入到Bundles列表中
    /// </summary>
    public void ParseVersionFile()
    {
        string url = Path.Combine(PathUtil.BundleResourePath ,AppConst.FileListName);
        string[] data = File.ReadAllLines(url);
        for (int i = 0; i < data.Length; i++)
        {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');
            bundleInfo.AssetName = info[0];
            bundleInfo.BundleName = info[1];
            bundleInfo.Dependencies = new List<string>(info.Length - 2);
            for (int j = 2; j < info.Length; j++)
            {
                bundleInfo.Dependencies.Add(info[j]);
            }
            Bundles.Add(bundleInfo.AssetName, bundleInfo);
            if (info[0].Contains("LuaScripts"))
            {
                Manager.LuaManager.LuaNames.Add(info[0]);
            }
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="bundleAssetName"></param>
    /// <param name="callback"></param>
    public void LoadAssetsAsync(string bundleAssetName,Action<Object> callback)
    {
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
        {
            EditorLoadAsset(bundleAssetName,callback);
            return;
        }
#endif
        StartCoroutine(LoadBundlesAsync(bundleAssetName, callback));
    }

    /// <summary>
    /// 协程 异步加载资源
    /// </summary>
    /// <param name="bundleAssetName"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator LoadBundlesAsync(string bundleAssetName,Action<Object> callback = null)
    {
        string bundleName = Bundles[bundleAssetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourePath,bundleName);
        bundlePath = PathUtil.GetStandardPath(bundlePath);
        List<string> dependencies= Bundles[bundleAssetName].Dependencies;
        if (dependencies != null && dependencies.Count > 0)
        {
            foreach (var t in dependencies)
            {
                yield return LoadBundlesAsync(t);//递归加载所有依赖
            }
        }
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        if (request.assetBundle == null)
        {
            Debug.Log("无法加载资源");
            yield break;
        }
        yield return request;
        AssetBundleRequest assetBundleRequest = request.assetBundle.LoadAssetAsync(bundleAssetName);
        yield return assetBundleRequest;
        callback?.Invoke(assetBundleRequest.asset);
    }

#if UNITY_EDITOR
    

    void EditorLoadAsset(string assetName, Action<Object> callback = null)
    {
        Object obj = AssetDatabase.LoadAssetAtPath(assetName, typeof(Object));
        if (obj == null)
        {
            Debug.Log("无法加载资源");            
        }
        callback?.Invoke(obj);
    }
#endif

    public void LoadUI(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(PathUtil.GetUIPath(name),callback);
    }
    
    public void LoadMusic(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(PathUtil.GetMusicPath(name),callback);
    }
    public void LoadSound(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(PathUtil.GetSoundPath(name),callback);
    }
    public void LoadEffect(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(PathUtil.GetEffectPath(name),callback);
    }
    public void LoadLua(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(name,callback);
    }
    public void LoadScene(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(PathUtil.GetScenePath(name),callback);
    }
}

