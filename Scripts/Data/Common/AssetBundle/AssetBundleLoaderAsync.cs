using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//使用案例
//镜像异步加载方式1：普通委托
//AssetBundleLoaderAsync async = AssetBundleMgr.Instance.LoadAsync(@"Role\environment.assetbundle", "Environment");
//async.OnLoadComplete = OnLoadComplete;
//镜像异步加载方式2：使用lambda表达式
//AssetBundleMgr.Instance.LoadAsync(@"Role\environment.assetbundle", "Environment")
//    .OnLoadComplete =(UnityEngine.Object obj) =>
//    {
//        Instantiate(obj);
//    };

/// <summary>
/// 异步加载资源包注：实际上调用的是AssetBundleMgr类的LoadAsync
/// </summary>
public class AssetBundleLoaderAsync : MonoBehaviour
{
    //资源绝对路径
    private string m_FullPath;
    //资源名
    private string m_Name;
    //资源加载创建请求
    private AssetBundleCreateRequest request;
    //资源包
    private AssetBundle bundle;
    //加载完成委托
    public System.Action<Object> OnLoadComplete;

    #region 初始化要加载的资源的路径和资源名称
    /// <summary>
    /// 初始化要加载的资源的路径和资源名称
    /// </summary>
    /// <param name="path">资源相对路径（AssetBundles/平台/内部完全路径(含后缀)）</param>
    /// <param name="name">资源名</param>
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

    #region 使用协程异步加载游戏资源，返回可使用的资源镜像，同时在完成镜像加载时销毁自身释放资源
    /// <summary>
    /// 异步加载获取打包资源
    /// </summary>
    /// <returns></returns>
    private IEnumerator Load()
    {
        //异步加载资源
        request = AssetBundle.LoadFromMemoryAsync(LocalFileMgr.Instance.GetBufffer(m_FullPath));
        yield return request;
        //获取资源
        bundle = request.assetBundle;
        if (OnLoadComplete != null)
        {
            //获取资源镜像
            OnLoadComplete(bundle.LoadAsset(m_Name));
            //OnLoadComplete(bundle.LoadAssetAsync(m_Name);
            //加载完成销毁自身（加载包）
            Destroy(gameObject);
        }
    }
    #endregion



    private void OnDestroy()
    {
        //卸载资源包
        if (bundle != null)
        {
            bundle.Unload(false);
        }
        m_FullPath = null;
        m_Name = null;
    }
}
