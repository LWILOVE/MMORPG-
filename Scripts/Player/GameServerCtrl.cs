using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区服控制器
/// </summary>
public class GameServerCtrl : SystemCtrlBase<GameServerCtrl>, ISystemCtrl
{

    #region 属性
    /// <summary>
    /// 进入游戏视图
    /// </summary>
    private UIGameServerEnterView m_GameServerEnterView;

    /// <summary>
    /// 选择区服视图
    /// </summary>
    private UIGameServerSelectView m_GameServerSelectView;

    /// <summary>
    /// 当前已选过的区服字典
    /// </summary>
    private Dictionary<int, List<RetGameServerEntity>> m_GameServerDic = new Dictionary<int, List<RetGameServerEntity>>();

    /// <summary>
    /// 当前点击的页签下标
    /// </summary>
    private int m_CurrentClickPageIndex = 0;

    /// <summary>
    /// 游戏服信息获取是否处于繁忙
    /// </summary>
    private bool m_isBusy = false;

    #endregion

    public GameServerCtrl()
    {
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.System_ServerTimeReturn, OnSystemServerTimeReturn);

        AddEventListener(ConstDefine.UIGameServerEnterView_Btn_EnterGame, GameServerEnterViewBtnEnterGameClick);
        AddEventListener(ConstDefine.UIGameServerEnterView_Btn_SelectGameServer, GameServerSelectViewBtnSelectClick);
        //服务器连接成功委托
        NetWorkSocket.Instance.OnConnectOk = OnConnectOkCallBack;
    }

    #region Dispose 资源释放
    /// <summary>
    /// 资源释放
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.System_ServerTimeReturn, OnSystemServerTimeReturn);
        RemoveEventListener(ConstDefine.UIGameServerEnterView_Btn_EnterGame, GameServerEnterViewBtnEnterGameClick);
        RemoveEventListener(ConstDefine.UIGameServerEnterView_Btn_SelectGameServer, GameServerSelectViewBtnSelectClick);
    }
    #endregion


    #region 协议
    /// <summary>
    /// 服务器返回的服务器时间
    /// </summary>
    /// <param name="buffer"></param>
    private void OnSystemServerTimeReturn(byte[] buffer)
    {
        System_ServerTimeReturnProto proto = System_ServerTimeReturnProto.GetProto(buffer);
        float localTime = proto.LocalTime;//本地时间
        long serverTime = proto.ServerTime;//服务器时间

        Debug.Log("服务器当前时间为：" + serverTime.ToString());
        //游戏ping值：客户端与服务器完成一次交互的时间/ms
        GlobalInit.Instance.PingValue = (int)((Time.realtimeSinceStartup * 1000 - localTime) * 0.5f);
        //客户端计算所得的服务器时间
        GlobalInit.Instance.GameServerTime = serverTime - GlobalInit.Instance.PingValue;
        Debug.Log("Ping:" + GlobalInit.Instance.PingValue);
        Debug.Log("GameServerTime:" + GlobalInit.Instance.GameServerTime.ToString());
        //更新最后的服务器登录时间
        UpdateLastLogOnServer(GlobalInit.Instance.CurrAccount, GlobalInit.Instance.CurrentSelectGameServer);
        //跳转到选人场景
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

    #region 选择区服界面布置
    /// <summary>
    /// 打开进入游戏服窗口
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
        Debug.Log("打开进入游戏窗口");
    }

    /// <summary>
    /// 选择区服按钮事件UI
    /// </summary>
    /// <param name="pageIndex"></param>
    private void OnPageClick(int pageIndex)
    {
        GetGameServer(pageIndex);
    }

    /// <summary>
    /// 获取页签
    /// </summary>
    private void GetGameServerPage()
    {
        //获取页签
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 0;
        dic["ChannelId"] = GlobalInit.ChannelId;
        dic["InnerVersion"] = GlobalInit.InnerVersion;
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnGetGameServerPageCallBack, isPost: true, dic: dic);
        Debug.Log("页签获取已完成");
    }

    #region OnGetGameServerPageCallBack 区服页签获取回调
    /// <summary>
    /// 区服页签获取回调
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetGameServerPageCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (obj.HasError)
        {
            Debug.Log("报错：" + obj.ErrorMsg);
            ShowMessage("登录提示", "请检查账号和密码是否正确");
        }
        else
        {
            List<RetGameServerPageEntity> list = JsonMapper.ToObject<List<RetGameServerPageEntity>>(obj.Value);
            if (m_GameServerSelectView != null)
            {
                list.Insert(0, new RetGameServerPageEntity() { Name = "推荐区服", PageIndex = 0 });
                m_GameServerSelectView.SetGameServerPageUI(list);
                //开始进行游戏服列表生成
                GetGameServer(0);
                Debug.Log("页签生成完成");
            }
        }
    }
    #endregion



    /// <summary>
    /// 游戏服点击事件
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
        //选择了服务器
        Debug.Log("您现在选择的服务器是：" + obj.Name);
    }

    #endregion

    #region button回调
    /// <summary>
    /// 选择区服方法
    /// </summary>
    /// <param name="p"></param>
    private void GameServerSelectViewBtnSelectClick(object[] p)
    {
        Debug.Log("打开选择区服窗口");
        //当点击选择区服时打开区服选择视图
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameServerSelect.ToString(), (GameObject obj) =>
        {
            m_GameServerSelectView = obj.GetComponent<UIGameServerSelectView>();
        }, () =>
        {
            //设置已选择的游戏服
            m_GameServerSelectView.SetSelectGameServerUI(GlobalInit.Instance.CurrentSelectGameServer);
            //窗口打开时先获取页签，再获取区服，同时，区服默认打开推荐区服
            GetGameServerPage();
        });

        m_GameServerSelectView.OnPageClick = OnPageClick;
        m_GameServerSelectView.OnGameServerClick = OnGameServerClick;
    }

    ///// <summary>
    ///// 服务器连接成功回调
    ///// </summary>
    //private void OnConnectOkCallBack()
    //{
    //    //当连接成功时，更新玩家信息
    //    UpdateLastLogOnServer(GlobalInit.Instance.CurrAccount, GlobalInit.Instance.CurrentSelectGameServer);
        
    //    //UILoadingCtrl.Instance.LoadToSelectRole();
    //}

    private void OpenGameServerSelectView()
    { }

    /// <summary>
    /// 获取区服
    /// </summary>
    /// <param name="pageIndex">页签下标</param>
    private void GetGameServer(int pageIndex)
    {
        //当本次游戏已经获取过该服的信息时，不再向服务器请求，而是直接调用方法
        if(m_GameServerDic.ContainsKey(pageIndex))
        {
            if (m_GameServerSelectView != null)
            {
                m_GameServerSelectView.SetGameServerUI(m_GameServerDic[pageIndex]);
            }
            return;
        }
        m_CurrentClickPageIndex = pageIndex;
        //如果游戏服信息获取正忙就返回
        if (m_isBusy == true)
        {
            return;
        }
        m_isBusy = true;
        //获取区服
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 1;//提醒服务器获取区服
        dic["pageIndex"] = pageIndex;
        dic["ChannelId"] = GlobalInit.ChannelId;
        dic["InnerVersion"] = GlobalInit.InnerVersion;
        //由于数据库存在差，所以此处待修复
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnGetGameServerCallBack, isPost: true, dic: dic);
        Debug.Log("区服获取完成");
    }

    #region OnGetGameServerCallBack 区服获取回调
    /// <summary>
    /// 区服获取回调
    /// </summary>
    /// <param name="obj"></param>
    private void OnGetGameServerCallBack(NetWorkHttp.CallBackArgs obj)
    {
        m_isBusy = false;
        if (obj.HasError)
        {
            Debug.Log("报错：" + obj.ErrorMsg);
            ShowMessage("登录提示", "请检查账号和密码是否正确");
        }
        else
        {
            Debug.Log("开始进行区服生成");
            List<RetGameServerEntity> list = JsonMapper.ToObject<List<RetGameServerEntity>>(obj.Value);

            m_GameServerDic[m_CurrentClickPageIndex] = list;

            if (m_GameServerSelectView != null)
            {
                m_GameServerSelectView.SetGameServerUI(list);
                Debug.Log("区服生成完成");
            }
        }
    }
    #endregion

    #region 服务器相关

    /// <summary>
    /// 点击进入游戏按钮时调用，试图与服务器建立连接
    /// </summary>
    /// <param name="p"></param>
    private void GameServerEnterViewBtnEnterGameClick(object[] p)
    {

        Debug.Log("连接IP：" + GlobalInit.Instance.CurrentSelectGameServer.Ip + "连接端口：" + GlobalInit.Instance.CurrentSelectGameServer.Port);
        //开始连接区服
        //NetWorkSocket.Instance.Connect(GlobalInit.Instance.CurrentSelectGameServer.Ip, GlobalInit.Instance.CurrentSelectGameServer.Port);
        //与服务器进行连接
        NetWorkSocket.Instance.Connect(GlobalInit.Instance.CurrentSelectGameServer.Ip, GlobalInit.Instance.CurrentSelectGameServer.Port);
    }
    #endregion

    /// <summary>
    ///当服务器连接成功时由NetWorkSocket调用
    /// </summary>
    private void OnConnectOkCallBack()
    {
        //与服务器进行时间数据比对
        System_SendLocalTimeProto proto = new System_SendLocalTimeProto();
        //将时间转化为以毫秒为单位
        proto.LocalTime = Time.realtimeSinceStartup * 1000;
        GlobalInit.Instance.CheckServerTime = Time.realtimeSinceStartup;//与服务器进行对表
        //向服务器获取当前时间
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
        //当连接成功时，更新玩家信息
        UpdateLastLogOnServer(GlobalInit.Instance.CurrAccount, GlobalInit.Instance.CurrentSelectGameServer);
    }



    /// <summary>
    /// 更新玩家最终打算进入的区服
    /// </summary>
    /// <param name="currentAccount"></param>
    /// <param name="currentGameServer"></param>
    private void UpdateLastLogOnServer(RetAccountEntity currentAccount, RetGameServerEntity currentGameServer)
    {
        //获取玩家准备进入的区服
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 2;
        dic["userId"] = currentAccount.Id;
        dic["lastServerId"] = currentGameServer.Id;
        dic["lastServerName"] = currentGameServer.Name;
        //由于数据库存在差，所以此处待修复
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/GameServer", OnUpdateLastLogOnServerCallBack, isPost: true, dic: dic);
    }

    /// <summary>
    /// 最后登录服务器信息更新
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
            Debug.Log("最后登录服务器信息更新完成");
        }
    }


    #endregion
}
