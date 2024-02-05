using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ��Ϣ����
/// </summary>
public class RoleInfoBase
{
    public int RoldId; //��ɫ���
    public string RoleNickName; //��ɫ�ǳ�
    public int CurrExp; //��ǰ����
    public int MaxExp;//�����
    public int Gold;
    public int Level;//�ȼ�
    public int MaxHP; //���HP
    public int CurrHP; //��ǰHP
    public int MaxMP; //���MP    
    public int CurrMP; //��ǰMP
    public int Attack; //������
    public int Defense; //����
    public int Hit; //����
    public int Dodge; //����
    public int Cri; //����
    public int Res; //����
    public int Fighting; //�ۺ�ս����
    public List<RoleInfoSkill> skillList;//�����б�
    public int[] PhySkillIds;//����������


    public RoleInfoBase()
    {
        skillList = new List<RoleInfoSkill>();
    }
    /// <summary>
    /// ���ý�ɫ����������ID
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
    /// ���ü��ܵ���ȴ����ʱ��
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
    /// ��ȡ��ʹ�õļ���ID
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
    /// ��ȡ���ܵȼ�
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
