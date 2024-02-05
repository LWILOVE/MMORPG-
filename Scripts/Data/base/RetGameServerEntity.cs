using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区服实体类
/// </summary>
public class RetGameServerEntity
{
    /// <summary>
    /// 区服号
    /// </summary>
    public int Id;
    /// <summary>
    /// 区服运营状态
    /// </summary>
    public int RunStatus;
    /// <summary>
    /// 是否推荐该服
    /// </summary>
    public bool IsCommand;
    /// <summary>
    /// 是否是新服
    /// </summary>
    public bool IsNew;
    /// <summary>
    /// 区服名
    /// </summary>
    public string Name;
    /// <summary>
    /// 区服Ip
    /// </summary>
    public string Ip;
    /// <summary>
    /// 区服端口号
    /// </summary>
    public int Port;

}
