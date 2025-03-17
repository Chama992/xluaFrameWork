using System;
using UnityEngine;

public class LuaUIBehaviour : LuaBehaviour
{
    private Action LuaOnOpen;
    private Action LuaOnClose;
    public override void InitLua(string luaName)
    {
        base.InitLua(luaName);
        ScriptEnv.Get("OnOpen",out LuaOnOpen);
        ScriptEnv.Get("OnClose",out LuaOnClose);
    }

    public void OnOpen()
    {
        LuaOnOpen?.Invoke();
    }

    public void OnClose()
    {
        LuaOnClose?.Invoke();
    }

    protected override void Clear()
    {
        base.Clear();
        LuaOnOpen = null;
        LuaOnClose = null;
    }
}
