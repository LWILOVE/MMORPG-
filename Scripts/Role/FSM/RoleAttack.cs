using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色攻击
/// </summary>
[System.Serializable]
public class RoleAttack 
{
    /// <summary>
    /// 角色状态机
    /// </summary>
    private RoleFSMMgr m_CurrentRoleFSMMgr = null;
    /// <summary>
    /// 角色控制器
    /// </summary>
    private RoleCtrl m_CurrentRoleCtrl = null;
    /// <summary>
    /// 敌人的列表
    /// </summary>
    private List<RoleCtrl> m_EnemyList = null;
    /// <summary>
    /// 搜索到的敌人列表
    /// </summary>
    private List<Collider> m_SearchList = null;

    /// <summary>
    /// 物理攻击信息链表（在外面自己赋值）
    /// </summary>
    public List<RoleAttackInfo> PhyAttackInfoList;

    /// <summary>
    /// 技能攻击信息链表（在外面自己赋值）
    /// </summary>
    public List<RoleAttackInfo> SkillInfoList;
    public void SetFSM(RoleFSMMgr fsm)
    {
        m_CurrentRoleFSMMgr = fsm;
        m_CurrentRoleCtrl = m_CurrentRoleFSMMgr.currRoleCtrl;
        m_EnemyList = new List<RoleCtrl>();
        m_SearchList = new List<Collider>();
        //PhyAttackInfoList = new List<RoleAttackInfo>();
        //SkillInfoList=new List<RoleAttackInfo>();
    }

    private RoleStateAttack m_RoleStateAttack;
    /// <summary>
    /// 后续技能编号
    /// </summary>
    public int FollowSkillId { get { return m_FollowSkillId; } }
    private int m_FollowSkillId;

    /// <summary>
    /// 是否自动战斗
    /// </summary>
    [HideInInspector]
    public bool IsAutoFight;

    /// <summary>
    /// 指定角色特效的专属路径
    /// </summary>
    public string EffectPath;

    /// <summary>
    /// 根据索引号获取攻击信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private RoleAttackInfo GetRoleAttackInfoByIndex(RoleAttackType type, int index)
    {
        if (type == RoleAttackType.PhyAttack)
        {
            for (int i = 0; i < PhyAttackInfoList.Count; i++)
            {
                if (PhyAttackInfoList[i].Index == index)
                {
                    return PhyAttackInfoList[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < SkillInfoList.Count; i++)
            {
                if (SkillInfoList[i].Index == index)
                {
                    return SkillInfoList[i];
                }
            }
        }
        return null;
    }


    public void ToAttackByIndex(RoleAttackType type, int index)
    {
#if DEBUG_ROLESTATE
        if (m_CurrentRoleFSMMgr == null || m_CurrentRoleFSMMgr.currRoleCtrl.IsRigidity)
        { return; }
        //获取特效名
        RoleAttackInfo info = GetRoleAttackInfoByIndex(type, index);
        if (info != null)
        {
            m_CurrentRoleFSMMgr.currRoleCtrl.CurrAttackInfo = info;
            GameObject obj = Object.Instantiate(info.EffectObject);
            obj.transform.position = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.position;
            obj.transform.rotation = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.rotation;
            Object.Destroy(obj, info.EffectLiftTime);
        }

        //震屏效果
        if (info.IsDOCameraShake && CameraManager.Instance != null)
        {
            CameraManager.Instance.CameraShake(info.CameraShakeDelay);
        }


        if (m_RoleStateAttack == null)
        {
            //获取攻击状态：使得我们能够方便地对攻击状态机中的参数进行修改
            m_RoleStateAttack = m_CurrentRoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
        }
        m_RoleStateAttack.AnimatorCondition = string.Format(type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill");
        m_RoleStateAttack.AnimationConditionValue = index;
        m_RoleStateAttack.AnimatorCurrentState = GameUtil.GetRoleAnimatorState(type, index);
        //Debug.Log(m_RoleStateAttack.AnimatorCondition + " " + m_RoleStateAttack.AnimationConditionValue + " " + m_RoleStateAttack.AnimatorCurrentState);
        //切换至攻击状态
        m_CurrentRoleFSMMgr.ChangeState(RoleState.Attack);
#endif
    }

    /// <summary>
    /// 发起攻击(返回值表示使用技能的成功与否)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    public bool ToAttack(RoleAttackType type, int skillId)
    {
        if (m_CurrentRoleFSMMgr == null || m_CurrentRoleFSMMgr.currRoleCtrl.IsRigidity)
        {
            if (type == RoleAttackType.SkillAttack)
            {
                //设置后续技能编号
                m_FollowSkillId = skillId;
            }
            return false; 
        }
        m_FollowSkillId = -1;
        #region 攻击的数值
        //角色类型：只有主角和怪物才有伤害类型计算的问题 
        if (m_CurrentRoleCtrl.CurrRoleType == RoleType.MainPlayer || m_CurrentRoleCtrl.CurrRoleType == RoleType.Monster)
        {
            //获取技能信息
            SkillEntity skillEntity = SkillDBModel.Instance.Get(skillId);
            if (skillEntity == null)
            { return false; }
            int skillLevel = m_CurrentRoleCtrl.CurrentRoleInfo.GetSkillLevel(skillId);
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndLevel(skillId, skillLevel);
            //检测主角是否有能量放技能
            if (m_CurrentRoleCtrl.CurrRoleType == RoleType.MainPlayer || m_CurrentRoleCtrl.CurrRoleType == RoleType.OtherPlayer)
            {
                if (type == RoleAttackType.SkillAttack)
                {
                    if (skillLevelEntity.SpendMP > m_CurrentRoleCtrl.CurrentRoleInfo.CurrMP)
                    {
                        Debug.Log("能量不足：" + skillLevelEntity.SpendMP);
                        return false;
                    }
                    else
                    {
                        m_CurrentRoleCtrl.CurrentRoleInfo.CurrMP -= skillLevelEntity.SpendMP;
                        if (m_CurrentRoleCtrl.CurrentRoleInfo.CurrMP < 0)
                        {
                            m_CurrentRoleCtrl.CurrentRoleInfo.CurrMP = 0;
                        }
                        if (m_CurrentRoleCtrl.OnMPChange != null)
                        {
                            m_CurrentRoleCtrl.OnMPChange(ValueChangeType.Subtract);
                        }
                    }
                } 
            }

            #region 索敌逻辑
            //进行索敌,获取敌人的名单m_EnemyList
            m_EnemyList.Clear();
            //只有主角进行攻击时是需要自己索敌的，怪自己的AI逻辑能索敌
            if (m_CurrentRoleCtrl.CurrRoleType == RoleType.MainPlayer)
            {
                #region 进行攻击索敌
                int attackTargetCount = skillEntity.AttackTargetCount;
                if (attackTargetCount == 1)
                {
                    #region 进行单体攻击索敌检测
                    //进行单体攻击
                    if (m_CurrentRoleCtrl.LockEnemy != null)
                    {
                        //若已经有锁定的敌人了，就添加到敌人列表中
                        m_EnemyList.Add(m_CurrentRoleCtrl.LockEnemy);
                    }
                    else
                    {
                        //若无锁定敌人，则进行球形判定检测
                        Collider[] searchList = Physics.OverlapSphere(m_CurrentRoleCtrl.gameObject.transform.position, skillEntity.AreaAttackRadius, 1 << LayerMask.NameToLayer("Player"));
                        m_SearchList.Clear();
                        if (searchList != null && searchList.Length > 0)
                        {
                            for (int i = 0; i < searchList.Length; i++)
                            {
                                RoleCtrl ctrl = searchList[i].GetComponent<RoleCtrl>();
                                if (ctrl != null)
                                {
                                    if (ctrl.CurrentRoleInfo.RoldId != m_CurrentRoleCtrl.CurrentRoleInfo.RoldId)
                                    {
                                        m_SearchList.Add(searchList[i]);
                                    }
                                }
                            }
                        }
                        //对检测到的敌人进行排序，攻击离自己最近的敌人
                        m_SearchList.Sort((Collider c1, Collider c2) =>
                        {
                            int ret = 0;
                            if (Vector3.Distance(c1.gameObject.transform.position, m_CurrentRoleCtrl.gameObject.transform.position) <
                           Vector3.Distance(c2.gameObject.transform.position, m_CurrentRoleCtrl.gameObject.transform.position))
                            {
                                ret = -1;
                            }
                            else
                            {
                                ret = 1;
                            }
                            return ret;
                        });
                        m_CurrentRoleCtrl.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();
                        m_EnemyList.Add(m_CurrentRoleCtrl.LockEnemy);
                    }
                    #endregion
                }
                else
                {
                    #region 进行群体攻击索敌检测
                    //需要攻击的敌人量
                    int needAttack = attackTargetCount;
                    //进行群体攻击
                    Collider[] searchList = Physics.OverlapSphere(m_CurrentRoleCtrl.gameObject.transform.position, skillEntity.AreaAttackRadius, 1 << LayerMask.NameToLayer("Player"));
                    m_SearchList.Clear();
                    if (searchList != null && searchList.Length > 0)
                    {
                        for (int i = 0; i < searchList.Length; i++)
                        {
                            RoleCtrl ctrl = searchList[i].GetComponent<RoleCtrl>();
                            if (ctrl != null)
                            {
                                if (ctrl.CurrentRoleInfo.RoldId != m_CurrentRoleCtrl.CurrentRoleInfo.RoldId)
                                {
                                    m_SearchList.Add(searchList[i]);
                                }
                            }
                        }
                    }
                    //对检测到的敌人进行排序，攻击离自己最近的敌人
                    m_SearchList.Sort((Collider c1, Collider c2) =>
                    {
                        int ret = 0;
                        if (Vector3.Distance(c1.gameObject.transform.position, m_CurrentRoleCtrl.gameObject.transform.position) <
                       Vector3.Distance(c2.gameObject.transform.position, m_CurrentRoleCtrl.gameObject.transform.position))
                        {
                            ret = -1;
                        }
                        else
                        {
                            ret = 1;
                        }
                        return ret;
                    });
                    //被锁定的敌人必然会受到攻击
                    if (m_CurrentRoleCtrl.LockEnemy != null)
                    {
                        m_EnemyList.Add(m_CurrentRoleCtrl.LockEnemy);
                        needAttack--;
                        //计算其他需要伤害的敌人
                        for (int i = 0; i < m_SearchList.Count; i++)
                        {
                            RoleCtrl ctrl = m_SearchList[i].GetComponent<RoleCtrl>();
                            if (ctrl.CurrentRoleInfo.RoldId != m_CurrentRoleCtrl.LockEnemy.CurrentRoleInfo.RoldId && ctrl.CurrRoleType != RoleType.MainPlayer)
                            {
                                if ((i + 1) > needAttack)
                                { break; }
                                //向列表中加入敌人
                                m_EnemyList.Add(ctrl);
                            }
                        }
                    }
                    else
                    {
                        if (m_SearchList.Count > 0)
                        {
                            m_CurrentRoleCtrl.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();
                            //计算需要伤害的敌人
                            for (int i = 0; i < m_SearchList.Count; i++)
                            {
                                RoleCtrl ctrl = m_SearchList[i].GetComponent<RoleCtrl>();
                                if (ctrl.CurrRoleType != RoleType.MainPlayer)
                                {
                                    if ((i + 1) > needAttack)
                                    { break; }
                                    //向列表中加入敌人
                                    m_EnemyList.Add(ctrl);
                                }
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else if(m_CurrentRoleCtrl.CurrRoleType == RoleType.Monster)
            {
                if (m_CurrentRoleCtrl.LockEnemy)
                {
                    m_EnemyList.Add(m_CurrentRoleCtrl.LockEnemy);
                }
            }
            #endregion
            
            #region 伤害逻辑
            //进行PVE OR PVP 伤害计算
            if (m_EnemyList.Count >= 0)
            {
                //PVE
                if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVE)
                {
                    //对怪物造成实质伤害
                    for (int i = 0; i < m_EnemyList.Count; i++)
                    {
                        RoleTransferAttackInfo roleTransferAttackInfo = CalculateHurtValue(m_EnemyList[i], skillLevelEntity);
                        m_EnemyList[i].ToHurt(roleTransferAttackInfo);
                    }
                }
                else if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVP)
                {
                    WorldMap_CurrRoleUseSkillProto proto = new WorldMap_CurrRoleUseSkillProto();
                    proto.SkillId = skillId;
                    proto.SkillLevel = skillLevel;
                    proto.RolePosX = m_CurrentRoleCtrl.transform.position.x;
                    proto.RolePosY = m_CurrentRoleCtrl.transform.position.y;
                    proto.RolePosZ = m_CurrentRoleCtrl.transform.position.z;
                    proto.RoleYAngle = m_CurrentRoleCtrl.transform.localEulerAngles.y;
                    
                    proto.BeAttackCount = m_EnemyList.Count;//受击者数量
                    proto.ItemList = new List<WorldMap_CurrRoleUseSkillProto.BeAttackItem>();

                    for (int i = 0; i < m_EnemyList.Count; i++)
                    {
                        proto.ItemList.Add(new WorldMap_CurrRoleUseSkillProto.BeAttackItem()
                        {
                            BeAttackRoleId = m_EnemyList[i].CurrentRoleInfo.RoldId
                        });
                    }
                    Debug.LogWarning("玩家使用技能");
                    NetWorkSocket.Instance.SendMessage(proto.ToArray());
                }
            }
        }
        #endregion

        ////若在PVE环境下，则播放攻击动画，若是PVP则先发消息通知服务器玩家要放动画了，再让服务器通知客户端使用技能TODO
        //if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVE)
        //{
        ////向服务器发送玩家使用技能信息
        //if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVP)
        //{
        //    SendPVPAttack(skillId);   
        //}
        PlayAttack(skillId);
        //}
        return true;
        #endregion
    }

    /// <summary>
    /// 播放攻击动画
    /// </summary>
    /// <param name="skillId"></param>
    public void PlayAttack(int skillId)
    {
        RoleAttackType type = SkillDBModel.Instance.Get(skillId).IsPhyAttack == 1 ? RoleAttackType.PhyAttack : RoleAttackType.SkillAttack;
        //获取特效名
        RoleAttackInfo info = GetRoleAttackInfo(type, skillId);
        //从AssetBundle中加载特效
        if (info == null)
        {
            Debug.Log("是不是特效忘记放了");
            return;
        }

        ////播放技能音效和角色叫声
        //m_CurrentRoleFSMMgr.currRoleCtrl.PlayAudio(info.FireAudio.AudioClipName,info.FireAudio.DelayTime);
        //m_CurrentRoleFSMMgr.currRoleCtrl.PlayAudio(info.AttactRoleAudio.AudioClipName,info.AttactRoleAudio.DelayTime);

        m_CurrentRoleFSMMgr.currRoleCtrl.CurrAttackInfo = info;
        EffectMgr.Instance.PlayEffect(EffectPath,info.EffectName,
            (Transform trans)=>
            {
                trans.transform.position = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.position;
                trans.transform.rotation = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.rotation;

                EffectMgr.Instance.DestroyEffect(trans, info.EffectLiftTime);
                //震屏效果
                if (info.IsDOCameraShake && CameraManager.Instance != null)
                {
                    CameraManager.Instance.CameraShake(info.CameraShakeDelay);
                }

                if (m_RoleStateAttack == null)
                {
                    //获取攻击状态：使得我们能够方便地对攻击状态机中的参数进行修改
                    m_RoleStateAttack = m_CurrentRoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
                }
                m_RoleStateAttack.AnimatorCondition = string.Format(type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill");
                m_RoleStateAttack.AnimationConditionValue = info.Index;
                m_RoleStateAttack.AnimatorCurrentState = GameUtil.GetRoleAnimatorState(type, info.Index);
                Debug.Log(m_RoleStateAttack.AnimatorCondition + " " + m_RoleStateAttack.AnimationConditionValue + " " + m_RoleStateAttack.AnimatorCurrentState);
                //切换至攻击状态
                m_CurrentRoleFSMMgr.ChangeState(RoleState.Attack);
            });
    }

    /// <summary>
    /// 根据技能ID获取技能信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    /// <returns></returns>
    private RoleAttackInfo GetRoleAttackInfo(RoleAttackType type, int skillId)
    {
        Debug.Log("正在查找攻击：" + type.ToString() + " ID:" + skillId);

        if (type == RoleAttackType.PhyAttack)
        {
            for (int i = 0; i < PhyAttackInfoList.Count; i++)
            {
                if (PhyAttackInfoList[i].SkillId == skillId)
                {
                    return PhyAttackInfoList[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < SkillInfoList.Count; i++)
            {
                if (SkillInfoList[i].SkillId == skillId)
                {
                    return SkillInfoList[i];
                }
            }
        }
        return null;
    }

    /// <summary>
    /// 进行攻击伤害计算
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="skillLevel"></param>
    /// <returns></returns>
    private RoleTransferAttackInfo CalculateHurtValue(RoleCtrl enemy, SkillLevelEntity skillLevelEntity)
    {
        if (enemy == null || skillLevelEntity == null)
        {
            Debug.Log("伤害计算失败");
            return null;
        }
        SkillEntity skillEntity = SkillDBModel.Instance.Get(skillLevelEntity.SkillId);
        if (skillEntity == null)
        {
            Debug.Log("技能不存在");
            return null;
        }
        RoleTransferAttackInfo roleTransferAttackInfo = new RoleTransferAttackInfo();
        //获取信息
        //攻击方编号
        roleTransferAttackInfo.AttackRoleId = m_CurrentRoleCtrl.CurrentRoleInfo.RoldId;
        //攻击方位置
        roleTransferAttackInfo.AttackRolePos = m_CurrentRoleCtrl.gameObject.transform.position;
        //受击方编号
        roleTransferAttackInfo.BeAttackRoleId = enemy.CurrentRoleInfo.RoldId;
        //技能ID
        roleTransferAttackInfo.SkillId = skillEntity.Id;
        //技能等级
        roleTransferAttackInfo.SkillLevel = skillLevelEntity.Level;
        //是否附加异常状态
        roleTransferAttackInfo.IsAbnormal = skillEntity.AbnormalState == 1;
        //伤害显示延时时间
        roleTransferAttackInfo.HurtDelayTime = m_CurrentRoleCtrl.CurrAttackInfo.HurtDelayTime;
        
        //进行伤害计算
        //攻击值=攻击方攻击力*（技能倍率*0.01f）
        float attackValue = m_CurrentRoleCtrl.CurrentRoleInfo.Attack * (skillLevelEntity.HurtValueRate * 0.01f);
        //基本伤害值=攻击值*攻击值/（攻击值+敌人防御值）
        float baseHurt =attackValue*(attackValue / (attackValue + enemy.CurrentRoleInfo.Defense));
        //计算爆率 = 0.05f+(攻击方暴击/（攻击方暴击+受击方抗性）)*0.1f
        float cri = 0.05f + (m_CurrentRoleCtrl.CurrentRoleInfo.Cri / (m_CurrentRoleCtrl.CurrentRoleInfo.Cri + enemy.CurrentRoleInfo.Res)) * 0.1f;
        //规则限定：当爆率无法超过50%
        cri = cri > 0.5f ? 0.5f : cri;
        //暴击判定
        bool isCri = Random.Range(0f,1f)<=cri;
        //计算爆伤：1.5倍默认
        float criHurt = isCri ? 1.5f : 1f;
        //生成伤害随机值
        float random = Random.Range(0.9f,1.1f);
        //最终伤害计算：基础伤害*暴伤*随机值
        int hurtValue = Mathf.RoundToInt( baseHurt * criHurt * random);
        hurtValue = hurtValue < 1 ? 1 : hurtValue;
        roleTransferAttackInfo.HurtValue = hurtValue;
        roleTransferAttackInfo.IsCri = isCri;
        if(roleTransferAttackInfo == null)
        { Debug.Log("信息BUG"); }
        return roleTransferAttackInfo;
    }


}
