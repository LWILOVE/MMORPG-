using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 死亡状态
/// </summary>
public class RoleStateDie : RoleStateAbstract
{

    /// <summary>
    /// 角色死亡委托 
    /// </summary>
    public Action OnDie;

    /// <summary>
    /// 角色销毁委托
    /// </summary>
    public Action OnDestroy;

    /// <summary>
    /// 开始死亡的时间
    /// </summary>
    private float m_BeginDieTime = 0f;

    /// <summary>
    /// 是否已经销毁
    /// </summary>
    private bool m_IsDestroy = false;
    public RoleStateDie(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// 实现基类 进入状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        m_IsDestroy = false;

        //当角色已经死很久了时，不需要播放倒下的过程
        if (CurrRoleFSMMgr.currRoleCtrl.IsDied)
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToDied.ToString(), true);
        }
        else
        {
            this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToDie.ToString(), true);
            EffectMgr.Instance.PlayEffect("download/Prefab/Effect/Common/", "Effect_PenXue",
                (Transform transEffect)=>
                {
                    if (transEffect != null)
                    {

                        transEffect.transform.position = CurrRoleFSMMgr.currRoleCtrl.gameObject.transform.position;
                        transEffect.transform.rotation = CurrRoleFSMMgr.currRoleCtrl.gameObject.transform.rotation;
                        EffectMgr.Instance.DestroyEffect(transEffect, 2f);
                    }
                });
            
            if (OnDie != null)
            { OnDie(); }
            m_BeginDieTime = 0f;
        }   
    }
    /// <summary>
    /// 实现基类 执行状态
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (CurrRoleFSMMgr.currRoleCtrl.IsDied)
        {
            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Died.ToString()))
            {
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Died);
            }
        }
        else
        {
            m_BeginDieTime += Time.deltaTime;

            //确保销毁工作仅执行一次
            if (!m_IsDestroy)
            {
                if (m_BeginDieTime >= 6)
                {
                    if (OnDestroy != null)
                    {
                        OnDestroy();
                        m_IsDestroy = true;
                    }
                    return;
                }
            }
            //获取当前的动画状态信息
            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Die.ToString()))
            {
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Die);
            }
        }
    }
    /// <summary>
    /// 实现基类 离开状态
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToDie.ToString(), false);
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToDied.ToString(),false);
    }
}
