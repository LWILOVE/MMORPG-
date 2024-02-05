using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色攻击信息（各类攻击信息）
/// </summary>
[System.Serializable]
public class RoleAttackInfo
{
    [Header("特效：名称：持续时间")]
    /// <summary>
    /// 特效名称（角色在真正的战斗场景中使用）
    /// </summary>
    public string EffectName;

    /// <summary>
    /// 特效的存活时间
    /// </summary>
    public float EffectLiftTime;

    [Header("是否震屏：持续时间")]
    /// <summary>
    /// 是否震屏
    /// </summary>
    public bool IsDOCameraShake = false;

    /// <summary>
    /// 震屏延迟时间
    /// </summary>
    public float CameraShakeDelay = 0.2f;

    [Header("攻击范围：伤害时间")]
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float AttackRange = 0f;

    /// <summary>
    /// 让敌人受伤的延迟时间
    /// </summary>
    public float HurtDelayTime = 0f;

    [Header("第几个技能：技能ID号")]
    /// <summary>
    /// 索引号
    /// </summary>
    public int Index;

    /// <summary>
    /// 技能ID
    /// </summary>
    public int SkillId;

#if DEBUG_ROLESTATE
    /// <summary>
    /// 特效预设（在测试环境下适用）
    /// </summary>
    public GameObject EffectObject;
#endif

    /// <summary>
    /// 开火声音
    /// </summary>
    public DelayAudioClip FireAudio;

    /// <summary>
    /// 攻击方的叫声
    /// </summary>
    public DelayAudioClip AttactRoleAudio;

    public bool isUse = false;
}


