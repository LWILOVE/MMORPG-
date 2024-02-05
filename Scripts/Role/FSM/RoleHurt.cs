using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色受伤
/// </summary>
public class RoleHurt
{
    private RoleFSMMgr m_CurrentRoleFSMMgr = null;

    /// <summary>
    /// 角色受伤委托
    /// </summary>
    public Action OnRoleHurt;

    public RoleHurt(RoleFSMMgr fsm)
    {
        m_CurrentRoleFSMMgr = fsm;
    }

    public IEnumerator ToHurt(RoleTransferAttackInfo roleTransferAttackInfo)
    {
        if (m_CurrentRoleFSMMgr == null)
        { yield break; }
        //死人不会受伤
        if (m_CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die)
        { yield break; }
        SkillEntity skillEntity = SkillDBModel.Instance.Get(roleTransferAttackInfo.SkillId);
        SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndLevel(roleTransferAttackInfo.SkillId, roleTransferAttackInfo.SkillLevel);
        if (skillEntity == null || skillLevelEntity == null)
        { yield break; }
        //延迟伤害时间
        yield return new WaitForSeconds(skillEntity.ShowHurtEffectDelaySecond);
        m_CurrentRoleFSMMgr.currRoleCtrl.CurrentRoleInfo.CurrHP -= roleTransferAttackInfo.HurtValue;
        //HUD显示
        int fontSize = 4;
        Color color = Color.red;
        if (roleTransferAttackInfo.IsCri)
        {
            fontSize = 8;
            color = Color.yellow;
        }
        //显示HUD文本
        UILoadingCtrl.Instance.CurrentUIScene.HUDText.NewText("-" + roleTransferAttackInfo.HurtValue, m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform, color, fontSize, 15f, -1f, 2.2f, (UnityEngine.Random.Range(0f, 2f) == 1 ? bl_Guidance.RightDown : bl_Guidance.LeftDown));
        if (OnRoleHurt != null)
        { OnRoleHurt(); }

        if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVP)
        {
            //若是玩家对决中受伤
            if (m_CurrentRoleFSMMgr.currRoleCtrl.CurrentRoleInfo.CurrHP <= 0)
            {
                m_CurrentRoleFSMMgr.currRoleCtrl.CurrentRoleInfo.CurrHP = 0;
                m_CurrentRoleFSMMgr.currRoleCtrl.ToDie();
                yield break;
            }
        }
        else
        {
            //若是打野中受伤
            if (m_CurrentRoleFSMMgr.currRoleCtrl.CurrentRoleInfo.CurrHP <= 0)
            {
                m_CurrentRoleFSMMgr.currRoleCtrl.CurrentRoleInfo.CurrHP = 0;
                m_CurrentRoleFSMMgr.currRoleCtrl.ToDie();
                yield break;
            }
        }

        //播放受伤特效
        EffectMgr.Instance.PlayEffect("download/Prefab/Effect/Common/", "Effect_Hurt",
            (Transform transEffect) =>
            {
                transEffect.transform.position = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.position;
                transEffect.transform.rotation = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.rotation;

                EffectMgr.Instance.DestroyEffect(transEffect, 2f);
            });
        //弹出受伤数字 HUDText（含暴击效果）
        //屏幕泛红
        if (!m_CurrentRoleFSMMgr.currRoleCtrl.IsRigidity)
        {
            m_CurrentRoleFSMMgr.ChangeState(RoleState.Hurt);
        }
    }
}
