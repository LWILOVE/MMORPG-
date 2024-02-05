using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoleInfoView : UIWindowViewBase
{
    /// <summary>
    /// 角色装备View
    /// </summary>
    [SerializeField]
    private UIRoleInfoEquipView m_UIRoleEquipView;

    /// <summary>
    /// 角色详情View
    /// </summary>
    [SerializeField]
    private UIRoleInfoDetailView m_UIRoleInfoDetailView;

    /// <summary>
    /// 设置角色信息
    /// </summary>
    public void SetRoleInfo(TransferData data)
    {
        m_UIRoleEquipView.SetUI(data);
        m_UIRoleInfoDetailView.SetUI(data);
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }

    
}
