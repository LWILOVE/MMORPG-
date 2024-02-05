using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 精灵实体
/// </summary>
public partial class SpriteEntity
{
    /// <summary>
    /// 物理攻击数组
    /// </summary>
    private int[] m_UsedPhyAttackArr;

    /// <summary>
    /// 角色可使用的物理攻击ID数组
    /// </summary>
    public int[] UsePhyAttackArr
    {
        get
        {
            if (string.IsNullOrEmpty(UsedPhyAttack))
            { return null; }
            if (m_UsedPhyAttackArr == null)
            {
                string[] arr = this.UsedPhyAttack.Split("_");
                m_UsedPhyAttackArr = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    m_UsedPhyAttackArr[i] = arr[i].ToInt();
                }
            }
            return m_UsedPhyAttackArr;
        }
    }

    /// <summary>
    /// 物理攻击数组
    /// </summary>
    private int[] m_UsedSkillListArr;
    /// <summary>
    /// 角色可使用的技能攻击ID数组
    /// </summary>
    public int[] UseSkillListArr
    {
        get
        {
            if (string.IsNullOrEmpty(UsedSkillList))
            { return null; }
            if (m_UsedSkillListArr == null)
            {
                string[] arr = this.UsedSkillList.Split("_");
                m_UsedSkillListArr = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    m_UsedSkillListArr[i] = arr[i].ToInt();
                }
            }
            return m_UsedSkillListArr;
        }
    }
}
