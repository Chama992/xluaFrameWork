using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XLua;


public class LuaManager : MonoBehaviour
{
    public List<string> LuaNames = new List<string>();
    public Dictionary<string,byte[]> LuaScripts;
    public LuaEnv LuaEnv;
    public Action InitFinished;
    private void Update()
    {
        if (LuaEnv != null)//释放资源
        {
            LuaEnv.Tick();
        }
        
    }

    private void OnDisable()
    {
        InitFinished = null;
    }

    public void Init( Action OnLuaInit)
    {
        InitFinished += OnLuaInit;
        LuaScripts = new Dictionary<string, byte[]>();
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
#if UNITY_EDITOR
        if (AppConst.GameMode == GameMode.EditorMode)
        {
            EditorLoadLuaScripts();
            return;
        }
        else
#endif     
            LoadLuaScripts();
    }
    /// <summary>
    /// lua调用
    /// </summary>
    /// <param name="name"></param>
    public void StartLua(string name)
    {
        LuaEnv.DoString($"require '{name}'");
    }
    /// <summary>
    /// 用于获取lua脚本内容
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private byte[] Loader(ref string name)
    {
        return GetLuaScripts(name);
    }

    /// <summary>
    /// 获取lua脚本内容
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public byte[] GetLuaScripts(string name)
    {
        name = name.Replace(".", "/");
        string fileName = PathUtil.GetLuaPath(name);
        byte[] luaScripts = null;
        if (!LuaScripts.TryGetValue(fileName, out luaScripts))
        {
            Debug.Log("lua script is not exist" + fileName);
        }
        return luaScripts;
    }

    void LoadLuaScripts()
    {
        foreach (var name in LuaNames)
        {
            Manager.ResourcesManager.LoadLua(name, (UnityEngine.Object o) =>
            {
                AddLuaScripts(name, (o as TextAsset).bytes);
                if (LuaScripts.Count >= LuaNames.Count)
                {
                    InitFinished?.Invoke();
                    LuaNames.Clear();
                    LuaNames = null;
                }
            });
        }
    }
#if UNITY_EDITOR
    void EditorLoadLuaScripts()
    {
        string[] luaFiles = Directory.GetFiles(PathUtil.LuaPath, "*.bytes", SearchOption.AllDirectories);
        for (int i = 0; i < luaFiles.Length; i++)
        {
            string fileName = PathUtil.GetStandardPath(luaFiles[i]);
            byte[] file = File.ReadAllBytes(fileName);
            AddLuaScripts(PathUtil.GetUnityPath(fileName), file);
        }
        InitFinished?.Invoke();
    }
#endif
    
    private void AddLuaScripts(string assetName, byte[] luaScript)
    {
        LuaScripts[assetName] = luaScript;
    }
    
}
