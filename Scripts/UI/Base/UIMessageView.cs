using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMessageView : MonoBehaviour
{
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    private Text lblTitle;

    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    private Text lblMessage;

    /// <summary>
    /// ȷ����ť
    /// </summary>
    [SerializeField]
    private Button btnOK;

    /// <summary>
    /// ȡ����ť
    /// </summary>
    [SerializeField] 
    private Button btnCancel;

    /// <summary>
    /// ȷ����ť�ص�
    /// </summary>
    public DelegateDefine.OnMessageOK OnOKClickHandler;

    /// <summary>
    /// ȡ����ť�ص�
    /// </summary>
    public DelegateDefine.OnMessageCancel OnCancelHandler;

    private void Awake()
    {
        EventTriggerListener.Get(btnOK.gameObject).onClick = BtnOkClickCallBack;
        EventTriggerListener.Get(btnCancel.gameObject).onClick = BtnCancelClickCallBack;

    }

    /// <summary>
    /// ȷ����ť���
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
    /// ȡ����ť���
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
    /// ��ʾ����
    /// </summary>
    /// <param name="title">����</param>
    /// <param name="message">����</param>
    /// <param name="type">����</param>
    /// <param name="okAction">ȷ���ص�</param>
    /// <param name="cancelAction">ȡ���ص�</param>
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
