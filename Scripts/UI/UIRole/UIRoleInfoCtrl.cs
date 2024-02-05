using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色信息窗口控制器
/// </summary>
public class UIRoleInfoCtrl : UIWindowViewBase
{
    public void OnBtnClickClose()
    {
        this.gameObject.SetActive(false);
    }

}
