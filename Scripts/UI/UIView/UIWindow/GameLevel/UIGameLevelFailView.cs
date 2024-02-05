using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家关卡战败UI
/// </summary>
public class UIGameLevelFailView : UIWindowViewBase
{
    /// <summary>
    /// 复活委托
    /// </summary>
    public Action OnResurgence;
    
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "Btn_Return":
                //失败回城也应该复活
                GlobalInit.Instance.currentPlayer.ToResurgence(RoleIdleState.IdleFight);
                UILoadingCtrl.Instance.LoadToWorldMap(PlayerCtrl.Instance.LastInWorldMapId);
                break;
            case "Btn_Resurgence":
                if (OnResurgence != null)
                {
                    OnResurgence();
                }
                break;
        }
    }
}
