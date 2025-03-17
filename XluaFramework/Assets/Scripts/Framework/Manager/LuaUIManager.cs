using System;
using System.Collections.Generic;
using UnityEngine;


public class LuaUIManager : MonoBehaviour
{
    Dictionary<string,GameObject> uiObjects = new Dictionary<string, GameObject>();
    Dictionary<string,Transform> UIGroups = new Dictionary<string, Transform>();
    private Transform UIParent;
    private void Awake()
    {
        UIParent = transform.parent.Find("UI");
    }
    
    public void SetUIGroup(List<string> group)
    {
        for (int i = 0; i < group.Count; i++)
        {
            GameObject go = new GameObject("Group-" + group[i]);
            go.transform.SetParent(UIParent, false);
            UIGroups.Add(group[i], go.transform);
        }
    }
    
    public Transform GetUIGroup(string name)
    {
        Transform ui = null;
        return UIGroups.TryGetValue(name, out ui) ? ui : null;
    }

    public void OpenUI(string uiName,string group,string luaName)
    {
        GameObject ui = null;
        if (uiObjects.TryGetValue(uiName,out ui))
        {
            LuaUIBehaviour luaUIBehaviour = ui.GetComponent<LuaUIBehaviour>();
            luaUIBehaviour.OnOpen();
            return;
        }
        Manager.ResourcesManager.LoadUI(uiName, (UnityEngine.Object o) =>
        {
            GameObject ui = Instantiate(o,this.transform) as GameObject;
            uiObjects.Add(uiName,ui);
            Transform parent = GetUIGroup(group);
            ui.transform.SetParent(parent, false);
            LuaUIBehaviour luaUIBehaviour = ui.AddComponent<LuaUIBehaviour>();
            luaUIBehaviour.InitLua(luaName);
            luaUIBehaviour.OnOpen();
        });
    }
}
