using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����ʵ��
/// </summary>
public partial class SpriteEntity
{
    /// <summary>
    /// ����������
    /// </summary>
    private int[] m_UsedPhyAttackArr;

    /// <summary>
    /// ��ɫ��ʹ�õ�������ID����
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
    /// ����������
    /// </summary>
    private int[] m_UsedSkillListArr;
    /// <summary>
    /// ��ɫ��ʹ�õļ��ܹ���ID����
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
