using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI���ڹ�����
/// </summary>
public class UIViewMgr : SingletonMiddle<UIViewMgr>
{
    private Dictionary<WindowUIType,ISystemCtrl> m_SystemCtrlDic = new Dictionary<WindowUIType,ISystemCtrl>();

    /// <summary>
    /// ���캯��
    /// </summary>
    public UIViewMgr()
    {
        //������ע�ᵽUI���ڹ�����
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
    /// ��UI���ڵ�;������ʵ�֣�
    /// </summary>
    /// <param type="">��ͼ����</param>
    public void OpenWindow(WindowUIType type)
    {
        m_SystemCtrlDic[type].OpenView(type);
    }


}
