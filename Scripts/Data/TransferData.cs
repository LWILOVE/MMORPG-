 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于用户数据传输的普通类
/// </summary>
[XLua.LuaCallCSharp]
public class TransferData
{
    public TransferData()
    {
        m_PutValues = new Dictionary<string, object>();
    }

    #region m_PutValues 数据字典
    /// <summary>
    /// 数据字典
    /// </summary>
    private Dictionary<string, object> m_PutValues;

    public Dictionary<string, object> PutValues
    {
        get { return m_PutValues; }
    }
    #endregion
    /// <summary>
    /// 存值
    /// </summary>
    /// <typeparam name="TM"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetValue<TM>(string key, TM value)
    {
        PutValues[key] = value;
    }

    /// <summary>
    /// 取值
    /// </summary>
    /// <typeparam name="TM">目标类型</typeparam>
    /// <param name="key">目标键</param>
    /// <returns></returns>
    public TM GetValue<TM>(string key)
    {
        if(PutValues.ContainsKey(key))
        {
            return (TM)PutValues[key];
        }
        return default(TM);
    }

    /// <summary>
    /// 取值
    /// </summary>
    /// <param name="key">目标键</param>
    /// <returns></returns>
    public object GetValue(string key)
    {
        if (PutValues.ContainsKey(key))
        {
            return PutValues[key];
        }
        return null;
    }
}
