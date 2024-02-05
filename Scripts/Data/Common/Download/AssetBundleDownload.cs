using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 资源包下载的主下载器
/// </summary>
public class AssetBundleDownload : SingletonMono<AssetBundleDownload>
{
    #region 变量
    /// <summary> 
    /// 版本文件网址
    /// </summary>
    private string m_VersionUrl;

    /// <summary>
    /// 版本文件初始化回调
    /// </summary>
    private Action<List<DownloadDataEntity>> m_OnInitVersion;

    /// <summary>
    /// 资源包下载器数组
    /// </summary>
    private AssetBundleDownloadRoutine[] m_Routine = new AssetBundleDownloadRoutine[DownloadMgr.DownloadRoutineNum];

    /// <summary>
    /// 资源包下载器索引
    /// </summary>
    private int m_RoutineIndex = 0;

    /// <summary>
    /// 是否下载完成
    /// </summary>
    private bool m_IsDownloadOver = false;

    /// <summary>
    /// 采样时间
    /// </summary>
    private float m_SamplingTime = 1;
    
    /// <summary>
    /// 已经下载的时间
    /// </summary>
    private float m_AlreadyTime = 0;
    
    /// <summary>
    /// 还要下载的时间
    /// </summary>
    private float m_LeftTime = 0f;
    
    /// <summary>
    /// 下载的速度
    /// </summary>
    private float m_Speed = 0f;

    /// <summary>
    /// 资源包的要下载总大小
    /// </summary>
    public int TotalSize
    {
        get;
        private set;
    }

    /// <summary>
    /// 资源包的资源要下载的总数
    /// </summary>
    public int TotalCount
    {
        get;
        private set;
    }
    #endregion

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    #region 主下载器协助资源初始化 OnStart DownloadVersion InitServerVersion DownloadFiles(开启协助下载器的搬砖生涯)
    protected override void OnStart()
    {
        base.OnStart();
        //去网站下载版本文件
        StartCoroutine(DownloadVersion(m_VersionUrl));
    }

    #region 主下载器协助下载资源管理器的最新版本文件  DownloadVersion InitServerVersion
    #region 最新版本文件初始化（创建资源下载物体和启动DownloadVersion）  InitServerVersion 
    /// <summary>
    /// 初始化http版本信息
    /// </summary>
    /// <param name="url">http对应的版本文件网址(全)</param>
    /// <param name="onInitVersionCallBack">版本初始化回调</param>
    public void InitServerVersion(string url, Action<List<DownloadDataEntity>> onInitVersionCallBack)
    {
        m_VersionUrl = url;
        m_OnInitVersion = onInitVersionCallBack;
    }
    #endregion

    #region  向版本文件所在网站下载版本文件协程 DownloadVersion
    /// <summary>
    /// 下载版本文件
    /// 向版本文件所在网站下载版本文件协程
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator DownloadVersion(string url)
    {
        Debug.Log("版本文件地址" + url);
        yield return new WaitForSeconds(0.1f);
        //去网站下载版本数据
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            //获取下载时间和下载进度
            float timeOut = Time.time;
            float progress = www.downloadProgress;

            //当数据申请流程没完成时
            while (www != null && !www.isDone)
            {
                //随时同步更新进程
                if (progress < www.downloadProgress)
                {
                    timeOut = Time.time;
                    progress = www.downloadProgress;
                }
                //如果下载超时
                if ((Time.time - timeOut) > DownloadMgr.DownloadTimeOut)
                {
                    Debug.Log("下载时间过长");
                    yield break;
                }
            }
            //如果下载完成，则获取下载的内容
            if (www != null && www.error == null)
            {
                //获取版本文件内容
                string content = www.downloadHandler.text;
                if (m_OnInitVersion != null)
                {
                    m_OnInitVersion(DownloadMgr.Instance.PackDownloadData(content));
                }
            }
            else
            {
                Debug.Log("下载失败 原因如下：" + www.error);
            }
        }
    }
    #endregion
    #endregion

    #region 使用资源包线程下载数据（含赋值，分配任务AddDownload和发放下载命令流程StartDownload） DownloadFiles
    /// <summary>
    /// 使用资源包线程下载数据（含赋值，分配任务和发放下载命令流程）
    /// </summary>
    /// <param name="downLoadList">要下载的数据列表</param>
    public void DownloadFiles(List<DownloadDataEntity> downLoadList)
    {
        TotalSize = 0;
        //为各个资源包线程添加下载脚本
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            {
                m_Routine[i] = gameObject.AddComponent<AssetBundleDownloadRoutine>();
            }
        }

        //启动每一个资源包线程分配下载任务
        for (int i = 0; i < downLoadList.Count; i++)
        {
            //循环利用下载器进行数据下载
            m_RoutineIndex = m_RoutineIndex % m_Routine.Length;
            m_Routine[m_RoutineIndex].AddDownload(downLoadList[i]);
            m_RoutineIndex++;
            TotalSize += downLoadList[i].Size;
            TotalCount++;
        }

        //启动每一个资源包线程开始进行资源下载
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            { continue; }
            m_Routine[i].StartDownload();
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// 更新玩家界面进度条
    /// </summary>
    protected override void OnUpdate()
    {
        base.OnUpdate();
        #region 同步初始化资源下载信息并开启版本资源下载完成回调
        //当存储需要下载的文件数量大于0，且没有下载完成时
        if (TotalCount > 0&& !m_IsDownloadOver)
        {
            //获取当前已经下载的文件的数量与大小
            int totalCompleteCount = CurrentCompleteTotalCount();
            totalCompleteCount = totalCompleteCount == 0 ? 1 : totalCompleteCount;
            int totalCompleteSize = CurrentCompleteTotalSize();

            //统计已经下载的时间与情况
            m_AlreadyTime += Time.deltaTime;
            if (m_AlreadyTime > m_SamplingTime&&m_Speed == 0)
            {
                //获取下载速度
                m_Speed = totalCompleteSize / m_SamplingTime;
            }

            //计算剩余时间 = （总大小-已经下载的大小）/速度
            if (m_Speed > 0)
            {
                m_LeftTime = (TotalSize - totalCompleteSize) / m_Speed;
            }
            if (m_LeftTime > 0)
            {
                string strLeftTime = string.Format("剩余时间{0}秒", m_LeftTime);
            }

            //打印下载进度
            string str = string.Format("正在下载{0}/{1}", totalCompleteCount, TotalCount);
            UISceneInitCtrl.Instance.SetProgress(str,totalCompleteCount/(float)TotalCount);
         
            if (totalCompleteCount == TotalCount)
            {
                m_IsDownloadOver = true;
                UISceneInitCtrl.Instance.SetProgress("资源更新完毕",1);
                if (DownloadMgr.Instance.OnInitComplete != null)
                {
                    DownloadMgr.Instance.OnInitComplete();
                }
            }
        }
        #endregion
    }

    #region 根据传过来的数据实体向资源管理器下载数据写入本地，同时将数据写入本地并更新版本文件 DownloadData（意义未知）
    /// <summary>
    /// 根据传过来的数据实体向Http网站下载数据写入本地，同时将数据写入本地并更新版本文件
    /// </summary>
    /// <param name="currDownloadData">要下载的数据实体</param>
    /// <param name="onComplete">下载完成回调</param>
    /// <returns></returns>
    public IEnumerator DownloadData(DownloadDataEntity currDownloadData, Action<bool> onComplete)
    {
        //获取目标资源的下载网址
        string dataUrl = "file://" + "C:/Users/User/Unity/MMORPG/AssetBundles/Windows/" + currDownloadData.FullName.Replace("\\", "/");//因为没有资源管理器加上正反斜杠问题的正化

        int dirIndex = currDownloadData.FullName.LastIndexOf('\\');
        if (dirIndex == -1)
        {
            dirIndex = currDownloadData.FullName.LastIndexOf('/');
        }

        //获取目标资源所在的文件夹（即文件的前一位如a\a.txt则是a）
        string path = currDownloadData.FullName.Substring(0, dirIndex);

        //获取玩家的本地存储路径
        string localFilePath = DownloadMgr.Instance.localFilePath + path;
        //在玩家本地创建资源存储文件夹
        if (!Directory.Exists(localFilePath))
        {
            Directory.CreateDirectory(localFilePath);
        }
        //进行数据下载
        using (UnityWebRequest www =UnityWebRequest.Get(dataUrl))
        {
            www.SendWebRequest();
            float timeout = Time.time;
            float progress = www.downloadProgress;
            while (www != null && !www.isDone)
            {
                if (progress < www.downloadProgress)
                {
                    timeout = Time.time;
                    progress = www.downloadProgress;
                }
                if ((Time.time - timeout) > DownloadMgr.DownloadTimeOut)
                {
                    if (onComplete != null)
                    {
                        Debug.Log("资源下载失败");
                        onComplete(false);
                    }
                    yield break;
                }
                //每次等待一帧再检查
                yield return null;
            }

            //如果数据下载成功无误，则将资源保存到用户本地文件夹上
            if (www != null && www.error == null)
            {
                using (FileStream fs = new FileStream(DownloadMgr.Instance.localFilePath + currDownloadData.FullName, FileMode.Create, FileAccess.ReadWrite))
                {
                    //将数据以二进制的方式写入
                    fs.Write(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
                }
            }
        }
        //将数据写入本地的版本文件
        DownloadMgr.Instance.ModifyLocalData(currDownloadData);

        //执行文件下载成功委托
        if (onComplete != null)
        {
            onComplete(true);
        }
    }
    #endregion

    #region 当前已经下载的文件总大小和数量 CurrentCompleteTotalSize CurrentCompleteTotalCount
    /// <summary>
    /// 当前已经下载的文件总大小
    /// </summary>
    /// <returns>已下载的文件大小</returns>
    public int CurrentCompleteTotalSize()
    {
        int completeTotalSize = 0;
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            { continue; }
            completeTotalSize += m_Routine[i].DownloadSize;
        }
        return completeTotalSize;
    }

    /// <summary>
    /// 当前已经下载的文件总数量
    /// </summary>
    /// <returns>已下载的文件数量</returns>
    public int CurrentCompleteTotalCount()
    {
        int completeTotalCount = 0;
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            { continue; }
            completeTotalCount += m_Routine[i].CompleteCount;
        }
        return completeTotalCount;
    }
    #endregion
}
