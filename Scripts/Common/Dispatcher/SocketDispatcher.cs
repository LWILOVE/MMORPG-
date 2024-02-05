using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// ����Э��۲���
/// </summary>
[LuaCallCSharp]
public class SocketDispatcher : IDisposable 
{

    #region ����
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
    /// �¼�ί��ԭ��
    /// </summary>
    /// <param name="param"></param>
    [CSharpCallLua]
    public delegate void OnActionHandler(byte[] buffer);

    public Dictionary<ushort, List<OnActionHandler>> dic = new Dictionary<ushort, List<OnActionHandler>>();

    #region AddEventListener �¼�ί���ֵ�����
    /// <summary> 
    /// �¼�ί���ֵ�����
    /// </summary>
    /// <param name="key">��ť��</param>
    /// <param name="handler">�����ί��</param>
    public void AddEventListener(ushort key, OnActionHandler handler)
    {
        //����ί������Ѵ��ڣ�����ӣ������½�һ��ί����������
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

    #region RemoveEventListener �¼�ί���ֵ���Ƴ�
    /// <summary>
    /// �¼�ί���ֵ���Ƴ�
    /// </summary>
    /// <param name="key">��ť��</param>
    /// <param name="handler">��ɾί��</param>
    public void RemoveEventListener(ushort key, OnActionHandler handler)
    {
        //����ί�б��Ƴ��󣬸�ί������Ծɴ���ί�У������������Ƴ���ί�����
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

    #region Dispatch �¼�ί���ֵ���ɷ�
    /// <summary>
    /// �¼�ί���ֵ���ɷ�
    /// </summary>
    /// <param name="key">��ť��</param>
    /// <param name="p">���ɷ�ί�еĲ���</param>
    public void Dispatch(ushort key,byte[] buffer)
    {
        //����ί���������Ҵ���ί��ʱ������Щ��Ϣ
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
