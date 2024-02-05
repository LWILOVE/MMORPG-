using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// ��Դ�����ص���������
/// </summary>
public class AssetBundleDownload : SingletonMono<AssetBundleDownload>
{
    #region ����
    /// <summary> 
    /// �汾�ļ���ַ
    /// </summary>
    private string m_VersionUrl;

    /// <summary>
    /// �汾�ļ���ʼ���ص�
    /// </summary>
    private Action<List<DownloadDataEntity>> m_OnInitVersion;

    /// <summary>
    /// ��Դ������������
    /// </summary>
    private AssetBundleDownloadRoutine[] m_Routine = new AssetBundleDownloadRoutine[DownloadMgr.DownloadRoutineNum];

    /// <summary>
    /// ��Դ������������
    /// </summary>
    private int m_RoutineIndex = 0;

    /// <summary>
    /// �Ƿ��������
    /// </summary>
    private bool m_IsDownloadOver = false;

    /// <summary>
    /// ����ʱ��
    /// </summary>
    private float m_SamplingTime = 1;
    
    /// <summary>
    /// �Ѿ����ص�ʱ��
    /// </summary>
    private float m_AlreadyTime = 0;
    
    /// <summary>
    /// ��Ҫ���ص�ʱ��
    /// </summary>
    private float m_LeftTime = 0f;
    
    /// <summary>
    /// ���ص��ٶ�
    /// </summary>
    private float m_Speed = 0f;

    /// <summary>
    /// ��Դ����Ҫ�����ܴ�С
    /// </summary>
    public int TotalSize
    {
        get;
        private set;
    }

    /// <summary>
    /// ��Դ������ԴҪ���ص�����
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

    #region ��������Э����Դ��ʼ�� OnStart DownloadVersion InitServerVersion DownloadFiles(����Э���������İ�ש����)
    protected override void OnStart()
    {
        base.OnStart();
        //ȥ��վ���ذ汾�ļ�
        StartCoroutine(DownloadVersion(m_VersionUrl));
    }

    #region ��������Э��������Դ�����������°汾�ļ�  DownloadVersion InitServerVersion
    #region ���°汾�ļ���ʼ����������Դ�������������DownloadVersion��  InitServerVersion 
    /// <summary>
    /// ��ʼ��http�汾��Ϣ
    /// </summary>
    /// <param name="url">http��Ӧ�İ汾�ļ���ַ(ȫ)</param>
    /// <param name="onInitVersionCallBack">�汾��ʼ���ص�</param>
    public void InitServerVersion(string url, Action<List<DownloadDataEntity>> onInitVersionCallBack)
    {
        m_VersionUrl = url;
        m_OnInitVersion = onInitVersionCallBack;
    }
    #endregion

    #region  ��汾�ļ�������վ���ذ汾�ļ�Э�� DownloadVersion
    /// <summary>
    /// ���ذ汾�ļ�
    /// ��汾�ļ�������վ���ذ汾�ļ�Э��
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    private IEnumerator DownloadVersion(string url)
    {
        Debug.Log("�汾�ļ���ַ" + url);
        yield return new WaitForSeconds(0.1f);
        //ȥ��վ���ذ汾����
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            //��ȡ����ʱ������ؽ���
            float timeOut = Time.time;
            float progress = www.downloadProgress;

            //��������������û���ʱ
            while (www != null && !www.isDone)
            {
                //��ʱͬ�����½���
                if (progress < www.downloadProgress)
                {
                    timeOut = Time.time;
                    progress = www.downloadProgress;
                }
                //������س�ʱ
                if ((Time.time - timeOut) > DownloadMgr.DownloadTimeOut)
                {
                    Debug.Log("����ʱ�����");
                    yield break;
                }
            }
            //���������ɣ����ȡ���ص�����
            if (www != null && www.error == null)
            {
                //��ȡ�汾�ļ�����
                string content = www.downloadHandler.text;
                if (m_OnInitVersion != null)
                {
                    m_OnInitVersion(DownloadMgr.Instance.PackDownloadData(content));
                }
            }
            else
            {
                Debug.Log("����ʧ�� ԭ�����£�" + www.error);
            }
        }
    }
    #endregion
    #endregion

    #region ʹ����Դ���߳��������ݣ�����ֵ����������AddDownload�ͷ���������������StartDownload�� DownloadFiles
    /// <summary>
    /// ʹ����Դ���߳��������ݣ�����ֵ����������ͷ��������������̣�
    /// </summary>
    /// <param name="downLoadList">Ҫ���ص������б�</param>
    public void DownloadFiles(List<DownloadDataEntity> downLoadList)
    {
        TotalSize = 0;
        //Ϊ������Դ���߳�������ؽű�
        for (int i = 0; i < m_Routine.Length; i++)
        {
            if (m_Routine[i] == null)
            {
                m_Routine[i] = gameObject.AddComponent<AssetBundleDownloadRoutine>();
            }
        }

        //����ÿһ����Դ���̷߳�����������
        for (int i = 0; i < downLoadList.Count; i++)
        {
            //ѭ������������������������
            m_RoutineIndex = m_RoutineIndex % m_Routine.Length;
            m_Routine[m_RoutineIndex].AddDownload(downLoadList[i]);
            m_RoutineIndex++;
            TotalSize += downLoadList[i].Size;
            TotalCount++;
        }

        //����ÿһ����Դ���߳̿�ʼ������Դ����
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
    /// ������ҽ��������
    /// </summary>
    protected override void OnUpdate()
    {
        base.OnUpdate();
        #region ͬ����ʼ����Դ������Ϣ�������汾��Դ������ɻص�
        //���洢��Ҫ���ص��ļ���������0����û���������ʱ
        if (TotalCount > 0&& !m_IsDownloadOver)
        {
            //��ȡ��ǰ�Ѿ����ص��ļ����������С
            int totalCompleteCount = CurrentCompleteTotalCount();
            totalCompleteCount = totalCompleteCount == 0 ? 1 : totalCompleteCount;
            int totalCompleteSize = CurrentCompleteTotalSize();

            //ͳ���Ѿ����ص�ʱ�������
            m_AlreadyTime += Time.deltaTime;
            if (m_AlreadyTime > m_SamplingTime&&m_Speed == 0)
            {
                //��ȡ�����ٶ�
                m_Speed = totalCompleteSize / m_SamplingTime;
            }

            //����ʣ��ʱ�� = ���ܴ�С-�Ѿ����صĴ�С��/�ٶ�
            if (m_Speed > 0)
            {
                m_LeftTime = (TotalSize - totalCompleteSize) / m_Speed;
            }
            if (m_LeftTime > 0)
            {
                string strLeftTime = string.Format("ʣ��ʱ��{0}��", m_LeftTime);
            }

            //��ӡ���ؽ���
            string str = string.Format("��������{0}/{1}", totalCompleteCount, TotalCount);
            UISceneInitCtrl.Instance.SetProgress(str,totalCompleteCount/(float)TotalCount);
         
            if (totalCompleteCount == TotalCount)
            {
                m_IsDownloadOver = true;
                UISceneInitCtrl.Instance.SetProgress("��Դ�������",1);
                if (DownloadMgr.Instance.OnInitComplete != null)
                {
                    DownloadMgr.Instance.OnInitComplete();
                }
            }
        }
        #endregion
    }

    #region ���ݴ�����������ʵ������Դ��������������д�뱾�أ�ͬʱ������д�뱾�ز����°汾�ļ� DownloadData������δ֪��
    /// <summary>
    /// ���ݴ�����������ʵ����Http��վ��������д�뱾�أ�ͬʱ������д�뱾�ز����°汾�ļ�
    /// </summary>
    /// <param name="currDownloadData">Ҫ���ص�����ʵ��</param>
    /// <param name="onComplete">������ɻص�</param>
    /// <returns></returns>
    public IEnumerator DownloadData(DownloadDataEntity currDownloadData, Action<bool> onComplete)
    {
        //��ȡĿ����Դ��������ַ
        string dataUrl = "file://" + "C:/Users/User/Unity/MMORPG/AssetBundles/Windows/" + currDownloadData.FullName.Replace("\\", "/");//��Ϊû����Դ��������������б�����������

        int dirIndex = currDownloadData.FullName.LastIndexOf('\\');
        if (dirIndex == -1)
        {
            dirIndex = currDownloadData.FullName.LastIndexOf('/');
        }

        //��ȡĿ����Դ���ڵ��ļ��У����ļ���ǰһλ��a\a.txt����a��
        string path = currDownloadData.FullName.Substring(0, dirIndex);

        //��ȡ��ҵı��ش洢·��
        string localFilePath = DownloadMgr.Instance.localFilePath + path;
        //����ұ��ش�����Դ�洢�ļ���
        if (!Directory.Exists(localFilePath))
        {
            Directory.CreateDirectory(localFilePath);
        }
        //������������
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
                        Debug.Log("��Դ����ʧ��");
                        onComplete(false);
                    }
                    yield break;
                }
                //ÿ�εȴ�һ֡�ټ��
                yield return null;
            }

            //����������سɹ���������Դ���浽�û������ļ�����
            if (www != null && www.error == null)
            {
                using (FileStream fs = new FileStream(DownloadMgr.Instance.localFilePath + currDownloadData.FullName, FileMode.Create, FileAccess.ReadWrite))
                {
                    //�������Զ����Ƶķ�ʽд��
                    fs.Write(www.downloadHandler.data, 0, www.downloadHandler.data.Length);
                }
            }
        }
        //������д�뱾�صİ汾�ļ�
        DownloadMgr.Instance.ModifyLocalData(currDownloadData);

        //ִ���ļ����سɹ�ί��
        if (onComplete != null)
        {
            onComplete(true);
        }
    }
    #endregion

    #region ��ǰ�Ѿ����ص��ļ��ܴ�С������ CurrentCompleteTotalSize CurrentCompleteTotalCount
    /// <summary>
    /// ��ǰ�Ѿ����ص��ļ��ܴ�С
    /// </summary>
    /// <returns>�����ص��ļ���С</returns>
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
    /// ��ǰ�Ѿ����ص��ļ�������
    /// </summary>
    /// <returns>�����ص��ļ�����</returns>
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
