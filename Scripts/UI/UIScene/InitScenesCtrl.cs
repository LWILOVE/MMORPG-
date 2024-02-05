using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScenesCtrl : UIWindowViewBase
{
    #region 作者模式模仿变量
    //当前下载量
    private int currDownload = 0;
    //总下载量
    private int allDownLoad = 120;
    #endregion
    protected override void OnStart()
    {
        Invoke("SourceInit",0.1f);
    }

    /// <summary>
    /// 资源初始化
    /// </summary>
    private void SourceInit()
    {
        //根据渠道配置确定资源下载地址
        DownloadMgr.DownloadBaseUrl = GlobalInit.Instance.CurrChannelInitConfig.SourceUrl;
        DownloadMgr.Instance.InitStreamingAssets(OnInitComplete);

    }

    /// <summary>
    /// 初始化完成回调
    /// </summary>
    private void OnInitComplete()
    {
        StartCoroutine(LoadLogOn());
    }

    /// <summary>
    /// 加载登录场景
    /// </summary>
    public IEnumerator LoadLogOn()
    {
        yield return new WaitForSeconds(delayTime);
        //进行场景跳转
        UILoadingCtrl.Instance.LoadToLogOn();
    }
}
