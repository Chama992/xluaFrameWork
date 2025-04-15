using System;
using UnityEngine;
using XLua;

[XLua.CSharpCallLua]
public delegate int CustomCall2(int a,int b);
public class GameStart : MonoBehaviour
{
    public GameMode GameMode;

    public class  Person
    {
        public int age;
        public string name;
    }
    private void Start()
    {
        AppConst.GameMode = this.GameMode;
        DontDestroyOnLoad(this.gameObject);
        // Manager.ResourcesManager.ParseVersionFile();
        Manager.LuaManager.Init(() =>
        {
            Manager.LuaManager.LuaEnv.DoString(@"  
            testTable = 
            {
               age = 1,  
               name = 'test'             
            }
        ");
            Person p = Manager.LuaManager.LuaEnv.Global.Get<Person>("testTable");
            
            // LuaFunction func = Manager.LuaManager.LuaEnv.Global.Get<LuaFunction>("add");
            // int result = (int)func.Call(3, 4)[0];  
            Debug.Log(p.name);

            // Manager.LuaManager.StartLua("main");
        });
    }
}
