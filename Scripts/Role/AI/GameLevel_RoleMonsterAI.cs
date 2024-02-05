using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// С��AI    
/// </summary>
public class GameLevel_RoleMonsterAI : IRoleAI
{
    #region ����
    /// <summary>
    /// Ѳ�߼��
    /// </summary>
    private float m_NextPatrolTime = 1f;
    /// <summary>
    /// �������
    /// </summary>
    private float m_NextAttackTime = 1f;
    /// <summary>
    /// �ֵ���Ϣ
    /// </summary>
    private RoleInfoMonster m_Info;
    /// <summary>
    /// Ҫʹ�õļ���ID
    /// </summary>
    private int m_UsedSkillId = 0;
    /// <summary>
    /// ��������
    /// </summary>
    private RoleAttackType m_RollAttackType;
    /// <summary>
    /// Ҫ�ƶ�����Ŀ���
    /// </summary>
    private Vector3 m_MoveToPoint;

    /// <summary>
    /// �Ƿ��ܹ��������߼��
    /// </summary>
    private RaycastHit hitInfo;

    /// <summary>
    /// ���߷����
    /// </summary>
    private Vector3 m_RayPoint;

    /// <summary>
    /// �´ε�˼��ʱ��
    /// </summary>
    private float m_NextThinkTime = 0f;

    /// <summary>
    /// ���Ƿ����ڷ���
    /// </summary>
    private bool m_IsDaze;

    //��ǰ��ɫ������
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
        //����ǰ��Ҳ������򷵻�
        if (GlobalInit.Instance == null || GlobalInit.Instance.currentPlayer == null)
        { return; }
        //�����ɫ�Ѿ����ˣ��Ͳ�������
        if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die || CurrRole.IsRigidity)
        { return; }
        if (CurrRole.LockEnemy == null)
        {
            //ִ��AI

            //���ֵ�ǰ���ڴ���״̬���������Ѳ��
            if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
            {
                if (Time.time > m_NextPatrolTime)
                {
                    m_NextPatrolTime = Time.time + UnityEngine.Random.Range(3f, 10f);
                    m_MoveToPoint = new Vector3(CurrRole.BornPoint.x + UnityEngine.Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange * 1),
                        CurrRole.BornPoint.y, CurrRole.BornPoint.z + UnityEngine.Random.Range(CurrRole.PatrolRange * -1, CurrRole.PatrolRange * 1));
                    //��Ѳ��
                    CurrRole.MoveTo(m_MoveToPoint);
                }
            }
            //������������,����ҽ��뷶Χ������
            if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.currentPlayer.transform.position) < CurrRole.ViewRange*2)
            {
                CurrRole.LockEnemy = GlobalInit.Instance.currentPlayer;
                //�´ι���ʱ��=��ǰʱ��+�ӳٹ���ʱ��
                m_NextAttackTime = Time.time + m_Info.SpriteEntity.DelaySec_Attack;
            }
        }
        else
        {
            //�������������������ȡ��
            if (CurrRole.LockEnemy.CurrentRoleInfo.CurrHP <= 0)
            {
                CurrRole.LockEnemy = null;
                return;
            }

            //С�ֽ��з���
            if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(3f, 5f))
            {
                //��С�ֽ�����Ϣ
                CurrRole.ToIdle(RoleIdleState.IdleFight);
                m_NextThinkTime = Time.time;
                m_IsDaze = true;
            }

            if (m_IsDaze)
            {
                if (Time.time > m_NextThinkTime + UnityEngine.Random.Range(0.5f, 1f))
                {
                    //�ý�ɫ��Ϣʱ�����ʱ����ʼ˼��
                    m_IsDaze = false;
                }
                else
                {
                    //������Ϣ�����붯
                    return;
                }
            }

            //ֻ�е����ڴ���״̬ʱ���ֲŻ����׷��
            if (CurrRole.CurrentRoleFSMMgr.currRoleStateEnum != RoleState.Idle)
            { return; }
            
            //����Χ���е���
            //1.��������Ұ��Χ�ڣ�׷��
            //2.��������Ұ��Χ�⣺ȡ������
            //��Ŀ��������Ұ��Χʱ��ȡ������
            if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.currentPlayer.transform.position) >= CurrRole.ViewRange*2)
            {
                CurrRole.LockEnemy = null;
                return;
            }
            //��Ŀ���Ծ��ڷ�Χ����׷��OR����
            //��ȡ�ֽ�Ҫʹ�õĹ����ֶ�
            //�ж������Ĺ�������
            if (m_Info.SpriteEntity.PhysicalAttackRate >= UnityEngine.Random.Range(0, 100))
            {
                //����������
                m_UsedSkillId = m_Info.SpriteEntity.UsePhyAttackArr[UnityEngine.Random.Range(0, m_Info.SpriteEntity.UsePhyAttackArr.Length)];
                m_RollAttackType = RoleAttackType.PhyAttack;
            }
            else
            {
                //�������ܹ���
                m_UsedSkillId = m_Info.SpriteEntity.UseSkillListArr[UnityEngine.Random.Range(0, m_Info.SpriteEntity.UseSkillListArr.Length)];
                m_RollAttackType = RoleAttackType.SkillAttack;
            }   
            //�ڼ����б��л�ȡָ�����ܵĹ�����Χ
            SkillEntity entity = SkillDBModel.Instance.Get(m_UsedSkillId);
            if (entity == null)
            { return; }
            //�ж������Ƿ��ڸü��ܵĹ�����Χ֮�У�������CD�����򹥻�����֮׷��
            if (Vector3.Distance(CurrRole.transform.position, GlobalInit.Instance.currentPlayer.transform.position) <= entity.AttackRange)
            {
                //����ɫ�Ѿ����빥����Χ����ʹ�ֳ�������
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
                    //��Ŀ���Բ��Χ���ҵ�
                    m_MoveToPoint = GameUtil.GetRandomPos(CurrRole.transform.position, CurrRole.LockEnemy.transform.position, entity.AttackRange);
                    //�ƶ���������Χ�ڵ������
                    CurrRole.MoveTo(m_MoveToPoint);
                }
            }
        }
    }

}
