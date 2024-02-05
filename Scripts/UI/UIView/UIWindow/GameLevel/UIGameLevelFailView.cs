using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ҹؿ�ս��UI
/// </summary>
public class UIGameLevelFailView : UIWindowViewBase
{
    /// <summary>
    /// ����ί��
    /// </summary>
    public Action OnResurgence;
    
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "Btn_Return":
                //ʧ�ܻس�ҲӦ�ø���
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
