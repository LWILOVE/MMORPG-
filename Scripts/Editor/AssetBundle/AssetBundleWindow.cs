    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

/// <summary>
/// AssetBundle管理窗口
/// </summary>
public class AssetBundleWindow : EditorWindow
{
    #region 属性
    /// <summary>
    /// 窗口所在位置
    /// </summary>
    private Vector2 pos;
    /// <summary>
    /// xml路径传输
    /// </summary>
    private AssetBundleDAL dal;
    /// <summary>
    /// xml数据列表
    /// </summary>
    private List<AssetBundleEntity> m_List;
    /// <summary>
    /// 打包数据字典
    /// <打包物编号,是否打包>
    /// </summary>
    private Dictionary<string, bool> m_Dic;
    /// <summary>
    /// 数据标记数组
    /// </summary>
    private string[] arrTag = { "All", "Scene", "Role", "Effect", "Audio","UI" ,"None" };
    /// <summary>
    /// 当前选中的数组的项
    /// </summary>
    private int tagIndex = 0;
    /// <summary>
    /// 选中的标记的索引（用于制作只要下拉列表就能一键选中要打包的文件的功能）
    /// </summary>
    private int selectTagIndex = -1;
    /// <summary>
    /// 打包平台数组
    /// </summary>
    private string[] arrBuildTarget = { "Windows", "Android", "iOS" };
    /// <summary>
    /// 选中的打包平台索引
    /// </summary>
    private int selectBuildTargetIndex = -1;
#if UNITY_STANDALONE_WIN
    /// <summary>
    /// 打包的目标平台
    /// </summary>
    private BuildTarget target = BuildTarget.StandaloneWindows;
    /// <summary>
    /// 打包目标平台的下标
    /// </summary>
    private int buildTargetIndex = 0;
#elif UNITY_ANDROID
    /// <summary>
    /// 打包的目标平台
    /// </summary>
    private BuildTarget target = BuildTarget.Android;
    /// <summary>
    /// 打包目标平台的下标
    /// </summary>
    private int buildTargetIndex = 1;
#elif UNITY_IPHONE
    /// <summary>
    /// 打包的目标平台
    /// </summary>
    private BuildTarget target = BuildTarget.iOS;
    /// <summary>
    /// 打包目标平台的下标
    /// </summary>
    private int buildTargetIndex = 2;
#endif
    #endregion
    private void OnEnable()
    {
        ///////////////////////设置打包数据字典
        //读取XML文件
        string xmlPath = Application.dataPath + @"\Scripts\Editor\AssetBundle\AssetBundleConfig.xml";
        dal = new AssetBundleDAL(xmlPath);
        //获取XML数据
        m_List = dal.GetList();
        m_Dic = new Dictionary<string, bool>();
        //将要打包的数据项屯进字典  
        for (int i = 0; i < m_List.Count; i++)
        {
            m_Dic[m_List[i].Key] = true;
        }
        ///////////////////////设置打包数据字典
    }

    /// <summary>
    /// 绘制窗口
    /// </summary>
    private void OnGUI()
    {
        //如果没有需要打包的东西，那么就不需要多干活
        if (m_List == null)
        {return;}
        #region GUI功能按钮
        GUILayout.BeginHorizontal("box");
        //EditorGUILayout.Popup:操作功能：下拉列表：每个占用一小块空间
        //(默认选择项,下拉列表内容,列表宽度)
        selectTagIndex = EditorGUILayout.Popup(tagIndex, arrTag, GUILayout.Width(98.5f));
        //制作只要选中其他的选项时就会自动勾选所有指定索引下的物品的功能
        if (selectTagIndex != tagIndex)
        {
            tagIndex = selectTagIndex;
            EditorApplication.delayCall = OnSelectTagCallBack;
        }
        ////一个废弃的功能，用于选中当前Tag指定的所有物品
        //if (GUILayout.Button("选定Tag", GUILayout.Width(98.5f)))
        //{
        //    EditorApplication.delayCall = OnSelectTagCallBack;
        //}
        //获取选中的打包平台
        selectBuildTargetIndex = EditorGUILayout.Popup(buildTargetIndex, arrBuildTarget, GUILayout.Width(98.5f));
        //更新打包平台设置
        if (selectBuildTargetIndex != buildTargetIndex)
        {
            buildTargetIndex = selectBuildTargetIndex;
            EditorApplication.delayCall = OnSelectTargetCallBack;
        }
        //if (GUILayout.Button("选定Target", GUILayout.Width(98.5f)))
        //{
        //    EditorApplication.delayCall = OnSelectTargetCallBack;
        //}
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("保存设置", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnSaveAssetBundleCallBack;
        }
        if (GUILayout.Button("打AssetBundle包", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnAssetBundleCallBack;
        }
        if (GUILayout.Button("清空AssetBundle包", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnClearAssetBundleCallBack;
        }
        if (GUILayout.Button("拷贝数据表", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnCopyDataTableCallBack;
        }
        if (GUILayout.Button("生成版本文件", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnCreateVersionFileCallBack;
        }
        ////将本行中未利用的部分补上空
        //EditorGUILayout.Space();
        ////设定占据空间的空
        ///(占据空间大小)
        //GUILayout.Space(10);
        GUILayout.EndHorizontal();
        #endregion

        #region GUI文件信息显示设置
        GUILayout.BeginHorizontal("box");
        //GUILayout.Label:操作功能：横条
        //(横条名，宽度)
        GUILayout.Space(25);
        GUILayout.Label("包名"); GUILayout.Label("标记",GUILayout.Width(75));   GUILayout.Label("文件夹", GUILayout.Width(75)); GUILayout.Label("初始资源", GUILayout.Width(75)); GUILayout.EndHorizontal();
        //在GUI窗口中启动新的竖读写
        GUILayout.BeginVertical();
        //在GUI窗口中开启滚动视图（感觉效果不明显）
        pos = EditorGUILayout.BeginScrollView(pos);
        for (int i = 0; i < m_List.Count; i++)
        {
            AssetBundleEntity entity = m_List[i];
            GUILayout.BeginHorizontal("box");
            m_Dic[entity.Key] = GUILayout.Toggle(m_Dic[entity.Key],"",GUILayout.Width(25));
            GUILayout.Label(entity.Name);
            GUILayout.Label(entity.Tag, GUILayout.Width(75));
            GUILayout.Label(entity.IsFolder.ToString(),GUILayout.Width(200));
            GUILayout.Label(entity.IsFirstData.ToString(),GUILayout.Width(200));
            GUILayout.EndHorizontal();
            foreach (string path in entity.PathList)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.Space(25);
                GUILayout.Label("全路径：" + path);
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        #endregion
    }

    #region 选定标签 OnSelectTagCallBack
    /// <summary>
    /// 标签选定回调
    /// (选中所有需要打包的文件)
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnSelectTagCallBack()
    {
        //根据选项设定内容
        switch (tagIndex)
        {
            case 0://全选
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = true;
                }
                break;
            case 1://场景
                foreach (AssetBundleEntity entity in m_List)
                {
                    //entity.Tag.Equals("Scene",StringComparison.CurrentCultureIgnoreCase)：判定：只有当XXX的内容为（参数1）时才返回true
                    m_Dic[entity.Key] = entity.Tag.Equals("Scene", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 2://角色
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = entity.Tag.Equals("Role", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 3://特效
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = entity.Tag.Equals("Effect", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 4://音乐
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = entity.Tag.Equals("Audio", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 5://UI
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = entity.Tag.Equals("UI", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 6://无
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = false;
                }
                break;

        }
        Debug.LogFormat("当前选中的Tag为：{0}", arrTag[tagIndex]);
    }
    #endregion

    #region 选定平台 OnSelectTargetCallBack
    /// <summary>
    /// 目标平台选定回调
    /// 选中要打包到的目标平台
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnSelectTargetCallBack()
    {
        switch (buildTargetIndex)
        {
            case 0:
                target = BuildTarget.StandaloneWindows;
                break;
            case 1:
                target = BuildTarget.Android;
                break;
            case 2:
                target = BuildTarget.iOS;
                break;
        }
        Debug.LogFormat("当前选中的打包平台为：{0}", arrBuildTarget[buildTargetIndex]);
    }
    #endregion

    #region 保存设置 OnSaveAssetBundleCallBack SaveFolderSettings SaveFileSetting
    /// <summary>
    /// 保存设置回调
    /// 设置包文件的保存路径
    /// </summary>
    private void OnSaveAssetBundleCallBack()
    {
        //需要打包的对象（这个链表就是DAL那里得到的链表的另一种格式而已）
        List<AssetBundleEntity> listNeedBuild = new List<AssetBundleEntity>();
        //将需要打包的数据加入列表
        foreach (AssetBundleEntity entity in m_List)
        {
            if (m_Dic[entity.Key])
            {
                entity.IsChecked = true;
                listNeedBuild.Add(entity);
            }
            else
            {
                entity.IsChecked = false;
                listNeedBuild.Add(entity);
            }
        }
        //设置文件夹与子文件夹的项的包名
        for (int i = 0; i < listNeedBuild.Count; i++)
        {
            //获取一个节点
            AssetBundleEntity entity = listNeedBuild[i];
            if (entity.IsFolder)
            {
                //若该节点是文件夹，则应该遍历其中内容，将其各自命名
                //制作绝对路径（全路径）
                string[] folderArr = new string[entity.PathList.Count];
                for (int j = 0; j < entity.PathList.Count; j++)
                {
                    folderArr[j] = Application.dataPath + "/" + entity.PathList[j];
                }
                SaveFolderSettings(folderArr, !entity.IsChecked);
            }
            else
            {
                //若该结点是普通物体，那就只需要设置该物体的属性即可
                //制作绝对路径（全路径）
                string[] folderArr = new string[entity.PathList.Count];
                for (int j = 0; j < entity.PathList.Count; j++)
                {
                    folderArr[j] = Application.dataPath + "/" + entity.PathList[j];
                    SaveFileSetting(folderArr[j], !entity.IsChecked);
                }
            }
        }
        Debug.Log("路径设置完成");
    }

    /// <summary>
    /// 设置目标文件夹下所有文件的Unity打包路径
    /// </summary>
    /// <param name="folderArr">文件夹的绝对路径</param>
    /// <param name="isSetNull">是否设置为空</param>
    private void SaveFolderSettings(string[] folderArr, bool isSetNull)
    {
        foreach (string folderPath in folderArr)
        {
            Debug.Log(folderPath);
            //获取文件夹下的文件
            string[] arrFile = Directory.GetFiles(folderPath);
            //对文件进行设置
            foreach (string filePath in arrFile)
            {
                //进行设置
                SaveFileSetting(filePath, isSetNull);
            }
            //搜索文件夹下的子文件夹
            string[] arrFolder = Directory.GetDirectories(folderPath);
            SaveFolderSettings(arrFolder, isSetNull);
        }
    }

    /// <summary>
    /// 设置目标文件的Unity打包路径
    /// </summary>
    /// <param name="filePath">目标文件的绝对路径（全路径）</param>
    /// <param name="isSetNull">是否为空</param>
    private void SaveFileSetting(string filePath, bool isSetNull)
    {
        //获取文件信息
        FileInfo file = new FileInfo(filePath);
        //根据文件后缀，判定不是资源信息存储文件
        if (!file.Extension.Equals(".meta", StringComparison.CurrentCultureIgnoreCase))
        {
            //获取文件的短路径（xml中去除后缀的路径）
            int index = filePath.IndexOf("Assets/", StringComparison.CurrentCultureIgnoreCase);
            string newPath = filePath.Substring(index);
            //去除文件的不必要后缀和前引，获取其设定名
            string fileName = newPath.Replace("Assets/", "").Replace(file.Extension, "");

            //设置文件后缀（场景后缀设置为unity3d,其他为assetbundle）
            string variant = file.Extension.Equals(".unity", StringComparison.CurrentCultureIgnoreCase) ?
                "unity3d" : "assetbundle";
            //在Unity中设置打包路径
            //(要设置打包文件的文件路径)
            AssetImporter import = AssetImporter.GetAtPath(newPath);
            //设置其包名和扩展名
            import.SetAssetBundleNameAndVariant(fileName, variant);
            //若想要设置为空
            if (isSetNull)
            {
                import.SetAssetBundleNameAndVariant(null, null);
            }
            //将设置好的信息进行保存
            import.SaveAndReimport();
        }
    }
    #endregion

    #region 进行打包 OnAssetBundleCallBack
    /// <summary>
    /// 打包回调(通过配置文件进行打包)
    /// （检索至目标路径并进行打包）
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnAssetBundleCallBack()
    {
        //包所在目录
        string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        Debug.Log("包目录");
        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        //调用打包方法
        BuildPipeline.BuildAssetBundles(toPath, BuildAssetBundleOptions.None, target);
        Debug.Log("打包完成");
    }
    #endregion

    #region 清空打包 OnClearAssetBundleCallBack
    /// <summary>
    /// 清空AssetBundle包回调
    /// （删除指定平台的文件夹）
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnClearAssetBundleCallBack()
    {
        //待清空路径
        string path = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        //若存在，则删除
        if (Directory.Exists(path))
        {
            //文件夹删除
            //(路径，是否删除子目录)
            Directory.Delete(path,true);
        }
        Debug.Log("已清空");
    }
    #endregion

    #region 数据复制 OnCopyDataTableCallBack
    /// <summary>
    /// 拷贝数据表回调
    /// （将在Download文件夹的.data文件复制到目标打包平台包）
    /// </summary>
    private void OnCopyDataTableCallBack()
    {
        //拷贝数据获取路径
        string fromPath = Application.dataPath + "/Download/DataTable";
        //拷贝数据放置路径
        string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex] + "/Download/DataTable";
        //调用拷贝方法
        IOUtil.CopyDirectory(fromPath, toPath);
        Debug.Log("拷贝完成,目标路径是：" + toPath);
    }
    #endregion

    #region 生成版本文件回调 OnCreateVersionFileCallBack
    /// <summary>
    /// 生成版本文件回调
    /// 格式：
    /// 第一第二行：平台和平台的认证手段
    /// 其他；
    /// 第一段：资源所在的编辑器位置 
    /// 第二段：资源的MD5加密形态
    /// 第三段：资源大小
    /// 第四段：资源是否是初始数据
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnCreateVersionFileCallBack()
    {
        //指定平台路径
        string path = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        //版本文件路径
        string strVersionFilePath = path + "/VersionFile.txt";
        //若当前版本文件已存在，则删除
        IOUtil.DeleteFile(strVersionFilePath);
        //文件内容
        StringBuilder sbContent = new StringBuilder();
        DirectoryInfo directory = new DirectoryInfo(path);
        //获取文件夹下的所有文件
        FileInfo[] arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < arrFiles.Length; i++)
        {
            FileInfo file = arrFiles[i];
            //获取文件的完整名称（路径+后缀）
            string fullName = file.FullName;
            //获取文件的平台后路径，如windows.XXX则要的是XXX
            string name = fullName.Substring(fullName.IndexOf(arrBuildTarget[buildTargetIndex]) + arrBuildTarget[buildTargetIndex].Length + 1);
            //获取文件的MD5加密形态
            string md5 = EncryptUtil.GetFileMD5(fullName);
            if (md5 == null)
            { continue; }
            //获取文件的大小（k）
            string size = Math.Ceiling(file.Length / 1024f).ToString();
            //是否是初始数据
            bool isFirstData = false;
            //是否需要更新
            bool isBreak = false;
            //从xml中获取数据列表进行数据核对，看数据是否是版本的初始信息（文件表中的信息）
            for (int j = 0; j < m_List.Count; j++)
            {
                foreach (string xmlPath in m_List[j].PathList)
                {
                    string tempPath = xmlPath;
                    if (xmlPath.IndexOf(".") != -1)
                    {
                        //截取文件信息到.之前（后缀前）
                        tempPath = xmlPath.Substring(0, xmlPath.IndexOf("."));
                    }
                    //如果文件名与xml文件完全符合，则说明当前版本未改变该系列文件
                    if (name.IndexOf(tempPath, StringComparison.CurrentCultureIgnoreCase) != -1)
                    {
                        isFirstData = m_List[j].IsFirstData;
                        isBreak = true;
                        break;
                    }
                }
                if (isBreak)
                { break; }
            }
            //判定是不是表格数据，表格数据是初始数据
            if (name.IndexOf("DataTable") != -1)
            {
                isFirstData = true;
            }
            //进行数据写入工作
            string strLine = string.Format("{0} {1} {2} {3}", name, md5, size, isFirstData ? 1 : 0);
            sbContent.AppendLine(strLine);
        }
        //创建TXT版本文件
        IOUtil.CreateTextFile(strVersionFilePath, sbContent.ToString());
        Debug.Log("版本文件创建成功，结果路径是：" + strVersionFilePath);
    }
    #endregion

    /// <summary>
    /// 打包方法
    /// </summary>
    /// <param name="entity"></param>
    private void BuildAssetBundle(AssetBundleEntity entity)
    {
        ////AssetBundleBuild:UnityEditor的类
        //AssetBundleBuild[] arrBuild = new AssetBundleBuild[1];
        //AssetBundleBuild build = new AssetBundleBuild();
        ////包名
        //build.assetBundleName = string.Format("{0}.{1}",entity.Name, (entity.Tag.Equals("Scene", StringComparison.CurrentCultureIgnoreCase) ? "unity3d" : "assetbundle"));
        ////资源路径
        //build.assetNames = entity.PathList.ToArray();
        //arrBuild[0] = build;
        ////存储路径
        //string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex] + entity.ToPath;
        ////若文件夹不存在，则新建文件夹
        //if (!System.IO.Directory.Exists(toPath))
        //{
        //    Directory.CreateDirectory(toPath);
        //}
        ////开始打包
        ////(目标路径，资源路径，打包手段（没清楚），打包目标平台)
        //BuildPipeline.BuildAssetBundles(toPath,arrBuild,BuildAssetBundleOptions.None,target);
    }
}
