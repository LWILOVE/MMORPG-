using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 区服进入窗口
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
        //根据按钮功能执行效果
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
