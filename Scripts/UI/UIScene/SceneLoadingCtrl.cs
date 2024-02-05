using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingCtrl : MonoBehaviour
{
    /// <summary>
    /// UI场景控制器
    /// </summary>
    public UISceneLoadingCtrl m_UILoadingCtrl;



    //当前加载进度
    [HideInInspector]
    public float m_CurrProgress = 0;
    [HideInInspector]
    public float LoadSpeed = 0;

    /// <summary>
    /// 已经加载过的场景
    /// </summary>
    [HideInInspector]
    public List<string> ScenesHasLoad;

    /// <summary>
    /// 异步加载信息获取
    /// </summary>
    [HideInInspector]
    public AsyncOperation m_Async = null;

    // Start is called before the first frame update
    void Start()
    {
        //1.添加加载完成销毁委托
        DelegateDefine.Instance.OnSceneLoadOk += OnSceneLoadOk;
        //开启加载协程
        StartCoroutine(LoadingScene());
        //当加载到其他窗口时，关闭所有窗口
        UIViewUtil.Instance.CloseAll();
        LoadSpeed = 0;
    }

    /// <summary>
    /// 当场景加载完成时将背景图销毁
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
    /// 场景ID加载协程
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadingScene()
    {
        yield return null;
        //使用AssetBundle进行场景下载
        //登录场景不需要下载
        if (UILoadingCtrl.Instance.CurrentSceneType == SceneType.LogOn)
        {
            m_Async = SceneManager.LoadSceneAsync((int)UILoadingCtrl.Instance.CurrentSceneType);
            m_Async.allowSceneActivation = false;
        }
        else 
        {
            //获取场景名
            string sceneName = GetSceneName(UILoadingCtrl.Instance.CurrentSceneType);
            //形成版本文件名
            string versionPath = "download/scene/" + sceneName + ".unity3d";
            //形成完整路径
            string fullPath = DownloadMgr.Instance.localFilePath + "download/scene/" + sceneName + ".unity3d";
            //检测目标路径是否存在
            if (File.Exists(fullPath))
            {
                StartCoroutine(LoadScene(fullPath, sceneName));
            }
            else 
            {
                //如果不存在，那么应该到资源管理器（伪）去下载
                DownloadDataEntity entity = DownloadMgr.Instance.GetServerData(versionPath);
                if (entity != null)
                {
                    //在下载完数据后开始从Assetbundle中加载场景
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
                    Debug.LogError("当前版本无此文件");
                }
            }
        }
    }

    /// <summary>
    /// 场景加载
    /// </summary>
    /// <param name="fullPath">待加载场景的完整路径(含后缀)</param>
    /// <param name="sceneName">待加载场景的场景名称</param>
    /// <returns></returns>
    private IEnumerator LoadScene(string fullPath, string sceneName)
    {
        if (!ScenesHasLoad.Contains(fullPath))
        {
            Debug.Log("加载场景" + fullPath);
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
        //m_Async.progress的范围是0-0.9，获取加载进度
        if (m_CurrProgress >= 0.9f)
        {
            m_CurrProgress = 1f;
        }
        //一切为了好看，为了好看的一切    
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
    /// 一切为了好看，为了好看的一切   
    /// </summary>
    public void CanGo()
    {
        m_Async.allowSceneActivation = true;
        Invoke("OnSceneLoadOk",0.5f);
    }


}
