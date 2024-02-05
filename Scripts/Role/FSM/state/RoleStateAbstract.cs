using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ״̬�ĳ�����ࣺ����ʵ�������ࣺ������
/// </summary>
public abstract class RoleStateAbstract
{
    /// <summary>
    /// ��ǰ��ɫ������״̬��������
    /// </summary>
    public RoleFSMMgr CurrRoleFSMMgr { get; private set; }
    /// <summary>
    /// ��ǰ�Ľ�ɫ����״̬��Ϣ
    /// </summary>
    public AnimatorStateInfo CurrRoleAnimatorStateInfo { get; set; }
    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="roleFSMMgr">״̬�����󣺻�ȡ��ɫ��FSM</param>
    public RoleStateAbstract(RoleFSMMgr roleFSMMgr)
    {
        CurrRoleFSMMgr = roleFSMMgr;
    }

    /// <summary>
    /// ����״̬�Ƿ��л����
    /// </summary>
    protected bool IsChangeOver
    {
        get;set;
    }

    /// <summary>
    /// ��ɫ����״̬
    /// </summary>
    public virtual void OnEnter() 
    {
        IsChangeOver = false;
    }
    /// <summary>
    /// ��ɫִ��״̬
    /// </summary>
    public virtual void OnUpdate() { }
    /// <summary>
    /// ��ɫ�뿪״̬
    /// </summary>
    public virtual void OnLeave() { }
}
