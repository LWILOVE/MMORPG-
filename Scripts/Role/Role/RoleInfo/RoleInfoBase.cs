using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色信息基类
/// </summary>
public class RoleInfoBase
{
    public int RoldId; //角色编号
    public string RoleNickName; //角色昵称
    public int CurrExp; //当前经验
    public int MaxExp;//最大经验
    public int Gold;
    public int Level;//等级
    public int MaxHP; //最大HP
    public int CurrHP; //当前HP
    public int MaxMP; //最大MP    
    public int CurrMP; //当前MP
    public int Attack; //攻击力
    public int Defense; //防御
    public int Hit; //命中
    public int Dodge; //闪避
    public int Cri; //暴击
    public int Res; //抗性
    public int Fighting; //综合战斗力
    public List<RoleInfoSkill> skillList;//技能列表
    public int[] PhySkillIds;//物理攻击数组


    public RoleInfoBase()
    {
        skillList = new List<RoleInfoSkill>();
    }
    /// <summary>
    /// 设置角色的物理攻击的ID
    /// </summary>
    /// <param name="phySkillIds"></param>
    /// <returns></returns>
    public void SetPhySkillId(string phySkillIds)
    {
        string[] ids = phySkillIds.Split(";");
        PhySkillIds = new int[ids.Length];
        for (int i = 0; i < ids.Length; i++)
        {
            PhySkillIds[i] = ids[i].ToInt();
        }
    }


    /// <summary>
    /// 设置技能的冷却结束时间
    /// </summary>
    /// <param name="skillId"></param>
    public void SetSkillCDEndTime(int skillId)
    {
        if (skillList.Count > 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                if (skillList[i].SkillId == skillId)
                {
                    skillList[i].SkillCDEndTime = skillList[i].SkillCDTime + Time.time;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 获取可使用的技能ID
    /// </summary>
    /// <returns></returns>
    public int GetCanUsedSkillId()
    {
        if (skillList.Count > 0)
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                if (Time.time > skillList[i].SkillCDEndTime && CurrMP >= skillList[i].SpendMP && skillList[i].isUsing == true)
                {
                    return skillList[i].SkillId;
                }
            }
        }
        return 0;
    }

    /// <summary>
    /// 获取技能等级
    /// </summary>
    /// <param name="skillId"></param>
    /// <returns></returns>
    public int GetSkillLevel(int skillId)
    {
        if (skillList == null)
        { return 1; }
        for (int i = 0; i < skillList.Count; i++)
        {
            if (skillList[i].SkillId == skillId)
            {
                return skillList[i].SkillLevel;
            }
        }
        return 1;
    }
}
