using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ���ݵĹ�����Ϣ��
/// </summary>
public class RoleTransferAttackInfo 
{
    /// <summary>
    /// �������ı��
    /// </summary>
    public int AttackRoleId;
    /// <summary>
    /// ��������λ��
    /// </summary>
    public Vector3 AttackRolePos;
    /// <summary>
    /// ���������ı��
    /// </summary>
    public int BeAttackRoleId;
    /// <summary>
    /// �˺���ֵ
    /// </summary>
    public int HurtValue;
    /// <summary>
    /// ������ʹ�õļ���Id
    /// </summary>
    public int SkillId;
    /// <summary>
    /// ������ʹ�õļ��ܵȼ�
    /// </summary>
    public int SkillLevel;
    /// <summary>
    /// �Ƿ񸽼��쳣״̬
    /// </summary>
    public bool IsAbnormal;
    /// <summary>
    /// �Ƿ񱩻�
    /// </summary>
    public bool IsCri;
    /// <summary>
    /// �˺���ʱʱ��
    /// </summary>
    public float HurtDelayTime;
}

