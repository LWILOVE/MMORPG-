using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRegView : UIWindowViewBase
{
    /// <summary>
    /// 用户名输入框
    /// </summary>
    public InputField txtUserName;
    /// <summary>
    /// 密码输入框
    /// </summary>
    public InputField txtPwd;
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        //根据按钮功能执行效果
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

