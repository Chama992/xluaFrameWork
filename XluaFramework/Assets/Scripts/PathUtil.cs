
using UnityEngine;

public class PathUtil
{
    
    public static readonly string AssetsPath = Application.dataPath;
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";
    public static string BundleOutPath = Application.streamingAssetsPath;

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
}
