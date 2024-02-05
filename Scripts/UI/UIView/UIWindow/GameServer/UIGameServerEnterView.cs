using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �������봰��
/// </summary>
public class UIGameServerEnterView : UIWindowViewBase
{
    public Text lblDefaultGameServer;

    public void SetUI(string gameServerName)
    {
        lblDefaultGameServer.text = gameServerName;
    }
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        //���ݰ�ť����ִ��Ч��
        switch (go.name)
        {
            case "Btn_SelectGameServer":
                UIDispatcher.Instance.Dispatch(ConstDefine.UIGameServerEnterView_Btn_SelectGameServer);
                break;
            case "Btn_EnterGame":
                UIDispatcher.Instance.Dispatch(ConstDefine.UIGameServerEnterView_Btn_EnterGame);
                break;
        };
    }
}
