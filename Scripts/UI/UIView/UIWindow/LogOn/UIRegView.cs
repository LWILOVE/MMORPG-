using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegView : UIWindowViewBase
{
    /// <summary>
    /// �û��������
    /// </summary>
    public InputField txtUserName;
    /// <summary>
    /// ���������
    /// </summary>
    public InputField txtPwd;
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        //���ݰ�ť����ִ��Ч��
        switch (go.name)
        {
            case "Btn_Reg":
                UIDispatcher.Instance.Dispatch(ConstDefine.UIRegView_Btn_Reg);
                break;
            case "Btn_ToLogOn":
                UIDispatcher.Instance.Dispatch(ConstDefine.UIRegView_Btn_ToLogOn);
                break;
        };
    }
}

