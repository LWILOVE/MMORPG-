using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 小怪AI    
/// </summary>
public class GameLevel_RoleMonsterAI : IRoleAI
{
    #region 变量
    /// <summary>
    /// 巡逻间隔
    /// </summary>
    private float m_NextPatrolTime = 1f;
    /// <summary>
    /// 攻击间隔
    /// </summary>
    private float m_NextAttackTime = 1f;
    /// <summary>
    /// 怪的信息
    /// </summary>
    private RoleInfoMonster m_Info;
    /// <summary>
    /// 要使用的技能ID
    /// </summary>
    private int m_UsedSkillId = 0;
    /// <summary>
    /// 攻击类型
    /// </summary>
    private RoleAttackType m_RollAttackType;
    /// <summary>
    /// 要移动到的目标点
    /// </summary>
    private Vector3 m_MoveToPoint;

    /// <summary>
    /// 是否能够行走射线检测
    /// </summary>
    private RaycastHit hitInfo;

    /// <summary>
    /// 射线发射点
    /// </summary>
    private Vector3 m_RayPoint;

    /// <summary>
    /// 下次的思考时间
    /// </summary>
    private float m_NextThinkTime = 0f;

    /// <summary>
    /// 怪是否正在发呆
    /// </summary>
    private bool m_IsDaze;

    //当前角色控制器
    public RoleCtrl CurrRole
    {
        get;
        set;
    }
    #endregion
    public GameLevel_RoleMonsterAI(RoleCtrl roleCtrl, RoleInfoMonster info)
    {
        CurrRole = roleCtrl;
        m_Info = info;
    }
    public void DoAI()
    {
        //若当前玩家不存在则返回
        if (GlobalInit.Instance == null || GlobalInit.Instance.currentPlayer == null)
        { return; }
        //如果角色已经挂了，就不攻击了
        if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die || CurrRole.IsRigidity)
        { return; }
        if (CurrRole.LockEnemy == null)
        {
            //执行AI

            //若怪当前处于待机状态，则命令怪巡逻
            if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
            {
                if (Time.time > m_NextPatrolTime)
                {
                    m_NextPatrolTime = Time.time + UnityEngine.Random.Range(3f, 10f);
                    m_MoveToPoint = new Vector3(CurrRole.BornPoint.x + UnityEngine.Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange * 1),
                        CurrRole.BornPoint.y, CurrRole.BornPoint.z + UnityEngine.Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange * 1));
                    //怪巡逻
                    CurrRole.MoveTo(m_MoveToPoint);
                }
            }
            //搜索附近敌人,若玩家进入范围则锁定
            if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.currentPlayer.transform.position) < CurrRole.ViewRange*2)
            {
                CurrRole.LockEnemy = GlobalInit.Instance.currentPlayer;
                //下次攻击时刻=当前时刻+延迟攻击时间
                m_NextAttackTime = Time.time + m_Info.SpriteEntity.DelaySec_Attack;
            }
        }
        else
        {
            //如果锁定敌人死亡，则取消
            if (CurrRole.LockEnemy.CurrentRoleInfo.CurrHP <= 0)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            //小怪进行发呆
            if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(3f, 5f))
            {
                //让小怪进行休息
                CurrRole.ToIdle(RoleIdleState.IdleFight);
                m_NextThinkTime = Time.time;
                m_IsDaze = true;
            }

            if (m_IsDaze)
            {
                if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(0.5f, 1f))
                {
                    //让角色休息时间过长时，开始思考
                    m_IsDaze = false;
                }
                else
                {
                    //正在休息，不想动
                    return;
                }
            }

            //只有当处于待机状态时，怪才会继续追人
            if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum != RoleState.Idle)
            { return; }
            
            //若范围内有敌人
            //1.敌人在视野范围内：追击
            //2.敌人在视野范围外：取消锁定
            //当目标脱离视野范围时，取消锁定
            if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.currentPlayer.transform.position) >= CurrRole.ViewRange*2)
            {
                CurrRole.LockEnemy = null;
                return;
            }
            //若目标仍旧在范围中则追击OR攻击
            //获取怪将要使用的攻击手段
            //判定触发的攻击类型
            if (m_Info.SpriteEntity.PhysicalAttackRate >= UnityEngine.Random.Range(0, 100))
            {
                //触发物理攻击
                m_UsedSkillId = m_Info.SpriteEntity.UsePhyAttackArr[UnityEngine.Random.Range(0, m_Info.SpriteEntity.UsePhyAttackArr.Length)];
                m_RollAttackType = RoleAttackType.PhyAttack;
            }
            else
            {
                //触发技能攻击
                m_UsedSkillId = m_Info.SpriteEntity.UseSkillListArr[UnityEngine.Random.Range(0, m_Info.SpriteEntity.UseSkillListArr.Length)];
                m_RollAttackType = RoleAttackType.SkillAttack;
            }   
            //在技能列表中获取指定技能的攻击范围
            SkillEntity entity = SkillDBModel.Instance.Get(m_UsedSkillId);
            if (entity == null)
            { return; }
            //判定敌人是否在该技能的攻击范围之中，若在且CD到了则攻击，反之追击
            if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.currentPlayer.transform.position) <= entity.AttackRange)
            {
                //若角色已经进入攻击范围，则使怪朝向主角
                CurrRole.transform.LookAt(new Vector3(CurrRole.LockEnemy.transform.position.x, CurrRole.transform.position.y, CurrRole.LockEnemy.transform.position.z));
                if (Time.time > m_NextAttackTime && CurrRole.CurrentRoleFSMMgr.currRoleStateEnum != RoleState.Attack)
                {
                    m_NextAttackTime = Time.time + UnityEngine.Random.Range(0f, 2f) + m_Info.SpriteEntity.Attack_Interval;
                    CurrRole.ToAttack(m_RollAttackType, m_UsedSkillId);
                }
            }
            else
            {
                if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
                {
                    //在目标半圆范围内找点
                    m_MoveToPoint = GameUtil.GetRandomPos(CurrRole.transform.position, CurrRole.LockEnemy.transform.position, entity.AttackRange);
                    //移动到攻击范围内的任意点
                    CurrRole.MoveTo(m_MoveToPoint);
                }
            }
        }
    }

}
