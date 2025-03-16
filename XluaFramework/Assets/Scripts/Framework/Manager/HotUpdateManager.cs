using System;
using UnityEngine;

public class HotUpdateManager : MonoBehaviour
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
  private void Awake()
  {
    resourcesManager = this.gameObject.AddComponent<ResourcesManager>();
    luaManager = this.gameObject.AddComponent<LuaManager>();
  }
}
