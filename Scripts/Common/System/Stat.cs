using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ͳ����
/// </summary>
public class Stat 
{
    //1.��ʼ��
    public static void Init()
    {
        //������ƽ̨��APPid

    }
    /// <summary>
    /// ע��ͳ��
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="nickName"></param>
    public static void Reg(int userId,string nickName)
    {
        
    }

    /// <summary>
    /// ��¼ͳ��
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="nickName"></param>
    public static void LogOn(int userId, string nickName)
    { }

    /// <summary>
    /// �����޸�ͳ��
    /// </summary>
    /// <param name="nickName"></param>
    public static void ChangeNickName(string nickName)
    { }

    /// <summary>
    /// ����ͳ��
    /// </summary>
    /// <param name="level"></param>
    public static void UpLevel(int level)
    { }

    /// <summary>
    /// ����ʼͳ��
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskName"></param>
    public static void TaskBegin(int taskId,string taskName)
    { }

    /// <summary>
    /// �������ͳ��
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskName"></param>
    /// <param name="status"></param>
    public static void TaskEnd(int taskId, string taskName, int status)
    { }

        /// <summary>
    /// �ؿ���ʼͳ��
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="gameLevelName"></param>
    public static void GameLevelBegin(int gameLevelId,string gameLevelName)
    { }

    /// <summary>
    /// �ؿ�����ͳ��
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="gameLevelName"></param>
    /// <param name="status"></param>
    /// <param name="star">�ؿ�����</param>
    public static void GameLevelEnd(int gameLevelId, string gameLevelName, int status,int star)
    { }

    /// <summary>
    /// ��ֵ��ʼͳ��
    /// </summary>
    /// <param name="orderId">������</param>
    /// <param name="productId">��Ʒ���</param>
    /// <param name="money">��ֵ���</param>
    /// <param name="type">��Ǯ����</param>
    /// <param name="virtualMoney">������һ�ȡ��</param>
    /// <param name="channelId">������</param>
    public static void ChargeBegin(string orderId,string productId,double money,string type,double virtualMoney,string channelId)
    { }

    /// <summary>
    /// ��ֵ���ͳ��
    /// </summary>
    public static void ChargeEnd()
    { }

    /// <summary>
    /// ���߹���ͳ��
    /// </summary>
    /// <param name="itemId">���߱��</param>
    /// <param name="itemName">����</param>
    /// <param name="price">�۸�</param>
    /// <param name="count">����</param>
    public static void BuyItem(int itemId, string itemName, int price,int count)
    { }

    /// <summary>
    /// ��������ͳ��
    /// </summary>
    /// <param name="itemId">���߱��</param>
    /// <param name="itemName">����</param>
    /// <param name="count">����</param>
    /// <param name="usedType">��;</param>
    public static void ItemUsed(int itemId, string itemName, int count,int usedType)
    { }

    /// <summary>
    /// �Զ����¼�
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddEvent(string key, string value)
    { }
}

