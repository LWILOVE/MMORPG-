using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬
/// </summary>
public class RoleStateAttack : RoleStateAbstract
{
    public RoleStateAttack(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }

    /// <summary>
    /// ����������ִ������
    /// </summary>
    public string AnimatorCondition;

    /// <summary>
    /// ���ɵģ�����������ִ������(����һ������)
    /// </summary>
    public string m_OldAnimatorCondition;

    /// <summary>
    /// ����ֵ
    /// </summary>
    public int AnimationConditionValue;
    
    /// <summary>
    /// ��ǰ��ɫ����״̬
    /// </summary>
    public RoleAnimatorState AnimatorCurrentState;

    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        CurrRoleFSMMgr.currRoleCtrl.PrevFightTime = Time.time;
        m_OldAnimatorCondition = AnimatorCondition;
        CurrRoleFSMMgr.currRoleCtrl.IsRigidity = true;
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(AnimatorCondition, AnimationConditionValue);
        //����ǰ�������
        if (CurrRoleFSMMgr.currRoleCtrl.LockEnemy != null)
        {
            CurrRoleFSMMgr.currRoleCtrl.transform.LookAt(CurrRoleFSMMgr.currRoleCtrl.LockEnemy.transform.position);
        }
        
    }
    /// <summary>
    /// ʵ�ֻ��� ִ��״̬
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        //��ȡ��ǰ�Ķ���״̬��Ϣ
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(AnimatorCurrentState.ToString()))
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)AnimatorCurrentState);
            //���������Ŵ�������1��ʱ�����ش���״̬
            if (CurrRoleAnimatorStateInfo.normalizedTime > 1)
            {
                CurrRoleFSMMgr.currRoleCtrl.IsRigidity = false;
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
        CurrRoleFSMMgr.currRoleCtrl.IsRigidity = false;
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(m_OldAnimatorCondition, 0);
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
    }
}
