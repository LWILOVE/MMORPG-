using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI窗口管理类
/// </summary>
public class UIViewMgr : SingletonMiddle<UIViewMgr>
{
    private Dictionary<WindowUIType,ISystemCtrl> m_SystemCtrlDic = new Dictionary<WindowUIType,ISystemCtrl>();

    /// <summary>
    /// 构造函数
    /// </summary>
    public UIViewMgr()
    {
        //将窗口注册到UI窗口管理器
        m_SystemCtrlDic.Add(WindowUIType.LogOn,AccountCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.Reg,AccountCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.GameServerEnter,GameServerCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.GameServerSelect,GameServerCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.RoleInfo, PlayerCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.GameLevelMap,GameLevelCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.GameLevelDetail,GameLevelCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.WorldMap,WorldMapCtrl.Instance);
        m_SystemCtrlDic.Add(WindowUIType.WorldMapFail, WorldMapCtrl.Instance);
    }

    /// <summary>
    /// 打开UI窗口的途径（非实现）
    /// </summary>
    /// <param type="">视图类型</param>
    public void OpenWindow(WindowUIType type)
    {
        m_SystemCtrlDic[type].OpenView(type);
    }


}
