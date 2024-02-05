using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ����
/// </summary>
[System.Serializable]
public class RoleAttack 
{
    /// <summary>
    /// ��ɫ״̬��
    /// </summary>
    private RoleFSMMgr m_CurrentRoleFSMMgr = null;
    /// <summary>
    /// ��ɫ������
    /// </summary>
    private RoleCtrl m_CurrentRoleCtrl = null;
    /// <summary>
    /// ���˵��б�
    /// </summary>
    private List<RoleCtrl> m_EnemyList = null;
    /// <summary>
    /// �������ĵ����б�
    /// </summary>
    private List<Collider> m_SearchList = null;

    /// <summary>
    /// ��������Ϣ�����������Լ���ֵ��
    /// </summary>
    public List<RoleAttackInfo> PhyAttackInfoList;

    /// <summary>
    /// ���ܹ�����Ϣ�����������Լ���ֵ��
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
    /// �������ܱ��
    /// </summary>
    public int FollowSkillId { get { return m_FollowSkillId; } }
    private int m_FollowSkillId;

    /// <summary>
    /// �Ƿ��Զ�ս��
    /// </summary>
    [HideInInspector]
    public bool IsAutoFight;

    /// <summary>
    /// ָ����ɫ��Ч��ר��·��
    /// </summary>
    public string EffectPath;

    /// <summary>
    /// ���������Ż�ȡ������Ϣ
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
        //��ȡ��Ч��
        RoleAttackInfo info = GetRoleAttackInfoByIndex(type, index);
        if (info != null)
        {
            m_CurrentRoleFSMMgr.currRoleCtrl.CurrAttackInfo = info;
            GameObject obj = Object.Instantiate(info.EffectObject);
            obj.transform.position = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.position;
            obj.transform.rotation = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.rotation;
            Object.Destroy(obj, info.EffectLiftTime);
        }

        //����Ч��
        if (info.IsDOCameraShake && CameraManager.Instance != null)
        {
            CameraManager.Instance.CameraShake(info.CameraShakeDelay);
        }


        if (m_RoleStateAttack == null)
        {
            //��ȡ����״̬��ʹ�������ܹ�����ضԹ���״̬���еĲ��������޸�
            m_RoleStateAttack = m_CurrentRoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
        }
        m_RoleStateAttack.AnimatorCondition = string.Format(type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill");
        m_RoleStateAttack.AnimationConditionValue = index;
        m_RoleStateAttack.AnimatorCurrentState = GameUtil.GetRoleAnimatorState(type, index);
        //Debug.Log(m_RoleStateAttack.AnimatorCondition + " " + m_RoleStateAttack.AnimationConditionValue + " " + m_RoleStateAttack.AnimatorCurrentState);
        //�л�������״̬
        m_CurrentRoleFSMMgr.ChangeState(RoleState.Attack);
#endif
    }

    /// <summary>
    /// ���𹥻�(����ֵ��ʾʹ�ü��ܵĳɹ����)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    public bool ToAttack(RoleAttackType type, int skillId)
    {
        if (m_CurrentRoleFSMMgr == null || m_CurrentRoleFSMMgr.currRoleCtrl.IsRigidity)
        {
            if (type == RoleAttackType.SkillAttack)
            {
                //���ú������ܱ��
                m_FollowSkillId = skillId;
            }
            return false; 
        }
        m_FollowSkillId = -1;
        #region ��������ֵ
        //��ɫ���ͣ�ֻ�����Ǻ͹�������˺����ͼ�������� 
        if (m_CurrentRoleCtrl.CurrRoleType == RoleType.MainPlayer || m_CurrentRoleCtrl.CurrRoleType == RoleType.Monster)
        {
            //��ȡ������Ϣ
            SkillEntity skillEntity = SkillDBModel.Instance.Get(skillId);
            if (skillEntity == null)
            { return false; }
            int skillLevel = m_CurrentRoleCtrl.CurrentRoleInfo.GetSkillLevel(skillId);
            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndLevel(skillId, skillLevel);
            //��������Ƿ��������ż���
            if (m_CurrentRoleCtrl.CurrRoleType == RoleType.MainPlayer || m_CurrentRoleCtrl.CurrRoleType == RoleType.OtherPlayer)
            {
                if (type == RoleAttackType.SkillAttack)
                {
                    if (skillLevelEntity.SpendMP > m_CurrentRoleCtrl.CurrentRoleInfo.CurrMP)
                    {
                        Debug.Log("�������㣺" + skillLevelEntity.SpendMP);
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

            #region �����߼�
            //��������,��ȡ���˵�����m_EnemyList
            m_EnemyList.Clear();
            //ֻ�����ǽ��й���ʱ����Ҫ�Լ����еģ����Լ���AI�߼�������
            if (m_CurrentRoleCtrl.CurrRoleType == RoleType.MainPlayer)
            {
                #region ���й�������
                int attackTargetCount = skillEntity.AttackTargetCount;
                if (attackTargetCount == 1)
                {
                    #region ���е��幥�����м��
                    //���е��幥��
                    if (m_CurrentRoleCtrl.LockEnemy != null)
                    {
                        //���Ѿ��������ĵ����ˣ�����ӵ������б���
                        m_EnemyList.Add(m_CurrentRoleCtrl.LockEnemy);
                    }
                    else
                    {
                        //�����������ˣ�����������ж����
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
                        //�Լ�⵽�ĵ��˽������򣬹������Լ�����ĵ���
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
                    #region ����Ⱥ�幥�����м��
                    //��Ҫ�����ĵ�����
                    int needAttack = attackTargetCount;
                    //����Ⱥ�幥��
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
                    //�Լ�⵽�ĵ��˽������򣬹������Լ�����ĵ���
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
                    //�������ĵ��˱�Ȼ���ܵ�����
                    if (m_CurrentRoleCtrl.LockEnemy != null)
                    {
                        m_EnemyList.Add(m_CurrentRoleCtrl.LockEnemy);
                        needAttack--;
                        //����������Ҫ�˺��ĵ���
                        for (int i = 0; i < m_SearchList.Count; i++)
                        {
                            RoleCtrl ctrl = m_SearchList[i].GetComponent<RoleCtrl>();
                            if (ctrl.CurrentRoleInfo.RoldId != m_CurrentRoleCtrl.LockEnemy.CurrentRoleInfo.RoldId && ctrl.CurrRoleType != RoleType.MainPlayer)
                            {
                                if ((i + 1) > needAttack)
                                { break; }
                                //���б��м������
                                m_EnemyList.Add(ctrl);
                            }
                        }
                    }
                    else
                    {
                        if (m_SearchList.Count > 0)
                        {
                            m_CurrentRoleCtrl.LockEnemy = m_SearchList[0].GetComponent<RoleCtrl>();
                            //������Ҫ�˺��ĵ���
                            for (int i = 0; i < m_SearchList.Count; i++)
                            {
                                RoleCtrl ctrl = m_SearchList[i].GetComponent<RoleCtrl>();
                                if (ctrl.CurrRoleType != RoleType.MainPlayer)
                                {
                                    if ((i + 1) > needAttack)
                                    { break; }
                                    //���б��м������
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
            
            #region �˺��߼�
            //����PVE OR PVP �˺�����
            if (m_EnemyList.Count >= 0)
            {
                //PVE
                if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVE)
                {
                    //�Թ������ʵ���˺�
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
                    
                    proto.BeAttackCount = m_EnemyList.Count;//�ܻ�������
                    proto.ItemList = new List<WorldMap_CurrRoleUseSkillProto.BeAttackItem>();

                    for (int i = 0; i < m_EnemyList.Count; i++)
                    {
                        proto.ItemList.Add(new WorldMap_CurrRoleUseSkillProto.BeAttackItem()
                        {
                            BeAttackRoleId = m_EnemyList[i].CurrentRoleInfo.RoldId
                        });
                    }
                    Debug.LogWarning("���ʹ�ü���");
                    NetWorkSocket.Instance.SendMessage(proto.ToArray());
                }
            }
        }
        #endregion

        ////����PVE�����£��򲥷Ź�������������PVP���ȷ���Ϣ֪ͨ���������Ҫ�Ŷ����ˣ����÷�����֪ͨ�ͻ���ʹ�ü���TODO
        //if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVE)
        //{
        ////��������������ʹ�ü�����Ϣ
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
    /// ���Ź�������
    /// </summary>
    /// <param name="skillId"></param>
    public void PlayAttack(int skillId)
    {
        RoleAttackType type = SkillDBModel.Instance.Get(skillId).IsPhyAttack == 1 ? RoleAttackType.PhyAttack : RoleAttackType.SkillAttack;
        //��ȡ��Ч��
        RoleAttackInfo info = GetRoleAttackInfo(type, skillId);
        //��AssetBundle�м�����Ч
        if (info == null)
        {
            Debug.Log("�ǲ�����Ч���Ƿ���");
            return;
        }

        ////���ż�����Ч�ͽ�ɫ����
        //m_CurrentRoleFSMMgr.currRoleCtrl.PlayAudio(info.FireAudio.AudioClipName,info.FireAudio.DelayTime);
        //m_CurrentRoleFSMMgr.currRoleCtrl.PlayAudio(info.AttactRoleAudio.AudioClipName,info.AttactRoleAudio.DelayTime);

        m_CurrentRoleFSMMgr.currRoleCtrl.CurrAttackInfo = info;
        EffectMgr.Instance.PlayEffect(EffectPath,info.EffectName,
            (Transform trans)=>
            {
                trans.transform.position = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.position;
                trans.transform.rotation = m_CurrentRoleFSMMgr.currRoleCtrl.gameObject.transform.rotation;

                EffectMgr.Instance.DestroyEffect(trans, info.EffectLiftTime);
                //����Ч��
                if (info.IsDOCameraShake && CameraManager.Instance != null)
                {
                    CameraManager.Instance.CameraShake(info.CameraShakeDelay);
                }

                if (m_RoleStateAttack == null)
                {
                    //��ȡ����״̬��ʹ�������ܹ�����ضԹ���״̬���еĲ��������޸�
                    m_RoleStateAttack = m_CurrentRoleFSMMgr.GetRoleState(RoleState.Attack) as RoleStateAttack;
                }
                m_RoleStateAttack.AnimatorCondition = string.Format(type == RoleAttackType.PhyAttack ? "ToPhyAttack" : "ToSkill");
                m_RoleStateAttack.AnimationConditionValue = info.Index;
                m_RoleStateAttack.AnimatorCurrentState = GameUtil.GetRoleAnimatorState(type, info.Index);
                Debug.Log(m_RoleStateAttack.AnimatorCondition + " " + m_RoleStateAttack.AnimationConditionValue + " " + m_RoleStateAttack.AnimatorCurrentState);
                //�л�������״̬
                m_CurrentRoleFSMMgr.ChangeState(RoleState.Attack);
            });
    }

    /// <summary>
    /// ���ݼ���ID��ȡ������Ϣ
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    /// <returns></returns>
    private RoleAttackInfo GetRoleAttackInfo(RoleAttackType type, int skillId)
    {
        Debug.Log("���ڲ��ҹ�����" + type.ToString() + " ID:" + skillId);

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
    /// ���й����˺�����
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="skillLevel"></param>
    /// <returns></returns>
    private RoleTransferAttackInfo CalculateHurtValue(RoleCtrl enemy, SkillLevelEntity skillLevelEntity)
    {
        if (enemy == null || skillLevelEntity == null)
        {
            Debug.Log("�˺�����ʧ��");
            return null;
        }
        SkillEntity skillEntity = SkillDBModel.Instance.Get(skillLevelEntity.SkillId);
        if (skillEntity == null)
        {
            Debug.Log("���ܲ�����");
            return null;
        }
        RoleTransferAttackInfo roleTransferAttackInfo = new RoleTransferAttackInfo();
        //��ȡ��Ϣ
        //���������
        roleTransferAttackInfo.AttackRoleId = m_CurrentRoleCtrl.CurrentRoleInfo.RoldId;
        //������λ��
        roleTransferAttackInfo.AttackRolePos = m_CurrentRoleCtrl.gameObject.transform.position;
        //�ܻ������
        roleTransferAttackInfo.BeAttackRoleId = enemy.CurrentRoleInfo.RoldId;
        //����ID
        roleTransferAttackInfo.SkillId = skillEntity.Id;
        //���ܵȼ�
        roleTransferAttackInfo.SkillLevel = skillLevelEntity.Level;
        //�Ƿ񸽼��쳣״̬
        roleTransferAttackInfo.IsAbnormal = skillEntity.AbnormalState == 1;
        //�˺���ʾ��ʱʱ��
        roleTransferAttackInfo.HurtDelayTime = m_CurrentRoleCtrl.CurrAttackInfo.HurtDelayTime;
        
        //�����˺�����
        //����ֵ=������������*�����ܱ���*0.01f��
        float attackValue = m_CurrentRoleCtrl.CurrentRoleInfo.Attack * (skillLevelEntity.HurtValueRate * 0.01f);
        //�����˺�ֵ=����ֵ*����ֵ/������ֵ+���˷���ֵ��
        float baseHurt =attackValue*(attackValue / (attackValue + enemy.CurrentRoleInfo.Defense));
        //���㱬�� = 0.05f+(����������/������������+�ܻ������ԣ�)*0.1f
        float cri = 0.05f + (m_CurrentRoleCtrl.CurrentRoleInfo.Cri / (m_CurrentRoleCtrl.CurrentRoleInfo.Cri + enemy.CurrentRoleInfo.Res)) * 0.1f;
        //�����޶����������޷�����50%
        cri = cri > 0.5f ? 0.5f : cri;
        //�����ж�
        bool isCri = Random.Range(0f,1f)<=cri;
        //���㱬�ˣ�1.5��Ĭ��
        float criHurt = isCri ? 1.5f : 1f;
        //�����˺����ֵ
        float random = Random.Range(0.9f,1.1f);
        //�����˺����㣺�����˺�*����*���ֵ
        int hurtValue = Mathf.RoundToInt( baseHurt * criHurt * random);
        hurtValue = hurtValue < 1 ? 1 : hurtValue;
        roleTransferAttackInfo.HurtValue = hurtValue;
        roleTransferAttackInfo.IsCri = isCri;
        if(roleTransferAttackInfo == null)
        { Debug.Log("��ϢBUG"); }
        return roleTransferAttackInfo;
    }


}
