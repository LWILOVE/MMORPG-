using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Menu
{
    /// <summary>
    /// 宏设定按钮
    /// </summary>
    [MenuItem("Violet/Settings")]
    public static void Setting()
    {
        //设定一个窗口方式1：需进行强转
        //参数：窗口对应的类
        SettingsWindow win = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow));
        //设置窗口标题
        win.titleContent = new GUIContent("全局设置");
        //展示窗口
        win.Show();
    }
    /// <summary>
    /// 创建打包文件夹按钮
    /// </summary>
    [MenuItem("Violet/AssetBundleCreate")]
    public static void AssetBundleCreate()
    {
        //设定一个窗口方式2：无需强转
        //参数：窗口对应的类
        AssetBundleWindow win = EditorWindow.GetWindow<AssetBundleWindow>();

        win.titleContent = new GUIContent("资源打包");

        win.Show();
    }

    /// <summary>
    /// 创建语言设置按钮:没开发
    /// </summary>
    [MenuItem("Violet/LanguageSetting")]
    public static void LanguageSetting()
    { 
        
    }

    /// <summary>
    /// 将初始资源拷贝到StreamingAssets 
    /// </summary>
    [MenuItem("Violet/CopyToStreamingAssets")]
    public static void AssetBundleCopyToStreamingAssets()
    {
        //要放置的路径
        string toPath = Application.streamingAssetsPath + "/AssetBundles/";
        //若文件夹已存在，则删除再重新放置
        if (Directory.Exists(toPath))
        {
            Directory.Delete(toPath, true);
        }
        Directory.CreateDirectory(toPath);

        //将文件拷贝下来
        IOUtil.CopyDirectory(Application.persistentDataPath,toPath);
        //刷新文件
        AssetDatabase.Refresh();
        Debug.Log("拷贝完成");
    }

}
