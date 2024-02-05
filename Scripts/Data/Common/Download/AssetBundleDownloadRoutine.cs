using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// 资源包下载器：用于模拟多线程下载
/// </summary>
public class AssetBundleDownloadRoutine : MonoBehaviour
{
    #region 变量
    /// <summary>
    /// 需要下载的数据实体列表
    /// </summary>
    private List<DownloadDataEntity> m_List = new List<DownloadDataEntity>();
    /// <summary>
    /// 当前正在下载的数据
    /// </summary>
    private DownloadDataEntity m_CurrentDownloadData;
    /// <summary>
    /// 需要下载的数据数量
    /// </summary>
    public int NeedDownloadCount
    {
        get;
        private set;
    }

    /// <summary>
    /// 已经下载完成的数量
    /// </summary>
    public int CompleteCount
    {
        get;
        private set;
    }

    /// <summary>
    /// 已经下载好的文件的大小
    /// </summary>
    private int m_DownloadSize;
    /// <summary>
    /// 目前已经下载的文件大小
    /// </summary>
    private int m_CurrentDownloadSize;

    /// <summary>
    /// 该下载器已经下载的大小
    /// </summary>
    public int DownloadSize
    {
        get { return m_DownloadSize + m_CurrentDownloadSize; }
    }

    /// <summary>
    /// 是否开始下载
    /// </summary>
    public bool IsStartDownload
    {
        get;
        private set;
    }
    #endregion

    #region 给资源包线程派发下载对象和下载命令 AddDownload StartDownload（开始Update）
    /// <summary>
    /// 添加下载对象
    /// </summary>
    /// <param name="entity">要下载的实体</param>
    public void AddDownload(DownloadDataEntity entity)
    {
        m_List.Add(entity);
    }
    /// <summary>
    /// 开始下载
    /// </summary>
    public void StartDownload()
    {
        IsStartDownload = true;
        NeedDownloadCount = m_List.Count;
    }
    #endregion

    private void Update()
    {
        //当下载命令下发时，资源包下载器开始从http网页下载数据
        if(IsStartDownload)
        {
            IsStartDownload = false;
            StartCoroutine(DownloadData());
        }
    }

    #region 检索资源包分配的每一个下载任务从资源管理器进行下载(一次一个)，并存入本地文件夹 DownloadData(启动ModifyLocalData)
    /// <summary>
    /// 检索资源包分配的每一个下载任务从资源管理器进行下载，并存入本地文件夹
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadData()
    {
        //如果下发命令了但是没有需要下载的文件返回
        if (NeedDownloadCount == 0)
        { yield break; }
        //取出列表的路径
        m_CurrentDownloadData = m_List[0];
        //网页路径
        string dataUrl =DownloadMgr.DownloadUrl + m_CurrentDownloadData.FullName;
        dataUrl =Path.Combine("file://" + "C:/Users/User/Unity/MMORPG/AssetBundles/Windows/",m_CurrentDownloadData.FullName.Replace("\\","/"));//因为没有资源管理器加上正反斜杠问题的正化
        //网页路径指定文件夹的位置
        int lastIndex = m_CurrentDownloadData.FullName.LastIndexOf("\\");
        if (lastIndex == -1)
        {
            lastIndex = m_CurrentDownloadData.FullName.LastIndexOf("/");
        }
        //文件夹创建
        if (lastIndex > -1)
        {
            int indexLast = m_CurrentDownloadData.FullName.LastIndexOf("\\");
            if (indexLast == -1)
            {
                indexLast = m_CurrentDownloadData.FullName.LastIndexOf("/");
            }
            //资源在本地的存储短路径(MMORPG后面的路径)（文件夹路径，不是文件）
            string path = m_CurrentDownloadData.FullName.Substring(0, indexLast);
            //获取玩家的本地存储路径
            string localFilePath = DownloadMgr.Instance.localFilePath + path;
            //如果玩家还没有存储文件夹，则创建
            if (!Directory.Exists(localFilePath))
            {
                Directory.CreateDirectory(localFilePath);
            }
        }

        //进行数据下载
        using (UnityWebRequest www =  UnityWebRequest.Get(dataUrl))
        {
            www.SendWebRequest();
            //获取下载时间与进程
            float timeout = Time.time;
            float progress = www.downloadProgress;
            while (www != null && !www.isDone)
            {
                if (progress < www.downloadProgress)
                {
                    timeout = Time.time;
                    progress = www.downloadProgress;
                    m_CurrentDownloadSize = (int)(m_CurrentDownloadData.Size * progress);
                }
                if ((Time.time - timeout) > DownloadMgr.DownloadTimeOut)
                {
                    Debug.Log("下载超时");
                    yield break;
                }
                //每次等待一帧再检查
                yield return null;
            }

            //如果数据下载成功无误，则将资源保存到用户本地文件夹上
            if (www != null && www.error == null)
            {
                using (FileStream fs = new FileStream(DownloadMgr.Instance.localFilePath + m_CurrentDownloadData.FullName, FileMode.Create, FileAccess.ReadWrite))
                {
                    //将数据以二进制的方式写入
                    fs.Write(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
                }
            }
        }

        //下载成功后初始化要下载的文件的大小，并统计入要下载的总大小中
        m_CurrentDownloadSize = 0;
        m_DownloadSize += m_CurrentDownloadData.Size;
        //将数据写入本地的版本文件
        DownloadMgr.Instance.ModifyLocalData(m_CurrentDownloadData);

        m_List.RemoveAt(0);
        CompleteCount++;
        if (m_List.Count == 0)
        {
            m_List.Clear();
        }
        else
        {
            IsStartDownload = true;
        }
    }
    #endregion
}
