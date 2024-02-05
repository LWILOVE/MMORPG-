using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������Ϣ��
/// </summary>
public class RoleInfoSkill
{
    /// <summary>
    /// ���ܱ��
    /// </summary>
    public int SkillId;
    /// <summary>
    /// ���ܵȼ�    
    /// </summary>
    public int SkillLevel;
    /// <summary>
    /// ���ܲ۱��
    /// </summary>
    public byte SlotsNo;
    /// <summary>
    /// ������ȴʱ��
    /// </summary>
    private float m_SkillCDTime = 0f;
    /// <summary>
    /// ������ȴʱ��
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
    /// ���ĵ�MP
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
    /// ��ȴ����ʱ��
    /// </summary>
    public float SkillCDEndTime;

    //�Լ��ӽ�ȥ��
    /// <summary>
    /// ��Ч��
    /// </summary>
    public float EffectName;
    /// <summary>
    /// ��Ч���ʱ��
    /// </summary>
    public float EffectLifeTime;

    /// <summary>
    /// �Ƿ�����ʹ��
    /// </summary>
    public bool isUsing = false;

}
