using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI°´Å¥µã»÷¹Û²ìÕß
/// </summary>
[XLua.LuaCallCSharp]
public class UIDispatcher : IDisposable
{
    private static UIDispatcher instance;

    public static UIDispatcher Instance
    {
        get 
        {
            if (instance == null)
            {
                instance = new UIDispatcher();
            }
            return instance;
        }
    }
    public void Dispose()
    {
    }

    [XLua.CSharpCallLua]
    public delegate void OnActionHandler(string[] param);
    public Dictionary<string, List<OnActionHandler>> dic = new Dictionary<string, List<OnActionHandler>>();

    /// <summary>
    /// Ìí¼Ó¼àÌý
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    public void AddEventListener(string key, OnActionHandler handler)
    {
        if (dic.ContainsKey(key))
        {
            dic[key].Add(handler);
        }
        else
        {
            List<OnActionHandler> listHandler = new List<OnActionHandler>();
            listHandler.Add(handler);
            dic[key] = listHandler;
        }
    }

    /// <summary>
    /// ÒÆ³ý¼àÌý
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener(string key, OnActionHandler handler)
    {
        if (dic.ContainsKey(key))
        {
            List<OnActionHandler> listHandler = dic[key];
            listHandler.Remove(handler);
            if (listHandler.Count == 0)
            {
                dic.Remove(key);
            }
        }
    }

    /// <summary>
    /// ÅÉ·¢¼àÌý
    /// </summary>
    /// <param name="key"></param>
    /// <param name="param"></param>
    public void Dispatch(string key, string[] param)
    {
        if(dic.ContainsKey(key))
        {
            List<OnActionHandler> listHandler = dic[key];
            if (listHandler != null && listHandler.Count > 0)
            {
                for (int i = 0; i < listHandler.Count; i++)
                {
                    if (listHandler[i] != null)
                    {
                        listHandler[i](param);
                    }
                }
            }
        }
    }

    public void Dispatch(string key)
    {
        Dispatch(key,null);
    }
}
