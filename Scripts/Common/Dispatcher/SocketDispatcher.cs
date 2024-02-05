using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// 网络协议观察者
/// </summary>
[LuaCallCSharp]
public class SocketDispatcher : IDisposable 
{

    #region 单例
    private static SocketDispatcher _Instance;
    public static SocketDispatcher Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new SocketDispatcher();
            }
            return _Instance;
        }
    }

    public virtual void Dispose()
    {

    }
    #endregion


    /// <summary>
    /// 事件委托原型
    /// </summary>
    /// <param name="param"></param>
    [CSharpCallLua]
    public delegate void OnActionHandler(byte[] buffer);

    public Dictionary<ushort, List<OnActionHandler>> dic = new Dictionary<ushort, List<OnActionHandler>>();

    #region AddEventListener 事件委托字典的添加
    /// <summary> 
    /// 事件委托字典的添加
    /// </summary>
    /// <param name="key">按钮名</param>
    /// <param name="handler">待添加委托</param>
    public void AddEventListener(ushort key, OnActionHandler handler)
    {
        //若该委托类别已存在，则添加，否则新建一个委托类别再添加
        if (dic.ContainsKey(key))
        {
            dic[key].Add(handler);
        }
        else
        {
            List<OnActionHandler> list = new List<OnActionHandler>();
            list.Add(handler);
            dic[key] = list;
        }
    }
    #endregion

    #region RemoveEventListener 事件委托字典的移除
    /// <summary>
    /// 事件委托字典的移除
    /// </summary>
    /// <param name="key">按钮名</param>
    /// <param name="handler">待删委托</param>
    public void RemoveEventListener(ushort key, OnActionHandler handler)
    {
        //若该委托被移除后，该委托类别仍旧存在委托，则保留，否则移除该委托类别
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
    #endregion

    #region Dispatch 事件委托字典的派发
    /// <summary>
    /// 事件委托字典的派发
    /// </summary>
    /// <param name="key">按钮名</param>
    /// <param name="p">待派发委托的参数</param>
    public void Dispatch(ushort key,byte[] buffer)
    {
        //当该委托类别存在且存有委托时发布这些消息
        if (dic.ContainsKey(key))
        {
            List<OnActionHandler> listHandler = dic[key];
            if (listHandler != null && listHandler.Count > 0)
            {
                for (int i = 0; i < listHandler.Count; i++)
                {
                    if (listHandler[i] != null)
                    {
                        listHandler[i](buffer);
                    }
                }
            }
        }
    }

    public void Dispatch(ushort key)
    {
        Dispatch(key,null);
    }
    #endregion

}
