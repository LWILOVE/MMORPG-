using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��������AI
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
    /// �������ļ�������
    /// </summary>
    private int m_PhyIndex = 0;

    /// <summary>
    /// �������ĵ����б�
    /// </summary>
    public List<Collider> m_SearchList = null;

    private Vector3 m_MoveToPoint;
    private RaycastHit hitInfo;
    private Vector3 m_RayPoint;
    public void DoAI()
    {
        if (currentRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die)
        { return; }
        //����ɫ�����Զ�ս��״̬
        if (currentRole.Attack.IsAutoFight)
        {
            AutoFightState();
        }
        else
        {
            NormalState();
        }
    }

    #region AutoFightState �Զ�ս��״̬
    /// <summary>
    /// �Զ�ս��״̬
    /// �߼���
    /// �һ�����ɫ���ж���δͨ��=>
    /// ��ǰ�����й֣�
    /// ��ǰ�Ѿ����������ˣ�Ŀ������=>ȡ������,Ŀ��δ����=>����ɫ�ɽ��еĹ����ֶ�=>���Ŀ���Ƿ��ڹ�����Χ��=>�ڣ����𹥻������ڣ��ƶ���Ŀ�����
    /// ��ǰ���������ˣ�������=>����Ŀ��
    /// ��ǰ�����޹֣��Ѿ�ͨ�أ����ƶ�:�ƶ�����һ����
    /// </summary>
    private void AutoFightState()
    {
        if (currentRole.IsRigidity)
        {
            return;
         }

        //�ж���ǰ�����Ƿ��Ѿ���ͨ��
        if (!GameLevelSceneCtrl.Instance.CurrRegionHasMonster)
        {
            //���Ѿ��ǹص��ˣ��򷵻�
            if (GameLevelSceneCtrl.Instance.CurrRegionIsLast)
            {
                return;
            }
            else
            {
                //������һ����
                currentRole.MoveTo(GameLevelSceneCtrl.Instance.NextRegionPlayerBornPos);
            }

        }
        else
        {
            //���й��������빥��

            //����ҵ�����Զ�ս��������е��˼��
            if (currentRole.LockEnemy == null)
            {
                //����Ұ��Χ���������ˣ���������ĵ�����ΪҪ�����ĵ���
                //�����������ˣ�����������ж����
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
                                Debug.Log("��ӵ���");
                            }
                        }
                    }
                }
                //�Լ�⵽�ĵ��˽������򣬹������Լ�����ĵ���
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
                    Debug.Log("��ǰĿ�꣺" + m_SearchList[0].gameObject.name);
                }
            }
            else
            {

                //�����ǰ����������
                //�������Ķ������ˣ�������Ϊnull������
                if (currentRole.LockEnemy.CurrentRoleInfo.CurrHP <= 0)
                {
                    currentRole.LockEnemy = null;
                    return;
                }
                //���Ҫʹ�õļ���ID�뼼������
                int skillId = currentRole.CurrentRoleInfo.GetCanUsedSkillId();
                RoleAttackType type;
                if (skillId>0)
                {
                    //ʹ�ü���   
                    //���ü���ID
                    type = RoleAttackType.SkillAttack;
                }
                else
                {
                    //ʹ���չ�
                    //������ͨID
                    skillId = currentRole.CurrentRoleInfo.PhySkillIds[m_PhyIndex];
                    type = RoleAttackType.PhyAttack;
                    m_PhyIndex++;   
                    if (m_PhyIndex >= currentRole.CurrentRoleInfo.PhySkillIds.Length)
                    {
                        m_PhyIndex = 0;
                    }
                }
                //�ж������Ƿ��ڽ�ɫ������Χ֮��
                SkillEntity entity = SkillDBModel.Instance.Get(skillId);//���ݹ���ID��ȡ����ʵ��
                if (entity == null)
                { return; }
                //�����ڹ�����Χ��
                if (Vector3.Distance(currentRole.transform.position, currentRole.LockEnemy.transform.position) <= (entity.AttackRange-0.5f))
                {
                    if (type == RoleAttackType.SkillAttack)
                    {
                        PlayerCtrl.Instance.OnSkillClick(skillId);
                    }
                    else
                    {
                        //��������
                        currentRole.ToAttack(type, skillId);
                    }
                }
                else
                {
                    //��׷���ٹ���
                    if (currentRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
                    {
                        //��Ŀ���Բ��Χ���ҵ�
                        m_MoveToPoint = GameUtil.GetRandomPos(currentRole.transform.position, currentRole.LockEnemy.transform.position, entity.AttackRange);

                        //m_RayPoint = new Vector3(m_MoveToPoint.x, m_MoveToPoint.y + 50, m_MoveToPoint.z);
                        //if (Physics.Raycast(m_RayPoint, new Vector3(0, -100, 0), out hitInfo, 1000f, 1 << LayerMask.NameToLayer("RegionMask")))
                        //{
                        //    return;
                        //}

                        //�ƶ���������Χ�ڵ������
                        currentRole.MoveTo(m_MoveToPoint);
                    }
                }
            }
        }
    }
    #endregion

    #region NormalState ��ͨ״̬
    /// <summary>
    /// ��ͨ״̬
    /// </summary>
    private void NormalState()
    {
        if (currentRole.PrevFightTime != 0)
        {
            //�����ϴ�ս��ʱ�䳬��10�����л���ͨ����
            if (Time.time > (currentRole.PrevFightTime + 10f))
            {
                currentRole.ToIdle();
                currentRole.PrevFightTime = 0;
            }
        }

        //�����Ǻ����������ˣ�����й���
        if (currentRole.LockEnemy != null && currentRole.CurrentRoleFSMMgr.currRoleStateEnum != RoleState.Attack)
        {
            //�����Ѿ���������
            if (currentRole.LockEnemy.CurrentRoleInfo.CurrHP <= 0)
            {
                currentRole.LockEnemy = null;
                return;
            }
            
            if (currentRole.CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Idle)
            {
                ///�Ƿ���м���ʹ��
                if (currentRole.Attack.FollowSkillId > 0)
                {
                    //ʹ�ú�������
                    PlayerCtrl.Instance.OnSkillClick(currentRole.Attack.FollowSkillId);
                    currentRole.ToAttack(RoleAttackType.SkillAttack, currentRole.Attack.FollowSkillId);
                }
                else
                {
                    //ʹ���չ�
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
