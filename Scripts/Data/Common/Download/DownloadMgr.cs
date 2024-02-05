using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
/// �������ع�����
/// </summary>
public class DownloadMgr : SingletonMiddle<DownloadMgr>
{
    #region ����
    /// <summary>
    /// ���س�ʱʱ��
    /// </summary>
    public const int DownloadTimeOut = 5;
    /// <summary>
    /// ��Դ���������
    /// </summary>
    public const int DownloadRoutineNum = 2;

    #region ʵ�ʵ���Դ��������ַ�����ޣ�
    /// <summary>
    /// ���ݵ����ػ�����ַ������վ��ģʽ��ʵ������Ҫ�ӷ�������ȡ��
    /// </summary>
    public static string DownloadBaseUrl = "http://192.168.31.178:8088/";
    /// <summary>
    /// �������ص�����http��ַ
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
    /// �������ݵ���Դ·�����洢�Ķ�д·����
    /// C:/Users/User/AppData/LocalLow/DefaultCompany/MMORPG/ƽ̨/
    /// </summary>
    public string localFilePath;

    /// <summary>
    /// ���ذ汾�ļ��洢·��
    /// ͨ����ƽ̨�󣬺�Downloadͬ��,��.../Android/VersionFile.txt,�������û��ļ�����MMORPG/VersionFile.txt(���������û�·��û�д����   )
    /// </summary>
    private string m_LocalVersionPath;

    /// <summary>
    /// �汾�ļ���
    /// </summary>
    public const string m_VersionFileName = "VersionFile.txt";

    /// <summary>
    /// �������ص������б���Ϊ�����Ķ�����Դ���¼������Դ��
    /// </summary>
    private List<DownloadDataEntity> m_NeedDownloadDataList = new List<DownloadDataEntity>();
    /// <summary>
    /// ���������б�
    /// </summary>
    private List<DownloadDataEntity> m_LocalDataList = new List<DownloadDataEntity>();

    /// <summary>
    /// �������ˣ���http��ȡ���������б�
    /// </summary>
    private List<DownloadDataEntity> m_ServerDataList;

    /// <summary>
    /// ��Դ��ʼ����ԭʼ·�������ļ�����Ҫ������ļ�·����
    /// ��../StreamingAssets/AssetBundles/..
    /// </summary>
    private string m_StreamingAssetsPath;

    /// <summary>
    /// ��Ϸ��ʼ����ɻص�����ɺ�ͨ����¼������
    /// </summary>
    public Action OnInitComplete;
    #endregion

    #region ��ʼ����Ҫ���������Դ���ػ���������������������DownLoadFiles
    #region ������Դ��ʼ�����а汾�ļ��ͼ����£�û�оʹ����ļ��Ĵ�����ȡ�ļ����е���Դ��ʼ�� InitStreamingAssets(�״�ReadStreamingAssetVersionFile���������أ� ����InitCheckVersion����Դ���������أ�)
    /// <summary>
    /// ������Դ��ʼ�����а汾�ļ��ͼ����£�û�оͽ��г��׵���Դ��ʼ��
    /// </summary>
    /// <param name="onInitComplete">����Դ��ʼ�����ʱ����</param>
    public void InitStreamingAssets(Action onInitComplete)
    {
        localFilePath = Application.persistentDataPath + "/Windows/"; 
        Debug.Log("�����ļ�·����" + localFilePath);
        OnInitComplete = onInitComplete;
        //���ñ��ذ汾�ļ�·��
        m_LocalVersionPath = localFilePath + m_VersionFileName;

        //�жϱ����Ƿ��Ѿ��а汾�ļ�   
        //����������£�������г�ʼ���ټ�����
        if (File.Exists(m_LocalVersionPath))
        {
            //���汾�ļ��Ƿ���Ҫ����
            InitCheckVersion();
        }
        else 
        {
            //�ļ���ʼ����Ҫ������ļ�·��
            //���û����Դ ִ�г�ʼ�� Ȼ���ټ�����
            m_StreamingAssetsPath = "file://" + Application.streamingAssetsPath + "/AssetBundles/Windows/";//UnityWebRequest���ر������ݵ�·��
//#if UNITY_ANDROID && !UNITY_EDITOR
//            m_StreamingAssetsPath = Application.streamingAssetsPath + "/AssetBundles/";//�ֻ���
//#endif
            //���ñ��ذ汾�ļ����·��
            string versionFileUrl = m_StreamingAssetsPath + m_VersionFileName;
            Debug.Log(versionFileUrl);
            //�ļ�д�뱾�ص�·����ReadStreamingAssetVersionFile->OnReadStreamingAssetOver->InitStreamingAssetList(�Ȱ���Դд�뱾�أ��ٰѰ汾�ļ�д�뱾��)
            //ע���˴��ǲ��ܹ�StreamingAssetsPath�н�����Դ���أ�����ʵ���ϴ���Դ������������Ҳ�У�Ҳ����DownloadUrl+m_VersionFileName����ֻ����Ϊ��û����Դ�������ų����²�
            GlobalInit.Instance.StartCoroutine(ReadStreamingAssetVersionFile(versionFileUrl,OnReadStreamingAssetOver));
        }
    }
    #endregion

    #region ���ݰ汾�ļ����ڵ�ַ���汾�ļ����������ش洢·����������InitCheckVersion����  ReadStreamingAssetVersionFile->OnReadStreamingAssetOver->InitStreamingAssetList(with AssetLoadToLocal)(�Ȱ���Դд�뱾�أ��ٰѰ汾�ļ�д�뱾��) 
    #region ���ݰ汾�ļ������ַ���ذ汾�ļ��������ð汾�ļ���ȡ��ɻص� ReadStreamingAssetVersionFile
    /// <summary>
    /// ���ݰ汾�ļ������ַ���ذ汾�ļ��������ð汾�ļ���ȡ��ɻص�
    /// </summary>
    /// <param name="fileUrl">�汾�ļ���ŵ�ַ</param>
    /// <param name="onReadStreamingAssetOver">�汾�ļ���ȡ���ʱ����</param>
    /// <returns></returns>
    private IEnumerator ReadStreamingAssetVersionFile(string fileUrl, Action<string> onReadStreamingAssetOver)
    {
        UISceneInitCtrl.Instance.SetProgress("�������������ڽ�����Դ��ʼ��", 0);
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
                //�汾�ļ���ȡʧ��
                Debug.LogError("�汾�ļ���ȡʧ��");
                //onReadStreamingAssetOver("");
            }
        }
    }
    #endregion

    #region ���ݴ������İ汾�ļ�����������Ϸ��Դ��ʼ��Э�� OnReadStreamingAssetOver
    /// <summary>
    /// ���ݴ������İ汾�ļ�����������Ϸ��Դ��ʼ��Э��
    /// </summary>
    /// <param name="content">�汾�ļ�����</param>
    private void OnReadStreamingAssetOver(string content)
    {
        GlobalInit.Instance.StartCoroutine(InitStreamingAssetList(content));
    }
    #endregion

    #region ���ݰ汾�ļ����ݣ�����AssetLoadToLocalЭ�̽��汾�ļ��Ͱ汾�ļ��м�¼����Դд�뱾�� InitStreamingAssetList
    /// <summary>
    /// ���ݰ汾�ļ����ݣ�����AssetLoadToLocalЭ�̽��汾�ļ��Ͱ汾�ļ��м�¼����Դд�뱾��
    /// </summary>
    /// <param name="content">�汾�ļ�����</param>
    /// <returns></returns>
    private IEnumerator InitStreamingAssetList(string content)
    {
        //���������İ汾�ļ��ǿյģ���ô�����»�ȥ����һ��
        if (string.IsNullOrEmpty(content))
        {
            Debug.Log("�汾�ļ�Ϊ��");
            InitCheckVersion();
            yield break;
        }
        //���ж�ȡ�ļ�����
        string[] arr = content.Split('\n');
        //ѭ����ѹ�ļ�������
        for (int i = 0; i < arr.Length; i++)
        {
            string[] arrInfo = arr[i].Split(" ");
            //��ȡ�ļ��Ķ�·��(��б��)����������б�ܴ��棩
            string fileUrl = arrInfo[0].Replace("\\","/");
            //���ļ���һ���뱾���ļ�
            yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + fileUrl, localFilePath + fileUrl));
            float value = (i + 1) / (float)arr.Length;
            UISceneInitCtrl.Instance.SetProgress(string.Format("��Դ��ʼ��(����������)��{0}/{1}", i + 1, arr.Length), value);
        }
        //���汾�ļ�д�뱾��
        yield return GlobalInit.Instance.StartCoroutine(AssetLoadToLocal(m_StreamingAssetsPath + m_VersionFileName, localFilePath + m_VersionFileName));
        //����Ƿ���Ҫ����
        InitCheckVersion();
    }
    #endregion

    #region ������Դ��ַ������Դ����ŵ����� AssetLoadToLocal
    /// <summary>
    /// ������Դ��ַ������Դ����ŵ�����
    /// </summary>
    /// <param name="fileUrl">��Դ������ַ</param>
    /// <param name="toPath">���ص���Դ����ڱ��ص�λ��</param>
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
                //�����ļ���
                if (lastIndexOf != -1)
                {
                    //������Դ����ļ���
                    string localPath = toPath.Substring(0, lastIndexOf);
                    if (!Directory.Exists(localPath))
                    {
                        Directory.CreateDirectory(localPath);
                    }
                }
                //���ļ���Դ�Զ�����ģʽд�뱾��
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

    #region ���汾����ϵ InitCheckVersion(�ٻ���������Э��)->PackDownloadData->OnInitVersionCallBack(with PackDownloadDataDic PackDownloadDataDic GetDownloadData)->��������������(DownloadFiles)
    #region ����Դ�������������µİ汾�ļ������뱾�ذ汾���к˶Ը��£������󴴽������Դ����ʵ�壬������DownloadVersion�� InitCheckVersion
    /// <summary>
    /// ��http�������µİ汾�ļ������뱾�ذ汾���к˶Ը���
    /// </summary>
    public void InitCheckVersion()
    {
        UISceneInitCtrl.Instance.SetProgress("���ڼ��汾���£�", 0);
        //�汾�ļ�������ַ/��ַ
        string strVersionUrl = DownloadUrl + m_VersionFileName;
        //��Ϊû����Դ�������������ڴ����ñ����ļ�������ַ
        strVersionUrl = "file://" + "C:/Users/User/Unity/MMORPG/AssetBundles/Windows/VersionFile.txt";
        //��ȡ�汾�ļ�
        AssetBundleDownload.Instance.InitServerVersion(strVersionUrl, OnInitVersionCallBack);
    }
    #endregion

    #region ���ذ汾�ļ��������Դ��Ϣ���� PackDownloadData
    /// <summary>
    /// ���ذ汾�ļ��������Դ��Ϣ����
    /// ���ǰ��̬�������ǰ��ʽ�ֲ�����̬�����β��õĸ�ʽ�ǣ�Dowload·����MD5��Ϣ����С���Ƿ��ǳ�ʼ���ݣ�
    /// </summary>
    /// <param name="content">�汾�ļ�����</param>
    /// <returns></returns>
    public List<DownloadDataEntity> PackDownloadData(string content)
    {
        List<DownloadDataEntity> list = new List<DownloadDataEntity>();
        //���н��зָ�
        string[] arrLines = content.Split("\n");
        for (int i = 0; i < arrLines.Length; i++)
        {
            //��ȡ�����汾��Ϣ��·����MD5��Ϣ����С���Ƿ��ǳ�ʼ���ݣ�
            string[] arrData = arrLines[i].Split(" ");
            //ע��ͷ������ƽ̨��ƽ̨���������ļ�����Ϣ
            if (arrData.Length == 4)
            {
                DownloadDataEntity entity = new DownloadDataEntity();
                //��ȡDownload·��
                entity.FullName = arrData[0];
                //��ȡMD5������̬
                entity.MD5 = arrData[1];
                entity.Size = arrData[2].ToInt();
                entity.IsFirstData = arrData[3].ToInt() == 1;
                list.Add(entity);
            }
        }
        return list;
    }
    #endregion

    #region ͨ����������http�汾�ļ���Ϣ�����ȶ�http�汾�ļ��ͱ��ذ汾�ļ�������Ϸ��ʼ����Դ���£���������Դ����������ɳ�ʼ������ OnInitVersionCallBack
    /// <summary>
    /// ͨ����������http�汾�ļ���Ϣ�����ȶ�http�汾�ļ��ͱ��ذ汾�ļ�������Ϸ��ʼ����Դ���£���������Դ����������ɳ�ʼ������
    /// </summary>
    /// <param name="serverDownloadData">�汾�ļ���Ϣʵ���б�</param>
    private void OnInitVersionCallBack(List<DownloadDataEntity> serverDownloadData)
    {
        //���µİ汾�ļ������б�
        m_ServerDataList = serverDownloadData;
        if (File.Exists(m_LocalVersionPath))
        {
            //�����ش��ڰ汾�ļ�����ôֻ��Ҫ������������˴�����������ݽ��жԱȣ���ⲻͬ�Ĳ��ֽ��и���
            //����http���ݹؼ���Ϣ�ֵ䡶����Download��·����������ϢMD5������̬��
            Dictionary<string, string> serverDic = PackDownloadDataDic(serverDownloadData);
            //��ȡ���ذ汾�ļ�������
            string content = IOUtil.GetFileText(m_LocalVersionPath);
            //�����������ݹؼ���Ϣ�ֵ䡶����Download��·����������ϢMD5������̬��
            Dictionary<string, string> clientDic = PackDownloadDataDic(content);
            //�����ذ汾�ļ������������б�
            m_LocalDataList = PackDownloadData(content);

            //�������ݶԱ�
            //1.��ȡ��׷�Ӽӵĳ�ʼ��Դ
            for (int i = 0; i < serverDownloadData.Count; i++)
            {
                //����������ǳ�ʼ��Դ��ͬʱ�ͻ����в����ڸ���Դ����ô�͸���
                if (serverDownloadData[i].IsFirstData && !clientDic.ContainsKey(serverDownloadData[i].FullName))
                {
                    //�����ݼ��������б�
                    m_NeedDownloadDataList.Add(serverDownloadData[i]);
                }
            }

            //2.�ȶԾ���Դ�������Ƿ���Ҫ����
            foreach (var item in clientDic)
            {
                //MD5���д洢������ÿ�����ݵľ���ֵ������в�ͬ����˵��������Ҫ�������ݸ�����
                if (serverDic.ContainsKey(item.Key) && serverDic[item.Key] != item.Value)
                {
                    //��ȡ����ʵ�壬��������Ҫ�����б���
                    DownloadDataEntity entity = GetDownloadData(item.Key, serverDownloadData);
                    if (entity != null)
                    {
                        m_NeedDownloadDataList.Add(entity); ;
                    }
                    else
                    {
                        Debug.Log("δ�ҵ�Ŀ������");
                    }
                }
            }
        }
        else
        {
            //����ֻ������״�������Ϸ����Ӧ�ý�����������
            //��ȡ��Ҫ���ص������б�
            for (int i = 0; i < serverDownloadData.Count; i++)
            {
                if (serverDownloadData[i].IsFirstData)
                {
                    m_NeedDownloadDataList.Add(serverDownloadData[i]);
                }
            }
        }

        //���������Ұ汾�������°汾������Ҫ���أ�
        if (m_NeedDownloadDataList.Count == 0)
        {
            UISceneInitCtrl.Instance.SetProgress("��ǰ�汾�����°汾���������", 1);
            if (OnInitComplete != null)
            {
                OnInitComplete();
            }
            return;
        }
        else
        {
            //�������Դ���̷߳����������������������
            AssetBundleDownload.Instance.DownloadFiles(m_NeedDownloadDataList);
        }
    }
    #endregion

    #region ���ݴ�����������ʵ�������װ���ݹؼ���Ϣ�ֵ� PackDownloadDataDic��http�ļ�ϵ��
    /// <summary>
    /// ���ݴ�����������ʵ�壬�����װ���ݹؼ���Ϣ�ֵ�
    /// �ֵ��ʽ�����ļ�·����eg:download/xx�����ļ����ݵ�MD5��̬��
    /// </summary>
    /// <param name="list">����ʵ���б�</param>
    /// <returns>���ݹؼ���Ϣ�ֵ�</returns>
    public Dictionary<string, string> PackDownloadDataDic(List<DownloadDataEntity> list)
    {
        //�������������ֵ䣬<����·���������ݵ�MD5��̬>
        Dictionary<string, string> dic = new Dictionary<string, string>();
        for (int i = 0; i < list.Count; i++)
        {
            dic[list[i].FullName] = list[i].MD5;
        }
        return dic;
    }
    #endregion

    #region  ���������ı����������ֵ䣨ʵ���Ϲ̶��ǰ汾�ļ���ʽ���ı��� PackDownloadDataDic�������ļ�ϵ��
    /// <summary>
    /// ���������ı����������ֵ䣨ʵ���Ϲ̶��ǰ汾�ļ���ʽ���ı���
    /// ����Դ·������Դ���ݡ�
    /// </summary>
    /// <param name="content">�ı�����</param>
    /// <returns></returns>
    public Dictionary<string, string> PackDownloadDataDic(string content)
    {
        //�������������ֵ䣬<����·���������ݵ�MD5��̬>
        Dictionary<string, string> dic = new Dictionary<string, string>();
        //����������Ҫ���������ݽ��в��
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

    #region ���ĳ�����Ƿ���������Դ�б��� GetDownloadData
    /// <summary>
    /// ���ĳ�����Ƿ���������Դ�б���
    /// </summary>
    /// <param name="fullName"> ���ݵ�ȫ·����ָ��ƽ̨֮���ȫ·���磺windows�ľ���windows��eg��download/xxx</param>
    /// <param name="list">�ܵ���Դ�б�</param>
    /// <returns>��·����Ӧ��ʵ��</returns>
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

    #region �汾�ļ�ͬ�� ModifyLocalData->SaveLocalVersion:���±��ذ汾�ļ������ͬ��

    #region ���²�ͬ�����ذ汾�ļ� ModifyLocalData����(SaveLocalVersion)
    /// <summary>
    /// ���ݴ�������Ҫ�޸ĵı������ݶ�����и��²�ͬ�����ذ汾�ļ�
    /// </summary>
    /// <param name="entity">��Դʵ��</param>
    public void ModifyLocalData(DownloadDataEntity entity)
    {
        if(m_LocalDataList == null)
        { return; }
        //Ŀ���ļ��Ƿ��Ѿ�����
        bool isExists = false;
        for (int i = 0; i < m_LocalDataList.Count; i++)
        {
            //���ļ����ڣ����޸����������
            if (m_LocalDataList[i].FullName.Equals(entity.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                m_LocalDataList[i].MD5 = entity.MD5;
                m_LocalDataList[i].Size = entity.Size;
                m_LocalDataList[i].IsFirstData = entity.IsFirstData;
                isExists = true;
                break;
            }
        }
        //���ļ������ڣ�������뱾�������б��Թ���������
        if (!isExists)
        {
            m_LocalDataList.Add(entity);
        }
        //ÿ���޸������ݶ�Ӧ�ñ���һ�°汾�ļ�
        SaveLocalVersion();
    }
    #endregion

    #region ���汾�صİ汾�ļ� SaveLocalVersion
    /// <summary>
    /// ���汾�صİ汾�ļ�
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

    #region ���ļ�·���ĳ�http·��������������ʵ�� GetServerData
    /// <summary>
    /// ���ļ�·���ĳ�http·��������������ʵ��
    /// </summary>
    /// <param name="path">ָ�汾�ļ��е�·��(������б��)</param>
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
