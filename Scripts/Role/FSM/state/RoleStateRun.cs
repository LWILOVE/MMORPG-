using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 跑动状态
/// </summary>
public class RoleStateRun : RoleStateAbstract
{
    //角色转身速度
    public float m_RotationSpeed = 0.2f;
    //角色转身的目标方位
    public Quaternion m_TargetQuaternion;
    /// <summary>
    /// 移动速度
    /// </summary>
    private float m_MoveSpeed = 0f;
    public RoleStateRun(RoleFSMMgr roleFSMMgr) : base(roleFSMMgr)
    {

    }
    /// <summary>
    /// 实现基类 进入状态
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToRun.ToString(), true);
    }
    /// <summary>
    /// 实现基类 执行状态
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        //获取当前的动画状态信息
        CurrRoleAnimatorStateInfo = CurrRoleFSMMgr.currRoleCtrl.Animator.GetCurrentAnimatorStateInfo(0);
        if (CurrRoleAnimatorStateInfo.IsName(RoleAnimatorState.Run.ToString()))
        {
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), (int)RoleAnimatorState.Run);
        }
        else
        {
            //解决有时候形态不转换而发生平移的问题
            CurrRoleFSMMgr.currRoleCtrl.Animator.SetInteger(ToAnimatorCondition.CurrState.ToString(), 0);
        }

        //如果前面无路可行或者路径不可前进 则跳转至待机模式
        if (CurrRoleFSMMgr.currRoleCtrl.AStartPath == null)
        {
            CurrRoleFSMMgr.currRoleCtrl.ToIdle();
            return;
        }
        //若路径已经走完，则切换到待机模式
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
        
        //这一套代码可以解决大部分的问题，但是存在一个致命错误，即当移动距离极小时，并未发生形态切换，但是却发生了移动，此时无法切换回待机状态
        //解决方案：在外面的跑和走之间提供一条通路

        //角色移动的方向
        Vector3 direction = Vector3.zero;
        //临时目标路径点
        Vector3 temp = new Vector3(CurrRoleFSMMgr.currRoleCtrl.AStartPath.vectorPath[CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex].x
            ,CurrRoleFSMMgr.currRoleCtrl.gameObject.transform.position.y,
            CurrRoleFSMMgr.currRoleCtrl.AStartPath.vectorPath[CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex].z);

        //计算角色移动的方向
        direction = temp - CurrRoleFSMMgr.currRoleCtrl.gameObject.transform.position;

        //方向归一化，（目的是让人物移动更丝滑）    
        direction = direction.normalized;

        m_MoveSpeed = CurrRoleFSMMgr.currRoleCtrl.ModifySpeed > 0 ? CurrRoleFSMMgr.currRoleCtrl.ModifySpeed : CurrRoleFSMMgr.currRoleCtrl.Speed;

        //角色实际速度设置
        direction = direction * Time.deltaTime * m_MoveSpeed;
        direction.y = 0;

        //角色转身
        if (m_RotationSpeed <= 1.0f)
        {
            //转身速度慢慢变快
            m_RotationSpeed += 0.2f * Time.deltaTime;
            //转身方向
            m_TargetQuaternion = Quaternion.LookRotation(direction);
            //转身
            CurrRoleFSMMgr.currRoleCtrl.transform.rotation = Quaternion.Lerp(CurrRoleFSMMgr.currRoleCtrl.transform.rotation, m_TargetQuaternion, m_RotationSpeed);
            //角色转身时，角色的画布不应该跟着角色转而是一直呈现在玩家面前:即反着来
            if (CurrRoleFSMMgr.currRoleCtrl.m_RoleCanvas != null)
            {
                //注：在你没有完全理解四元数前个人极度不推荐使用四元数转换欧拉角，XYZW不是1:1转换的
                CurrRoleFSMMgr.currRoleCtrl.m_RoleCanvas.transform.localEulerAngles = CurrRoleFSMMgr.currRoleCtrl.transform.rotation.eulerAngles * -1;
            }
        }
        else
        {
            m_RotationSpeed = 0;
        }
        //判定角色是否应该向下一个点进行移动
        float dis = Vector3.Distance(CurrRoleFSMMgr.currRoleCtrl.transform.position,temp);
        //当到达临时目标点了，则进行换点
        if (dis <= (direction.magnitude + 0.1f))
        {
            CurrRoleFSMMgr.currRoleCtrl.AStartCurrWayPointIndex++;
        }

        //使用角色控制器控制角色移动
        CurrRoleFSMMgr.currRoleCtrl.m_CharacterController.Move(direction);

    }
    /// <summary>
    /// 实现基类 离开状态
    /// </summary>
    public override void OnLeave()
    {
        base.OnLeave();
        this.CurrRoleFSMMgr.currRoleCtrl.Animator.SetBool(ToAnimatorCondition.ToRun.ToString(), false);
    }

}


