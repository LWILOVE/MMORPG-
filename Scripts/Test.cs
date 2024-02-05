using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Xml.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

public class Test : MonoBehaviour
{
    // ����AssetBundle·��
    private string assetBundlePath = "download/prefab/roleprefab/player/camerafollowandrotate.assetbundle";

    // ����Prefab���ƣ���Ҫ��AssetBundle�а�����Prefab��
    private string prefabName = "camerafollowandrotate";
    void Start()
    {
        DownloadMgr.Instance.localFilePath = Application.persistentDataPath + "/Windows/";
        AssetBundleMgr.Instance.LoadOrDownload<GameObject>(assetBundlePath,prefabName,
            (GameObject obj)=>
        {
            Instantiate(obj);
        },type:0);

    }

    IEnumerator YourCoroutine()
    {
        Debug.Log("Coroutine started");

        // ģ���ʱ����
        yield return new WaitForSeconds(3.0f);

        Debug.Log("Coroutine completed");
    }
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
                Debug.Log(content);
                //if (m_OnInitVersion != null)
                //{
                //    m_OnInitVersion(DownloadMgr.Instance.PackDownloadData(content));
                //}
            }
            else
            {
                Debug.Log("����ʧ�� ԭ�����£�" + www.error);
            }
        }
    }

    //private IEnumerator DownloadVersion(string url)
    //{
    //    yield return null;
    //    //ȥ��վ���ذ汾����
    //    using (UnityWebRequest www = UnityWebRequest.Get(url))
    //    {
    //        www.SendWebRequest();
    //        //��ȡ����ʱ������ؽ���
    //        float timeOut = Time.time;
    //        float progress = www.downloadProgress;

    //        //��������������û���ʱ
    //        while (www != null && !www.isDone)
    //        {
    //            //��ʱͬ�����½���
    //            if (progress < www.downloadProgress)
    //            {
    //                timeOut = Time.time;
    //                progress = www.downloadProgress;
    //            }
    //            //������س�ʱ
    //            if ((Time.time - timeOut) > DownloadMgr.DownloadTimeOut)
    //            {
    //                Debug.Log("����ʱ�����");
    //                yield break;
    //            }
    //        }
    //        yield return www;
    //        //���������ɣ����ȡ���ص�����
    //        if (www != null && www.error == null)
    //        {
    //            //��ȡ�汾�ļ�����
    //            string content = www.downloadHandler.text;
    //            Debug.Log(content);
    //        }
    //        else
    //        {
    //            Debug.Log("����ʧ�� ԭ�����£�" + www.error);
    //        }
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    UILoadingCtrl.Instance.LoadToWorldMap(1);
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    roleCtrl.ToHurt();
        //}
        //return;
    }
    //void GetIPv4Address()
    //{
    //    try
    //    {
    //        // ��ȡ��������ӿ�
    //        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

    //        // ����ÿ������ӿ�
    //        foreach (NetworkInterface networkInterface in networkInterfaces)
    //        {
    //            // �ų���������ӿںͷǻ�ӿ�
    //            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
    //                !networkInterface.Description.ToLowerInvariant().Contains("virtual") &&
    //                !networkInterface.Description.ToLowerInvariant().Contains("pseudo"))
    //            {
    //                // ��ȡ����ӿڵ�IP����
    //                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

    //                // ����ÿ��IP��ַ
    //                foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
    //                {
    //                    // �ҵ���һ��IPv4��ַ
    //                    if (ipAddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    //                    {
    //                        Debug.Log($"��ǰʹ�õ�IPv4��ַ: {ipAddressInfo.Address}");
    //                        //return; // �ҵ���ֱ�ӷ���
    //                    }
    //                }
    //            }
    //        }

    //        Debug.LogWarning("δ�ҵ�IPv4��ַ��");
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError($"�����쳣: {ex.Message}");
    //    }
    //}
}
