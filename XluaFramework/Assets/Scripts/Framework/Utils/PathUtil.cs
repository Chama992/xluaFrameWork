
using UnityEngine;

public class PathUtil
{
    
    public static readonly string AssetsPath = Application.dataPath;
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";
    public static string BundleOutPath = Application.streamingAssetsPath;
    public static string ReadPath = Application.streamingAssetsPath;
    public static string ReadWritePath = Application.persistentDataPath;
    public static string LuaPath  = "Assets/BuildResources/LuaScripts";
    
    
    public static string BundleResourePath
    {
        get
        {
            if (AppConst.GameMode == GameMode.UpdateMode)
            {
                return ReadWritePath;
            }
            return ReadPath;
        }
    }

    public static string GetUnityPath(string file)
    {
        if (string.IsNullOrEmpty(file))
        {
            return string.Empty;
        }
        return file.Substring(file.IndexOf("Assets"));
    }

    public static string GetStandardPath(string file)
    {
        if (string.IsNullOrEmpty(file))
        {
            return string.Empty;
        }
        return file.Trim().Replace("\\","/");
    }

    public static string GetLuaPath(string file)
    {
        return string.Format("Assets/BuildResources/LuaScripts/{0}.bytes", file);
    }
    public static string GetUIPath(string file)
    {
        return string.Format("Assets/BuildResources/UI/Prefabs/{0}.prefab", file);
    }
    public static string GetMusicPath(string file)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", file);
    }
    public static string GetSoundPath(string file)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}.bytes", file);
    }
    public static string GetEffectPath(string file)
    {
        return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", file);
    }
    public static string GetSpritesPath(string file)
    {
        return string.Format("Assets/BuildResources/Sprites/{0}", file);
    }
    public static string GetScenePath(string file)
    {
        return string.Format("Assets/BuildResources/Scene/{0}.unity", file);
    }
    
}
