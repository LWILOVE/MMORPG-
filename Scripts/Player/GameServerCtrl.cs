using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����������
/// </summary>
public class GameServerCtrl : SystemCtrlBase<GameServerCtrl>, ISystemCtrl
{

    #region ����
    /// <summary>
    /// ������Ϸ��ͼ
    /// </summary>
    private UIGameServerEnterView m_GameServerEnterView;

    /// <summary>
    /// ѡ��������ͼ
    /// </summary>
    private UIGameServerSelectView m_GameServerSelectView;

    /// <summary>
    /// ��ǰ��ѡ���������ֵ�
    /// </summary>
    private Dictionary<int, List<RetGameServerEntity>> m_GameServerDic = new Dictionary<int, List<RetGameServerEntity>>();

    /// <summary>
    /// ��ǰ�����ҳǩ�±�
    /// </summary>
    private int m_CurrentClickPageIndex = 0;

    /// <summary>
    /// ��Ϸ����Ϣ��ȡ�Ƿ��ڷ�æ
    /// </summary>
    private bool m_isBusy = false;

    #endregion

    public GameServerCtrl()
    {
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.System_ServerTimeReturn, OnSystemServerTimeReturn);

        AddEventListener(ConstDefine.UIGameServerEnterView_Btn_EnterGame, GameServerEnterViewBtnEnterGameClick);
        AddEventListener(ConstDefine.UIGameServerEnterView_Btn_SelectGameServer, GameServerSelectViewBtnSelectClick);
        //���������ӳɹ�ί��
        NetWorkSocket.Instance.OnConnectOk = OnConnectOkCallBack;
    }

    #region Dispose ��Դ�ͷ�
    /// <summary>
    /// ��Դ�ͷ�
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.System_ServerTimeReturn, OnSystemServerTimeReturn);
        RemoveEventListener(ConstDefine.UIGameServerEnterView_Btn_EnterGame, GameServerEnterViewBtnEnterGameClick);
        RemoveEventListener(ConstDefine.UIGameServerEnterView_Btn_SelectGameServer, GameServerSelectViewBtnSelectClick);
    }
    #endregion


    #region Э��
    /// <summary>
    /// ���������صķ�����ʱ��
    /// </summary>
    /// <param name="buffer"></param>
    private void OnSystemServerTimeReturn(byte[] buffer)
    {
        System_ServerTimeReturnProto proto = System_ServerTimeReturnProto.GetProto(buffer);
        float localTime = proto.LocalTime;//����ʱ��
        long serverTime = proto.ServerTime;//������ʱ��

        Debug.Log("��������ǰʱ��Ϊ��" + serverTime.ToString());
        //��Ϸpingֵ���ͻ�������������һ�ν�����ʱ��/ms
        GlobalInit.Instance.PingValue = (int)((Time.realtimeSinceStartup * 1000 - localTime) * 0.5f);
        //�ͻ��˼������õķ�����ʱ��
        GlobalInit.Instance.GameServerTime = serverTime - GlobalInit.Instance.PingValue;
        Debug.Log("Ping:" + GlobalInit.Instance.PingValue);
        Debug.Log("GameServerTime:" + GlobalInit.Instance.GameServerTime.ToString());
        //�������ķ�������¼ʱ��
        UpdateLastLogOnServer(GlobalInit.Instance.CurrAccount, GlobalInit.Instance.CurrentSelectGameServer);
        //��ת��ѡ�˳���
        UILoadingCtrl.Instance.LoadToSelectRole();
    }
    #endregion

    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.GameServerEnter:
                OpenGameServerEnterView();
                break;
            case WindowUIType.GameServerSelect:
                OpenGameServerSelectView();
                break;
        }
    }

    #region ѡ���������沼��
    /// <summary>
    /// �򿪽�����Ϸ������
    /// </summary>
    private void OpenGameServerEnterView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameServerEnter.ToString(), (GameObject obj) =>
        {
            m_GameServerEnterView = obj.GetComponent<UIGameServerEnterView>();
        }, () =>
        {
            m_GameServerEnterView.SetUI(GlobalInit.Instance.CurrentSelectGameServer.Name);
        });
        Debug.Log("�򿪽�����Ϸ����");
    }

    /// <summary>
    /// ѡ��������ť�¼�UI
    /// </summary>
    /// <param name="pageIndex"></param>
    private void OnPageClick(int pageIndex)
    {
        GetGameServer(pageIndex);
    }

    /// <summary>
    /// ��ȡҳǩ
    /// </summary>
    private void GetGameServerPage()
    {
        //��ȡҳǩ
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 0;
        dic["ChannelId"] = GlobalInit.ChannelId;
        dic["InnerVersion"] = GlobalInit.InnerVersion;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnGetGameServerPageCallBack, isPost: true, dic: dic);
        Debug.Log("ҳǩ��ȡ�����");
    }

    #region OnGetGameServerPageCallBack ����ҳǩ��ȡ�ص�
    /// <summary>
    /// ����ҳǩ��ȡ�ص�
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetGameServerPageCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)
        {
            Debug.Log("����" + obj.ErrorMsg);
            ShowMessage("��¼��ʾ", "�����˺ź������Ƿ���ȷ");
        }
        else
        {
            List<RetGameServerPageEntity> list = JsonMapper.ToObject<List<RetGameServerPageEntity>>(obj.Value);
            if (m_GameServerSelectView != null)
            {
                list.Insert(0, new RetGameServerPageEntity() { Name = "�Ƽ�����", PageIndex = 0 });
                m_GameServerSelectView.SetGameServerPageUI(list);
                //��ʼ������Ϸ���б�����
                GetGameServer(0);
                Debug.Log("ҳǩ�������");
            }
        }
    }
    #endregion



    /// <summary>
    /// ��Ϸ������¼�
    /// </summary>
    /// <param name="obj"></param>
    private void OnGameServerClick(RetGameServerEntity obj)
    {
        m_GameServerSelectView.Close();
        GlobalInit.Instance.CurrentSelectGameServer = obj;
        if (m_GameServerEnterView != null)
        {
            m_GameServerEnterView.SetUI(GlobalInit.Instance.CurrentSelectGameServer.Name);
        }
        //ѡ���˷�����
        Debug.Log("������ѡ��ķ������ǣ�" + obj.Name);
    }

    #endregion

    #region button�ص�
    /// <summary>
    /// ѡ����������
    /// </summary>
    /// <param name="p"></param>
    private void GameServerSelectViewBtnSelectClick(object[] p)
    {
        Debug.Log("��ѡ����������");
        //�����ѡ������ʱ������ѡ����ͼ
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameServerSelect.ToString(), (GameObject obj) =>
        {
            m_GameServerSelectView = obj.GetComponent<UIGameServerSelectView>();
        }, () =>
        {
            //������ѡ�����Ϸ��
            m_GameServerSelectView.SetSelectGameServerUI(GlobalInit.Instance.CurrentSelectGameServer);
            //���ڴ�ʱ�Ȼ�ȡҳǩ���ٻ�ȡ������ͬʱ������Ĭ�ϴ��Ƽ�����
            GetGameServerPage();
        });

        m_GameServerSelectView.OnPageClick = OnPageClick;
        m_GameServerSelectView.OnGameServerClick = OnGameServerClick;
    }

    ///// <summary>
    ///// ���������ӳɹ��ص�
    ///// </summary>
    //private void OnConnectOkCallBack()
    //{
    //    //�����ӳɹ�ʱ�����������Ϣ
    //    UpdateLastLogOnServer(GlobalInit.Instance.CurrAccount, GlobalInit.Instance.CurrentSelectGameServer);
        
    //    //UILoadingCtrl.Instance.LoadToSelectRole();
    //}

    private void OpenGameServerSelectView()
    { }

    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <param name="pageIndex">ҳǩ�±�</param>
    private void GetGameServer(int pageIndex)
    {
        //��������Ϸ�Ѿ���ȡ���÷�����Ϣʱ����������������󣬶���ֱ�ӵ��÷���
        if(m_GameServerDic.ContainsKey(pageIndex))
        {
            if (m_GameServerSelectView != null)
            {
                m_GameServerSelectView.SetGameServerUI(m_GameServerDic[pageIndex]);
            }
            return;
        }
        m_CurrentClickPageIndex = pageIndex;
        //�����Ϸ����Ϣ��ȡ��æ�ͷ���
        if (m_isBusy == true)
        {
            return;
        }
        m_isBusy = true;
        //��ȡ����
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 1;//���ѷ�������ȡ����
        dic["pageIndex"] = pageIndex;
        dic["ChannelId"] = GlobalInit.ChannelId;
        dic["InnerVersion"] = GlobalInit.InnerVersion;
        //�������ݿ���ڲ���Դ˴����޸�
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnGetGameServerCallBack, isPost: true, dic: dic);
        Debug.Log("������ȡ���");
    }

    #region OnGetGameServerCallBack ������ȡ�ص�
    /// <summary>
    /// ������ȡ�ص�
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetGameServerCallBack(NetWorkHttp.CallBackArgs obj)
    {
        m_isBusy = false;
        if (obj.HasError)
        {
            Debug.Log("����" + obj.ErrorMsg);
            ShowMessage("��¼��ʾ", "�����˺ź������Ƿ���ȷ");
        }
        else
        {
            Debug.Log("��ʼ������������");
            List<RetGameServerEntity> list = JsonMapper.ToObject<List<RetGameServerEntity>>(obj.Value);

            m_GameServerDic[m_CurrentClickPageIndex] = list;

            if (m_GameServerSelectView != null)
            {
                m_GameServerSelectView.SetGameServerUI(list);
                Debug.Log("�����������");
            }
        }
    }
    #endregion

    #region ���������

    /// <summary>
    /// ���������Ϸ��ťʱ���ã���ͼ���������������
    /// </summary>
    /// <param name="p"></param>
    private void GameServerEnterViewBtnEnterGameClick(object[] p)
    {

        Debug.Log("����IP��" + GlobalInit.Instance.CurrentSelectGameServer.Ip + "���Ӷ˿ڣ�" + GlobalInit.Instance.CurrentSelectGameServer.Port);
        //��ʼ��������
        //NetWorkSocket.Instance.Connect(GlobalInit.Instance.CurrentSelectGameServer.Ip, GlobalInit.Instance.CurrentSelectGameServer.Port);
        //���������������
        NetWorkSocket.Instance.Connect(GlobalInit.Instance.CurrentSelectGameServer.Ip, GlobalInit.Instance.CurrentSelectGameServer.Port);
    }
    #endregion

    /// <summary>
    ///�����������ӳɹ�ʱ��NetWorkSocket����
    /// </summary>
    private void OnConnectOkCallBack()
    {
        //�����������ʱ�����ݱȶ�
        System_SendLocalTimeProto proto = new System_SendLocalTimeProto();
        //��ʱ��ת��Ϊ�Ժ���Ϊ��λ
        proto.LocalTime = Time.realtimeSinceStartup * 1000;
        GlobalInit.Instance.CheckServerTime = Time.realtimeSinceStartup;//����������жԱ�
        //���������ȡ��ǰʱ��
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
        //�����ӳɹ�ʱ�����������Ϣ
        UpdateLastLogOnServer(GlobalInit.Instance.CurrAccount, GlobalInit.Instance.CurrentSelectGameServer);
    }



    /// <summary>
    /// ����������մ�����������
    /// </summary>
    /// <param name="currentAccount"></param>
    /// <param name="currentGameServer"></param>
    private void UpdateLastLogOnServer(RetAccountEntity currentAccount, RetGameServerEntity currentGameServer)
    {
        //��ȡ���׼�����������
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 2;
        dic["userId"] = currentAccount.Id;
        dic["lastServerId"] = currentGameServer.Id;
        dic["lastServerName"] = currentGameServer.Name;
        //�������ݿ���ڲ���Դ˴����޸�
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnUpdateLastLogOnServerCallBack, isPost: true, dic: dic);
    }

    /// <summary>
    /// ����¼��������Ϣ����
    /// </summary>
    /// <param name="obj"></param>
    private void OnUpdateLastLogOnServerCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)
        {
            Debug.Log(obj.ErrorMsg);
        }
        else
        {
            Debug.Log("����¼��������Ϣ�������");
        }
    }


    #endregion
}
