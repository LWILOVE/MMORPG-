using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主城UIView
/// </summary>
public class UISceneMainCityView : UISceneViewBase
{
    /// <summary>
    /// 自动战斗容器
    /// </summary>
    [SerializeField]
    private GameObject AutoFightContainer;

    [SerializeField]
    /// <summary>
    /// 自动战斗按钮
    /// </summary>
    private GameObject BtnAutoFight;

    [SerializeField]
    /// <summary>
    /// 自动战斗取消按钮
    /// </summary>
    private GameObject BtnCancelAutoFight;
    /// <summary>
    /// 唤醒
    /// </summary>
    protected override void OnAwake()
    {
        base.OnAwake();
    }

    /// <summary>
    /// 开始
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();
        if (OnLoadComplete != null)
        {
            OnLoadComplete();
        }
        AutoFightContainer.SetActive(UILoadingCtrl.Instance.CurrentSceneType == SceneType.ShanGu);
    }

    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <param name="go"></param>
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "Btn_Ignore"://忽略
                ChangeMenuState(go);
                break;
            case "Btn_Role"://角色
                UIViewMgr.Instance.OpenWindow(WindowUIType.RoleInfo);
                break;
            case "Btn_WorldMap"://世界地图
                UIViewMgr.Instance.OpenWindow(WindowUIType.WorldMap);
                break;
            case "Btn_GameLevel"://关卡
                UIViewMgr.Instance.OpenWindow(WindowUIType.GameLevelMap);
                break;
            case "Btn_Recharge"://充值
                LuaHelper.Instance.LoadLuaView("RechargeCtrl");
                break;
            case "Btn_AutoFight"://自动战斗
                AutoFight(true);
                break;
            case "Btn_CancelAutoFight"://取消自动战斗
                AutoFight(false);
                break;
        }
    }

    /// <summary>
    /// 设置自动战斗
    /// </summary>
    /// <param name="isAutoFight"></param>
    private void AutoFight(bool isAutoFight)
    {
        BtnAutoFight.SetActive(!isAutoFight);
        BtnCancelAutoFight.SetActive(isAutoFight);
        //设置主角自动战斗状态
        GlobalInit.Instance.currentPlayer.Attack.IsAutoFight = isAutoFight;
    }

    /// <summary>
    /// 修改菜单显示状态
    /// </summary>
    /// <param name="go"></param>
    private void ChangeMenuState(GameObject go)
    {
        UIMainCityMenusView.Instance.ChangeState(() =>
        {
            //回调
            go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y * -1, go.transform.localScale.z);
        });
    }

    /// <summary>
    /// 销毁
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }
}
