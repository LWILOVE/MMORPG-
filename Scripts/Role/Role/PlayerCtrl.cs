using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器
/// </summary>
public class PlayerCtrl : SystemCtrlBase<PlayerCtrl>, ISystemCtrl
{
    /// <summary>
    /// 角色信息视图
    /// </summary>
    private UIRoleInfoView m_UIRoleInfoView;

    /// <summary>
    /// 主角色信息
    /// </summary>
    private RoleInfoMainPlayer m_MainPlayerRoleInfo;

    /// <summary>
    /// 最后一次进入的世界地图ID
    /// </summary>
    public int LastInWorldMapId;
    /// <summary>
    /// 最后一次进入的世界地图坐标
    /// </summary>
    public string LastInWorldMapPos;

    public PlayerCtrl()
    {
        //添加监听
        //服务器返回元宝更新消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleData_MondeyChangeReturn,OnRoleDataMoneyChangeReturn);
        //服务器返回充值消息
        UIDispatcher.Instance.AddEventListener(ConstDefine.RechargeOK,OnRechargeOK);
    }

    #region 服务器 OnRoleDataMoneyChangeReturn OnRechargeOK
    /// <summary>
    /// 服务器返回元宝更新消息
    /// </summary>
    /// <param name="buffer"></param>
    private void OnRoleDataMoneyChangeReturn(byte[] buffer)
    {
        RoleData_MondeyChangeReturnProto proto = RoleData_MondeyChangeReturnProto.GetProto(buffer);
        GlobalInit.Instance.MainPlayerInfo.Money = proto.CurrMoney;

        if (UIMainCityRoleInfoView.Instance != null)
        {
            //刷新UI
            UIMainCityRoleInfoView.Instance.SetMoney(proto.CurrMoney);
        }
        //这里可以统计元宝的增删减改，便于后续进行打点统计
    }
    /// <summary>
    /// 充值成功后，刷新UI
    /// </summary>
    /// <param name="param"></param>
    private void OnRechargeOK(string[] param)
    {
        //TODO
        int money = param[0].ToInt();
    }
    #endregion

    /// <summary>
    /// 打开视图
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.RoleInfo:
                OpenRoleInfoView();
                break;
        }
    }

    #region 角色基本属性的初始化（服务器向） SetMainCityRoleData SetMainCityRoleInfo SetMainCityRoleSkillInfo
    /// <summary>
    /// 设置角色信息和技能信息
    /// </summary>
    public void SetMainCityRoleData()
    {
        SetMainCityRoleInfo();
        SetMainCityRoleSkillInfo();
    }

    /// <summary>
    /// 设置主城UI的角色信息
    /// </summary>
    private void SetMainCityRoleInfo()
    {
        m_MainPlayerRoleInfo = GlobalInit.Instance.currentPlayer.CurrentRoleInfo as RoleInfoMainPlayer;
        if (m_MainPlayerRoleInfo != null)
        {
            //角色头像  
            string headPic = string.Empty;
            JobEntity jobEntity = JobDBModel.Instance.Get(m_MainPlayerRoleInfo.JobId);

            if (jobEntity != null)
            {
                headPic = jobEntity.HeadPic;
            }

            GlobalInit.Instance.currentPlayer.OnHPChange = OnHPChangeCallBack;
            GlobalInit.Instance.currentPlayer.OnMPChange = OnMPChangeCallBack;
            UIMainCityRoleInfoView.Instance.SetUI(headPic, m_MainPlayerRoleInfo.RoleNickName, m_MainPlayerRoleInfo.Level,
                m_MainPlayerRoleInfo.Money, m_MainPlayerRoleInfo.Gold, m_MainPlayerRoleInfo.CurrHP, m_MainPlayerRoleInfo.MaxHP, m_MainPlayerRoleInfo.CurrMP, m_MainPlayerRoleInfo.MaxMP);
        }
        else
        {
            Debug.LogError("请检查角色信息是否设置");
        }

    }
    /// <summary>
    /// 设置主城UI上角色技能信息
    /// </summary>
    private void SetMainCityRoleSkillInfo()
    {
        RoleInfoMainPlayer mainPlayerRoleInfo = (RoleInfoMainPlayer)GlobalInit.Instance.currentPlayer.CurrentRoleInfo;
        List<TransferData> list = new List<TransferData>();
        for (int i = 0; i < mainPlayerRoleInfo.skillList.Count; i++)
        {
            TransferData data = new TransferData();
            data.SetValue(ConstDefine.SkillSlotsNo, mainPlayerRoleInfo.skillList[i].SlotsNo);
            data.SetValue(ConstDefine.SkillId, mainPlayerRoleInfo.skillList[i].SkillId);
            data.SetValue(ConstDefine.SkillLevel, mainPlayerRoleInfo.skillList[i].SkillLevel);

            SkillEntity entity = SkillDBModel.Instance.Get(mainPlayerRoleInfo.skillList[i].SkillId);
            if (entity != null)
            {
                data.SetValue(ConstDefine.SkillPic, entity.SkillPic);
            }

            SkillLevelEntity skillLevelEntity = SkillLevelDBModel.Instance.GetEntityBySkillIdAndLevel(mainPlayerRoleInfo.skillList[i].SkillId, mainPlayerRoleInfo.skillList[i].SkillLevel);
            if (skillLevelEntity != null)
            {
                data.SetValue(ConstDefine.SkillCDTime, skillLevelEntity.SkillCDTime);
            }
            list.Add(data);
        }
        UIMainCitySkillView.Instance.SetUI(list, OnSkillClick);

    }
    #endregion


    /// <summary>
    /// 打开角色信息窗口
    /// </summary>
    public void OpenRoleInfoView()
    {
        Debug.Log("正在为你打开角色信息面板");
        UIViewUtil.Instance.LoadWindow(WindowUIType.RoleInfo.ToString(), (GameObject obj) =>
        { 
            m_UIRoleInfoView = obj.GetComponent<UIRoleInfoView>();
            RoleInfoMainPlayer roleInfo = ((RoleInfoMainPlayer)GlobalInit.Instance.currentPlayer.CurrentRoleInfo);
            TransferData data = new TransferData();
            //基础信息
            data.SetValue(ConstDefine.JobId, roleInfo.JobId);
            data.SetValue(ConstDefine.NickName, roleInfo.RoleNickName);
            data.SetValue(ConstDefine.Level, roleInfo.Level);
            data.SetValue(ConstDefine.Fighting, roleInfo.Fighting);
            //资源信息
            data.SetValue(ConstDefine.Money, roleInfo.Money);
            data.SetValue(ConstDefine.Gold, roleInfo.Gold);
            data.SetValue(ConstDefine.CurrHP, roleInfo.CurrHP);
            data.SetValue(ConstDefine.MaxHP, roleInfo.MaxHP);
            data.SetValue(ConstDefine.CurrMP, roleInfo.CurrMP);
            data.SetValue(ConstDefine.MaxMP, roleInfo.MaxMP);
            data.SetValue(ConstDefine.CurrExp, roleInfo.CurrExp);
            data.SetValue(ConstDefine.Attack, roleInfo.Attack);
            data.SetValue(ConstDefine.Defense, roleInfo.Defense);
            data.SetValue(ConstDefine.Dodge, roleInfo.Dodge);
            data.SetValue(ConstDefine.Hit, roleInfo.Hit);
            data.SetValue(ConstDefine.Cri, roleInfo.Cri);
            data.SetValue(ConstDefine.Res, roleInfo.Res);
            if (data == null)
            {
                Debug.Log("数据BUG");
            }
            if (m_UIRoleInfoView == null)
            {
                Debug.Log("角色BUG");
            }
            m_UIRoleInfoView.SetRoleInfo(data);
        });
    }

    /// <summary>
    /// 技能按钮点击回调
    /// </summary>
    /// <param name="skillId"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnSkillClick(int skillId)
    {
        Debug.Log("你这是想放技能吗？");
        bool isSuccess = GlobalInit.Instance.currentPlayer.ToAttack(RoleAttackType.SkillAttack, skillId);
        if (isSuccess)
        {
            GlobalInit.Instance.currentPlayer.CurrentRoleInfo.SetSkillCDEndTime(skillId);
            //进入CD
            UIMainCitySkillView.Instance.BeginCD(skillId);
        }
    }

    #region 掉血和掉蓝 OnHPChangeCallBack OnMPChangeCallBack
    /// <summary>
    /// 血条变化委托回调
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnHPChangeCallBack(ValueChangeType type)
    {
        if (m_MainPlayerRoleInfo == null)
        { return; }
        UIMainCityRoleInfoView.Instance.SetHP(m_MainPlayerRoleInfo.CurrHP, m_MainPlayerRoleInfo.MaxHP);
    }
    /// <summary>
    /// 蓝条变化委托回调
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnMPChangeCallBack(ValueChangeType type)
    {
        if (m_MainPlayerRoleInfo == null)
        { return; }
        Debug.Log("我掉蓝了");
        UIMainCityRoleInfoView.Instance.SetMP(m_MainPlayerRoleInfo.CurrMP, m_MainPlayerRoleInfo.MaxMP);
    }
    #endregion

}
