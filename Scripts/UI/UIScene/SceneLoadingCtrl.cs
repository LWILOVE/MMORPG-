using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingCtrl : MonoBehaviour
{
    /// <summary>
    /// UI����������
    /// </summary>
    public UISceneLoadingCtrl m_UILoadingCtrl;



    //��ǰ���ؽ���
    [HideInInspector]
    public float m_CurrProgress = 0;
    [HideInInspector]
    public float LoadSpeed = 0;

    /// <summary>
    /// �Ѿ����ع��ĳ���
    /// </summary>
    [HideInInspector]
    public List<string> ScenesHasLoad;

    /// <summary>
    /// �첽������Ϣ��ȡ
    /// </summary>
    [HideInInspector]
    public AsyncOperation m_Async = null;

    // Start is called before the first frame update
    void Start()
    {
        //1.��Ӽ����������ί��
        DelegateDefine.Instance.OnSceneLoadOk += OnSceneLoadOk;
        //��������Э��
        StartCoroutine(LoadingScene());
        //�����ص���������ʱ���ر����д���
        UIViewUtil.Instance.CloseAll();
        LoadSpeed = 0;
    }

    /// <summary>
    /// �������������ʱ������ͼ����
    /// </summary>
    private void OnSceneLoadOk()
    {
        if (m_UILoadingCtrl != null)
        {
            Destroy(m_UILoadingCtrl.gameObject);
            Destroy(gameObject);
        }
    }

    private string GetSceneName(SceneType type)
    {
        string ret=string.Empty;
        switch (type)
        {
            case SceneType.SelectRole:ret= "basicscene_selectrole"; break;
            case SceneType.MainCity:ret = "cityscene_maincity";break;
            case SceneType.HangZhou:ret = "cityscene_hangzhou";break;
            case SceneType.ChangAn:ret = "cityscene_changan";break;
            case SceneType.WuYiShan:ret = "levelscene_wuyishan"; break;
            case SceneType.MeiLing:ret = "levelscene_meiling";break;
            case SceneType.ChangBaiShan:ret = "levelscene_changbaishan";break;
            case SceneType.XueLangGu:ret = "levelscene_xuelanggu";break;
            case SceneType.DunHuang:ret = "levelscene_dunhuang";break;
            case SceneType.JiuQuan:ret = "levelscene_jiuquan";break;
            case SceneType.DiWangMuLV1:ret = "largelevelscene_diwangmulv1";break;
            case SceneType.ShanGu:ret = "levelscene_shangu";break;
        }
        return ret;
    }

    /// <summary>
    /// ����ID����Э��
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadingScene()
    {
        yield return null;
        //ʹ��AssetBundle���г�������
        //��¼��������Ҫ����
        if (UILoadingCtrl.Instance.CurrentSceneType == SceneType.LogOn)
        {
            m_Async = SceneManager.LoadSceneAsync((int)UILoadingCtrl.Instance.CurrentSceneType);
            m_Async.allowSceneActivation = false;
        }
        else 
        {
            //��ȡ������
            string sceneName = GetSceneName(UILoadingCtrl.Instance.CurrentSceneType);
            //�γɰ汾�ļ���
            string versionPath = "download/scene/" + sceneName + ".unity3d";
            //�γ�����·��
            string fullPath = DownloadMgr.Instance.localFilePath + "download/scene/" + sceneName + ".unity3d";
            //���Ŀ��·���Ƿ����
            if (File.Exists(fullPath))
            {
                StartCoroutine(LoadScene(fullPath, sceneName));
            }
            else 
            {
                //��������ڣ���ôӦ�õ���Դ��������α��ȥ����
                DownloadDataEntity entity = DownloadMgr.Instance.GetServerData(versionPath);
                if (entity != null)
                {
                    //�����������ݺ�ʼ��Assetbundle�м��س���
                    StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity,
                        (bool isSuccess) =>
                        {
                            if (isSuccess)
                            {
                                StartCoroutine(LoadScene(fullPath, sceneName));
                            }
                        }));
                }
                else
                {
                    Debug.LogError("��ǰ�汾�޴��ļ�");
                }
            }
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="fullPath">�����س���������·��(����׺)</param>
    /// <param name="sceneName">�����س����ĳ�������</param>
    /// <returns></returns>
    private IEnumerator LoadScene(string fullPath, string sceneName)
    {
        if (!ScenesHasLoad.Contains(fullPath))
        {
            Debug.Log("���س���" + fullPath);
            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(LocalFileMgr.Instance.GetBufffer(fullPath));
            yield return request;
            AssetBundle bundle = request.assetBundle;
            ScenesHasLoad.Add(fullPath);
        }
        m_Async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        m_Async.allowSceneActivation = false;
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Async == null)
        {
            return;
        }
        m_CurrProgress = m_Async.progress;
        //m_Async.progress�ķ�Χ��0-0.9����ȡ���ؽ���
        if (m_CurrProgress >= 0.9f)
        {
            m_CurrProgress = 1f;
        }
        //һ��Ϊ�˺ÿ���Ϊ�˺ÿ���һ��    
        if (LoadSpeed < 1)
        {
            LoadSpeed += 0.04f;
        }
        m_UILoadingCtrl.SetProgressValue(m_CurrProgress * LoadSpeed, UILoadingCtrl.Instance.CurrentSceneType);
        if ((m_CurrProgress * LoadSpeed) >= 0.9f)
        {
            Invoke("CanGo", 1f);
        }
    }


    private void OnDestroy()
    {
        DelegateDefine.Instance.OnSceneLoadOk -= OnSceneLoadOk;
        m_Async = null;
    }

    /// <summary>
    /// һ��Ϊ�˺ÿ���Ϊ�˺ÿ���һ��   
    /// </summary>
    public void CanGo()
    {
        m_Async.allowSceneActivation = true;
        Invoke("OnSceneLoadOk",0.5f);
    }


}
