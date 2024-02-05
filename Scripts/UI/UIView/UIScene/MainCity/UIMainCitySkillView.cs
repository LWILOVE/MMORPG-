using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主角色技能UI面板
/// </summary>
public class UIMainCitySkillView : UISubViewBase
{
    public static UIMainCitySkillView Instance;

    protected override void OnAwake()
    {
        base.OnAwake();
        m_Dic = new Dictionary<int, UIMainCitySkillSlotsView>();
        Instance = this;
    }
    [SerializeField]
    private UIMainCitySkillSlotsView Btn_Skill1;
    [SerializeField]
    private UIMainCitySkillSlotsView Btn_Skill2;
    [SerializeField]
    private UIMainCitySkillSlotsView Btn_Skill3;
    [SerializeField]
    private UIMainCitySkillSlotsView Btn_AddHP;
    [SerializeField]
    private UIMainCitySkillSlotsView Btn_Attack;

    private Dictionary<int, UIMainCitySkillSlotsView> m_Dic;

    /// <summary>
    /// 技能槽点击
    /// </summary>
    public Action<int> OnSkillClick;

    public void SetUI(List<TransferData> list, Action<int> onSkillClick)
    {
        for (int j = 0; j < GlobalInit.Instance.currentPlayer.CurrentRoleInfo.skillList.Count; j++)
        {
            GlobalInit.Instance.currentPlayer.CurrentRoleInfo.skillList[j].isUsing = false;
        }

        for (int i = 0; i < list.Count; i++)
        {

            int skillSlotsNo = list[i].GetValue<byte>(ConstDefine.SkillSlotsNo);
            int skillId = list[i].GetValue<int>(ConstDefine.SkillId);
            int skillLevel = list[i].GetValue<int>(ConstDefine.SkillLevel);
            float skillCDTime = list[i].GetValue<float>(ConstDefine.SkillCDTime);
            string skillPic = list[i].GetValue<string>(ConstDefine.SkillPic);
            for (int j = 0; j < GlobalInit.Instance.currentPlayer.CurrentRoleInfo.skillList.Count; j++)
            {
                if (GlobalInit.Instance.currentPlayer.CurrentRoleInfo.skillList[j].SkillId == skillId)
                {
                    GlobalInit.Instance.currentPlayer.CurrentRoleInfo.skillList[j].isUsing = true;
                }
            }
            switch (skillSlotsNo)
            {
                case 1:
                    Btn_Skill1.SetUI(skillId, skillLevel, skillPic, skillCDTime, onSkillClick);
                    m_Dic[skillId] = Btn_Skill1;
                    break;
                case 2:
                    Btn_Skill2.SetUI(skillId, skillLevel, skillPic, skillCDTime, onSkillClick);
                    m_Dic[skillId] = Btn_Skill2;
                    break;
                case 3:
                    Btn_Skill3.SetUI(skillId, skillLevel, skillPic, skillCDTime, onSkillClick);
                    m_Dic[skillId] = Btn_Skill3;
                    break;
                case 123:
                    Btn_Attack.SetUI(skillId, skillLevel, skillPic, skillCDTime, onSkillClick);
                    m_Dic[skillId] = Btn_Attack;
                    break;
            }
        }
    }

    /// <summary>
    /// 开始冷却
    /// </summary>
    /// <param name="skillId"></param>
    public void BeginCD(int skillId)
    {
        if (m_Dic.ContainsKey(skillId))
        {
            UIMainCitySkillSlotsView view = m_Dic[skillId];
            view.BeginCD();
        }
    }
}

