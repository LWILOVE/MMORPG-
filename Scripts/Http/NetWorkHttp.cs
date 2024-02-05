using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using LitJson;
using System.Net.NetworkInformation;

/// <summary>
/// Http通讯管理类
/// </summary>
public class NetWorkHttp : SingletonMono<NetWorkHttp>
{
    #region 属性
    /// <summary>
    /// Web请求回调委托
    /// </summary>
    private XLuaCustomExport.NetWorkSendDataCallBack m_CallBack;

    /// <summary>
    /// Web请求回调数据对象
    /// </summary>
    private CallBackArgs m_CallBackArgs;
    /// <summary>
    /// 是否繁忙
    /// </summary>
    private bool m_isBusy = false;

    public bool IsBusy
    {
        get { return m_isBusy; }
    }
    #endregion
    protected override void OnStart()
    {
        base.OnStart();
        m_CallBackArgs = new CallBackArgs();
    }
    #region SendData 发送Web数据
    /// <summary>
    /// 发送Web数据
    /// </summary>
    /// <param name="url">定位地址(即要访问的网站)</param>
    /// <param name="callBack">回调结果</param>
    /// <param name="isPost">是否进行数据传输</param>
    /// <param name="dic">数据</param>
    public void SendData(string url, XLuaCustomExport.NetWorkSendDataCallBack callBack, bool isPost = false, Dictionary<string, object> dic = null)
    {
        if (m_isBusy)
        { return; }
        m_isBusy = true;
        m_CallBack = callBack;

        if (!isPost)
        {
            GetUrl(url);
        }
        else
        {
            //进行Web加密
            if (dic != null)
            {
                //客户端电脑唯一识别码
                dic["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                //客户端设备型号
                dic["deviceModel"] = DeviceUtil.DeviceModel;
                long t = GlobalInit.Instance.CurrentServerTime;
                //签名:=时间戳:客户端ID
                dic["sign"] = EncryptUtil.Md5(String.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));
                //时间戳
                dic["t"] = t;
            }
            Debug.Log("正在准备进行数据推送");
            PostUrl(url, dic == null ? "" : JsonMapper.ToJson(dic));
        }
    }
    #endregion

    #region PostUrl Post请求
    /// <summary>
    /// Post请求
    /// </summary>
    /// <param name="url">定位地址</param>
    /// <param name="json">数据</param>
    private void PostUrl(string url, string json)
    {
        //网页表单类
        WWWForm form = new WWWForm();
        //添加空间：（键,值）
        form.AddField("", json);
        UnityWebRequest data = UnityWebRequest.Post(url, form);

        Debug.Log("正在进行数据推送");
        StartCoroutine(Request(data));
    }
    #endregion

    #region GetUrl Get请求
    /// <summary>
    /// Get请求
    /// </summary>
    /// <param name="url">定位地址（用户所在的地址）</param>
    private void GetUrl(string url)
    {
        ///注：WWW类型目前已经被Unity抛弃，UnityWebRequest是WWW的上位替代品
        UnityWebRequest data = UnityWebRequest.Get(url);
        StartCoroutine(Request(data));
    }
    #endregion

    #region Request 请求数据库
    /// <summary>
    /// 请求网址许可
    /// </summary>
    /// <param name="data">数据</param>
    /// <returns></returns>
    private IEnumerator Request(UnityWebRequest data)
    {
        Debug.Log("数据正在运输");
        yield return data.SendWebRequest();
        m_isBusy = false;
        Debug.Log(data.downloadHandler.text);
        //若数据无误则允许
        if (string.IsNullOrEmpty(data.error))
        {
            if (data.downloadHandler.text == "null")
            {
                Debug.Log("数据为空");
                if (m_CallBack != null)
                {
                    m_CallBackArgs.HasError = true;
                    m_CallBackArgs.ErrorMsg = "未请求到数据";
                    m_CallBack(m_CallBackArgs);
                }
            }
            else
            {
                Debug.Log("数据申请");
                if (m_CallBack != null)
                {
                    m_CallBackArgs.HasError = false;
                    m_CallBackArgs.Value = data.downloadHandler.text;
                    m_CallBack(m_CallBackArgs);
                }
            }

        }
        else
        {
            Debug.Log("数据申请失败");
            if (m_CallBack != null)
            {
                m_CallBackArgs.HasError = true;
                m_CallBackArgs.ErrorMsg = data.error;
                m_CallBack(m_CallBackArgs);
            }
        }
        data.Dispose();
    }
    #endregion

    #region CallBackArgs Web请求回调数据类
    /// <summary>
    /// Web请求回调数据类
    /// 继承的EventArgs也不懂有啥用
    /// </summary>
    public class CallBackArgs : EventArgs
    {
        /// <summary>
        /// 是否有错
        /// </summary>
        public bool HasError;

        /// <summary>
        /// 数据
        /// </summary>
        public string Value;

        /// <summary>
        /// 报错原因
        /// </summary>
        public string ErrorMsg;

    }
    #endregion
}
