using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoleInfoView : UIWindowViewBase
{
    /// <summary>
    /// ��ɫװ��View
    /// </summary>
    [SerializeField]
    private UIRoleInfoEquipView m_UIRoleEquipView;

    /// <summary>
    /// ��ɫ����View
    /// </summary>
    [SerializeField]
    private UIRoleInfoDetailView m_UIRoleInfoDetailView;

    /// <summary>
    /// ���ý�ɫ��Ϣ
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
