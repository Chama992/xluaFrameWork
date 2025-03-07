using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BundleBuild : Editor
{
    [MenuItem("Build Bundles/Windows")]
    static void BuildWindows()
    {
        Build(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Build Bundles/Android")]
    static void BuildAndroid()
    {
        Build(BuildTarget.Android);
    }
    [MenuItem("Build Bundles/Ios")]
    static void BuildiOS()
    {
        Build(BuildTarget.iOS);
    }

    static void Build(BuildTarget buildTarget)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))
            {
                continue;
            }
            Debug.Log(files[i]);
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            
            string fileName = PathUtil.GetStandardPath(files[i]);//获取完整路径
            string assetName = PathUtil.GetUnityPath(fileName);//获取unity下的相对路径
            assetBundle.assetNames = new []{assetName}; //这个组里包含的文件名均会被打包到同一个ab包里
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower();
            assetBundle.assetBundleName = bundleName + ".ab";//ab包的名字
            assetBundleBuilds.Add(assetBundle);
        }

        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }

        Directory.CreateDirectory(PathUtil.BundleOutPath);
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath,assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None,buildTarget);
        
    }
}
