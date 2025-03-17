using System;
using UnityEngine;

public class Manager : MonoBehaviour
{
  private static ResourcesManager resourcesManager;
  private static LuaManager luaManager;
  public  static ResourcesManager ResourcesManager
  {
    get
    {
      return resourcesManager;
    }
  }
  public  static LuaManager LuaManager
  {
    get
    {
      return luaManager;
    }
  }

  private static LuaUIManager luaUIManager;
  public  static LuaUIManager LuaUIManager
  {
    get
    {
      return luaUIManager;
    }
  }
  
  private void Awake()
  {
    resourcesManager = this.gameObject.AddComponent<ResourcesManager>();
    luaManager = this.gameObject.AddComponent<LuaManager>();
    luaUIManager = this.gameObject.AddComponent<LuaUIManager>();
  }
}
