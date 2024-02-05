using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有场景控制器的基类
/// </summary>
public class GameSceneCtrlBase : MonoBehaviour
{
    
    /// <summary>
    /// 角色主城类
    /// </summary>
    protected UISceneMainCityView m_MainCityView;

    private void Awake()
    {
        OnAwake();
        //检测角色是否有相机，有就不需要了
        if (CameraManager.Instance == null)
        {
            AssetBundleMgr.Instance.LoadOrDownload(string.Format("Download/Prefab/RolePrefab/Player/CameraFollowAndRotate.assetbundle"), "CameraFollowAndRotate",
                (GameObject obj)=>
                {
                    GameObject.Instantiate(obj);
                });
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        //加载主城UI
        UILoadingCtrl.Instance.LoadSceneUI(SceneUIType.MainCity, OnLoadUIMainCityViewComplete);
        //OnStart();
        //EffectMgr.Instance.Init(this);

    }

    /// <summary>
    /// 主城UI加载完成回调
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void OnLoadUIMainCityViewComplete(GameObject obj) 
    {
        m_MainCityView = obj.GetComponent<UISceneMainCityView>();
        OnStart();
        EffectMgr.Instance.Init(this);
        //服务器返回充值消息
        UIDispatcher.Instance.AddEventListener(ConstDefine.RechargeOK,OnRechargeOK);
    }

    private void OnRechargeOK(string[] param)
    {
        int money = param[0].ToInt();
        //更新玩家身上的元宝
        int oldMoney = GlobalInit.Instance.MainPlayerInfo.Money;
        int addMoney = money - oldMoney;
        GlobalInit.Instance.MainPlayerInfo.Money = money;
        GlobalInit.Instance.MainPlayerInfo.TotalRechargeMoney += addMoney;
        if (UIMainCityRoleInfoView.Instance != null)
        {
            //刷新UI
            UIMainCityRoleInfoView.Instance.SetMoney(money);
        }
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate();    
    }

    private void OnDestroy()
    {
        EffectMgr.Instance.Clear();
        BeforeOnDestroy();
    }

    private void OnRechargeReturn(byte[] buffer)
    {
        RoleData_RechargeReturnProto proto = RoleData_RechargeReturnProto.GetProto(buffer);
        //更新玩家身上的元宝
        GlobalInit.Instance.MainPlayerInfo.Money = proto.Money;
        if (UIMainCityRoleInfoView.Instance != null)
        {
            //刷新元宝UI
            UIMainCityRoleInfoView.Instance.SetMoney(proto.Money);
        }
    }

    protected virtual void OnAwake()
    { }
    protected virtual void OnStart()
    { }
    protected virtual void OnUpdate() 
    { }
    protected virtual void BeforeOnDestroy()
    { }
}
