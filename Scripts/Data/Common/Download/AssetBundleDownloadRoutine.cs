using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// ��Դ��������������ģ����߳�����
/// </summary>
public class AssetBundleDownloadRoutine : MonoBehaviour
{
    #region ����
    /// <summary>
    /// ��Ҫ���ص�����ʵ���б�
    /// </summary>
    private List<DownloadDataEntity> m_List = new List<DownloadDataEntity>();
    /// <summary>
    /// ��ǰ�������ص�����
    /// </summary>
    private DownloadDataEntity m_CurrentDownloadData;
    /// <summary>
    /// ��Ҫ���ص���������
    /// </summary>
    public int NeedDownloadCount
    {
        get;
        private set;
    }

    /// <summary>
    /// �Ѿ�������ɵ�����
    /// </summary>
    public int CompleteCount
    {
        get;
        private set;
    }

    /// <summary>
    /// �Ѿ����غõ��ļ��Ĵ�С
    /// </summary>
    private int m_DownloadSize;
    /// <summary>
    /// Ŀǰ�Ѿ����ص��ļ���С
    /// </summary>
    private int m_CurrentDownloadSize;

    /// <summary>
    /// ���������Ѿ����صĴ�С
    /// </summary>
    public int DownloadSize
    {
        get { return m_DownloadSize + m_CurrentDownloadSize; }
    }

    /// <summary>
    /// �Ƿ�ʼ����
    /// </summary>
    public bool IsStartDownload
    {
        get;
        private set;
    }
    #endregion

    #region ����Դ���߳��ɷ����ض������������ AddDownload StartDownload����ʼUpdate��
    /// <summary>
    /// ������ض���
    /// </summary>
    /// <param name="entity">Ҫ���ص�ʵ��</param>
    public void AddDownload(DownloadDataEntity entity)
    {
        m_List.Add(entity);
    }
    /// <summary>
    /// ��ʼ����
    /// </summary>
    public void StartDownload()
    {
        IsStartDownload = true;
        NeedDownloadCount = m_List.Count;
    }
    #endregion

    private void Update()
    {
        //�����������·�ʱ����Դ����������ʼ��http��ҳ��������
        if(IsStartDownload)
        {
            IsStartDownload = false;
            StartCoroutine(DownloadData());
        }
    }

    #region ������Դ�������ÿһ�������������Դ��������������(һ��һ��)�������뱾���ļ��� DownloadData(����ModifyLocalData)
    /// <summary>
    /// ������Դ�������ÿһ�������������Դ�������������أ������뱾���ļ���
    /// </summary>
    /// <returns></returns>
    private IEnumerator DownloadData()
    {
        //����·������˵���û����Ҫ���ص��ļ�����
        if (NeedDownloadCount == 0)
        { yield break; }
        //ȡ���б��·��
        m_CurrentDownloadData = m_List[0];
        //��ҳ·��
        string dataUrl =DownloadMgr.DownloadUrl + m_CurrentDownloadData.FullName;
        dataUrl =Path.Combine("file://" + "C:/Users/User/Unity/MMORPG/AssetBundles/Windows/",m_CurrentDownloadData.FullName.Replace("\\","/"));//��Ϊû����Դ��������������б�����������
        //��ҳ·��ָ���ļ��е�λ��
        int lastIndex = m_CurrentDownloadData.FullName.LastIndexOf("\\");
        if (lastIndex == -1)
        {
            lastIndex = m_CurrentDownloadData.FullName.LastIndexOf("/");
        }
        //�ļ��д���
        if (lastIndex > -1)
        {
            int indexLast = m_CurrentDownloadData.FullName.LastIndexOf("\\");
            if (indexLast == -1)
            {
                indexLast = m_CurrentDownloadData.FullName.LastIndexOf("/");
            }
            //��Դ�ڱ��صĴ洢��·��(MMORPG�����·��)���ļ���·���������ļ���
            string path = m_CurrentDownloadData.FullName.Substring(0, indexLast);
            //��ȡ��ҵı��ش洢·��
            string localFilePath = DownloadMgr.Instance.localFilePath + path;
            //�����һ�û�д洢�ļ��У��򴴽�
            if (!Directory.Exists(localFilePath))
            {
                Directory.CreateDirectory(localFilePath);
            }
        }

        //������������
        using (UnityWebRequest www =  UnityWebRequest.Get(dataUrl))
        {
            www.SendWebRequest();
            //��ȡ����ʱ�������
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
                    Debug.Log("���س�ʱ");
                    yield break;
                }
                //ÿ�εȴ�һ֡�ټ��
                yield return null;
            }

            //����������سɹ���������Դ���浽�û������ļ�����
            if (www != null && www.error == null)
            {
                using (FileStream fs = new FileStream(DownloadMgr.Instance.localFilePath + m_CurrentDownloadData.FullName, FileMode.Create, FileAccess.ReadWrite))
                {
                    //�������Զ����Ƶķ�ʽд��
                    fs.Write(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
                }
            }
        }

        //���سɹ����ʼ��Ҫ���ص��ļ��Ĵ�С����ͳ����Ҫ���ص��ܴ�С��
        m_CurrentDownloadSize = 0;
        m_DownloadSize += m_CurrentDownloadData.Size;
        //������д�뱾�صİ汾�ļ�
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
