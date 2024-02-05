using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO
/// </summary>
public class RoleStateSelect : RoleStateAbstract
{
    /// <summary>
    /// ¹¹Ôìº¯Êý
    /// </summary>
    /// <param name="roleFSMMgr"></param>
    public RoleStateSelect(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToSelect.ToString(),true);
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Select.ToString()))
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(),(int)RoleAnimatorState.Select);
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1)
            {
                CurrRoleFSMMgr.currRoleCtrl.ToIdle();
            }
        }
    }

    public override void OnLeave()
    {
        base.OnLeave();
        CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToSelect.ToString(),false);
    }
}
