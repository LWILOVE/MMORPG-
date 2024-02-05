    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������ʵ����
/// </summary>
public class AssetBundleEntity 
{
    /// <summary>
    /// ���Ψһ���
    /// </summary>
    public string Key;
    /// <summary>
    /// ����
    /// </summary>
    public string Name;
    /// <summary>
    /// ���
    /// </summary>
    public string Tag;

    /// <summary>
    /// �Ƿ����ļ���
    /// </summary>
    public bool IsFolder;

    /// <summary>
    /// �Ƿ��ǳ�������
    /// </summary>
    public bool IsFirstData;

    /// <summary>
    /// �Ƿ�ѡ��
    /// </summary>
    public bool IsChecked;
    private List<string> m_PathList = new List<string>();
    /// <summary>
    /// ��Ʒȫ·������
    /// </summary>
    public List<string> PathList
    {
        get { return m_PathList; }
    }
}
