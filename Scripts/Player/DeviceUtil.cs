using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �豸��Ϣ��
/// </summary>
public class DeviceUtil 
{
    /// <summary>
    /// �ͻ��˵�Ψһʶ����
    /// </summary>
    public static string DeviceIdentifier
    {
        get
        {
            //��ȡ�豸��Ψһʶ����
            return SystemInfo.deviceUniqueIdentifier;
        }
    }

    /// <summary>
    /// �ͻ����豸�ͺ�
    /// </summary>
    public static string DeviceModel
    {
        get
        {

#if UNITY_IPHONE && !UNITY_EDITOR
            //������ϵͳΪƻ�����������ķ�ʽ
            return Device.generation.ToString();
#else
            //��ȡ���Ե��豸�ͺ�
            return SystemInfo.deviceModel;
#endif
        }
    }
}
