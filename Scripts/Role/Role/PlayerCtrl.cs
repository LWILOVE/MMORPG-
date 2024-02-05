using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ҿ�����
/// </summary>
public class PlayerCtrl : SystemCtrlBase<PlayerCtrl>, ISystemCtrl
{
    /// <summary>
    /// ��ɫ��Ϣ��ͼ
    /// </summary>
    private UIRoleInfoView m_UIRoleInfoView;

    /// <summary>
    /// ����ɫ��Ϣ
    /// </summary>
    private RoleInfoMainPlayer m_MainPlayerRoleInfo;

    /// <summary>
    /// ���һ�ν���������ͼID
    /// </summary>
    public int LastInWorldMapId;
    /// <summary>
    /// ���һ�ν���������ͼ����
    /// </summary>
    public string LastInWorldMapPos;

    public PlayerCtrl()
    {
        //��Ӽ���
        //����������Ԫ��������Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleData_MondeyChangeReturn,OnRoleDataMoneyChangeReturn);
        //���������س�ֵ��Ϣ
        UIDispatcher.Instance.AddEventListener(ConstDefine.RechargeOK,OnRechargeOK);
    }

    #region ������ OnRoleDataMoneyChangeReturn OnRechargeOK
    /// <summary>
    /// ����������Ԫ��������Ϣ
    /// </summary>
    /// <param name="buffer"></param>
    private void OnRoleDataMoneyChangeReturn(byte[] buffer)
    {
        RoleData_MondeyChangeReturnProto proto = RoleData_MondeyChangeReturnProto.GetProto(buffer);
        GlobalInit.Instance.MainPlayerInfo.Money = proto.CurrMoney;

        if (UIMainCityRoleInfoView.Instance != null)
        {
            //ˢ��UI
            UIMainCityRoleInfoView.Instance.SetMoney(proto.CurrMoney);
        }
        //�������ͳ��Ԫ������ɾ���ģ����ں������д��ͳ��
    }
    /// <summary>
    /// ��ֵ�ɹ���ˢ��UI
    /// </summary>
    /// <param name="param"></param>
    private void OnRechargeOK(string[] param)
    {
        //TODO
        int money = param[0].ToInt();
    }
    #endregion

    /// <summary>
    /// ����ͼ
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

    #region ��ɫ�������Եĳ�ʼ������������ SetMainCityRoleData SetMainCityRoleInfo SetMainCityRoleSkillInfo
    /// <summary>
    /// ���ý�ɫ��Ϣ�ͼ�����Ϣ
    /// </summary>
    public void SetMainCityRoleData()
    {
        SetMainCityRoleInfo();
        SetMainCityRoleSkillInfo();
    }

    /// <summary>
    /// ��������UI�Ľ�ɫ��Ϣ
    /// </summary>
    private void SetMainCityRoleInfo()
    {
        m_MainPlayerRoleInfo = GlobalInit.Instance.currentPlayer.CurrentRoleInfo as RoleInfoMainPlayer;
        if (m_MainPlayerRoleInfo != null)
        {
            //��ɫͷ��  
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
            Debug.LogError("�����ɫ��Ϣ�Ƿ�����");
        }

    }
    /// <summary>
    /// ��������UI�Ͻ�ɫ������Ϣ
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
    /// �򿪽�ɫ��Ϣ����
    /// </summary>
    public void OpenRoleInfoView()
    {
        Debug.Log("����Ϊ��򿪽�ɫ��Ϣ���");
        UIViewUtil.Instance.LoadWindow(WindowUIType.RoleInfo.ToString(), (GameObject obj) =>
        { 
            m_UIRoleInfoView = obj.GetComponent<UIRoleInfoView>();
            RoleInfoMainPlayer roleInfo = ((RoleInfoMainPlayer)GlobalInit.Instance.currentPlayer.CurrentRoleInfo);
            TransferData data = new TransferData();
            //������Ϣ
            data.SetValue(ConstDefine.JobId, roleInfo.JobId);
            data.SetValue(ConstDefine.NickName, roleInfo.RoleNickName);
            data.SetValue(ConstDefine.Level, roleInfo.Level);
            data.SetValue(ConstDefine.Fighting, roleInfo.Fighting);
            //��Դ��Ϣ
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
                Debug.Log("����BUG");
            }
            if (m_UIRoleInfoView == null)
            {
                Debug.Log("��ɫBUG");
            }
            m_UIRoleInfoView.SetRoleInfo(data);
        });
    }

    /// <summary>
    /// ���ܰ�ť����ص�
    /// </summary>
    /// <param name="skillId"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnSkillClick(int skillId)
    {
        Debug.Log("��������ż�����");
        bool isSuccess = GlobalInit.Instance.currentPlayer.ToAttack(RoleAttackType.SkillAttack, skillId);
        if (isSuccess)
        {
            GlobalInit.Instance.currentPlayer.CurrentRoleInfo.SetSkillCDEndTime(skillId);
            //����CD
            UIMainCitySkillView.Instance.BeginCD(skillId);
        }
    }

    #region ��Ѫ�͵��� OnHPChangeCallBack OnMPChangeCallBack
    /// <summary>
    /// Ѫ���仯ί�лص�
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
    /// �����仯ί�лص�
    /// </summary>
    /// <param name="type"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnMPChangeCallBack(ValueChangeType type)
    {
        if (m_MainPlayerRoleInfo == null)
        { return; }
        Debug.Log("�ҵ�����");
        UIMainCityRoleInfoView.Instance.SetMP(m_MainPlayerRoleInfo.CurrMP, m_MainPlayerRoleInfo.MaxMP);
    }
    #endregion

}
