using System;
using UnityEngine;
using XLua;


public class LuaBehaviour : MonoBehaviour
{
    private LuaEnv luaEnv = Manager.LuaManager.LuaEnv;
    protected LuaTable ScriptEnv;
    // private Action LuaAwake;
    // private Action LuaStart;
    private Action LuaInit;
    private Action LuaUpdate;
    private Action LuaOnDestroy;
    public string luaName;
    private void Awake()
    {
        ScriptEnv = luaEnv.NewTable();
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        ScriptEnv.SetMetaTable(meta);
        meta.Dispose();
        ScriptEnv.Set("self",this);
    }

    public virtual void InitLua(string luaName)
    {
        luaEnv.DoString(Manager.LuaManager.GetLuaScripts(luaName),luaName, ScriptEnv);
        
        // ScriptEnv.Get("Awake",out LuaAwake);
        ScriptEnv.Get("OnInit",out LuaInit);
        ScriptEnv.Get("OnUpdate",out LuaUpdate);
        ScriptEnv.Get("OnDestroy",out LuaOnDestroy);
        LuaInit?.Invoke();
        // LuaAwake?.Invoke();
    }

    private void Update()
    {
        LuaUpdate?.Invoke();
    }

    private void OnDestroy()
    {
        LuaOnDestroy?.Invoke();
        Clear();
    }

    private void OnApplicationQuit()
    {
        LuaOnDestroy?.Invoke();
        Clear();
    }
    

    protected virtual void Clear()
    {
        // LuaAwake = null;
        // LuaStart = null;
        LuaInit = null;
        LuaUpdate = null;
        LuaOnDestroy = null;
        ScriptEnv?.Dispose();
        ScriptEnv = null;
    }
}
