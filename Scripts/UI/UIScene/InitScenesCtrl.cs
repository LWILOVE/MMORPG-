using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScenesCtrl : UIWindowViewBase
{
    #region ����ģʽģ�±���
    //��ǰ������
    private int currDownload = 0;
    //��������
    private int allDownLoad = 120;
    #endregion
    protected override void OnStart()
    {
        Invoke("SourceInit",0.1f);
    }

    /// <summary>
    /// ��Դ��ʼ��
    /// </summary>
    private void SourceInit()
    {
        //������������ȷ����Դ���ص�ַ
        DownloadMgr.DownloadBaseUrl = GlobalInit.Instance.CurrChannelInitConfig.SourceUrl;
        DownloadMgr.Instance.InitStreamingAssets(OnInitComplete);

    }

    /// <summary>
    /// ��ʼ����ɻص�
    /// </summary>
    private void OnInitComplete()
    {
        StartCoroutine(LoadLogOn());
    }

    /// <summary>
    /// ���ص�¼����
    /// </summary>
    public IEnumerator LoadLogOn()
    {
        yield return new WaitForSeconds(delayTime);
        //���г�����ת
        UILoadingCtrl.Instance.LoadToLogOn();
    }
}
