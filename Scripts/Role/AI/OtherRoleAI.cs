using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherRoleAI : IRoleAI
{
    /// <summary>
    /// 当前角色
    /// </summary>
    public RoleCtrl currentRole
    {
        get;
        set;
    }

    /// <summary>
    /// 目标点
    /// </summary>
    private Vector3 m_TargetPos;
    /// <summary>
    /// 服务器时间
    /// </summary>
    private long m_ServerTime;
    /// <summary>
    /// 到达目标点需要的时间
    /// </summary>
    private int m_NeedTime;

    public OtherRoleAI(RoleCtrl roleCtrl)
    {
        currentRole = roleCtrl;
    }

    public void DoAI()
    {
        return;
    }
    /// <summary>
    /// 角色移动
    /// </summary>
    /// <param name="targetPos">目标点</param>
    /// <param name="serverTime">服务器时间</param>
    /// <param name="needTime">移动需要的时间</param>
    public void MoveTo(Vector3 targetPos, long serverTime, int needTime)
    {
        m_TargetPos = targetPos;
        m_ServerTime = serverTime;
        m_NeedTime = needTime;
        currentRole.Seeker.StartPath(currentRole.transform.position, targetPos, p => OnAStarFinish(p));
    }

    /// <summary>
    /// A星算法计算移动的理论时间和实际时间
    /// </summary>
    /// <param name="p">寻路路径</param>
    private void OnAStarFinish(Path p)
    {
        //获取路径长
        float pathLen = GameUtil.GetPathLen(p.vectorPath);

        //获取本次移动与服务器进行交互的时间差=当前服务器时间-协议发过来的服务器时间
        long delayTime = GlobalInit.Instance.GetCurrentServerTime() - m_ServerTime;
        //其他玩家看到的移动时间=移动总时间-延迟时间
        long realMoveTime = m_NeedTime - delayTime;
        if (realMoveTime <= 0)
        { realMoveTime = 100; };
        //获取经修正后玩家真实的移动速度
        currentRole.ModifySpeed = pathLen / (realMoveTime * 0.001f);
        currentRole.MoveTo(m_TargetPos);
    }
}
