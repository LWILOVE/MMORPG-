using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ʹ�ð���1
//using (AssetBundleLoader loader = new AssetBundleLoader(@"Role\environment.assetbundle"))
//{
//    GameObject obj = loader.LoadAsset<GameObject>("Environment");
//    Instantiate(obj);
//}

//ʹ�ð���2
//GameObject obj = AssetBundleMgr.Instance.Load(@"Role\environment.assetbundle", "Environment");
//Instantiate(obj);

/// <summary>
/// ͬ��������Դ��
/// </summary>
public class AssetBundleLoader :IDisposable
{
    /// <summary>
    /// ��Դ��
    /// </summary>
    private AssetBundle bundle;

    #region ���캯������������·������Դ��ͬ��������Դ��Ӧ����Դ�� AssetBundleLoader
    /// <summary>
    /// ��Դͬ�����أ�����һ��AssetBundle��ʽ�ı�����
    /// </summary>
    /// <param name="assetBundlePath">��Դ����Դ����·������������Ӧ����MMORPG/����ģ����ڱ��·������ȫ·������������Ĳ���Ϊtrue��</param>
    public AssetBundleLoader(string assetBundlePath)
    {
        //�ļ���ȫ·��
        string fullPath = DownloadMgr.Instance.localFilePath + assetBundlePath;
        byte[] bytes = LocalFileMgr.Instance.GetBufffer(fullPath);
        if (bytes == null)
        {
            Debug.Log("�޷��ҵ�Ŀ���ļ�");
        }
        //��Byte��ʽ�����ݶ�ȡ����ΪAssetBundleģʽ
        bundle = AssetBundle.LoadFromMemory(bytes);
        
    }
    #endregion

    #region ������Դ���ƴ���Դ���м��س��ɲ�������Դ����
    /// <summary>
    /// ���ɿ�ʹ�õ���Դ����
    /// </summary>
    /// <typeparam name="T">���ص���Դ����Դ����</typeparam>
    /// <param name="name">��Դ��</param>
    /// <returns></returns>
    public T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        if (bundle == null)
        {
            Debug.Log("�ļ���Ϊ��");
            return default(T);
        }
        T asset = bundle.LoadAsset(name) as T;
        //���׷�©
        if (asset == null)
        {
            Debug.Log("��Դ����Asset��" + name);
            // ��ȡ AssetBundle ��������Դ������
            string[] assetNames = bundle.GetAllAssetNames();
            // �����Դ����
            foreach (string assetName in assetNames)
            {
                Debug.Log("Ŀ������" + assetName +  "  ��������" + name);
            }
        }
        //����AssetBundle��ʽ����Դ�������ɿ�ʹ�õľ���
        return asset;
    }   

    /// <summary>
    /// ������Դ���ƴ���Դ���м��س��ɲ�����Unity��Դ����
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UnityEngine.Object LoadAsset(string name)
    {
        return bundle.LoadAsset(name);
    }
    #endregion

    /// <summary>
    /// ������ʱ���ո���Դ������߳���Ч��
    /// </summary>
    public void Dispose()
    {
        if (bundle != null)
        {
            //ж��
            bundle.Unload(false);
        }
    }


}
