using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// �˻�������
/// </summary>
public class AccountCtrl : SystemCtrlBase<AccountCtrl>,ISystemCtrl
{
    #region ����
    /// <summary>
    /// ��¼������ͼ
    /// </summary>
    private UILogOnView m_LogOnView;

    /// <summary>
    /// ע�ᴰ����ͼ
    /// </summary>
    private UIRegView m_RegView;

    /// <summary>
    /// �Ƿ��Զ���¼
    /// </summary>
    private bool m_IsAutoLogOn;
    
    #endregion 

    #region ���캯��
    /// <summary>
    /// ���캯��
    /// </summary>
    public AccountCtrl()
    {
        //��ӵ�¼��ť����
        AddEventListener(ConstDefine.UILogOnView_Btn_LogOn, LogOnViewBtnLogOnClick);
        AddEventListener(ConstDefine.UILogOnView_Btn_ToReg, LogOnViewBtnToRegClick);

        //���ע�ᰴť����
        AddEventListener(ConstDefine.UIRegView_Btn_Reg, RegViewBtnRegClick);
        AddEventListener(ConstDefine.UIRegView_Btn_ToLogOn, RegViewBtnToLogOnClick);
        
    }
    #endregion

    #region LogOnViewBtnToRegClick ȥע�ᰴť���
    /// <summary>
    /// ȥע�ᰴť���
    /// </summary>
    /// <param name="param"></param>
    private void LogOnViewBtnToRegClick(object[] param)
    {
        m_LogOnView.CloseAndOpenNext(WindowUIType.Reg);
    }
    #endregion


    #region OpenLogOnView �򿪵�¼����
    /// <summary>
    /// �򿪵�¼����
    /// </summary>
    public void OpenLogOnView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.LogOn.ToString(),
            (GameObject obj)=>
            {
                m_LogOnView = obj.GetComponent<UILogOnView>();
            });
    }
    #endregion

    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.LogOn:
                OpenLogOnView();
                break;
            case WindowUIType.Reg:
                OpenRegView();
                break;
        }
    }


    #region ��¼ϵ��
    #region QuickLogOn ���ٵ�¼
    /// <summary>
    /// ���ٵ�¼
    /// </summary>
    public void QuickLogOn()
    {
        //1.�ж������Ƿ��˺ţ������������ע���ӽǣ��������Զ���¼
        if (!PlayerPrefs.HasKey(ConstDefine.LogOn_AccountID))
        {
            Debug.Log("δ��⵽�˺ţ�����ע��");
            this.OpenView(WindowUIType.Reg);
        }
        else
        {
            //�����Զ���¼
            Debug.Log("�����Զ���¼");
            //m_IsAutoLogOn = true;
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic["Type"] = 1;
            //dic["UserName"] = PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName);
            //dic["Pwd"] = PlayerPrefs.GetString(ConstDefine.LogOn_AccountPwd);
            //Debug.Log("�����˺���:" +  dic["UserName"] + "  �����ǣ�" +  dic["Pwd"]);
            Debug.Log("�����˺��ǣ�"+ PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName)+" �����ǣ�" + PlayerPrefs.GetString(ConstDefine.LogOn_AccountPwd));
            //�Զ���¼��Ϊ��ʾ�˺�����
            this.OpenView(WindowUIType.LogOn);

            ////�������ݿ���ڲ���Դ˴����޸�
            //NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/Account", OnLogOnCallBack, isPost: true, dic: dic);
        }
    }

    #region ע��ϵ��
    #region OpenRegView ��ע�ᴰ��
    /// <summary>
    /// ��ע�ᴰ��
    /// </summary>
    public void OpenRegView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.Reg.ToString(),
            (GameObject obj) =>
            {
                m_RegView = obj.GetComponent<UIRegView>();
            });
        Debug.Log("��ע�ᴰ��");
    }

    #region RegViewBtnRegClick ע�ᰴť���
    /// <summary>
    /// ע�ᰴť���
    /// </summary>
    /// <param name="param"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void RegViewBtnRegClick(object[] param)
    {
        Debug.Log("ע����");
        if (string.IsNullOrEmpty(m_RegView.txtUserName.text))
        {
            Debug.Log("�˻���Ϊ��");
            MessageCtrl.Instance.Show("ע����ʾ", "�������û���");
            return;
        }
        if (string.IsNullOrEmpty(m_RegView.txtPwd.text))
        {
            Debug.Log("����Ϊ��");
            MessageCtrl.Instance.Show("ע����ʾ", "����������");
            return;
        }

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 0;
        dic["UserName"] = m_RegView.txtUserName.text;
        dic["Pwd"] = m_RegView.txtPwd.text;
        dic["ChannelId"] = 0;
        Debug.Log("ǰ�����ݿ�");
        //�������ݿ���ڲ���Դ˴����޸�
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/Account", OnRegCallBack, isPost: true, dic: dic);
        Debug.Log("�ɹ��߳����ݿ�");
    }

    /// <summary>
    /// ע��ص�
    /// </summary>
    /// <param name="obj"></param>
    private void OnRegCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)
        {
            Debug.Log(obj.ErrorMsg);
        }
        else
        {
            RetValue ret = JsonMapper.ToObject<RetValue>(obj.Value);
            if (ret.HasError)
            {
                Debug.Log("����" + ret.ErrorMsg);
            }
            else
            {
                Debug.Log("ע��ɹ� =" + ret.Value);
                RetAccountEntity entity = JsonMapper.ToObject<RetAccountEntity>(ret.Value.ToString());

                GlobalInit.Instance.CurrAccount = entity;

                SetCurrrentSelectGameServer(entity);

                //���û�ע�����������U3D���ƿռ���(�׳Ʊ��ش洢)
                PlayerPrefs.SetInt(ConstDefine.LogOn_AccountID, entity.Id);
                PlayerPrefs.SetString(ConstDefine.LogOn_AccountUserName, m_RegView.txtUserName.text);
                PlayerPrefs.SetString(ConstDefine.LogOn_AccountPwd, m_RegView.txtPwd.text);
                Debug.Log("�˺���Ϣע�����");

                ////����ע��ͳ��  
                //Stat.Reg((int)ret.Value, m_RegView.txtUserName.text);

                //ע��ɹ��Ժ�Ӧ�ùرձ�����
                m_RegView.CloseAndOpenNext(WindowUIType.GameServerEnter);
            }

        }
        Debug.Log("����ע��");
    }

    #region SetCurrrentSelectGameServer �趨��ǰѡ�еķ�����
    /// <summary>
    /// �趨��ǰѡ�еķ�����
    /// </summary>
    /// <param name="entity">�û�ʵ��</param>
    private void SetCurrrentSelectGameServer(RetAccountEntity entity)
    {

        RetGameServerEntity currentGameServerEntity = new RetGameServerEntity();
        currentGameServerEntity.Id = entity.LastServerId;
        currentGameServerEntity.Name = entity.LastServerName;
        currentGameServerEntity.Ip = entity.LastServerIP;
        currentGameServerEntity.Port = entity.LastServerPort;
        GlobalInit.Instance.CurrentSelectGameServer = currentGameServerEntity;
    }
    #endregion

    #endregion
    #endregion
    #endregion
    #endregion

    #region LogOnViewBtnLogOnClick ��¼��ť���
    /// <summary>
    /// ��¼��ť���
    /// </summary>
    /// <param name="param"></param>
    private void LogOnViewBtnLogOnClick(object[] param)
    {
        Debug.Log("��¼���");
        if (string.IsNullOrEmpty(m_LogOnView.txtUserName.text))
        {
            Debug.Log("�˻���Ϊ��");
            ShowMessage("�������û���", "��¼��ʾ");
            return;
        }
        if (string.IsNullOrEmpty(m_LogOnView.txtPwd.text))
        {
            Debug.Log("����Ϊ��");
            ShowMessage("����������","��¼��ʾ");
            return;
        }

        m_IsAutoLogOn=false;

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 1;
        dic["UserName"] = m_LogOnView.txtUserName.text;
        dic["Pwd"] = m_LogOnView.txtPwd.text;
        Debug.Log("ǰ�����ݿ�"); 
        //�������ݿ���ڲ���Դ˴����޸�
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/Account", OnLogOnCallBack, isPost: true, dic:dic);
        Debug.Log("�ɹ��߳����ݿ�");
    }
    #endregion

    #region OnLogOnCallBack ��¼�ص�
    /// <summary>
    /// ��¼�ص�
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLogOnCallBack(NetWorkHttp.CallBackArgs obj)
    {
        Debug.Log("��¼�ص�����");
        if (obj.HasError)
        {
            Debug.Log("����" + obj.ErrorMsg);
            ShowMessage("�����˺ź������Ƿ���ȷ", "��¼��ʾ");
        }
        else
        {
            Debug.Log(obj.Value);
            RetValue ret = JsonMapper.ToObject<RetValue>(obj.Value);
            if (ret.HasError)
            {
                ShowMessage( "�����˺ź������Ƿ���ȷ", "��¼��ʾ");
                Debug.Log("����" + ret.ErrorMsg);
            }
            else
            {
                Debug.Log("��¼�ɹ� =" + ret.Value);
                RetAccountEntity entity = JsonMapper.ToObject<RetAccountEntity>(ret.Value.ToString());

                GlobalInit.Instance.CurrAccount = entity;

                SetCurrrentSelectGameServer(entity);

                string userName = string.Empty;
                //���е�¼ͳ��:�Զ���¼ʱ�û������û���ǰ����
                if (m_IsAutoLogOn)
                {
                    userName = PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName);
                    UIViewMgr.Instance.OpenWindow(WindowUIType.GameServerEnter);
                    Debug.Log("����Ϸ������");   
                }
                else
                {

                    //���û�ע�����������U3D���ƿռ���(�׳Ʊ��ش洢)
                    PlayerPrefs.SetInt(ConstDefine.LogOn_AccountID, entity.Id);
                    PlayerPrefs.SetString(ConstDefine.LogOn_AccountUserName, m_LogOnView.txtUserName.text);
                    PlayerPrefs.SetString(ConstDefine.LogOn_AccountPwd, m_LogOnView.txtPwd.text);

                    userName = m_LogOnView.txtUserName.text;
                    //����Ϸ������
                    m_LogOnView.CloseAndOpenNext(WindowUIType.GameServerEnter);
                    Debug.Log("�����Ϸ�����ڴ�");
                }
                //TODO
                //Stat.LogOn((int)entity.Id, userName);
            }
        }
    }

    #region RegViewBtnToLogOnClick ���ص�¼��ť���
    /// <summary>
    /// ���ص�¼��ť���
    /// </summary>
    /// <param name="param"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void RegViewBtnToLogOnClick(object[] param)
    {
        m_RegView.CloseAndOpenNext(WindowUIType.LogOn);
    }
    #endregion
    #endregion
    #endregion



    #region Dispose ��Դ�ͷ�
    /// <summary>
    /// ��Դ�ͷ�
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        RemoveEventListener(ConstDefine.UILogOnView_Btn_LogOn, LogOnViewBtnLogOnClick);
        RemoveEventListener(ConstDefine.UILogOnView_Btn_ToReg, LogOnViewBtnToRegClick);

        RemoveEventListener(ConstDefine.UIRegView_Btn_Reg, RegViewBtnRegClick);
        RemoveEventListener(ConstDefine.UIRegView_Btn_ToLogOn, RegViewBtnToLogOnClick);
    }
    #endregion
}
