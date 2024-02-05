using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// 资源加载管理类
/// </summary>
public class AssetBundleMgr : SingletonMiddle<AssetBundleMgr>
{
    #region 变量
    /// <summary>
    /// 依赖文件配置
    /// </summary>
    private AssetBundleManifest m_Manifest;

    /// <summary>
    /// 镜像字典
    /// </summary>
    private Dictionary<string, Object> m_AssetDic = new Dictionary<string, Object>();

    /// <summary>
    /// 依赖项字典，在跨场景时释放
    /// </summary>
    private Dictionary<string, AssetBundleLoader> m_DpsAssetBundleLoaderDic = new Dictionary<string, AssetBundleLoader>();
    #endregion

    #region Load 同步加载游戏物体，返回一个GameObject类型
    /// <summary>
    /// 同步加载游戏物体，返回一个GameObject类型
    /// 不创造物体
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject Load(string path, string name)
    {
#if DISABLE_ASSETBUNDLE
        //todo
        //return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", path.Replace("assetbundle", "prefab")));
#else
        using (AssetBundleLoader loader = new AssetBundleLoader(path))
        {
            return loader.LoadAsset<GameObject>(name);
        }
#endif
        return null;
    }
    #endregion

    #region LoadAndClone 同步加载并克隆游戏物体，返回一个GameObject类型
    /// <summary>
    /// 同步加载并克隆游戏物体，返回一个GameObject类型
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public GameObject LoadAndClone(string path, string name)
    {

#if DISABLE_ASSETBUNDLE
        //TODO
        //GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(string.Format("Assets/{0}", path.Replace("assetbundle", "prefab")));
        //return Object.Instantiate(obj);
#else
        using (AssetBundleLoader loader = new AssetBundleLoader(path))
        {
            GameObject obj = loader.LoadAsset<GameObject>(name);
            return Object.Instantiate(obj);
        }
#endif
        return null;
    }
    #endregion

    #region LoadAsync 创建打包资源异步加载物体，并返回一个存储好要加载的资源的路径和名称的异步对象
    /// <summary>
    /// 创建打包资源异步加载物体，并返回一个存储好要加载的资源的路径和名称的异步对象
    /// </summary>
    /// <param name="path">资源相对路径（AssetBundles/平台/内部完全路径(含后缀)）</param>
    /// <param name="name">资源名</param>
    /// <returns></returns>
    public AssetBundleLoaderAsync LoadAsync(string path, string name)
    {
        GameObject obj = new GameObject("AssetBundleLoadAsync");
        AssetBundleLoaderAsync async = obj.AddComponent<AssetBundleLoaderAsync>();
        async.Init(path, name);
        return async;
    }
    #endregion

    #region LoadOrDownload 方式1：加载游戏物体 方式2：加载其他类型文件（同时会完成文件配置）
    /// <summary>
    /// 加载或者下载方法
    /// </summary>
    /// <param name="path">mmorpg/xxx的xxx</param>
    /// <param name="name">文件名</param>
    /// <param name="onComplete">工作完成回调</param>
    /// <param name="type">0=GameObjectORPrefab,1=PNG</param>
    public void LoadOrDownload<T>(string path, string name, System.Action<T> onComplete, byte type)
        where T : Object
    {
        lock (this)
        {
            if (onComplete != null)
            {
                string fullPath = DownloadMgr.Instance.localFilePath + path;
                Debug.Log("要加载的路径" + fullPath);
                //1.加载总依赖文件配置(配置文件加载必须选择同步加载)
                LoadManifestBundle();
                //2.获取加载项的所有依赖配置文件
                string[] arrDps = m_Manifest.GetAllDependencies(path);
                //3.检测所需目标依赖配置文件是否已经下载完成
                CheckDps(0, arrDps, () =>
                {
                    if (!File.Exists(fullPath))
                    {
                        Debug.Log("未检测到目标资源，开始加载");
                        //若本地不存在该路径下的资源，则从服务器进行下载
                        DownloadDataEntity entity = DownloadMgr.Instance.GetServerData(path);
                        if (entity != null)
                        {
                            //将下载到的数据写入本地
                            AssetBundleDownload.Instance.StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity,
                                (bool isSuccess) =>
                                {
                                    //配置文件下载成功
                                    if (isSuccess)
                                    {
                                        #region 路径曾经成功配置过
                                        //如果该路径下系列文件曾经加载过，就返回指定文件的镜像
                                        if (m_AssetDic.ContainsKey(fullPath))
                                        {
                                            if (onComplete != null)
                                            { onComplete(m_AssetDic[fullPath] as T); }
                                            return;
                                        }
                                        #endregion

                                        #region 路径未曾成功配置过
                                        //如果还没存在，则将路径下所有的文件加入镜像字典和依赖项字典
                                        for (int i = 0; i < arrDps.Length; i++)
                                        {
                                            if (!m_AssetDic.ContainsKey((DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()))
                                            {
                                                //实例化
                                                AssetBundleLoader loader = new AssetBundleLoader(arrDps[i]);
                                                Object obj = loader.LoadAsset<Object>(arrDps[i]);
                                                //将依赖项的loader加入字典
                                                m_DpsAssetBundleLoaderDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = loader;
                                                m_AssetDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = obj;

                                            }
                                        }
                                        //直接加载
                                        using (AssetBundleLoader loader = new AssetBundleLoader(fullPath))
                                        {
                                            if (onComplete != null)
                                            {
                                                Object obj = loader.LoadAsset<T>(name);
                                                //进行回调
                                                onComplete(obj as T);
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        Debug.Log("配置文件下载失败");
                                    }
                                }));
                        }
                    }
                    else
                    {
                        //如果主资源存在,加载主资源
                        if (m_AssetDic.ContainsKey(fullPath.ToLower()))
                        {
                            Debug.Log("目标已入库，开始加载");
                            if (onComplete != null)
                            { onComplete(m_AssetDic[fullPath.ToLower()] as T); }
                            return;
                        }

                        //加载依赖项和镜像
                        for (int i = 0; i < arrDps.Length; i++)
                        {
                            if (!m_DpsAssetBundleLoaderDic.ContainsKey((DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()))
                            {
                                //实例化       
                                AssetBundleLoader loader = new AssetBundleLoader(arrDps[i]);
                                //获取其文件化路径
                                Object obj = loader.LoadAsset(GameUtil.GetFileName(arrDps[i]));
                                //将依赖项的loader加入字典
                                m_DpsAssetBundleLoaderDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = loader;
                                m_AssetDic[(DownloadMgr.Instance.localFilePath + arrDps[i]).ToLower()] = obj;
                            }
                        }

                        using (AssetBundleLoader loader = new AssetBundleLoader(path))
                        {
                            if (onComplete != null)
                            {
                                Debug.Log("目标未入库，开始加载");
                                Object obj = loader.LoadAsset<T>(name);
                                
                                m_AssetDic[fullPath.ToLower()] = obj;
                                //进行回调
                                onComplete(obj as T);
                            }
                        }
                    }
                });
            }
        }
    }

    /// <summary>
    /// 加载或者下载方法
    /// </summary>
    /// <param name="path">mmorpg/xxx的xxx</param>
    /// <param name="name">文件名</param>
    /// <param name="onComplete">工作完成回调</param>
    public void LoadOrDownload(string path, string name, System.Action<GameObject> onComplete)
    {
        LoadOrDownload<GameObject>(path, name, onComplete, 0);
    }

    /// <summary>
    /// 加载依赖配置文件(本质上是同步机器的依赖文件配置信息)
    /// </summary>
    private void LoadManifestBundle()
    {
        //如果加载过了就返回
        if (m_Manifest != null)
        { return; }
        //配置文件包名
        string assetName = string.Empty;
#if UNITY_STANDALONE_WIN
        assetName = string.Format("Windows");
#elif UNITY_ANDROID
        assetName = string.Format("Android");
#elif UNITY_IPHONE
        assetName = string.Format("iOS");
#endif
        Debug.Log("正在下载主依赖文件");

        using (AssetBundleLoader loader = new AssetBundleLoader(assetName))
        {
            //加载文件夹的总依赖项
            m_Manifest = loader.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (m_Manifest == null)
            {
                Debug.Log("文件总依赖项加载失败");
            }
            else
            {
                Debug.Log("总依赖配置文件名：" + m_Manifest.name);
            }
        }
    }

    /// <summary>
    /// 利用递归检查依赖项
    /// </summary>
    /// <param name="index">下标</param>
    /// <param name="arrDps">依赖项数组</param>
    /// <param name="onComplete">完成委托</param>
    private void CheckDps(int index, string[] arrDps, System.Action onComplete)
    {
        if (arrDps == null || arrDps.Length == 0)
        {
            if (onComplete != null)
            { onComplete(); }
            return;
        }

        //获取依赖文件位置
        string fullPath = DownloadMgr.Instance.localFilePath + arrDps[index];
        if (!File.Exists(fullPath))
        {
            //如果本地没有该配置文件，则从服务器下载
            DownloadDataEntity entity = DownloadMgr.Instance.GetServerData(arrDps[index]);
            if (entity != null)
            {
                //将文件写入到本地，并检测下一个配置文件
                AssetBundleDownload.Instance.StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity, (bool isSuccess) =>
                {
                    index++;
                    if (index == arrDps.Length)
                    {
                        if (onComplete != null)
                        { onComplete(); }
                        return;
                    }
                    CheckDps(index, arrDps, onComplete);
                }));
            }
            else
            {
                Debug.Log(arrDps[index] + "不存在");
                arrDps[index] = "download/source/uisource/uicommon/bg_007.assetbundle";
                Debug.Log(arrDps[index]);
                DownloadDataEntity entity2 = DownloadMgr.Instance.GetServerData(arrDps[index]);
                if(entity2==null)
                {
                    Debug.Log("依旧没找到");
                }
                //强行修复
                //将文件写入到本地，并检测下一个配置文件
                AssetBundleDownload.Instance.StartCoroutine(AssetBundleDownload.Instance.DownloadData(entity2, (bool isSuccess) =>
                {
                    index++;
                    if (index == arrDps.Length)
                    {
                        if (onComplete != null)
                        { onComplete(); }
                        return;
                    }
                    CheckDps(index, arrDps, onComplete);
                }));
            }
        }
        else
        {
            index++;
            if (index == arrDps.Length)
            {
                if (onComplete != null)
                { onComplete(); }
                return;
            }
            CheckDps(index, arrDps, onComplete);
        }
    }

    #endregion

}
