using System;
using UnityEngine;


public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    private void Start()
    {
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this.gameObject);
        Manager.ResourcesManager.ParseVersionFile();
        Manager.LuaManager.Init(() =>
        {
            Manager.LuaManager.StartLua("main");
        });
        
    }
}
