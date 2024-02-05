using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色技能信息类
/// </summary>
public class RoleInfoSkill
{
    /// <summary>
    /// 技能编号
    /// </summary>
    public int SkillId;
    /// <summary>
    /// 技能等级    
    /// </summary>
    public int SkillLevel;
    /// <summary>
    /// 技能槽编号
    /// </summary>
    public byte SlotsNo;
    /// <summary>
    /// 技能冷却时间
    /// </summary>
    private float m_SkillCDTime = 0f;
    /// <summary>
    /// 技能冷却时间
    /// </summary>
    public float SkillCDTime
    {
        get 
        {
            if (m_SkillCDTime <= 0)
            {
                m_SkillCDTime = SkillLevelDBModel.Instance.GetEntityBySkillIdAndLevel(SkillId,SkillLevel).SkillCDTime;
            }
            return m_SkillCDTime;
        }
    }
    private int m_SpendMP;
    /// <summary>
    /// 消耗的MP
    /// </summary>
    public int SpendMP
    {
        get 
        {
            if (m_SpendMP == 0)
            {
                m_SpendMP = SkillLevelDBModel.Instance.GetEntityBySkillIdAndLevel(SkillId,SkillLevel).SpendMP;
            }
            return m_SpendMP;
        }
    }
    /// <summary>
    /// 冷却结束时间
    /// </summary>
    public float SkillCDEndTime;

    //自己加进去的
    /// <summary>
    /// 特效名
    /// </summary>
    public float EffectName;
    /// <summary>
    /// 特效存活时间
    /// </summary>
    public float EffectLifeTime;

    /// <summary>
    /// 是否正在使用
    /// </summary>
    public bool isUsing = false;

}
