using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色传递的攻击信息类
/// </summary>
public class RoleTransferAttackInfo 
{
    /// <summary>
    /// 攻击方的编号
    /// </summary>
    public int AttackRoleId;
    /// <summary>
    /// 攻击方的位置
    /// </summary>
    public Vector3 AttackRolePos;
    /// <summary>
    /// 被攻击方的编号
    /// </summary>
    public int BeAttackRoleId;
    /// <summary>
    /// 伤害数值
    /// </summary>
    public int HurtValue;
    /// <summary>
    /// 攻击方使用的技能Id
    /// </summary>
    public int SkillId;
    /// <summary>
    /// 攻击方使用的技能等级
    /// </summary>
    public int SkillLevel;
    /// <summary>
    /// 是否附加异常状态
    /// </summary>
    public bool IsAbnormal;
    /// <summary>
    /// 是否暴击
    /// </summary>
    public bool IsCri;
    /// <summary>
    /// 伤害延时时间
    /// </summary>
    public float HurtDelayTime;
}

