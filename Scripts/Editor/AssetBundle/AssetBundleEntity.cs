    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 打包管理器实体类
/// </summary>
public class AssetBundleEntity 
{
    /// <summary>
    /// 打包唯一编号
    /// </summary>
    public string Key;
    /// <summary>
    /// 名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 标记
    /// </summary>
    public string Tag;

    /// <summary>
    /// 是否是文件夹
    /// </summary>
    public bool IsFolder;

    /// <summary>
    /// 是否是初版数据
    /// </summary>
    public bool IsFirstData;

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsChecked;
    private List<string> m_PathList = new List<string>();
    /// <summary>
    /// 物品全路径集合
    /// </summary>
    public List<string> PathList
    {
        get { return m_PathList; }
    }
}
