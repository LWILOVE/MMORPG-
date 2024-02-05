using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据统计类
/// </summary>
public class Stat 
{
    //1.初始化
    public static void Init()
    {
        //第三方平台的APPid

    }
    /// <summary>
    /// 注册统计
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="nickName"></param>
    public static void Reg(int userId,string nickName)
    {
        
    }

    /// <summary>
    /// 登录统计
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="nickName"></param>
    public static void LogOn(int userId, string nickName)
    { }

    /// <summary>
    /// 名称修改统计
    /// </summary>
    /// <param name="nickName"></param>
    public static void ChangeNickName(string nickName)
    { }

    /// <summary>
    /// 升级统计
    /// </summary>
    /// <param name="level"></param>
    public static void UpLevel(int level)
    { }

    /// <summary>
    /// 任务开始统计
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskName"></param>
    public static void TaskBegin(int taskId,string taskName)
    { }

    /// <summary>
    /// 任务结束统计
    /// </summary>
    /// <param name="taskId"></param>
    /// <param name="taskName"></param>
    /// <param name="status"></param>
    public static void TaskEnd(int taskId, string taskName, int status)
    { }

        /// <summary>
    /// 关卡开始统计
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="gameLevelName"></param>
    public static void GameLevelBegin(int gameLevelId,string gameLevelName)
    { }

    /// <summary>
    /// 关卡结束统计
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="gameLevelName"></param>
    /// <param name="status"></param>
    /// <param name="star">关卡评级</param>
    public static void GameLevelEnd(int gameLevelId, string gameLevelName, int status,int star)
    { }

    /// <summary>
    /// 充值开始统计
    /// </summary>
    /// <param name="orderId">订单号</param>
    /// <param name="productId">产品编号</param>
    /// <param name="money">充值金额</param>
    /// <param name="type">金钱类型</param>
    /// <param name="virtualMoney">虚拟货币获取量</param>
    /// <param name="channelId">渠道号</param>
    public static void ChargeBegin(string orderId,string productId,double money,string type,double virtualMoney,string channelId)
    { }

    /// <summary>
    /// 充值完成统计
    /// </summary>
    public static void ChargeEnd()
    { }

    /// <summary>
    /// 道具购买统计
    /// </summary>
    /// <param name="itemId">道具编号</param>
    /// <param name="itemName">名称</param>
    /// <param name="price">价格</param>
    /// <param name="count">数量</param>
    public static void BuyItem(int itemId, string itemName, int price,int count)
    { }

    /// <summary>
    /// 道具消耗统计
    /// </summary>
    /// <param name="itemId">道具编号</param>
    /// <param name="itemName">名称</param>
    /// <param name="count">数量</param>
    /// <param name="usedType">用途</param>
    public static void ItemUsed(int itemId, string itemName, int count,int usedType)
    { }

    /// <summary>
    /// 自定义事件
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public static void AddEvent(string key, string value)
    { }
}

