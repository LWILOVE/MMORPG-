using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[XLua.CSharpCallLua]
public class MessageCtrl : SystemCtrlBase<MessageCtrl>
{
    private UIMessageView m_UIMessageView;

    public void Show(string message, string title = "ÌבÊ¾", MessageViewType type = MessageViewType.Ok,
        DelegateDefine.OnMessageShow onShow = null, DelegateDefine.OnMessageOK onOk = null, DelegateDefine.OnMessageCancel onCancel = null, string path = "Pan_Message")
    {
        if (m_UIMessageView != null)
        {
            m_UIMessageView.Show(title, message, type, onShow, onOk, onCancel);
        }
        else
        {
            AssetBundleMgr.Instance.LoadOrDownload(String.Format("Download/Prefab/UIPrefab/UIWindow/{0}.assetbundle",path),path,
                (GameObject obj)=>
                {
                    GameObject m_MessageObj = UnityEngine.Object.Instantiate(obj);
                    m_MessageObj.transform.parent = UILoadingCtrl.Instance.CurrentUIScene.Container_Center;
                    m_MessageObj.transform.localPosition = Vector3.zero;
                    m_MessageObj.transform.localScale = Vector3.one;
                    m_MessageObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

                    m_UIMessageView = m_MessageObj.GetComponent<UIMessageView>();
                    if (m_UIMessageView != null)
                    {
                        m_UIMessageView.Show(title,message,type,onShow,onOk,onCancel);
                    }
                });
        }
    }
}
    
