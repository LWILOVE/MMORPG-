using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ϵͳ���������ࣺ������һЩ�Ƚϳ��õ���ͨ��
/// ��ǰ�У�
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
    /// ��ʾ����
    /// </summary>
    /// <param name="title">����</param>
    /// <param name="message">����</param>
    /// <param name="type">����</param>
    /// <param name="okAction">ȷ���ص�</param>
    /// <param name="cancelAction">ȡ���ص�</param>
    protected void ShowMessage(string title, string message, MessageViewType type = MessageViewType.Ok, DelegateDefine.OnMessageShow okAction = null, DelegateDefine.OnMessageCancel cancelAction = null)
    {
        MessageCtrl.Instance.Show(title,message,type,okAction,onCancel: cancelAction);
    }
    
    /// <summary>
    /// ��Ӽ���
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    protected void AddEventListener(string key,UIDispatcher.OnActionHandler handler)
    {
        UIDispatcher.Instance.AddEventListener(key,handler);
    }

    /// <summary>
    /// �Ƴ�����
    /// </summary>
    /// <param name="key"></param>
    /// <param name="handler"></param>
    protected void RemoveEventListener(string key, UIDispatcher.OnActionHandler handler)
    {
        UIDispatcher.Instance.RemoveEventListener(key,handler);
    }
}
