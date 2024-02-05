using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//使用案例1
//using (AssetBundleLoader loader = new AssetBundleLoader(@"Role\environment.assetbundle"))
//{
//    GameObject obj = loader.LoadAsset<GameObject>("Environment");
//    Instantiate(obj);
//}

//使用案例2
//GameObject obj = AssetBundleMgr.Instance.Load(@"Role\environment.assetbundle", "Environment");
//Instantiate(obj);

/// <summary>
/// 同步加载资源包
/// </summary>
public class AssetBundleLoader :IDisposable
{
    /// <summary>
    /// 资源包
    /// </summary>
    private AssetBundle bundle;

    #region 构造函数，根据所传路径从资源包同步加载资源对应的资源包 AssetBundleLoader
    /// <summary>
    /// 资源同步加载（返回一个AssetBundle格式的变量）
    /// </summary>
    /// <param name="assetBundlePath">资源在资源包的路径（即若完整应该是MMORPG/后面的，若在别的路径可填全路径，并让下面的参数为true）</param>
    public AssetBundleLoader(string assetBundlePath)
    {
        //文件的全路径
        string fullPath = DownloadMgr.Instance.localFilePath + assetBundlePath;
        byte[] bytes = LocalFileMgr.Instance.GetBufffer(fullPath);
        if (bytes == null)
        {
            Debug.Log("无法找到目标文件");
        }
        //将Byte格式的内容读取翻译为AssetBundle模式
        bundle = AssetBundle.LoadFromMemory(bytes);
        
    }
    #endregion

    #region 根据资源名称从资源包中加载出可操作的资源镜像
    /// <summary>
    /// 生成可使用的资源镜像
    /// </summary>
    /// <typeparam name="T">加载的资源的资源类型</typeparam>
    /// <param name="name">资源名</param>
    /// <returns></returns>
    public T LoadAsset<T>(string name) where T : UnityEngine.Object
    {
        if (bundle == null)
        {
            Debug.Log("文件包为空");
            return default(T);
        }
        T asset = bundle.LoadAsset(name) as T;
        //二阶防漏
        if (asset == null)
        {
            Debug.Log("资源已在Asset内" + name);
            // 获取 AssetBundle 中所有资源的名称
            string[] assetNames = bundle.GetAllAssetNames();
            // 输出资源名称
            foreach (string assetName in assetNames)
            {
                Debug.Log("目标名：" + assetName +  "  加载名：" + name);
            }
        }
        //加载AssetBundle格式的资源，并生成可使用的镜像
        return asset;
    }   

    /// <summary>
    /// 根据资源名称从资源包中加载出可操作的Unity资源镜像
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UnityEngine.Object LoadAsset(string name)
    {
        return bundle.LoadAsset(name);
    }
    #endregion

    /// <summary>
    /// 在销毁时回收该资源包以提高程序效率
    /// </summary>
    public void Dispose()
    {
        if (bundle != null)
        {
            //卸载
            bundle.Unload(false);
        }
    }


}
