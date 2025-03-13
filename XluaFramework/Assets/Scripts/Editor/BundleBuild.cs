using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    [MenuItem("Build Bundles/iOS")]
    static void BuildiOS()
    {
        Build(BuildTarget.iOS);
    }

    static void Build(BuildTarget buildTarget)
    {
        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath, "*", SearchOption.AllDirectories);
        
        List<string> buildInfos = new List<string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".meta"))//不打包meta文件
            {
                continue;
            }
            Debug.Log(files[i]);
            AssetBundleBuild assetBundle = new AssetBundleBuild();
            
            string fileName = PathUtil.GetStandardPath(files[i]);//获取完整路径
            string assetName = PathUtil.GetUnityPath(fileName);//获取unity下的相对路径 ex:asset/....
            assetBundle.assetNames = new []{assetName}; //这个组里包含的文件名均会被打包到同一个ab包里
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath, "").ToLower(); // ui/emample
            assetBundle.assetBundleName = bundleName + ".ab";//ab包的名字 ex: ui/emample.ab
            assetBundleBuilds.Add(assetBundle);
            //放入依赖中
            List<string> dependencies = GetFileDependency(assetName);
            string buildInfo = assetName + "|" + bundleName + ".ab";
            if (dependencies.Count > 0 )
            {
                buildInfo = buildInfo + "|" + string.Join("|", dependencies);
            }
            buildInfos.Add(buildInfo);
        }
        
        if (Directory.Exists(PathUtil.BundleOutPath))
        {
            Directory.Delete(PathUtil.BundleOutPath, true);
        }
        Directory.CreateDirectory(PathUtil.BundleOutPath);
        File.WriteAllLines(PathUtil.BundleOutPath+ "/" + AppConst.FileListName,buildInfos);
        BuildPipeline.BuildAssetBundles(PathUtil.BundleOutPath,assetBundleBuilds.ToArray(), BuildAssetBundleOptions.None,buildTarget);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 获取文件依赖
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static List<string> GetFileDependency(string fileName)
    {
        List<string> dependencies = new List<string>();
        string[] files = AssetDatabase.GetDependencies(fileName);
        dependencies = files.Where( file => !file.EndsWith(".cs") && !file.Equals((fileName))).ToList();
        return dependencies;
    }
}
