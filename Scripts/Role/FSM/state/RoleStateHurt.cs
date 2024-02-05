using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬
/// </summary>
public class RoleStateHurt : RoleStateAbstract
{
    public RoleStateHurt(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToHurt.ToString(), true);
    }
    /// <summary>
    /// ʵ�ֻ��� ִ��״̬
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        //��ȡ��ǰ�Ķ���״̬��Ϣ
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Hurt.ToString()))
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Hurt);
            //���������Ŵ�������1��ʱ�����ش���״̬
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1)
            {
                CurrRoleFSMMgr.currRoleCtrl.ToIdle(RoleIdleState.IdleFight);
            }
        }
    }
    /// <summary>
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToHurt.ToString(), false);
    }
}
