using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// ��Դ���ع�����
/// </summary>
public class AssetBundleMgr : SingletonMiddle<AssetBundleMgr>
{
    #region ����
    /// <summary>
    /// �����ļ�����
    /// </summary>
    private AssetBundleManifest m_Manifest;

    /// <summary>
    /// �����ֵ�
    /// </summary>
    private Dictionary<string, Object> m_AssetDic = new Dictionary<string, Object>();

    /// <summary>
    /// �������ֵ䣬�ڿ糡��ʱ�ͷ�
    /// </summary>
    private Dictionary<string, AssetBundleLoader> m_DpsAssetBundleLoaderDic = new Dictionary<string, AssetBundleLoader>();
    #endregion

    #region Load ͬ��������Ϸ���壬����һ��GameObject����
    /// <summary>
    /// ͬ��������Ϸ���壬����һ��GameObject����
    /// ����������
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject Load(string path, string name)
    {
#if DISABLE_ASSETBUNDLE
        //todo
        //return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", path.Replace("assetbundle", "prefab")));
#else
        using (AssetBundleLoader loader = new AssetBundleLoader(path))
        {
            return loader.LoadAsset<GameObject>(name);
        }
#endif
        return null;
    }
    #endregion

    #region LoadAndClone ͬ�����ز���¡��Ϸ���壬����һ��GameObject����
    /// <summary>
    /// ͬ�����ز���¡��Ϸ���壬����һ��GameObject����
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject LoadAndClone(string path, string name)
    {

#if DISABLE_ASSETBUNDLE
        //TODO
        //GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", path.Replace("assetbundle", "prefab")));
        //return Object.Instantiate(obj);
#else
        using (AssetBundleLoader loader = new AssetBundleLoader(path))
        {
            GameObject obj = loader.LoadAsset<GameObject>(name);
            return Object.Instantiate(obj);
        }
#endif
        return null;
    }
    #endregion

    #region LoadAsync ���������Դ�첽�������壬������һ���洢��Ҫ���ص���Դ��·�������Ƶ��첽����
    /// <summary>
    /// ���������Դ�첽�������壬������һ���洢��Ҫ���ص���Դ��·�������Ƶ��첽����
    /// </summary>
    /// <param name="path">��Դ���·����AssetBundles/ƽ̨/�ڲ���ȫ·��(����׺)��</param>
    /// <param name="name">��Դ��</param>
    /// <returns></returns>
    public AssetBundleLoaderAsync LoadAsync(string path, string name)
    {
        GameObject obj = new GameObject("AssetBundleLoadAsync");
        AssetBundleLoaderAsync async = obj.AddComponent<AssetBundleLoaderAsync>();
        async.Init(path, name);
        return async;
    }
    #endregion

    #region LoadOrDownload ��ʽ1��������Ϸ���� ��ʽ2���������������ļ���ͬʱ������ļ����ã�
    /// <summary>
    /// ���ػ������ط���
    /// </summary>
    /// <param name="path">mmorpg/xxx��xxx</param>
    /// <param name="name">�ļ���</param>
    /// <param name="onComplete">������ɻص�</param>
    /// <param name="type">0=GameObjectORPrefab,1=PNG</param>
    public void LoadOrDownload<T>(string path, string name, System.Action<T> onComplete, byte type)
        where T : Object
    {
        lock (this)
        {
            if (onComplete != null)
            {
                string fullPath = DownloadMgr.Instance.localFilePath + path;
                Debug.Log("Ҫ���ص�·��" + fullPath);
                //1.�����������ļ�����(�����ļ����ر���ѡ��ͬ������)
                LoadManifestBundle();
                //2.��ȡ��������������������ļ�
                string[] arrDps = m_Manifest.GetAllDependencies(path);
                //3.�������Ŀ�����������ļ��Ƿ��Ѿ��������
                CheckDps(0, arrDps, () =>
                {
                    if (!File.Exists(fullPath))
                    {
                        Debug.Log("δ��⵽Ŀ����Դ����ʼ����");
                        //�����ز����ڸ�·���µ���Դ����ӷ�������������
                        DownloadDataEntity entity = DownloadMgr.Instance.GetServerData(path);
                        if (entity != null)
                        {
                            //�����ص�������д�뱾��
                            AssetBundleDownload.Instance.StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity,
                                (bool isSuccess) =>
                                {
                                    //�����ļ����سɹ�
                                    if (isSuccess)
                                    {
                                        #region ·�������ɹ����ù�
                                        //�����·����ϵ���ļ��������ع����ͷ���ָ���ļ��ľ���
                                        if (m_AssetDic.ContainsKey(fullPath))
                                        {
                                            if (onComplete != null)
                                            { onComplete(m_AssetDic[fullPath] as T); }
                                            return;
                                        }
                                        #endregion

                                        #region ·��δ���ɹ����ù�
                                        //�����û���ڣ���·�������е��ļ����뾵���ֵ���������ֵ�
                                        for (int i = 0; i < arrDps.Length; i++)
                                        {
                                            if (!m_AssetDic.ContainsKey((DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()))
                                            {
                                                //ʵ����
                                                AssetBundleLoader loader = new AssetBundleLoader(arrDps[i]);
                                                Object obj = loader.LoadAsset<Object>(arrDps[i]);
                                                //���������loader�����ֵ�
                                                m_DpsAssetBundleLoaderDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = loader;
                                                m_AssetDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = obj;

                                            }
                                        }
                                        //ֱ�Ӽ���
                                        using (AssetBundleLoader loader = new AssetBundleLoader(fullPath))
                                        {
                                            if (onComplete != null)
                                            {
                                                Object obj = loader.LoadAsset<T>(name);
                                                //���лص�
                                                onComplete(obj as T);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        Debug.Log("�����ļ�����ʧ��");
                                    }
                                }));
                        }
                    }
                    else
                    {
                        //�������Դ����,��������Դ
                        if (m_AssetDic.ContainsKey(fullPath.ToLower()))
                        {
                            Debug.Log("Ŀ������⣬��ʼ����");
                            if (onComplete != null)
                            { onComplete(m_AssetDic[fullPath.ToLower()] as T); }
                            return;
                        }

                        //����������;���
                        for (int i = 0; i < arrDps.Length; i++)
                        {
                            if (!m_DpsAssetBundleLoaderDic.ContainsKey((DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()))
                            {
                                //ʵ����       
                                AssetBundleLoader loader = new AssetBundleLoader(arrDps[i]);
                                //��ȡ���ļ���·��
                                Object obj = loader.LoadAsset(GameUtil.GetFileName(arrDps[i]));
                                //���������loader�����ֵ�
                                m_DpsAssetBundleLoaderDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = loader;
                                m_AssetDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = obj;
                            }
                        }

                        using (AssetBundleLoader loader = new AssetBundleLoader(path))
                        {
                            if (onComplete != null)
                            {
                                Debug.Log("Ŀ��δ��⣬��ʼ����");
                                Object obj = loader.LoadAsset<T>(name);
                                
                                m_AssetDic[fullPath.ToLower()] = obj;
                                //���лص�
                                onComplete(obj as T);
                            }
                        }
                    }
                });
            }
        }
    }

    /// <summary>
    /// ���ػ������ط���
    /// </summary>
    /// <param name="path">mmorpg/xxx��xxx</param>
    /// <param name="name">�ļ���</param>
    /// <param name="onComplete">������ɻص�</param>
    public void LoadOrDownload(string path, string name, System.Action<GameObject> onComplete)
    {
        LoadOrDownload<GameObject>(path, name, onComplete, 0);
    }

    /// <summary>
    /// �������������ļ�(��������ͬ�������������ļ�������Ϣ)
    /// </summary>
    private void LoadManifestBundle()
    {
        //������ع��˾ͷ���
        if (m_Manifest != null)
        { return; }
        //�����ļ�����
        string assetName = string.Empty;
#if UNITY_STANDALONE_WIN
        assetName = string.Format("Windows");
#elif UNITY_ANDROID
        assetName = string.Format("Android");
#elif UNITY_IPHONE
        assetName = string.Format("iOS");
#endif
        Debug.Log("���������������ļ�");

        using (AssetBundleLoader loader = new AssetBundleLoader(assetName))
        {
            //�����ļ��е���������
            m_Manifest = loader.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (m_Manifest == null)
            {
                Debug.Log("�ļ������������ʧ��");
            }
            else
            {
                Debug.Log("�����������ļ�����" + m_Manifest.name);
            }
        }
    }

    /// <summary>
    /// ���õݹ���������
    /// </summary>
    /// <param name="index">�±�</param>
    /// <param name="arrDps">����������</param>
    /// <param name="onComplete">���ί��</param>
    private void CheckDps(int index, string[] arrDps, System.Action onComplete)
    {
        if (arrDps == null || arrDps.Length == 0)
        {
            if (onComplete != null)
            { onComplete(); }
            return;
        }

        //��ȡ�����ļ�λ��
        string fullPath = DownloadMgr.Instance.localFilePath + arrDps[index];
        if (!File.Exists(fullPath))
        {
            //�������û�и������ļ�����ӷ���������
            DownloadDataEntity entity = DownloadMgr.Instance.GetServerData(arrDps[index]);
            if (entity != null)
            {
                //���ļ�д�뵽���أ��������һ�������ļ�
                AssetBundleDownload.Instance.StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity, (bool isSuccess) =>
                {
                    index++;
                    if (index == arrDps.Length)
                    {
                        if (onComplete != null)
                        { onComplete(); }
                        return;
                    }
                    CheckDps(index, arrDps, onComplete);
                }));
            }
            else
            {
                Debug.Log(arrDps[index] + "������");
                arrDps[index] = "download/source/uisource/uicommon/bg_007.assetbundle";
                Debug.Log(arrDps[index]);
                DownloadDataEntity entity2 = DownloadMgr.Instance.GetServerData(arrDps[index]);
                if(entity2==null)
                {
                    Debug.Log("����û�ҵ�");
                }
                //ǿ���޸�
                //���ļ�д�뵽���أ��������һ�������ļ�
                AssetBundleDownload.Instance.StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity2, (bool isSuccess) =>
                {
                    index++;
                    if (index == arrDps.Length)
                    {
                        if (onComplete != null)
                        { onComplete(); }
                        return;
                    }
                    CheckDps(index, arrDps, onComplete);
                }));
            }
        }
        else
        {
            index++;
            if (index == arrDps.Length)
            {
                if (onComplete != null)
                { onComplete(); }
                return;
            }
            CheckDps(index, arrDps, onComplete);
        }
    }

    #endregion

}
