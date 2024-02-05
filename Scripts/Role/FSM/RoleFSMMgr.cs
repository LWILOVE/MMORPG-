using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ����״̬��������
/// </summary>
public class RoleFSMMgr
{
    /// <summary>
    /// ��ǰ��ɫ������
    /// </summary>
    public RoleCtrl currRoleCtrl { get; private set; }
    /// <summary>
    /// ��ǰ��ɫ״̬ö��
    /// </summary>
    public RoleState currRoleStateEnum = RoleState.None;
    /// <summary>
    /// ��ǰ��ɫ״̬�����ƽ�ɫ״̬
    /// </summary>
    public RoleStateAbstract currRoleState { get; private set; }
    /// <summary>
    /// ��ɫ״̬�ֵ䣺�洢��ɫ״̬��
    /// ������ɫ״̬ö��
    /// ֵ����ɫ״̬��Ӧ
    /// </summary>
    public Dictionary<RoleState, RoleStateAbstract> m_RoleStateDic;
    /// <summary>
    /// Ҫ�л��Ĵ���״̬
    /// </summary>
    public RoleIdleState ToIdleState
    { get; set; }

    /// <summary>
    /// ��ǰ�Ĵ���״̬
    /// </summary>
    public RoleIdleState CurrentIdleState
    { get; set; }

    /// <summary>
    /// ��ǰ״̬ÿһ֡������Ҫ���µ�
    /// </summary>
    public void OnUpdate()
    {
        if (currRoleState != null)
        {
            currRoleState.OnUpdate();
        }
    }

    /// <summary>
    /// ���캯��
    /// </summary>
    public RoleFSMMgr(RoleCtrl currRoleCtrl,Action onDie,Action onDestroy)
    {
        this.currRoleCtrl = currRoleCtrl;

        ///����״̬��ʵ������ͬʱ����ɫ״̬�ֵ�Ҳʵʱ����
        m_RoleStateDic = new Dictionary<RoleState, RoleStateAbstract>();
        m_RoleStateDic[RoleState.Idle] = new RoleStateIdle(this);
        m_RoleStateDic[RoleState.Run] = new RoleStateRun(this);
        m_RoleStateDic[RoleState.Attack] = new RoleStateAttack(this);
        m_RoleStateDic[RoleState.Hurt] = new RoleStateHurt(this);
        m_RoleStateDic[RoleState.Die] = new RoleStateDie(this);
        m_RoleStateDic[RoleState.Select] = new RoleStateSelect(this);

        RoleStateDie dieState = (RoleStateDie)m_RoleStateDic[RoleState.Die];
        dieState.OnDie = onDie;
        dieState.OnDestroy = onDestroy;

        //����ֵ����е�ǰ״̬ö�٣���ô��ǰ״̬�͵��ڵ�ǰ״̬ö��
        if (m_RoleStateDic.ContainsKey(currRoleStateEnum))
        {
            currRoleState = m_RoleStateDic[currRoleStateEnum];
        }
    }

    /// <summary>
    /// ����״̬���е�״̬
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public RoleStateAbstract GetRoleState(RoleState state)
    {
        if (!m_RoleStateDic.ContainsKey(state))
        {
            return null;
        }
        return m_RoleStateDic[state];
    }

    /// <summary>
    /// ����״̬�л�
    /// </summary>
    /// <param name="newState">Ҫ�л���״̬ö��</param>
    public void ChangeState(RoleState newState)
    {
        //�����״̬���Ǿ�״̬�������л�
        if (currRoleStateEnum == newState && currRoleStateEnum != RoleState.Idle
            && currRoleStateEnum !=RoleState.Attack) return;
        //���ù�ȥ״̬���뿪��������״̬�л��������
        if (currRoleState != null)
        {
            currRoleState.OnLeave();
        }
        //������ǰ״̬ö��
        currRoleStateEnum = newState;
        //������ǰ״̬:��ǰ״̬����״̬
        currRoleState = m_RoleStateDic[newState];
        //����ǰ״̬�Ǵ���״̬,���л�����Ӧ�Ĵ���״̬
        if (currRoleStateEnum == RoleState.Idle)
        {
            //Ϊ��ǰ����״̬��ֵ
            CurrentIdleState = ToIdleState;
        }
        //��������״̬�Ľ��뷽������״̬�������˿��
        currRoleState.OnEnter();
    }
}
