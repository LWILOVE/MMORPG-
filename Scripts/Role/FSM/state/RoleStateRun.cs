using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �ܶ�״̬
/// </summary>
public class RoleStateRun : RoleStateAbstract
{
    //��ɫת���ٶ�
    public float m_RotationSpeed = 0.2f;
    //��ɫת���Ŀ�귽λ
    public Quaternion m_TargetQuaternion;
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    private float m_MoveSpeed = 0f;
    public RoleStateRun(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// ʵ�ֻ��� ����״̬
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToRun.ToString(), true);
    }
    /// <summary>
    /// ʵ�ֻ��� ִ��״̬
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        //��ȡ��ǰ�Ķ���״̬��Ϣ
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Run.ToString()))
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Run);
        }
        else
        {
            //�����ʱ����̬��ת��������ƽ�Ƶ�����
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
        }

        //���ǰ����·���л���·������ǰ�� ����ת������ģʽ
        if (CurrRoleFSMMgr.currRoleCtrl.AStartPath == null)
        {
            CurrRoleFSMMgr.currRoleCtrl.ToIdle();
            return;
        }
        //��·���Ѿ����꣬���л�������ģʽ
        if (CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex >= CurrRoleFSMMgr.currRoleCtrl.AStartPath.vectorPath.Count)
        {
            CurrRoleFSMMgr.currRoleCtrl.AStartPath = null;
            if (CurrRoleFSMMgr.currRoleCtrl.PrevFightTime==0||Time.time > (CurrRoleFSMMgr.currRoleCtrl.PrevFightTime + 10f))
            {
                CurrRoleFSMMgr.currRoleCtrl.ToIdle();
            }
            else
            {
                CurrRoleFSMMgr.currRoleCtrl.ToIdle(RoleIdleState.IdleFight);
            }
            return;
        }
        
        //��һ�״�����Խ���󲿷ֵ����⣬���Ǵ���һ���������󣬼����ƶ����뼫Сʱ����δ������̬�л�������ȴ�������ƶ�����ʱ�޷��л��ش���״̬
        //�����������������ܺ���֮���ṩһ��ͨ·

        //��ɫ�ƶ��ķ���
        Vector3 direction = Vector3.zero;
        //��ʱĿ��·����
        Vector3 temp = new Vector3(CurrRoleFSMMgr.currRoleCtrl.AStartPath.vectorPath[CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex].x
            ,CurrRoleFSMMgr.currRoleCtrl.gameObject.transform.position.y,
            CurrRoleFSMMgr.currRoleCtrl.AStartPath.vectorPath[CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex].z);

        //�����ɫ�ƶ��ķ���
        direction = temp - CurrRoleFSMMgr.currRoleCtrl.gameObject.transform.position;

        //�����һ������Ŀ�����������ƶ���˿����    
        direction = direction.normalized;

        m_MoveSpeed = CurrRoleFSMMgr.currRoleCtrl.ModifySpeed > 0 ? CurrRoleFSMMgr.currRoleCtrl.ModifySpeed : CurrRoleFSMMgr.currRoleCtrl.Speed;

        //��ɫʵ���ٶ�����
        direction = direction * Time.deltaTime * m_MoveSpeed;
        direction.y = 0;

        //��ɫת��
        if (m_RotationSpeed <= 1.0f)
        {
            //ת���ٶ��������
            m_RotationSpeed += 0.2f * Time.deltaTime;
            //ת����
            m_TargetQuaternion = Quaternion.LookRotation(direction);
            //ת��
            CurrRoleFSMMgr.currRoleCtrl.transform.rotation = Quaternion.Lerp(CurrRoleFSMMgr.currRoleCtrl.transform.rotation, m_TargetQuaternion, m_RotationSpeed);
            //��ɫת��ʱ����ɫ�Ļ�����Ӧ�ø��Ž�ɫת����һֱ�����������ǰ:��������
            if (CurrRoleFSMMgr.currRoleCtrl.m_RoleCanvas != null)
            {
                //ע������û����ȫ�����Ԫ��ǰ���˼��Ȳ��Ƽ�ʹ����Ԫ��ת��ŷ���ǣ�XYZW����1:1ת����
                CurrRoleFSMMgr.currRoleCtrl.m_RoleCanvas.transform.localEulerAngles = CurrRoleFSMMgr.currRoleCtrl.transform.rotation.eulerAngles * -1;
            }
        }
        else
        {
            m_RotationSpeed = 0;
        }
        //�ж���ɫ�Ƿ�Ӧ������һ��������ƶ�
        float dis = Vector3.Distance(CurrRoleFSMMgr.currRoleCtrl.transform.position,temp);
        //��������ʱĿ����ˣ�����л���
        if (dis <= (direction.magnitude + 0.1f))
        {
            CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex++;
        }

        //ʹ�ý�ɫ���������ƽ�ɫ�ƶ�
        CurrRoleFSMMgr.currRoleCtrl.m_CharacterController.Move(direction);

    }
    /// <summary>
    /// ʵ�ֻ��� �뿪״̬
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToRun.ToString(), false);
    }

}


