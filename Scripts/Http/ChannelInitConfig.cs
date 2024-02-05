using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 渠道信息类
/// </summary>
public class ChannelInitConfig
{
    /// <summary>
    /// 账号服务器时间
    /// </summary>
    public long ServerTime;

    /// <summary>
    /// 资源地址
    /// </summary>
    public string SourceUrl;

    /// <summary>
    /// 充值回调地址
    /// </summary>
    public string RechargeUrl;

    /// <summary>
    /// TD统计账号
    /// </summary>
    public string TDAppId;

    /// <summary>
    /// 是否开启TD统计
    /// </summary>
    public bool IsOpenTD;

    /// <summary>
    /// 充值服务器识别码
    /// </summary>
    public int PayServerNo;
}
