using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色有限状态机管理器
/// </summary>
public class RoleFSMMgr
{
    /// <summary>
    /// 当前角色控制器
    /// </summary>
    public RoleCtrl currRoleCtrl { get; private set; }
    /// <summary>
    /// 当前角色状态枚举
    /// </summary>
    public RoleState currRoleStateEnum = RoleState.None;
    /// <summary>
    /// 当前角色状态：控制角色状态
    /// </summary>
    public RoleStateAbstract currRoleState { get; private set; }
    /// <summary>
    /// 角色状态字典：存储角色状态包
    /// 键：角色状态枚举
    /// 值：角色状态反应
    /// </summary>
    public Dictionary<RoleState, RoleStateAbstract> m_RoleStateDic;
    /// <summary>
    /// 要切换的待机状态
    /// </summary>
    public RoleIdleState ToIdleState
    { get; set; }

    /// <summary>
    /// 当前的待机状态
    /// </summary>
    public RoleIdleState CurrentIdleState
    { get; set; }

    /// <summary>
    /// 当前状态每一帧都是需要更新的
    /// </summary>
    public void OnUpdate()
    {
        if (currRoleState != null)
        {
            currRoleState.OnUpdate();
        }
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public RoleFSMMgr(RoleCtrl currRoleCtrl,Action onDie,Action onDestroy)
    {
        this.currRoleCtrl = currRoleCtrl;

        ///有限状态机实例化的同时，角色状态字典也实时更新
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

        //如果字典里有当前状态枚举，那么当前状态就等于当前状态枚举
        if (m_RoleStateDic.ContainsKey(currRoleStateEnum))
        {
            currRoleState = m_RoleStateDic[currRoleStateEnum];
        }
    }

    /// <summary>
    /// 返回状态机中的状态
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
    /// 进行状态切换
    /// </summary>
    /// <param name="newState">要切换的状态枚举</param>
    public void ChangeState(RoleState newState)
    {
        //如果新状态就是旧状态，无需切换
        if (currRoleStateEnum == newState && currRoleStateEnum != RoleState.Idle
            && currRoleStateEnum !=RoleState.Attack) return;
        //调用过去状态的离开方法，让状态切换更加灵活
        if (currRoleState != null)
        {
            currRoleState.OnLeave();
        }
        //修正当前状态枚举
        currRoleStateEnum = newState;
        //修正当前状态:当前状态是新状态
        currRoleState = m_RoleStateDic[newState];
        //若当前状态是待机状态,则切换至对应的待机状态
        if (currRoleStateEnum == RoleState.Idle)
        {
            //为当前待机状态赋值
            CurrentIdleState = ToIdleState;
        }
        //调用现在状态的进入方法，让状态进入更加丝滑
        currRoleState.OnEnter();
    }
}
