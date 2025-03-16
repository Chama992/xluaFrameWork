using System;
using UnityEngine;


public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this.gameObject);
        HotUpdateManager.ResourcesManager.ParseVersionFile();
        HotUpdateManager.LuaManager.Init(() =>
        {
            HotUpdateManager.LuaManager.StartLua("main");
        });
        
    }
}
