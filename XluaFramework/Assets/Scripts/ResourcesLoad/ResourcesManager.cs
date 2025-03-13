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
        public string AssetName;
        public string BundleName;
        public List<string> Dependencies;
    
    }
    private Dictionary<string, BundleInfo> Bundles = new Dictionary<string, BundleInfo>();

    public GameMode GameMode;
    private void Awake()
    {
        AppConst.GameMode = this.GameMode;
    }


    private void Start()
    {
        ParseVersionFile();
        LoadUI("Canvas",OnLoadComplete);
    }

    private void OnLoadComplete(Object obj)
    {
        GameObject GGO = (GameObject)GameObject.Instantiate(obj);
            
    }
    private void ParseVersionFile()
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
        }
    }

    public void LoadAssetsAsync(string bundleAssetName,Action<Object> callback)
    {
        if (AppConst.GameMode == GameMode.EditorMode)
        {
            EditorLoadAsset(bundleAssetName,callback);
        }
        StartCoroutine(LoadBundlesAsync(bundleAssetName, callback));
    }

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
                yield return LoadBundlesAsync(t);
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

    void EditorLoadAsset(string assetName, Action<Object> callback = null)
    {
        Object obj = AssetDatabase.LoadAssetAtPath(assetName, typeof(Object));
        if (obj == null)
        {
            Debug.Log("无法加载资源");            
        }
        callback?.Invoke(obj);
    }

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
        LoadAssetsAsync(PathUtil.GetLuaPath(name),callback);
    }
    public void LoadScene(string name, Action<Object> callback = null)
    {
        LoadAssetsAsync(PathUtil.GetScenePath(name),callback);
    }
}

