using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 系统控制器基类：里面有一些比较常用的普通类
/// 当前有：
/// ShowMessage,AddEventListener,RemoveEventListener,Log,LogError,PlayerCtrl
/// </summary>
public class SystemCtrlBase<T> : IDisposable where T : new()  
{
    private static T _Instance;
    public static T Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new T();
            }
            return _Instance;
        }
    }

    public virtual void Dispose()
    {

    }

    /// <summary>
    /// 显示窗口
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <param name="type">类型</param>
    /// <param name="okAction">确定回调</param>
    /// <param name="cancelAction">取消回调</param>
    protected void ShowMessage(string title, string message, MessageViewType type = MessageViewType.Ok, DelegateDefine.OnMessageShow okAction = null, DelegateDefine.OnMessageCancel cancelAction = null)
    {
        MessageCtrl.Instance.Show(title,message,type,okAction,onCancel: cancelAction);
    }
    
    /// <summary>
    /// 添加监听
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    protected void AddEventListener(string key,UIDispatcher.OnActionHandler handler)
    {
        UIDispatcher.Instance.AddEventListener(key,handler);
    }

    /// <summary>
    /// 移除监听
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    protected void RemoveEventListener(string key, UIDispatcher.OnActionHandler handler)
    {
        UIDispatcher.Instance.RemoveEventListener(key,handler);
    }
}
