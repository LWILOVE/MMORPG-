using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour的扩展类
/// </summary>
public static class MomoUtil
{


    /// <summary>
    /// 数组置空类
    /// </summary>
    /// <param name="arr"></param>
    public static void SetNull(this MonoBehaviour[] arr)
    {
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
            arr = null;
        }
    }

    /// <summary>
    /// 移动组件数组置空类
    /// </summary>
    /// <param name="arr"></param>
    public static void SetNull(this Transform[] arr)
    {
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
            arr = null;
        }
    }

    /// <summary>
    /// 精灵图纸数组置空类
    /// </summary>
    /// <param name="arr"></param>
    public static void SetNull(this Sprite[] arr)
    {
        if (arr != null)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = null;
            }
            arr = null;
        }
    }


}
