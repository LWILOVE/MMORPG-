using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 设备信息类
/// </summary>
public class DeviceUtil 
{
    /// <summary>
    /// 客户端的唯一识别码
    /// </summary>
    public static string DeviceIdentifier
    {
        get
        {
            //获取设备的唯一识别码
            return SystemInfo.deviceUniqueIdentifier;
        }
    }

    /// <summary>
    /// 客户端设备型号
    /// </summary>
    public static string DeviceModel
    {
        get
        {

#if UNITY_IPHONE && !UNITY_EDITOR
            //若操作系统为苹果则采用下面的方式
            return Device.generation.ToString();
#else
            //获取电脑的设备型号
            return SystemInfo.deviceModel;
#endif
        }
    }
}
