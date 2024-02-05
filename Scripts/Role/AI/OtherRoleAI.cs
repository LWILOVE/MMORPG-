using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherRoleAI : IRoleAI
{
    /// <summary>
    /// ��ǰ��ɫ
    /// </summary>
    public RoleCtrl currentRole
    {
        get;
        set;
    }

    /// <summary>
    /// Ŀ���
    /// </summary>
    private Vector3 m_TargetPos;
    /// <summary>
    /// ������ʱ��
    /// </summary>
    private long m_ServerTime;
    /// <summary>
    /// ����Ŀ�����Ҫ��ʱ��
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
    /// ��ɫ�ƶ�
    /// </summary>
    /// <param name="targetPos">Ŀ���</param>
    /// <param name="serverTime">������ʱ��</param>
    /// <param name="needTime">�ƶ���Ҫ��ʱ��</param>
    public void MoveTo(Vector3 targetPos, long serverTime, int needTime)
    {
        m_TargetPos = targetPos;
        m_ServerTime = serverTime;
        m_NeedTime = needTime;
        currentRole.Seeker.StartPath(currentRole.transform.position, targetPos, p => OnAStarFinish(p));
    }

    /// <summary>
    /// A���㷨�����ƶ�������ʱ���ʵ��ʱ��
    /// </summary>
    /// <param name="p">Ѱ··��</param>
    private void OnAStarFinish(Path p)
    {
        //��ȡ·����
        float pathLen = GameUtil.GetPathLen(p.vectorPath);

        //��ȡ�����ƶ�����������н�����ʱ���=��ǰ������ʱ��-Э�鷢�����ķ�����ʱ��
        long delayTime = GlobalInit.Instance.GetCurrentServerTime() - m_ServerTime;
        //������ҿ������ƶ�ʱ��=�ƶ���ʱ��-�ӳ�ʱ��
        long realMoveTime = m_NeedTime - delayTime;
        if (realMoveTime <= 0)
        { realMoveTime = 100; };
        //��ȡ�������������ʵ���ƶ��ٶ�
        currentRole.ModifySpeed = pathLen / (realMoveTime * 0.001f);
        currentRole.MoveTo(m_TargetPos);
    }
}
