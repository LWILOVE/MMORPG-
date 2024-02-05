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
/// ȫ�ֳ�ʼ����
/// </summary>
[XLua.LuaCallCSharp]
public class GlobalInit : MonoBehaviour
{
    public InitWay currentInitWay = InitWay.AuthorUse;

    #region ����
    #region ����
    public static GlobalInit Instance;
    private void Awake()
    {
        //������Ϸ֡��
        Application.targetFrameRate = 60;
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region ����������
    /// <summary>
    /// ��ǰ�˺�
    /// </summary>
    [HideInInspector]
    public RetAccountEntity CurrAccount;
    /// <summary>
    /// ��ǰ���
    /// </summary>
    public RoleCtrl currentPlayer;
    /// <summary>
    /// ���ע����
    /// </summary>
    public string CurrRoleNickName;
    /// <summary>
    /// С��ͼ��������
    /// </summary>
    public float smallMapFollowRate;
    /// <summary>
    /// ��ǰѡ�е���Ϸ��
    /// </summary>
    [HideInInspector]
    public RetGameServerEntity CurrentSelectGameServer;
    /// <summary>
    /// �ӷ������л�ȡ����ʱ���
    /// </summary>
    public long ServerTime = 0;
    /// <summary>
    /// �����������ʱ��ȶԵĿͻ���ʱ��
    /// </summary>
    public float CheckServerTime;
    /// <summary>
    /// ��ϷĿǰ��pingֵ
    /// </summary>
    public int PingValue;
    /// <summary>
    /// �ͻ�����Ϊ�ķ�����ʱ��
    /// </summary>
    public long GameServerTime;
    #endregion

    #region Http���ʱ���
    /// <summary>
    /// ר�ŷ������û��ͷ���������ַ��ע���������������Դ����ַ������
    /// </summary>
    public static string WebAccountUrl;
    /// <summary>
    /// ����ID
    /// </summary>
    public static int ChannelId;
    /// <summary>
    /// �ڲ��汾��
    /// </summary>
    public static int InnerVersion;
    /// <summary>
    /// ��ǰ�������ã��ں�һЩ������ר������Ϣ��
    /// </summary>
    public ChannelInitConfig CurrChannelInitConfig;
    #endregion

    /// <summary>
    /// ��ǰ����
    /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static string Channel = "Windows/";
#elif UNITY_ANDROID
    public static string DownloadUrl = DownloadBaseUrl + "Android/";
#elif UNITY_IPHONE
        public static string DownloadUrl = DownloadBaseUrl + "iOS/";
#endif

    //�޷���Э�����ί��
    public delegate void OnReceiveProtoHandler(ushort protoCode, byte[] buffer);
    //����ί��
    public OnReceiveProtoHandler OnReceiveProto;

    #region ��ɫ�͵������
    /// <summary>
    /// T4M��Sharder
    /// </summary>
    public Shader T4MShaeder;
    /// <summary>
    /// ��պ�Shader
    /// </summary>
    public Shader MogoSkyBoxShader;
    /// <summary>
    /// ��������
    /// </summary>
    public AnimationCurve UIAnimationCurve;
    /// <summary>
    /// ����ɫ�����ֵ�
    /// </summary>
    public Dictionary<int, GameObject> JobObjectDic = new Dictionary<int, GameObject>();
    /// <summary>
    /// ������Ϣ
    /// </summary>
    public RoleInfoMainPlayer MainPlayerInfo;
    #endregion

    #endregion
    #region ʱ��ϵ
    /// <summary>
    /// ��ȡ��ǰ������ʱ��������ʽ
    /// </summary>
    /// <returns></returns>
    public string GetCurrentTime()
    {
        return DateTime.Now.ToString("yyyyMMddHHmmssfff");
    }
    /// <summary>
    /// ��ȡ��ǰ�ķ�����ʱ��
    /// </summary>
    /// <returns></returns>
    public long GetCurrentServerTime()
    {
        return (int)(((Time.realtimeSinceStartup - CheckServerTime) * 1000)) + GameServerTime;
    }
    /// <summary>
    /// ʱ��=��Ϸ����������ʱ��+ʱ���
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
    /// ��ȡ��1970�굽���ڵ�ʱ��
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
        // Mac �����ߴ����� -- �״�����󣬵ڶ��λ�ʹ���ϴγߴ�
        Resolution[] reslution = Screen.resolutions;
        float standard_width = reslution[reslution.Length - 1].width;
        float standard_height = ((standard_width * 9) / 16);
        Screen.SetResolution(Convert.ToInt32(standard_width * 0.8f), Convert.ToInt32(standard_height * 0.8f), false);
#endif
        
        //�ӷ�������ȡ������Ϣ
        CurrChannelInitConfig = new ChannelInitConfig();

        //��ʼ����������
        InitChannelConfig(ref WebAccountUrl, ref ChannelId, ref InnerVersion);

        //ǰ������������ַ������ַ�������źͰ汾�ţ�������Դ����
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

    #region �ӱ���XML�ļ��г�ʼ��������Ϣ�Լ�Ϊ���������ṩ��Դ��ŵ�ַ��Ϣ InitChannelConfig
    /// <summary>
    /// �ӱ���XML�ļ��г�ʼ��������Ϣ�Լ�Ϊ���������ṩ��Դ��ŵ�ַ��Ϣ
    /// </summary>
    /// <param name="webAccountUrl">http��Դ��Ż�����ַ</param>
    /// <param name="channelId">������</param>
    /// <param name="innerVersion">�ڲ��汾��</param>
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

    #region ���ݷ��������ص���Ϣ����ɵ�ǰ�������ã������������ʱ���������������ί�� OnInitCallBack
    /// <summary>
    /// �������ݿⷵ�ص���Ϣ����ɵ�ǰ�������ã������������ʱ���������������ί��
    /// </summary>
    /// <param name="obj">���������ص���Ϣ</param>
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
                MessageCtrl.Instance.Show("��ʾ", msg);
            }
        }
    }
    #endregion
}
