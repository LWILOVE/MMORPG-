using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour����չ��
/// </summary>
public static class MomoUtil
{


    /// <summary>
    /// �����ÿ���
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
    /// �ƶ���������ÿ���
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
    /// ����ͼֽ�����ÿ���
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
