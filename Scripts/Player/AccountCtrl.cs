using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 账户控制器
/// </summary>
public class AccountCtrl : SystemCtrlBase<AccountCtrl>,ISystemCtrl
{
    #region 属性
    /// <summary>
    /// 登录窗口视图
    /// </summary>
    private UILogOnView m_LogOnView;

    /// <summary>
    /// 注册窗口视图
    /// </summary>
    private UIRegView m_RegView;

    /// <summary>
    /// 是否自动登录
    /// </summary>
    private bool m_IsAutoLogOn;
    
    #endregion 

    #region 构造函数
    /// <summary>
    /// 构造函数
    /// </summary>
    public AccountCtrl()
    {
        //添加登录按钮监听
        AddEventListener(ConstDefine.UILogOnView_Btn_LogOn, LogOnViewBtnLogOnClick);
        AddEventListener(ConstDefine.UILogOnView_Btn_ToReg, LogOnViewBtnToRegClick);

        //添加注册按钮监听
        AddEventListener(ConstDefine.UIRegView_Btn_Reg, RegViewBtnRegClick);
        AddEventListener(ConstDefine.UIRegView_Btn_ToLogOn, RegViewBtnToLogOnClick);
        
    }
    #endregion

    #region LogOnViewBtnToRegClick 去注册按钮点击
    /// <summary>
    /// 去注册按钮点击
    /// </summary>
    /// <param name="param"></param>
    private void LogOnViewBtnToRegClick(object[] param)
    {
        m_LogOnView.CloseAndOpenNext(WindowUIType.Reg);
    }
    #endregion


    #region OpenLogOnView 打开登录窗口
    /// <summary>
    /// 打开登录窗口
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


    #region 登录系列
    #region QuickLogOn 快速登录
    /// <summary>
    /// 快速登录
    /// </summary>
    public void QuickLogOn()
    {
        //1.判定本地是否创账号，若存在则进入注册视角，若有则自动登录
        if (!PlayerPrefs.HasKey(ConstDefine.LogOn_AccountID))
        {
            Debug.Log("未检测到账号，进行注册");
            this.OpenView(WindowUIType.Reg);
        }
        else
        {
            //进行自动登录
            Debug.Log("进行自动登录");
            //m_IsAutoLogOn = true;
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic["Type"] = 1;
            //dic["UserName"] = PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName);
            //dic["Pwd"] = PlayerPrefs.GetString(ConstDefine.LogOn_AccountPwd);
            //Debug.Log("您的账号是:" +  dic["UserName"] + "  密码是：" +  dic["Pwd"]);
            Debug.Log("您的账号是："+ PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName)+" 密码是：" + PlayerPrefs.GetString(ConstDefine.LogOn_AccountPwd));
            //自动登录改为显示账号密码
            this.OpenView(WindowUIType.LogOn);

            ////由于数据库存在差，所以此处待修复
            //NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/Account", OnLogOnCallBack, isPost: true, dic: dic);
        }
    }

    #region 注册系列
    #region OpenRegView 打开注册窗口
    /// <summary>
    /// 打开注册窗口
    /// </summary>
    public void OpenRegView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.Reg.ToString(),
            (GameObject obj) =>
            {
                m_RegView = obj.GetComponent<UIRegView>();
            });
        Debug.Log("打开注册窗口");
    }

    #region RegViewBtnRegClick 注册按钮点击
    /// <summary>
    /// 注册按钮点击
    /// </summary>
    /// <param name="param"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void RegViewBtnRegClick(object[] param)
    {
        Debug.Log("注册点击");
        if (string.IsNullOrEmpty(m_RegView.txtUserName.text))
        {
            Debug.Log("账户名为空");
            MessageCtrl.Instance.Show("注册提示", "请输入用户名");
            return;
        }
        if (string.IsNullOrEmpty(m_RegView.txtPwd.text))
        {
            Debug.Log("密码为空");
            MessageCtrl.Instance.Show("注册提示", "请输入密码");
            return;
        }

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 0;
        dic["UserName"] = m_RegView.txtUserName.text;
        dic["Pwd"] = m_RegView.txtPwd.text;
        dic["ChannelId"] = 0;
        Debug.Log("前往数据库");
        //由于数据库存在差，所以此处待修复
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/Account", OnRegCallBack, isPost: true, dic: dic);
        Debug.Log("成功走出数据库");
    }

    /// <summary>
    /// 注册回调
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
                Debug.Log("报错：" + ret.ErrorMsg);
            }
            else
            {
                Debug.Log("注册成功 =" + ret.Value);
                RetAccountEntity entity = JsonMapper.ToObject<RetAccountEntity>(ret.Value.ToString());

                GlobalInit.Instance.CurrAccount = entity;

                SetCurrrentSelectGameServer(entity);

                //将用户注册的数据屯入U3D的云空间中(俗称本地存储)
                PlayerPrefs.SetInt(ConstDefine.LogOn_AccountID, entity.Id);
                PlayerPrefs.SetString(ConstDefine.LogOn_AccountUserName, m_RegView.txtUserName.text);
                PlayerPrefs.SetString(ConstDefine.LogOn_AccountPwd, m_RegView.txtPwd.text);
                Debug.Log("账号信息注册完成");

                ////进行注册统计  
                //Stat.Reg((int)ret.Value, m_RegView.txtUserName.text);

                //注册成功以后应该关闭本窗口
                m_RegView.CloseAndOpenNext(WindowUIType.GameServerEnter);
            }

        }
        Debug.Log("发现注册");
    }

    #region SetCurrrentSelectGameServer 设定当前选中的服务器
    /// <summary>
    /// 设定当前选中的服务器
    /// </summary>
    /// <param name="entity">用户实体</param>
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

    #region LogOnViewBtnLogOnClick 登录按钮点击
    /// <summary>
    /// 登录按钮点击
    /// </summary>
    /// <param name="param"></param>
    private void LogOnViewBtnLogOnClick(object[] param)
    {
        Debug.Log("登录点击");
        if (string.IsNullOrEmpty(m_LogOnView.txtUserName.text))
        {
            Debug.Log("账户名为空");
            ShowMessage("请输入用户名", "登录提示");
            return;
        }
        if (string.IsNullOrEmpty(m_LogOnView.txtPwd.text))
        {
            Debug.Log("密码为空");
            ShowMessage("请输入密码","登录提示");
            return;
        }

        m_IsAutoLogOn=false;

        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["Type"] = 1;
        dic["UserName"] = m_LogOnView.txtUserName.text;
        dic["Pwd"] = m_LogOnView.txtPwd.text;
        Debug.Log("前往数据库"); 
        //由于数据库存在差，所以此处待修复
        NetWorkHttp.Instance.SendData(GlobalInit.WebAccountUrl + "api/Account", OnLogOnCallBack, isPost: true, dic:dic);
        Debug.Log("成功走出数据库");
    }
    #endregion

    #region OnLogOnCallBack 登录回调
    /// <summary>
    /// 登录回调
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLogOnCallBack(NetWorkHttp.CallBackArgs obj)
    {
        Debug.Log("登录回调开启");
        if (obj.HasError)
        {
            Debug.Log("报错：" + obj.ErrorMsg);
            ShowMessage("请检查账号和密码是否正确", "登录提示");
        }
        else
        {
            Debug.Log(obj.Value);
            RetValue ret = JsonMapper.ToObject<RetValue>(obj.Value);
            if (ret.HasError)
            {
                ShowMessage( "请检查账号和密码是否正确", "登录提示");
                Debug.Log("报错" + ret.ErrorMsg);
            }
            else
            {
                Debug.Log("登录成功 =" + ret.Value);
                RetAccountEntity entity = JsonMapper.ToObject<RetAccountEntity>(ret.Value.ToString());

                GlobalInit.Instance.CurrAccount = entity;

                SetCurrrentSelectGameServer(entity);

                string userName = string.Empty;
                //进行登录统计:自动登录时用户名即用户当前名，
                if (m_IsAutoLogOn)
                {
                    userName = PlayerPrefs.GetString(ConstDefine.LogOn_AccountUserName);
                    UIViewMgr.Instance.OpenWindow(WindowUIType.GameServerEnter);
                    Debug.Log("打开游戏服窗口");   
                }
                else
                {

                    //将用户注册的数据屯入U3D的云空间中(俗称本地存储)
                    PlayerPrefs.SetInt(ConstDefine.LogOn_AccountID, entity.Id);
                    PlayerPrefs.SetString(ConstDefine.LogOn_AccountUserName, m_LogOnView.txtUserName.text);
                    PlayerPrefs.SetString(ConstDefine.LogOn_AccountPwd, m_LogOnView.txtPwd.text);

                    userName = m_LogOnView.txtUserName.text;
                    //打开游戏服窗口
                    m_LogOnView.CloseAndOpenNext(WindowUIType.GameServerEnter);
                    Debug.Log("完成游戏服窗口打开");
                }
                //TODO
                //Stat.LogOn((int)entity.Id, userName);
            }
        }
    }

    #region RegViewBtnToLogOnClick 返回登录按钮点击
    /// <summary>
    /// 返回登录按钮点击
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



    #region Dispose 资源释放
    /// <summary>
    /// 资源释放
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
