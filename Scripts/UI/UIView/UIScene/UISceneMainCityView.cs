using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����UIView
/// </summary>
public class UISceneMainCityView : UISceneViewBase
{
    /// <summary>
    /// �Զ�ս������
    /// </summary>
    [SerializeField]
    private GameObject AutoFightContainer;

    [SerializeField]
    /// <summary>
    /// �Զ�ս����ť
    /// </summary>
    private GameObject BtnAutoFight;

    [SerializeField]
    /// <summary>
    /// �Զ�ս��ȡ����ť
    /// </summary>
    private GameObject BtnCancelAutoFight;
    /// <summary>
    /// ����
    /// </summary>
    protected override void OnAwake()
    {
        base.OnAwake();
    }

    /// <summary>
    /// ��ʼ
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
    /// ��ť���
    /// </summary>
    /// <param name="go"></param>
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "Btn_Ignore"://����
                ChangeMenuState(go);
                break;
            case "Btn_Role"://��ɫ
                UIViewMgr.Instance.OpenWindow(WindowUIType.RoleInfo);
                break;
            case "Btn_WorldMap"://�����ͼ
                UIViewMgr.Instance.OpenWindow(WindowUIType.WorldMap);
                break;
            case "Btn_GameLevel"://�ؿ�
                UIViewMgr.Instance.OpenWindow(WindowUIType.GameLevelMap);
                break;
            case "Btn_Recharge"://��ֵ
                LuaHelper.Instance.LoadLuaView("RechargeCtrl");
                break;
            case "Btn_AutoFight"://�Զ�ս��
                AutoFight(true);
                break;
            case "Btn_CancelAutoFight"://ȡ���Զ�ս��
                AutoFight(false);
                break;
        }
    }

    /// <summary>
    /// �����Զ�ս��
    /// </summary>
    /// <param name="isAutoFight"></param>
    private void AutoFight(bool isAutoFight)
    {
        BtnAutoFight.SetActive(!isAutoFight);
        BtnCancelAutoFight.SetActive(isAutoFight);
        //���������Զ�ս��״̬
        GlobalInit.Instance.currentPlayer.Attack.IsAutoFight = isAutoFight;
    }

    /// <summary>
    /// �޸Ĳ˵���ʾ״̬
    /// </summary>
    /// <param name="go"></param>
    private void ChangeMenuState(GameObject go)
    {
        UIMainCityMenusView.Instance.ChangeState(() =>
        {
            //�ص�
            go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y * -1, go.transform.localScale.z);
        });
    }

    /// <summary>
    /// ����
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
    }
}
