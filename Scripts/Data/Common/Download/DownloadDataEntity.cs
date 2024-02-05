using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据下载实体
/// </summary>
public class DownloadDataEntity 
{
    /// <summary>
    /// 数据的DownLoad路径（指在平台之后的全路径如：windows的就是windows后（eg：download/xxx）
    /// </summary>
    public string FullName;
    /// <summary>
    /// 数据的MD5加密形态
    /// </summary>
    public string MD5;
    /// <summary>
    /// 数据的大小(k)
    /// </summary>
    public int Size;
    /// <summary>
    /// 是否是游戏初始数据（即开始游戏所必须的数据）
    /// </summary>
    public bool IsFirstData;
    
}
