using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 数据下载管理器
/// </summary>
public class DownloadMgr : SingletonMiddle<DownloadMgr>
{
    #region 变量
    /// <summary>
    /// 下载超时时间
    /// </summary>
    public const int DownloadTimeOut = 5;
    /// <summary>
    /// 资源包最大数量
    /// </summary>
    public const int DownloadRoutineNum = 2;

    #region 实际的资源管理器网址（暂无）
    /// <summary>
    /// 数据的下载基础地址（这是站点模式，实际上是要从服务器读取）
    /// </summary>
    public static string DownloadBaseUrl = "http://192.168.31.178:8088/";
    /// <summary>
    /// 数据下载的完整http地址
    /// Windows/
    /// </summary>
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public static string DownloadUrl = DownloadBaseUrl + "Windows/";
#elif UNITY_ANDROID
    public static string DownloadUrl = DownloadBaseUrl + "Android/";
#elif UNITY_IPHONE
        public static string DownloadUrl = DownloadBaseUrl + "iOS/";
#endif
    #endregion

    /// <summary>
    /// 本地数据的资源路径（存储的读写路径）
    /// C:/Users/User/AppData/LocalLow/DefaultCompany/MMORPG/平台/
    /// </summary>
    public string localFilePath;

    /// <summary>
    /// 本地版本文件存储路径
    /// 通常在平台后，和Download同级,如.../Android/VersionFile.txt,但是在用户文件中是MMORPG/VersionFile.txt(很正常，用户路径没有打包层   )
    /// </summary>
    private string m_LocalVersionPath;

    /// <summary>
    /// 版本文件名
    /// </summary>
    public const string m_VersionFileName = "VersionFile.txt";

    /// <summary>
    /// 必须下载的数据列表（分为发生改动的资源和新加入的资源）
    /// </summary>
    private List<DownloadDataEntity> m_NeedDownloadDataList = new List<DownloadDataEntity>();
    /// <summary>
    /// 本地数据列表
    /// </summary>
    private List<DownloadDataEntity> m_LocalDataList = new List<DownloadDataEntity>();

    /// <summary>
    /// 服务器端（从http获取）的数据列表
    /// </summary>
    private List<DownloadDataEntity> m_ServerDataList;

    /// <summary>
    /// 资源初始化的原始路径（流文件下需要打包的文件路径）
    /// 即../StreamingAssets/AssetBundles/..
    /// </summary>
    private string m_StreamingAssetsPath;

    /// <summary>
    /// 游戏初始化完成回调（完成后通往登录场景）
    /// </summary>
    public Action OnInitComplete;
    #endregion

    #region 初始化主要操作完成资源本地化并启动主下载器和它的DownLoadFiles
    #region 进行资源初始化，有版本文件就检查更新，没有就从流文件的打包层获取文件进行的资源初始化 InitStreamingAssets(首次ReadStreamingAssetVersionFile（本地下载） 其他InitCheckVersion（资源管理器下载）)
    /// <summary>
    /// 进行资源初始化，有版本文件就检查更新，没有就进行彻底的资源初始化
    /// </summary>
    /// <param name="onInitComplete">当资源初始化完成时调用</param>
    public void InitStreamingAssets(Action onInitComplete)
    {
        localFilePath = Application.persistentDataPath + "/Windows/"; 
        Debug.Log("本地文件路径：" + localFilePath);
        OnInitComplete = onInitComplete;
        //设置本地版本文件路径
        m_LocalVersionPath = localFilePath + m_VersionFileName;

        //判断本地是否已经有版本文件   
        //若有则检查更新，无则进行初始化再检查更新
        if (File.Exists(m_LocalVersionPath))
        {
            //检查版本文件是否需要更新
            InitCheckVersion();
        }
        else 
        {
            //文件初始化需要打包的文件路径
            //如果没有资源 执行初始化 然后再检查更新
            m_StreamingAssetsPath = "file://" + Application.streamingAssetsPath + "/AssetBundles/Windows/";//UnityWebRequest下载本地数据的路径
//#if UNITY_ANDROID && !UNITY_EDITOR
//            m_StreamingAssetsPath = Application.streamingAssetsPath + "/AssetBundles/";//手机端
//#endif
            //设置本地版本文件存放路径
            string versionFileUrl = m_StreamingAssetsPath + m_VersionFileName;
            Debug.Log(versionFileUrl);
            //文件写入本地的路径：ReadStreamingAssetVersionFile->OnReadStreamingAssetOver->InitStreamingAssetList(先把资源写入本地，再把版本文件写入本地)
            //注：此处是才能够StreamingAssetsPath中进行资源下载，但是实际上从资源管理器中下载也行（也就是DownloadUrl+m_VersionFileName），只是因为我没有资源管理器才出次下策
            GlobalInit.Instance.StartCoroutine(ReadStreamingAssetVersionFile(versionFileUrl,OnReadStreamingAssetOver));
        }
    }
    #endregion

    #region 根据版本文件所在地址将版本文件下载至本地存储路径，并启动InitCheckVersion方法  ReadStreamingAssetVersionFile->OnReadStreamingAssetOver->InitStreamingAssetList(with AssetLoadToLocal)(先把资源写入本地，再把版本文件写入本地) 
    #region 根据版本文件存放网址下载版本文件，并调用版本文件读取完成回调 ReadStreamingAssetVersionFile
    /// <summary>
    /// 根据版本文件存放网址下载版本文件，并调用版本文件读取完成回调
    /// </summary>
    /// <param name="fileUrl">版本文件存放地址</param>
    /// <param name="onReadStreamingAssetOver">版本文件读取完成时调用</param>
    /// <returns></returns>
    private IEnumerator ReadStreamingAssetVersionFile(string fileUrl, Action<string> onReadStreamingAssetOver)
    {
        UISceneInitCtrl.Instance.SetProgress("初次启动，正在进行资源初始化", 0);
        using (UnityWebRequest www = UnityWebRequest.Get(fileUrl))
        {
            yield return www.SendWebRequest(); ;
            if (www.error == null)
            {
                if (onReadStreamingAssetOver != null)
                {
                    onReadStreamingAssetOver(www.downloadHandler.text);
                }
            }
            else
            {
                //版本文件读取失败
                Debug.LogError("版本文件读取失败");
                //onReadStreamingAssetOver("");
            }
        }
    }
    #endregion

    #region 根据传过来的版本文件内容启用游戏资源初始化协程 OnReadStreamingAssetOver
    /// <summary>
    /// 根据传过来的版本文件内容启用游戏资源初始化协程
    /// </summary>
    /// <param name="content">版本文件内容</param>
    private void OnReadStreamingAssetOver(string content)
    {
        GlobalInit.Instance.StartCoroutine(InitStreamingAssetList(content));
    }
    #endregion

    #region 根据版本文件内容，利用AssetLoadToLocal协程将版本文件和版本文件中记录的资源写入本地 InitStreamingAssetList
    /// <summary>
    /// 根据版本文件内容，利用AssetLoadToLocal协程将版本文件和版本文件中记录的资源写入本地
    /// </summary>
    /// <param name="content">版本文件内容</param>
    /// <returns></returns>
    private IEnumerator InitStreamingAssetList(string content)
    {
        //若传过来的版本文件是空的，那么就重新回去下载一遍
        if (string.IsNullOrEmpty(content))
        {
            Debug.Log("版本文件为空");
            InitCheckVersion();
            yield break;
        }
        //逐行读取文件内容
        string[] arr = content.Split('\n');
        //循环解压文件到本地
        for (int i = 0; i < arr.Length; i++)
        {
            string[] arrInfo = arr[i].Split(" ");
            //获取文件的短路径(反斜杠)（所以用正斜杠代替）
            string fileUrl = arrInfo[0].Replace("\\","/");
            //将文件逐一存入本地文件
            yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + fileUrl, localFilePath + fileUrl));
            float value = (i + 1) / (float)arr.Length;
            UISceneInitCtrl.Instance.SetProgress(string.Format("资源初始化(不消耗流量)：{0}/{1}", i + 1, arr.Length), value);
        }
        //将版本文件写入本地
        yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + m_VersionFileName, localFilePath + m_VersionFileName));
        //检查是否需要更新
        InitCheckVersion();
    }
    #endregion

    #region 根据资源网址下载资源并存放到本地 AssetLoadToLocal
    /// <summary>
    /// 根据资源网址下载资源并存放到本地
    /// </summary>
    /// <param name="fileUrl">资源所在网址</param>
    /// <param name="toPath">下载的资源存放在本地的位置</param>
    /// <returns></returns>
    private IEnumerator AssetLoadToLocal(string fileUrl, string toPath)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(fileUrl))
        {
            yield return www.SendWebRequest();
            if (www.error == null)
            {
                int lastIndexOf = toPath.LastIndexOf("\\");
                if (lastIndexOf == -1)
                {
                    lastIndexOf = toPath.LastIndexOf("/");
                }
                //创建文件夹
                if (lastIndexOf != -1)
                {
                    //制作资源存放文件夹
                    string localPath = toPath.Substring(0, lastIndexOf);
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }
                }
                //将文件资源以二进制模式写入本地
                using (FileStream fs = File.Create(toPath, www.downloadHandler.data.Length))
                {
                    fs.Write(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
                    fs.Close();
                }
            }
        }
    }
    #endregion
    #endregion

    #region 检查版本更新系 InitCheckVersion(召唤主下载器协助)->PackDownloadData->OnInitVersionCallBack(with PackDownloadDataDic PackDownloadDataDic GetDownloadData)->启动主下载器的(DownloadFiles)
    #region 向资源服务器下载最新的版本文件，并与本地版本进行核对更新（启动后创建打包资源下载实体，并启动DownloadVersion） InitCheckVersion
    /// <summary>
    /// 向http下载最新的版本文件，并与本地版本进行核对更新
    /// </summary>
    public void InitCheckVersion()
    {
        UISceneInitCtrl.Instance.SetProgress("正在检查版本更新！", 0);
        //版本文件所在网址/地址
        string strVersionUrl = DownloadUrl + m_VersionFileName;
        //因为没有资源服务器，所以在此利用本地文件代替网址
        strVersionUrl = "file://" + "C:/Users/User/Unity/MMORPG/AssetBundles/Windows/VersionFile.txt";
        //读取版本文件
        AssetBundleDownload.Instance.InitServerVersion(strVersionUrl, OnInitVersionCallBack);
    }
    #endregion

    #region 返回版本文件记叙的资源信息链表 PackDownloadData
    /// <summary>
    /// 返回版本文件记叙的资源信息链表
    /// 打包前形态：按打包前格式分布的形态，本次采用的格式是（Dowload路径，MD5信息，大小，是否是初始数据）
    /// </summary>
    /// <param name="content">版本文件内容</param>
    /// <returns></returns>
    public List<DownloadDataEntity> PackDownloadData(string content)
    {
        List<DownloadDataEntity> list = new List<DownloadDataEntity>();
        //按行进行分割
        string[] arrLines = content.Split("\n");
        for (int i = 0; i < arrLines.Length; i++)
        {
            //获取各个版本信息（路径，MD5信息，大小，是否是初始数据）
            string[] arrData = arrLines[i].Split(" ");
            //注：头两行是平台，平台依赖配置文件的信息
            if (arrData.Length == 4)
            {
                DownloadDataEntity entity = new DownloadDataEntity();
                //获取Download路径
                entity.FullName = arrData[0];
                //获取MD5加密形态
                entity.MD5 = arrData[1];
                entity.Size = arrData[2].ToInt();
                entity.IsFirstData = arrData[3].ToInt() == 1;
                list.Add(entity);
            }
        }
        return list;
    }
    #endregion

    #region 通过传过来的http版本文件信息链表，比对http版本文件和本地版本文件进行游戏初始化资源更新，并发布资源下载命令完成初始化工作 OnInitVersionCallBack
    /// <summary>
    /// 通过传过来的http版本文件信息链表，比对http版本文件和本地版本文件进行游戏初始化资源更新，并发布资源下载命令完成初始化工作
    /// </summary>
    /// <param name="serverDownloadData">版本文件信息实体列表</param>
    private void OnInitVersionCallBack(List<DownloadDataEntity> serverDownloadData)
    {
        //最新的版本文件数据列表
        m_ServerDataList = serverDownloadData;
        if (File.Exists(m_LocalVersionPath))
        {
            //若本地存在版本文件，那么只需要将其与服务器端传输过来的数据进行对比，检测不同的部分进行更新
            //制作http数据关键信息字典《数据Download层路径，数据信息MD5加密形态》
            Dictionary<string, string> serverDic = PackDownloadDataDic(serverDownloadData);
            //获取本地版本文件的内容
            string content = IOUtil.GetFileText(m_LocalVersionPath);
            //制作本地数据关键信息字典《数据Download层路径，数据信息MD5加密形态》
            Dictionary<string, string> clientDic = PackDownloadDataDic(content);
            //将本地版本文件数据制作成列表
            m_LocalDataList = PackDownloadData(content);

            //进行数据对比
            //1.获取新追加加的初始资源
            for (int i = 0; i < serverDownloadData.Count; i++)
            {
                //如果该数据是初始资源，同时客户端中不存在该资源，那么就更新
                if (serverDownloadData[i].IsFirstData && !clientDic.ContainsKey(serverDownloadData[i].FullName))
                {
                    //将数据加入下载列表
                    m_NeedDownloadDataList.Add(serverDownloadData[i]);
                }
            }

            //2.比对旧资源，看看是否需要更新
            foreach (var item in clientDic)
            {
                //MD5码中存储了我们每条数据的具体值，如果有不同，则说明我们需要进行数据更新了
                if (serverDic.ContainsKey(item.Key) && serverDic[item.Key] != item.Value)
                {
                    //获取数据实体，将其加入必要下载列表中
                    DownloadDataEntity entity = GetDownloadData(item.Key, serverDownloadData);
                    if (entity != null)
                    {
                        m_NeedDownloadDataList.Add(entity); ;
                    }
                    else
                    {
                        Debug.Log("未找到目标数据");
                    }
                }
            }
        }
        else
        {
            //若这只是玩家首次启动游戏，则应该进行数据下载
            //获取需要下载的数据列表
            for (int i = 0; i < serverDownloadData.Count; i++)
            {
                if (serverDownloadData[i].IsFirstData)
                {
                    m_NeedDownloadDataList.Add(serverDownloadData[i]);
                }
            }
        }

        //如果发现玩家版本就是最新版本，则不需要下载，
        if (m_NeedDownloadDataList.Count == 0)
        {
            UISceneInitCtrl.Instance.SetProgress("当前版本是最新版本，无需更新", 1);
            if (OnInitComplete != null)
            {
                OnInitComplete();
            }
            return;
        }
        else
        {
            //向各个资源包线程发布下载命令进行数据下载
            AssetBundleDownload.Instance.DownloadFiles(m_NeedDownloadDataList);
        }
    }
    #endregion

    #region 根据传过来的数据实体链表封装数据关键信息字典 PackDownloadDataDic（http文件系）
    /// <summary>
    /// 根据传过来的数据实体，链表封装数据关键信息字典
    /// 字典格式：（文件路径（eg:download/xx），文件内容的MD5形态）
    /// </summary>
    /// <param name="list">数据实体列表</param>
    /// <returns>数据关键信息字典</returns>
    public Dictionary<string, string> PackDownloadDataDic(List<DownloadDataEntity> list)
    {
        //数据下载详情字典，<数据路径名，数据的MD5形态>
        Dictionary<string, string> dic = new Dictionary<string, string>();
        for (int i = 0; i < list.Count; i++)
        {
            dic[list[i].FullName] = list[i].MD5;
        }
        return dic;
    }
    #endregion

    #region  根据所传文本内容制作字典（实际上固定是版本文件格式的文本） PackDownloadDataDic（本地文件系）
    /// <summary>
    /// 根据所传文本内容制作字典（实际上固定是版本文件格式的文本）
    /// 《资源路径，资源内容》
    /// </summary>
    /// <param name="content">文本内容</param>
    /// <returns></returns>
    public Dictionary<string, string> PackDownloadDataDic(string content)
    {
        //数据下载详情字典，<数据路径名，数据的MD5形态>
        Dictionary<string, string> dic = new Dictionary<string, string>();
        //将传过来的要解析的内容进行拆解
        string[] arrLines = content.Split("\n");
        for (int i = 0; i < arrLines.Length; i++)
        {
            string[] arrData = arrLines[i].Split("");
            if (arrData.Length == 4)
            {
                dic[arrData[0]] = arrData[1];
            }
        }
        return dic;
    }
    #endregion

    #region 检测某数据是否在所传资源列表中 GetDownloadData
    /// <summary>
    /// 检测某数据是否在所传资源列表中
    /// </summary>
    /// <param name="fullName"> 数据的全路径（指在平台之后的全路径如：windows的就是windows后（eg：download/xxx</param>
    /// <param name="list">总的资源列表</param>
    /// <returns>该路径对应的实体</returns>
    private DownloadDataEntity GetDownloadData(string fullName, List<DownloadDataEntity> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].FullName.Equals(fullName, StringComparison.CurrentCultureIgnoreCase))
            {
                return list[i];
            }
        }
        return null;
    }
    #endregion
    #endregion
    #endregion

    #region 版本文件同步 ModifyLocalData->SaveLocalVersion:更新本地版本文件并完成同步

    #region 更新并同步本地版本文件 ModifyLocalData启动(SaveLocalVersion)
    /// <summary>
    /// 根据传过来的要修改的本地数据对其进行更新并同步本地版本文件
    /// </summary>
    /// <param name="entity">资源实体</param>
    public void ModifyLocalData(DownloadDataEntity entity)
    {
        if(m_LocalDataList == null)
        { return; }
        //目标文件是否已经存在
        bool isExists = false;
        for (int i = 0; i < m_LocalDataList.Count; i++)
        {
            //若文件存在，则修改里面的数据
            if (m_LocalDataList[i].FullName.Equals(entity.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                m_LocalDataList[i].MD5 = entity.MD5;
                m_LocalDataList[i].Size = entity.Size;
                m_LocalDataList[i].IsFirstData = entity.IsFirstData;
                isExists = true;
                break;
            }
        }
        //若文件不存在，则将其加入本地数据列表，以供后续操作
        if (!isExists)
        {
            m_LocalDataList.Add(entity);
        }
        //每次修改完数据都应该保存一下版本文件
        SaveLocalVersion();
    }
    #endregion

    #region 保存本地的版本文件 SaveLocalVersion
    /// <summary>
    /// 保存本地的版本文件
    /// </summary>
    private void SaveLocalVersion()
    {
        StringBuilder sbContent = new StringBuilder();
        for (int i = 0; i < m_LocalDataList.Count; i++)
        {
            sbContent.AppendLine(String.Format("{0} {1} {2} {3}", m_LocalDataList[i].FullName, m_LocalDataList[i].MD5, m_LocalDataList[i].Size, m_LocalDataList[i].IsFirstData?1:0));
        }

        IOUtil.CreateTextFile(m_LocalVersionPath, sbContent.ToString());
    }
    #endregion

    #endregion

    #region 将文件路径改成http路径并返回其数据实体 GetServerData
    /// <summary>
    /// 将文件路径改成http路径并返回其数据实体
    /// </summary>
    /// <param name="path">指版本文件中的路径(可用正斜杠)</param>
    /// <returns></returns>
    public DownloadDataEntity GetServerData(string path)
    {
        if (m_ServerDataList == null)
        { return null; }
        for(int i = 0;i<m_ServerDataList.Count;i++)
        {
            if (path.Replace("/", "\\").Equals(m_ServerDataList[i].FullName,StringComparison.CurrentCultureIgnoreCase))
            {
                return m_ServerDataList[i];
            }
        }
        return null;
    }
    #endregion
}
