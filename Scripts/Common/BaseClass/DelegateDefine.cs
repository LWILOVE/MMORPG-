using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// �����������ί��
/// </summary>
public class DelegateDefine:SingletonMiddle<DelegateDefine>
{
    
    /// <summary>
    /// �����������ί��
    /// </summary>
    public Action OnSceneLoadOk;

    /// <summary>
    /// ������ʼ�����
    /// </summary>
    public Action ChannelInitOK;

    /// <summary>
    /// ��Ϣ
    /// </summary>
    public delegate void OnMessageShow();

    public delegate void OnMessageOK();

    public delegate void OnMessageCancel();

    /// <summary>
    /// ������ʾ
    /// </summary>
    [CSharpCallLua]
    public delegate void OnViewShow();

    /// <summary>
    /// �������ػ���
    /// </summary>
    [CSharpCallLua]
    public delegate void OnViewHide();

    /// <summary>
    /// ���OK��ť
    /// </summary>
    [CSharpCallLua]
    public delegate void OnOK();

    /// <summary>
    /// ���ȡ����ť
    /// </summary>
    [CSharpCallLua]
    public delegate void OnCancel();

    /// <summary>
    /// ���ȷ��ѡ��������ť
    /// </summary>
    /// <param name="value"></param>
    [CSharpCallLua]
    public delegate void OnChooseCount(int value);
}
