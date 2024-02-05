using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ʹ�ð���
//�����첽���ط�ʽ1����ͨί��
//AssetBundleLoaderAsync async = AssetBundleMgr.Instance.LoadAsync(@"Role\environment.assetbundle", "Environment");
//async.OnLoadComplete = OnLoadComplete;
//�����첽���ط�ʽ2��ʹ��lambda���ʽ
//AssetBundleMgr.Instance.LoadAsync(@"Role\environment.assetbundle", "Environment")
//    .OnLoadComplete =(UnityEngine.Object obj) =>
//    {
//        Instantiate(obj);
//    };

/// <summary>
/// �첽������Դ��ע��ʵ���ϵ��õ���AssetBundleMgr���LoadAsync
/// </summary>
public class AssetBundleLoaderAsync : MonoBehaviour
{
    //��Դ����·��
    private string m_FullPath;
    //��Դ��
    private string m_Name;
    //��Դ���ش�������
    private AssetBundleCreateRequest request;
    //��Դ��
    private AssetBundle bundle;
    //�������ί��
    public System.Action<Object> OnLoadComplete;

    #region ��ʼ��Ҫ���ص���Դ��·������Դ����
    /// <summary>
    /// ��ʼ��Ҫ���ص���Դ��·������Դ����
    /// </summary>
    /// <param name="path">��Դ���·����AssetBundles/ƽ̨/�ڲ���ȫ·��(����׺)��</param>
    /// <param name="name">��Դ��</param>
    public void Init(string path, string name)
    {
        m_FullPath = DownloadMgr.Instance.localFilePath + path;
        m_Name = name;
    }
    #endregion

    private void Start()
    {
        StartCoroutine(Load());
    }

    #region ʹ��Э���첽������Ϸ��Դ�����ؿ�ʹ�õ���Դ����ͬʱ����ɾ������ʱ���������ͷ���Դ
    /// <summary>
    /// �첽���ػ�ȡ�����Դ
    /// </summary>
    /// <returns></returns>
    private IEnumerator Load()
    {
        //�첽������Դ
        request = AssetBundle.LoadFromMemoryAsync(LocalFileMgr.Instance.GetBufffer(m_FullPath));
        yield return request;
        //��ȡ��Դ
        bundle = request.assetBundle;
        if (OnLoadComplete != null)
        {
            //��ȡ��Դ����
            OnLoadComplete(bundle.LoadAsset(m_Name));
            //OnLoadComplete(bundle.LoadAssetAsync(m_Name);
            //������������������ذ���
            Destroy(gameObject);
        }
    }
    #endregion



    private void OnDestroy()
    {
        //ж����Դ��
        if (bundle != null)
        {
            bundle.Unload(false);
        }
        m_FullPath = null;
        m_Name = null;
    }
}
