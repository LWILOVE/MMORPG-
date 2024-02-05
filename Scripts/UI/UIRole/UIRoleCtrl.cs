using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoleCtrl : UIWindowViewBase
{
    public GameObject GBT;
    /// <summary>
    /// ���ͷ��򿪽�ɫ��Ϣ����
    /// </summary>
    public void OnBtnClickOpenRoleInfo()
    {
        if (GBT == null)
        {
            Debug.Log("����һ������");
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
