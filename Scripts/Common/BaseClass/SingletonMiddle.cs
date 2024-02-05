using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例中心能够为每个继承它的子类创建一个单例（即子类统一单例化）
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonMiddle<T> : IDisposable where T : new()
{
    private static T _Instance;
    public static T Instance
    {
        get 
        {
            if (_Instance == null)
            {
                _Instance = new T();
            }
            return _Instance;
        }
    }

    public virtual void Dispose()
    {
        
    }
}
