using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������ʵ��
/// </summary>
public class DownloadDataEntity 
{
    /// <summary>
    /// ���ݵ�DownLoad·����ָ��ƽ̨֮���ȫ·���磺windows�ľ���windows��eg��download/xxx��
    /// </summary>
    public string FullName;
    /// <summary>
    /// ���ݵ�MD5������̬
    /// </summary>
    public string MD5;
    /// <summary>
    /// ���ݵĴ�С(k)
    /// </summary>
    public int Size;
    /// <summary>
    /// �Ƿ�����Ϸ��ʼ���ݣ�����ʼ��Ϸ����������ݣ�
    /// </summary>
    public bool IsFirstData;
    
}
