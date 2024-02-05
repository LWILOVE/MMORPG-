using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色状态的抽象基类：不可实例化的类：抽象类
/// </summary>
public abstract class RoleStateAbstract
{
    /// <summary>
    /// 当前角色的有限状态机管理器
    /// </summary>
    public RoleFSMMgr CurrRoleFSMMgr { get; private set; }
    /// <summary>
    /// 当前的角色动画状态信息
    /// </summary>
    public AnimatorStateInfo CurrRoleAnimatorStateInfo { get; set; }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="roleFSMMgr">状态机需求：获取角色的FSM</param>
    public RoleStateAbstract(RoleFSMMgr roleFSMMgr)
    {
        CurrRoleFSMMgr = roleFSMMgr;
    }

    /// <summary>
    /// 动画状态是否切换完成
    /// </summary>
    protected bool IsChangeOver
    {
        get;set;
    }

    /// <summary>
    /// 角色进入状态
    /// </summary>
    public virtual void OnEnter() 
    {
        IsChangeOver = false;
    }
    /// <summary>
    /// 角色执行状态
    /// </summary>
    public virtual void OnUpdate() { }
    /// <summary>
    /// 角色离开状态
    /// </summary>
    public virtual void OnLeave() { }
}
