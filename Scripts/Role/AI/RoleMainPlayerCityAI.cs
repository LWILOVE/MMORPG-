using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主角主城AI
/// </summary>
public class RoleMainPlayerCityAI : IRoleAI
{
    public RoleCtrl currentRole
    { get; set; }
    public RoleMainPlayerCityAI(RoleCtrl roleCtrl)
    {
        this.currentRole = roleCtrl;
        m_SearchList = new List<Collider>();
    }

    /// <summary>
    /// 物理攻击的技能索引
    /// </summary>
    private int m_PhyIndex = 0;

    /// <summary>
    /// 搜索到的敌人列表
    /// </summary>
    public List<Collider> m_SearchList = null;

    private Vector3 m_MoveToPoint;
    private RaycastHit hitInfo;
    private Vector3 m_RayPoint;
    public void DoAI()
    {
        if (currentRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die)
        { return; }
        //若角色处于自动战斗状态
        if (currentRole.Attack.IsAutoFight)
        {
            AutoFightState();
        }
        else
        {
            NormalState();
        }
    }

    #region AutoFightState 自动战斗状态
    /// <summary>
    /// 自动战斗状态
    /// 逻辑：
    /// 挂机：角色可行动且未通关=>
    /// 当前区域有怪：
    /// 当前已经有锁定敌人：目标死亡=>取消锁定,目标未死亡=>检测角色可进行的攻击手段=>检测目标是否在攻击范围内=>在：发起攻击，不在：移动到目标身边
    /// 当前无锁定敌人：检测敌人=>锁定目标
    /// 当前区域无怪：已经通关？不移动:移动到下一区域
    /// </summary>
    private void AutoFightState()
    {
        if (currentRole.IsRigidity)
        {
            return;
         }

        //判定当前区域是否已经可通过
        if (!GameLevelSceneCtrl.Instance.CurrRegionHasMonster)
        {
            //若已经是关底了，则返回
            if (GameLevelSceneCtrl.Instance.CurrRegionIsLast)
            {
                return;
            }
            else
            {
                //进入下一区域
                currentRole.MoveTo(GameLevelSceneCtrl.Instance.NextRegionPlayerBornPos);
            }

        }
        else
        {
            //进行怪物搜索与攻击

            //若玩家点击了自动战斗：则进行敌人检测
            if (currentRole.LockEnemy == null)
            {
                //在视野范围内搜索敌人，并以最近的敌人作为要锁定的敌人
                //若无锁定敌人，则进行球形判定检测
                Collider[] searchList = Physics.OverlapSphere(currentRole.gameObject.transform.position, 1000f, 1 << LayerMask.NameToLayer("Player"));
                m_SearchList.Clear();
                if (searchList != null && searchList.Length > 0)
                {
                    for (int i = 0; i < searchList.Length; i++)
                    {
                        RoleCtrl ctrl = searchList[i].GetComponent<RoleCtrl>();
                        if (ctrl != null)
                        {
                            if (ctrl.CurrentRoleInfo.RoldId != currentRole.CurrentRoleInfo.RoldId)
                            {
                                m_SearchList.Add(searchList[i]);
                                Debug.Log("添加敌人");
                            }
                        }
                    }
                }
                //对检测到的敌人进行排序，攻击离自己最近的敌人
                m_SearchList.Sort((Collider c1, Collider c2) =>
                {
                    int ret = 0;
                    if (Vector3.Distance(c1.gameObject.transform.position, currentRole.gameObject.transform.position) <
                   Vector3.Distance(c2.gameObject.transform.position, currentRole.gameObject.transform.position))
                    {
                        ret = -1;
                    }
                    else
                    {
                        ret = 1;
                    }
                    return ret;
                });
                if (m_SearchList.Count > 0)
                {
                    currentRole.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();
                    Debug.Log("当前目标：" + m_SearchList[0].gameObject.name);
                }
            }
            else
            {

                //如果当前有锁定敌人
                //若锁定的对象死了，则设置为null并返回
                if (currentRole.LockEnemy.CurrentRoleInfo.CurrHP <= 0)
                {
                    currentRole.LockEnemy = null;
                    return;
                }
                //检测要使用的技能ID与技能类型
                int skillId = currentRole.CurrentRoleInfo.GetCanUsedSkillId();
                RoleAttackType type;
                if (skillId>0)
                {
                    //使用技能   
                    //设置技能ID
                    type = RoleAttackType.SkillAttack;
                }
                else
                {
                    //使用普攻
                    //设置普通ID
                    skillId = currentRole.CurrentRoleInfo.PhySkillIds[m_PhyIndex];
                    type = RoleAttackType.PhyAttack;
                    m_PhyIndex++;   
                    if (m_PhyIndex >= currentRole.CurrentRoleInfo.PhySkillIds.Length)
                    {
                        m_PhyIndex = 0;
                    }
                }
                //判定敌人是否在角色攻击范围之中
                SkillEntity entity = SkillDBModel.Instance.Get(skillId);//根据攻击ID获取攻击实体
                if (entity == null)
                { return; }
                //敌人在攻击范围内
                if (Vector3.Distance(currentRole.transform.position, currentRole.LockEnemy.transform.position) <= (entity.AttackRange-0.5f))
                {
                    if (type == RoleAttackType.SkillAttack)
                    {
                        PlayerCtrl.Instance.OnSkillClick(skillId);
                    }
                    else
                    {
                        //攻击敌人
                        currentRole.ToAttack(type, skillId);
                    }
                }
                else
                {
                    //先追击再攻击
                    if (currentRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
                    {
                        //在目标半圆范围内找点
                        m_MoveToPoint = GameUtil.GetRandomPos(currentRole.transform.position, currentRole.LockEnemy.transform.position, entity.AttackRange);

                        //m_RayPoint = new Vector3(m_MoveToPoint.x, m_MoveToPoint.y + 50, m_MoveToPoint.z);
                        //if (Physics.Raycast(m_RayPoint, new Vector3(0, -100, 0), out hitInfo, 1000f, 1 << LayerMask.NameToLayer("RegionMask")))
                        //{
                        //    return;
                        //}

                        //移动到攻击范围内的任意点
                        currentRole.MoveTo(m_MoveToPoint);
                    }
                }
            }
        }
    }
    #endregion

    #region NormalState 普通状态
    /// <summary>
    /// 普通状态
    /// </summary>
    private void NormalState()
    {
        if (currentRole.PrevFightTime != 0)
        {
            //若离上次战斗时间超过10秒则切回普通待机
            if (Time.time > (currentRole.PrevFightTime + 10f))
            {
                currentRole.ToIdle();
                currentRole.PrevFightTime = 0;
            }
        }

        //若主角含有锁定敌人，则进行攻击
        if (currentRole.LockEnemy != null && currentRole.CurrentRoleFSMMgr.currRoleStateEnum != RoleState.Attack)
        {
            //怪若已经死亡则换人
            if (currentRole.LockEnemy.CurrentRoleInfo.CurrHP <= 0)
            {
                currentRole.LockEnemy = null;
                return;
            }
            
            if (currentRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
            {
                ///是否进行技能使用
                if (currentRole.Attack.FollowSkillId > 0)
                {
                    //使用后续技能
                    PlayerCtrl.Instance.OnSkillClick(currentRole.Attack.FollowSkillId);
                    currentRole.ToAttack(RoleAttackType.SkillAttack, currentRole.Attack.FollowSkillId);
                }
                else
                {
                    //使用普攻
                    int skillId = currentRole.CurrentRoleInfo.PhySkillIds[m_PhyIndex];
                    currentRole.ToAttack(RoleAttackType.PhyAttack, skillId);
                    m_PhyIndex++;
                    if (m_PhyIndex >= currentRole.CurrentRoleInfo.PhySkillIds.Length)
                    {
                        m_PhyIndex = 0;
                    }
                }
            }
        }
    }
    #endregion
}
