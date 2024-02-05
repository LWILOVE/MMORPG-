using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using UnityEngine;

#if UNTIY_IPHONE
using UnityEngine.iOS;
#endif
/// <summary>
/// 全局初始化类
/// </summary>
[XLua.LuaCallCSharp]
public class GlobalInit : MonoBehaviour
{
    public InitWay currentInitWay = InitWay.AuthorUse;

    #region 变量
    #region 单例
    public static GlobalInit Instance;
    private void Awake()
    {
        //设置游戏帧率
        Application.targetFrameRate = 60;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region 服务器数据
    /// <summary>
    /// 当前账号
    /// </summary>
    [HideInInspector]
    public RetAccountEntity CurrAccount;
    /// <summary>
    /// 当前玩家
    /// </summary>
    public RoleCtrl currentPlayer;
    /// <summary>
    /// 玩家注册名
    /// </summary>
    public string CurrRoleNickName;
    /// <summary>
    /// 小地图跟随速率
    /// </summary>
    public float smallMapFollowRate;
    /// <summary>
    /// 当前选中的游戏服
    /// </summary>
    [HideInInspector]
    public RetGameServerEntity CurrentSelectGameServer;
    /// <summary>
    /// 从服务器中获取到的时间戳
    /// </summary>
    public long ServerTime = 0;
    /// <summary>
    /// 与服务器进行时间比对的客户端时间
    /// </summary>
    public float CheckServerTime;
    /// <summary>
    /// 游戏目前的ping值
    /// </summary>
    public int PingValue;
    /// <summary>
    /// 客户端认为的服务器时间
    /// </summary>
    public long GameServerTime;
    #endregion

    #region Http访问变量
    /// <summary>
    /// 专门服务于用户和服务器的网址（注意区分与服务于资源的网址的区别）
    /// </summary>
    public static string WebAccountUrl;
    /// <summary>
    /// 渠道ID
    /// </summary>
    public static int ChannelId;
    /// <summary>
    /// 内部版本号
    /// </summary>
    public static int InnerVersion;
    /// <summary>
    /// 当前渠道配置（内涵一些本渠道专属的消息）
    /// </summary>
    public ChannelInitConfig CurrChannelInitConfig;
    #endregion

    /// <summary>
    /// 当前渠道
    /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static string Channel = "Windows/";
#elif UNITY_ANDROID
    public static string DownloadUrl = DownloadBaseUrl + "Android/";
#elif UNITY_IPHONE
        public static string DownloadUrl = DownloadBaseUrl + "iOS/";
#endif

    //无返回协议接收委托
    public delegate void OnReceiveProtoHandler(ushort protoCode, byte[] buffer);
    //定义委托
    public OnReceiveProtoHandler OnReceiveProto;

    #region 角色和地形相关
    /// <summary>
    /// T4M的Sharder
    /// </summary>
    public Shader T4MShaeder;
    /// <summary>
    /// 天空盒Shader
    /// </summary>
    public Shader MogoSkyBoxShader;
    /// <summary>
    /// 动画曲线
    /// </summary>
    public AnimationCurve UIAnimationCurve;
    /// <summary>
    /// 主角色镜像字典
    /// </summary>
    public Dictionary<int, GameObject> JobObjectDic = new Dictionary<int, GameObject>();
    /// <summary>
    /// 主角信息
    /// </summary>
    public RoleInfoMainPlayer MainPlayerInfo;
    #endregion

    #endregion
    #region 时间系
    /// <summary>
    /// 获取当前年月日时分秒毫秒格式
    /// </summary>
    /// <returns></returns>
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff");
    }
    /// <summary>
    /// 获取当前的服务器时间
    /// </summary>
    /// <returns></returns>
    public long GetCurrentServerTime()
    {
        return (int)(((Time.realtimeSinceStartup - CheckServerTime) * 1000)) + GameServerTime;
    }
    /// <summary>
    /// 时间=游戏启动以来的时间+时间戳
    /// </summary>
    [HideInInspector]
    public long CurrentServerTime
    {
        get
        {
            if (CurrChannelInitConfig == null)
            {
                return (long)Time.unscaledTime;
            }       
            else
            {
                return CurrChannelInitConfig.ServerTime + (long)Time.unscaledTime;
            }
        }
    }

    private void OnGetTimeCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (!obj.HasError)
        {
            ServerTime = long.Parse(obj.Value);
        }
    }

    /// <summary>
    /// 获取从1970年到现在的时间
    /// </summary>
    /// <returns></returns>
    private long GetRealTime()
    {
        //var 
        long time_Real = 0;
        return time_Real;
    }

    #endregion

    private void Start()
    {
#if UNITY_STANDALONE_OSX
        // Mac 启动尺寸适配 -- 首次适配后，第二次会使用上次尺寸
        Resolution[] reslution = Screen.resolutions;
        float standard_width = reslution[reslution.Length - 1].width;
        float standard_height = ((standard_width * 9) / 16);
        Screen.SetResolution(Convert.ToInt32(standard_width * 0.8f), Convert.ToInt32(standard_height * 0.8f), false);
#endif
        
        //从服务器获取渠道信息
        CurrChannelInitConfig = new ChannelInitConfig();

        //初始化渠道配置
        InitChannelConfig(ref WebAccountUrl, ref ChannelId, ref InnerVersion);

        //前往数据下载网址按照网址，渠道号和版本号，下载资源内容
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic["ChannelId"] = ChannelId;
        dic["InnerVersion"] = InnerVersion; 
        NetWorkHttp.Instance.SendData(WebAccountUrl + "api/init", OnInitCallBack, isPost: true, dic: dic);


//#if UNITY_STANDALONE_WIN
//        string localFilePath = Application.persistentDataPath + "/";
//        //AppDebug.Log(localFilePath);
//        if (!Directory.Exists(localFilePath))
//        {
//            Directory.CreateDirectory(localFilePath);
//        }
//#endif
    }

    private void Update()
    {

    }

    #region 从本地XML文件中初始化渠道信息以及为本地数据提供资源存放地址信息 InitChannelConfig
    /// <summary>
    /// 从本地XML文件中初始化渠道信息以及为本地数据提供资源存放地址信息
    /// </summary>
    /// <param name="webAccountUrl">http资源存放基本网址</param>
    /// <param name="channelId">渠道号</param>
    /// <param name="innerVersion">内部版本号</param>
    private void InitChannelConfig(ref string webAccountUrl, ref int channelId, ref int innerVersion)
    {
        TextAsset asset = Resources.Load("Config/ChannelConfig") as TextAsset;
        XDocument xDoc = XDocument.Parse(asset.text);
        XElement root = xDoc.Root;
        webAccountUrl = root.Element("WebAccountUrl").Attribute("Value").Value;
        channelId = root.Element("ChannelId").Attribute("Value").Value.ToInt();
        innerVersion = root.Element("InnerVersion").Attribute("Value").Value.ToInt();
    }
    #endregion

    #region 根据服务器返回的消息，完成当前渠道配置，并在完成配置时调用渠道配置完成委托 OnInitCallBack
    /// <summary>
    /// 根据数据库返回的消息，完成当前渠道配置，并在完成配置时调用渠道配置完成委托
    /// </summary>
    /// <param name="obj">服务器返回的消息</param>
    private void OnInitCallBack(NetWorkHttp.CallBackArgs obj)
    {
        if (!obj.HasError)
        {
            string item = obj.Value;
            LitJson.JsonData data = LitJson.JsonMapper.ToObject(obj.Value);

            bool hasError = (bool)data["HasError"];
            if (!hasError)
            {
                LitJson.JsonData config = LitJson.JsonMapper.ToObject(data["Value"].ToString());
                Debug.Log("config==" + data["Value"].ToString());
                CurrChannelInitConfig.ServerTime = long.Parse(config["ServerTime"].ToString());
                CurrChannelInitConfig.SourceUrl = config["SourceUrl"].ToString();
                CurrChannelInitConfig.RechargeUrl = config["RechargeUrl"].ToString();
                CurrChannelInitConfig.TDAppId = config["TDAppId"].ToString();
                CurrChannelInitConfig.IsOpenTD = int.Parse(config["IsOpenTD"].ToString()) == 1;
                Debug.Log("ServerTime==" + CurrChannelInitConfig.ServerTime);
                Debug.Log("SourceUrl==" + CurrChannelInitConfig.SourceUrl);
                if (DelegateDefine.Instance.ChannelInitOK != null)
                {
                    DelegateDefine.Instance.ChannelInitOK();
                }
            }   
            else
            {
                string msg = data["ErrorMsg"].ToString();
                MessageCtrl.Instance.Show("提示", msg);
            }
        }
    }
    #endregion
}
