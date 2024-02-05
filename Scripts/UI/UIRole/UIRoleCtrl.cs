using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoleCtrl : UIWindowViewBase
{
    public GameObject GBT;
    /// <summary>
    /// 点击头像打开角色信息窗口
    /// </summary>
    public void OnBtnClickOpenRoleInfo()
    {
        if (GBT == null)
        {
            Debug.Log("发现一坨垃圾");
            UIViewUtil.Instance.LoadWindow(WindowUIType.RoleInfo.ToString(),
                 (GameObject obj) =>
                 {
                     GBT = obj;
                 });
        }
        else
        {
            GBT.SetActive(true);
        }
    }
}
