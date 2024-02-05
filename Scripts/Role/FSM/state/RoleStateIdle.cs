using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 待机状态
/// </summary>
public class RoleStateIdle : RoleStateAbstract
{
    /// <summary>
    /// 下次切换休闲的世界
    /// </summary>
    private float m_NextChangeTime = 0f;

    /// <summary>
    /// 切换的间隔
    /// </summary>
    /// <param name="roleFSMMgr"></param>
    private float m_ChageStep = 5f  ;

    /// <summary>
    /// 待机状态的运行时间
    /// </summary>
    private float m_RuningTime = 0;

    /// <summary>
    /// 是否正在播放休闲动画
    /// </summary>
    private bool m_IsXiuXian = false;

    public RoleStateIdle(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// 实现基类 进入状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();

        //if (CurrRoleFSMMgr.currRoleCtrl.CurrRoleType == RoleType.MainPlayer)
        //{

        //    //切换至休闲待机OR战斗待机
        //    if (CurrRoleFSMMgr.CurrentIdleState == RoleIdleState.IdleNormal)
        //    {
        //        m_NextChangeTime = Time.time + m_ChageStep;
        //        m_IsXiuXian = false;

        //        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleNormal.ToString(), true);
        //    }
        //    else
        //    {
        //        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleFight.ToString(), true);
        //    }
        //    m_RuningTime = 0;
        //}
        if (CurrRoleFSMMgr.currRoleCtrl.gameObject.tag == "Player")
        {

            //切换至休闲待机OR战斗待机
            if (CurrRoleFSMMgr.CurrentIdleState == RoleIdleState.IdleNormal)
            {
                m_NextChangeTime = Time.time + m_ChageStep;
                m_IsXiuXian = false;

                this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleNormal.ToString(), true);
            }
            else
            {
                this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleFight.ToString(), true);
            }
            m_RuningTime = 0;
        }
        else
        {
            this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleFight.ToString(), true);
        }
    }
    /// <summary>
    /// 实现基类 执行状态
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (CurrRoleFSMMgr.currRoleCtrl.CurrRoleType == RoleType.MainPlayer)
        {
            if (!IsChangeOver)
            {
                if (CurrRoleFSMMgr.CurrentIdleState == RoleIdleState.IdleNormal)
                {
                    //获取当前的动画状态信息
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
                    //若当前处于休闲态，则不进行待机
                    if (!m_IsXiuXian)
                    {
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Normal.ToString()))
                        {
                            //设置当前动画状态条件的目的是 防止频繁进入相同动画状态
                            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Idle_Normal);
                            m_RuningTime += Time.deltaTime;
                            //解决因状态转换过快而导致的角色动作融合不充分的问题
                            if (m_RuningTime > 0.1f)
                            {
                                IsChangeOver = true;
                            }
                            else
                            {
                                //防止怪原地跑
                                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
                            }
                        }
                    }
                    else
                    {
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.XiuXian.ToString()))
                        {
                            //设置当前动画状态条件的目的是 防止频繁进入相同动画状态
                            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.XiuXian);
                            IsChangeOver = true;
                        }
                    }
                }
                else
                {
                    //获取当前的动画状态信息
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
                    if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))
                    {
                        //设置当前动画状态条件的目的是 防止频繁进入相同动画状态
                        CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);
                        m_RuningTime += Time.deltaTime;
                        //解决因状态转换过快而导致的角色动作融合不充分的问题
                        if (m_RuningTime > 0.1f)
                        {
                            IsChangeOver = true;
                        }
                    }
                    else
                    {
                        //防止怪原地跑
                        CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
                    }
                }
            }
            //==================待机与休闲动画切换
            if (CurrRoleFSMMgr.CurrentIdleState == RoleIdleState.IdleNormal)
            {
                if (Time.time > m_NextChangeTime)
                {
                    m_NextChangeTime = Time.time + m_ChageStep;
                    m_IsXiuXian = true;
                    IsChangeOver = false;
                    CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleNormal.ToString(), false);
                    CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToXiuXian.ToString(), true);
                }

                //当前如果正在休闲中，则切换待机
                if (m_IsXiuXian)
                {
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
                    if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.XiuXian.ToString()) && CurrRoleAnimatorStateInfo.normalizedTime > 1)
                    {
                        m_IsXiuXian = false;
                        IsChangeOver = false;
                        CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToXiuXian.ToString(), false);
                        CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleNormal.ToString(), true);
                    }
                }
            }
        }
        else
        {
            //怪的待机状态
            //获取当前的动画状态信息
            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))
            {
                //设置当前动画状态条件的目的是 防止频繁进入相同动画状态
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);
                m_RuningTime += Time.deltaTime;
                if (m_RuningTime > 0.1f)
                {
                    IsChangeOver = true;
                }
            }
            else
            {
                //防止怪原地跑
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(),0);
            }
        }
    }   
    
    /// <summary>
    /// 实现基类 离开状态
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        //当当前客户端或者其他玩家要离开待机状态时，我们都应该进行调整
        if (CurrRoleFSMMgr.currRoleCtrl.CurrRoleType == RoleType.MainPlayer || CurrRoleFSMMgr.currRoleCtrl.CurrRoleType == RoleType.OtherPlayer)
        {
            if (CurrRoleFSMMgr.CurrentIdleState == RoleIdleState.IdleNormal)
            {
                this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleNormal.ToString(), false);
                this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToXiuXian.ToString(), false);
            }
            else
            {
                this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleFight.ToString(), false);
            }
        }
        else
        {
            this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleFight.ToString(), false);
            this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToIdleNormal.ToString(), false);
        }
    }
}
