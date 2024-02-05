using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬
/// </summary>
public class RoleStateDie : RoleStateAbstract
{

    /// <summary>
    /// ��ɫ����ί�� 
    /// </summary>
    public Action OnDie;

    /// <summary>
    /// ��ɫ����ί��
    /// </summary>
    public Action OnDestroy;

    /// <summary>
    /// ��ʼ������ʱ��
    /// </summary>
    private float m_BeginDieTime = 0f;

    /// <summary>
    /// �Ƿ��Ѿ�����
    /// </summary>
    private bool m_IsDestroy = false;
    public RoleStateDie(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        m_IsDestroy = false;

        //����ɫ�Ѿ����ܾ���ʱ������Ҫ���ŵ��µĹ���
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
    /// ʵ�ֻ��� ִ��״̬
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

            //ȷ�����ٹ�����ִ��һ��
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
            //��ȡ��ǰ�Ķ���״̬��Ϣ
            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Die.ToString()))
            {
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Die);
            }
        }
    }
    /// <summary>
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToDie.ToString(), false);
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToDied.ToString(),false);
    }
}
