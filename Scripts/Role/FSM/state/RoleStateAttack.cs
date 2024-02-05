using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 攻击状态
/// </summary>
public class RoleStateAttack : RoleStateAbstract
{
    public RoleStateAttack(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }

    /// <summary>
    /// 动画控制器执行条件
    /// </summary>
    public string AnimatorCondition;

    /// <summary>
    /// （旧的）动画控制器执行条件(即上一个动画)
    /// </summary>
    public string m_OldAnimatorCondition;

    /// <summary>
    /// 条件值
    /// </summary>
    public int AnimationConditionValue;
    
    /// <summary>
    /// 当前角色动画状态
    /// </summary>
    public RoleAnimatorState AnimatorCurrentState;

    /// <summary>
    /// 实现基类 进入状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        CurrRoleFSMMgr.currRoleCtrl.PrevFightTime = Time.time;
        m_OldAnimatorCondition = AnimatorCondition;
        CurrRoleFSMMgr.currRoleCtrl.IsRigidity = true;
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(AnimatorCondition, AnimationConditionValue);
        //攻击前朝向敌人
        if (CurrRoleFSMMgr.currRoleCtrl.LockEnemy != null)
        {
            CurrRoleFSMMgr.currRoleCtrl.transform.LookAt(CurrRoleFSMMgr.currRoleCtrl.LockEnemy.transform.position);
        }
        
    }
    /// <summary>
    /// 实现基类 执行状态
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        //获取当前的动画状态信息
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(AnimatorCurrentState.ToString()))
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)AnimatorCurrentState);
            //当动画播放次数超过1次时，返回待机状态
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1)
            {
                CurrRoleFSMMgr.currRoleCtrl.IsRigidity = false;
                CurrRoleFSMMgr.currRoleCtrl.ToIdle(RoleIdleState.IdleFight);
            }
        }
    }
    /// <summary>
    /// 实现基类 离开状态
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.currRoleCtrl.IsRigidity = false;
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(m_OldAnimatorCondition, 0);
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
    }
}
