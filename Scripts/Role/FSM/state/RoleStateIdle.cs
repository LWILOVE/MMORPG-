using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����״̬
/// </summary>
public class RoleStateIdle : RoleStateAbstract
{
    /// <summary>
    /// �´��л����е�����
    /// </summary>
    private float m_NextChangeTime = 0f;

    /// <summary>
    /// �л��ļ��
    /// </summary>
    /// <param name="roleFSMMgr"></param>
    private float m_ChageStep = 5f  ;

    /// <summary>
    /// ����״̬������ʱ��
    /// </summary>
    private float m_RuningTime = 0;

    /// <summary>
    /// �Ƿ����ڲ������ж���
    /// </summary>
    private bool m_IsXiuXian = false;

    public RoleStateIdle(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();

        //if (CurrRoleFSMMgr.currRoleCtrl.CurrRoleType == RoleType.MainPlayer)
        //{

        //    //�л������д���ORս������
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

            //�л������д���ORս������
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
    /// ʵ�ֻ��� ִ��״̬
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
                    //��ȡ��ǰ�Ķ���״̬��Ϣ
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
                    //����ǰ��������̬���򲻽��д���
                    if (!m_IsXiuXian)
                    {
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Normal.ToString()))
                        {
                            //���õ�ǰ����״̬������Ŀ���� ��ֹƵ��������ͬ����״̬
                            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Idle_Normal);
                            m_RuningTime += Time.deltaTime;
                            //�����״̬ת����������µĽ�ɫ�����ںϲ���ֵ�����
                            if (m_RuningTime > 0.1f)
                            {
                                IsChangeOver = true;
                            }
                            else
                            {
                                //��ֹ��ԭ����
                                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
                            }
                        }
                    }
                    else
                    {
                        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.XiuXian.ToString()))
                        {
                            //���õ�ǰ����״̬������Ŀ���� ��ֹƵ��������ͬ����״̬
                            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.XiuXian);
                            IsChangeOver = true;
                        }
                    }
                }
                else
                {
                    //��ȡ��ǰ�Ķ���״̬��Ϣ
                    CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
                    if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))
                    {
                        //���õ�ǰ����״̬������Ŀ���� ��ֹƵ��������ͬ����״̬
                        CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);
                        m_RuningTime += Time.deltaTime;
                        //�����״̬ת����������µĽ�ɫ�����ںϲ���ֵ�����
                        if (m_RuningTime > 0.1f)
                        {
                            IsChangeOver = true;
                        }
                    }
                    else
                    {
                        //��ֹ��ԭ����
                        CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
                    }
                }
            }
            //==================���������ж����л�
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

                //��ǰ������������У����л�����
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
            //�ֵĴ���״̬
            //��ȡ��ǰ�Ķ���״̬��Ϣ
            CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
            if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Idle_Fight.ToString()))
            {
                //���õ�ǰ����״̬������Ŀ���� ��ֹƵ��������ͬ����״̬
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Idle_Fight);
                m_RuningTime += Time.deltaTime;
                if (m_RuningTime > 0.1f)
                {
                    IsChangeOver = true;
                }
            }
            else
            {
                //��ֹ��ԭ����
                CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(),0);
            }
        }
    }   
    
    /// <summary>
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        //����ǰ�ͻ��˻����������Ҫ�뿪����״̬ʱ�����Ƕ�Ӧ�ý��е���
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
