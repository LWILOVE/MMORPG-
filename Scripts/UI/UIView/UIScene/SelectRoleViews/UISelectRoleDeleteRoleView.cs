using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectRoleDeleteRoleView : UIViewBase
{
    /// <summary>
    /// 输入OK的框
    /// </summary>
    [SerializeField]
    private InputField inputField;

    /// <summary>
    /// 提示信息
    /// </summary>
    [SerializeField]
    private Text lblTip;

    /// <summary>
    /// 移动目标点
    /// </summary>
    private Vector3 m_MoveTargetPos;

    private Action m_OnBtnOkClick;

    protected override void OnAwake()
    {
        base.OnAwake();
        //transform.localPosition = new Vector3(0, 1000, 0);
    }

    protected override void OnStart()
    {
        base.OnStart();
        //transform.DOLocalMove(Vector3.zero,0.2f).SetAutoKill(false).SetEase(GlobalInit.Instance.UIAnimationCurve).Pause();
    }

    protected override void OnBtnClick(GameObject go)
    {
        switch (go.name)
        {
            case "Btn_Close":
                Close();
                break;
            case "Btn_OK":
                OnBtnOkClick();
                break;
        }
    }

    public void Show(string nickName,Action OnBtnOkClick)
    {
        m_OnBtnOkClick = OnBtnOkClick;

        lblTip.text = String.Format("您确定要删除<color=#0002FF>{0}</color>吗?",nickName);
        //transform.DOPlayForward();
    }
    private void OnBtnOkClick()
    {
        if (string.IsNullOrEmpty(inputField.text) || !inputField.text.Equals("ok",System.StringComparison.CurrentCultureIgnoreCase))
        {
            MessageCtrl.Instance.Show("请输入OK删除角色");
            return;
        }

        if (m_OnBtnOkClick != null)
        {
            m_OnBtnOkClick();
        }
    }

    public void Close()
    {
        //transform.DOPlayBackwards();
        this.gameObject.SetActive(false);
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        inputField = null;
        lblTip = null;
        m_OnBtnOkClick = null;
    }
}
