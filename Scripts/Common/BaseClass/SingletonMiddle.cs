using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������ܹ�Ϊÿ���̳��������ഴ��һ��������������ͳһ��������
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
