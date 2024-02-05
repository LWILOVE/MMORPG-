using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ��¼����UI��ͼ
/// </summary>
public class UILogOnView : UIWindowViewBase
{
    public System.Action OnBtnLogOnClick;
    /// <summary>
    /// �û���
    /// </summary>
    public InputField txtUserName;
    /// <summary>
    /// ����
    /// </summary>
    public InputField txtPwd;
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        //���ݰ�ť����ִ��Ч��
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
        //����Tab��������
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
            // ������������û�н��㣬���ȡ����
            txtUserName.ActivateInputField();
        }
    }
}
