using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using LitJson;
using System.Net.NetworkInformation;

/// <summary>
/// HttpͨѶ������
/// </summary>
public class NetWorkHttp : SingletonMono<NetWorkHttp>
{
    #region ����
    /// <summary>
    /// Web����ص�ί��
    /// </summary>
    private XLuaCustomExport.NetWorkSendDataCallBack m_CallBack;

    /// <summary>
    /// Web����ص����ݶ���
    /// </summary>
    private CallBackArgs m_CallBackArgs;
    /// <summary>
    /// �Ƿ�æ
    /// </summary>
    private bool m_isBusy = false;

    public bool IsBusy
    {
        get { return m_isBusy; }
    }
    #endregion
    protected override void OnStart()
    {
        base.OnStart();
        m_CallBackArgs = new CallBackArgs();
    }
    #region SendData ����Web����
    /// <summary>
    /// ����Web����
    /// </summary>
    /// <param name="url">��λ��ַ(��Ҫ���ʵ���վ)</param>
    /// <param name="callBack">�ص����</param>
    /// <param name="isPost">�Ƿ�������ݴ���</param>
    /// <param name="dic">����</param>
    public void SendData(string url, XLuaCustomExport.NetWorkSendDataCallBack callBack, bool isPost = false, Dictionary<string, object> dic = null)
    {
        if (m_isBusy)
        { return; }
        m_isBusy = true;
        m_CallBack = callBack;

        if (!isPost)
        {
            GetUrl(url);
        }
        else
        {
            //����Web����
            if (dic != null)
            {
                //�ͻ��˵���Ψһʶ����
                dic["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;
                //�ͻ����豸�ͺ�
                dic["deviceModel"] = DeviceUtil.DeviceModel;
                long t = GlobalInit.Instance.CurrentServerTime;
                //ǩ��:=ʱ���:�ͻ���ID
                dic["sign"] = EncryptUtil.Md5(String.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));
                //ʱ���
                dic["t"] = t;
            }
            Debug.Log("����׼��������������");
            PostUrl(url, dic == null ? "" : JsonMapper.ToJson(dic));
        }
    }
    #endregion

    #region PostUrl Post����
    /// <summary>
    /// Post����
    /// </summary>
    /// <param name="url">��λ��ַ</param>
    /// <param name="json">����</param>
    private void PostUrl(string url, string json)
    {
        //��ҳ����
        WWWForm form = new WWWForm();
        //��ӿռ䣺����,ֵ��
        form.AddField("", json);
        UnityWebRequest data = UnityWebRequest.Post(url, form);

        Debug.Log("���ڽ�����������");
        StartCoroutine(Request(data));
    }
    #endregion

    #region GetUrl Get����
    /// <summary>
    /// Get����
    /// </summary>
    /// <param name="url">��λ��ַ���û����ڵĵ�ַ��</param>
    private void GetUrl(string url)
    {
        ///ע��WWW����Ŀǰ�Ѿ���Unity������UnityWebRequest��WWW����λ���Ʒ
        UnityWebRequest data = UnityWebRequest.Get(url);
        StartCoroutine(Request(data));
    }
    #endregion

    #region Request �������ݿ�
    /// <summary>
    /// ������ַ���
    /// </summary>
    /// <param name="data">����</param>
    /// <returns></returns>
    private IEnumerator Request(UnityWebRequest data)
    {
        Debug.Log("������������");
        yield return data.SendWebRequest();
        m_isBusy = false;
        Debug.Log(data.downloadHandler.text);
        //����������������
        if (string.IsNullOrEmpty(data.error))
        {
            if (data.downloadHandler.text == "null")
            {
                Debug.Log("����Ϊ��");
                if (m_CallBack != null)
                {
                    m_CallBackArgs.HasError = true;
                    m_CallBackArgs.ErrorMsg = "δ��������";
                    m_CallBack(m_CallBackArgs);
                }
            }
            else
            {
                Debug.Log("��������");
                if (m_CallBack != null)
                {
                    m_CallBackArgs.HasError = false;
                    m_CallBackArgs.Value = data.downloadHandler.text;
                    m_CallBack(m_CallBackArgs);
                }
            }

        }
        else
        {
            Debug.Log("��������ʧ��");
            if (m_CallBack != null)
            {
                m_CallBackArgs.HasError = true;
                m_CallBackArgs.ErrorMsg = data.error;
                m_CallBack(m_CallBackArgs);
            }
        }
        data.Dispose();
    }
    #endregion

    #region CallBackArgs Web����ص�������
    /// <summary>
    /// Web����ص�������
    /// �̳е�EventArgsҲ������ɶ��
    /// </summary>
    public class CallBackArgs : EventArgs
    {
        /// <summary>
        /// �Ƿ��д�
        /// </summary>
        public bool HasError;

        /// <summary>
        /// ����
        /// </summary>
        public string Value;

        /// <summary>
        /// ����ԭ��
        /// </summary>
        public string ErrorMsg;

    }
    #endregion
}
