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
    // 定义AssetBundle路径
    private string assetBundlePath = "download/prefab/roleprefab/player/camerafollowandrotate.assetbundle";

    // 定义Prefab名称（需要在AssetBundle中包含该Prefab）
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

        // 模拟耗时操作
        yield return new WaitForSeconds(3.0f);

        Debug.Log("Coroutine completed");
    }
    private IEnumerator DownloadVersion(string url)
    {
        Debug.Log("版本文件地址" + url);
        yield return new WaitForSeconds(0.1f);
        //去网站下载版本数据
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            www.SendWebRequest();
            //获取下载时间和下载进度
            float timeOut = Time.time;
            float progress = www.downloadProgress;

            //当数据申请流程没完成时
            while (www != null && !www.isDone)
            {
                //随时同步更新进程
                if (progress < www.downloadProgress)
                {
                    timeOut = Time.time;
                    progress = www.downloadProgress;
                }
                //如果下载超时
                if ((Time.time - timeOut) > DownloadMgr.DownloadTimeOut)
                {
                    Debug.Log("下载时间过长");
                    yield break;
                }
            }
            //如果下载完成，则获取下载的内容
            if (www != null && www.error == null)
            {
                //获取版本文件内容
                string content = www.downloadHandler.text;
                Debug.Log(content);
                //if (m_OnInitVersion != null)
                //{
                //    m_OnInitVersion(DownloadMgr.Instance.PackDownloadData(content));
                //}
            }
            else
            {
                Debug.Log("下载失败 原因如下：" + www.error);
            }
        }
    }

    //private IEnumerator DownloadVersion(string url)
    //{
    //    yield return null;
    //    //去网站下载版本数据
    //    using (UnityWebRequest www = UnityWebRequest.Get(url))
    //    {
    //        www.SendWebRequest();
    //        //获取下载时间和下载进度
    //        float timeOut = Time.time;
    //        float progress = www.downloadProgress;

    //        //当数据申请流程没完成时
    //        while (www != null && !www.isDone)
    //        {
    //            //随时同步更新进程
    //            if (progress < www.downloadProgress)
    //            {
    //                timeOut = Time.time;
    //                progress = www.downloadProgress;
    //            }
    //            //如果下载超时
    //            if ((Time.time - timeOut) > DownloadMgr.DownloadTimeOut)
    //            {
    //                Debug.Log("下载时间过长");
    //                yield break;
    //            }
    //        }
    //        yield return www;
    //        //如果下载完成，则获取下载的内容
    //        if (www != null && www.error == null)
    //        {
    //            //获取版本文件内容
    //            string content = www.downloadHandler.text;
    //            Debug.Log(content);
    //        }
    //        else
    //        {
    //            Debug.Log("下载失败 原因如下：" + www.error);
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
    //        // 获取所有网络接口
    //        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

    //        // 遍历每个网络接口
    //        foreach (NetworkInterface networkInterface in networkInterfaces)
    //        {
    //            // 排除虚拟网络接口和非活动接口
    //            if (networkInterface.OperationalStatus == OperationalStatus.Up &&
    //                !networkInterface.Description.ToLowerInvariant().Contains("virtual") &&
    //                !networkInterface.Description.ToLowerInvariant().Contains("pseudo"))
    //            {
    //                // 获取网络接口的IP属性
    //                IPInterfaceProperties ipProperties = networkInterface.GetIPProperties();

    //                // 遍历每个IP地址
    //                foreach (UnicastIPAddressInformation ipAddressInfo in ipProperties.UnicastAddresses)
    //                {
    //                    // 找到第一个IPv4地址
    //                    if (ipAddressInfo.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
    //                    {
    //                        Debug.Log($"当前使用的IPv4地址: {ipAddressInfo.Address}");
    //                        //return; // 找到后直接返回
    //                    }
    //                }
    //            }
    //        }

    //        Debug.LogWarning("未找到IPv4地址。");
    //    }
    //    catch (System.Exception ex)
    //    {
    //        Debug.LogError($"发生异常: {ex.Message}");
    //    }
    //}
}
