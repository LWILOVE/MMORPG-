using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageView : MonoBehaviour
{
    /// <summary>
    /// 标题
    /// </summary>
    [SerializeField]
    private Text lblTitle;

    /// <summary>
    /// 内容
    /// </summary>
    [SerializeField]
    private Text lblMessage;

    /// <summary>
    /// 确定按钮
    /// </summary>
    [SerializeField]
    private Button btnOK;

    /// <summary>
    /// 取消按钮
    /// </summary>
    [SerializeField] 
    private Button btnCancel;

    /// <summary>
    /// 确定按钮回调
    /// </summary>
    public DelegateDefine.OnMessageOK OnOKClickHandler;

    /// <summary>
    /// 取消按钮回调
    /// </summary>
    public DelegateDefine.OnMessageCancel OnCancelHandler;

    private void Awake()
    {
        EventTriggerListener.Get(btnOK.gameObject).onClick = BtnOkClickCallBack;
        EventTriggerListener.Get(btnCancel.gameObject).onClick = BtnCancelClickCallBack;

    }

    /// <summary>
    /// 确定按钮点击
    /// </summary>
    /// <param name="go"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void BtnOkClickCallBack(GameObject go)
    {
        if (OnOKClickHandler != null)
        {
            OnOKClickHandler();
        }
        Close();
    }

    /// <summary>
    /// 取消按钮点击
    /// </summary>
    /// <param name="go"></param>
    private void BtnCancelClickCallBack(GameObject go)
    {
        if (OnCancelHandler != null)
        {
            OnCancelHandler();
        }
        Close();
    }

    private void Close()
    {
        gameObject.transform.localPosition = new Vector3(0, 5000, 0);
    }

    /// <summary>
    /// 显示窗口
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="message">内容</param>
    /// <param name="type">类型</param>
    /// <param name="okAction">确定回调</param>
    /// <param name="cancelAction">取消回调</param>
    public void Show(string title,string message,MessageViewType type = MessageViewType.Ok,
         DelegateDefine.OnMessageShow onShow = null, DelegateDefine.OnMessageOK onOk = null, DelegateDefine.OnMessageCancel onCancel = null)
    {
        if (onShow != null)
        {
            onShow();
        }

        gameObject.transform.localPosition = Vector3.zero;
        lblTitle.text = title;
        lblMessage.text = message;

        switch (type)
        {
            case MessageViewType.Ok:
                btnOK.transform.localPosition = Vector3.zero;
                btnCancel.gameObject.SetActive(false);
                break;
            case MessageViewType.OkAndCancel:
                btnOK.transform.localPosition = new Vector3(-75,0,0);
                btnCancel.gameObject.SetActive(true);
                break;
        }

        OnOKClickHandler = onOk;
        OnCancelHandler = onCancel;
    }


}
