using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���г����������Ļ���
/// </summary>
public class GameSceneCtrlBase : MonoBehaviour
{
    
    /// <summary>
    /// ��ɫ������
    /// </summary>
    protected UISceneMainCityView m_MainCityView;

    private void Awake()
    {
        OnAwake();
        //����ɫ�Ƿ���������оͲ���Ҫ��
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
        //��������UI
        UILoadingCtrl.Instance.LoadSceneUI(SceneUIType.MainCity, OnLoadUIMainCityViewComplete);
        //OnStart();
        //EffectMgr.Instance.Init(this);

    }

    /// <summary>
    /// ����UI������ɻص�
    /// </summary>
    /// <param name="obj"></param>
    protected virtual void OnLoadUIMainCityViewComplete(GameObject obj) 
    {
        m_MainCityView = obj.GetComponent<UISceneMainCityView>();
        OnStart();
        EffectMgr.Instance.Init(this);
        //���������س�ֵ��Ϣ
        UIDispatcher.Instance.AddEventListener(ConstDefine.RechargeOK,OnRechargeOK);
    }

    private void OnRechargeOK(string[] param)
    {
        int money = param[0].ToInt();
        //����������ϵ�Ԫ��
        int oldMoney = GlobalInit.Instance.MainPlayerInfo.Money;
        int addMoney = money - oldMoney;
        GlobalInit.Instance.MainPlayerInfo.Money = money;
        GlobalInit.Instance.MainPlayerInfo.TotalRechargeMoney += addMoney;
        if (UIMainCityRoleInfoView.Instance != null)
        {
            //ˢ��UI
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
        //����������ϵ�Ԫ��
        GlobalInit.Instance.MainPlayerInfo.Money = proto.Money;
        if (UIMainCityRoleInfoView.Instance != null)
        {
            //ˢ��Ԫ��UI
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
