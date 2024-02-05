using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 登录窗口UI视图
/// </summary>
public class UILogOnView : UIWindowViewBase
{
    public System.Action OnBtnLogOnClick;
    /// <summary>
    /// 用户名
    /// </summary>
    public InputField txtUserName;
    /// <summary>
    /// 密码
    /// </summary>
    public InputField txtPwd;
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        //根据按钮功能执行效果
        switch (go.name)
        {
            case "Btn_LogOn":
                UIDispatcher.Instance.Dispatch(ConstDefine.UILogOnView_Btn_LogOn);               
                break;
            case "Btn_ToReg":
                UIDispatcher.Instance.Dispatch(ConstDefine.UILogOnView_Btn_ToReg);
                break;
        };
    }
    private void Update()
    {
        //输入Tab更换焦点
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchFocus();
        }
    } 
    private void SwitchFocus()
    {
        if (txtUserName.isFocused)
        {
            txtPwd.ActivateInputField();
        }
        else
        {
            // 如果密码输入框没有焦点，则获取焦点
            txtUserName.ActivateInputField();
        }
    }
}
