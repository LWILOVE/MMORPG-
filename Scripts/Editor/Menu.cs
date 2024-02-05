using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Menu
{
    /// <summary>
    /// ���趨��ť
    /// </summary>
    [MenuItem("Violet/Settings")]
    public static void Setting()
    {
        //�趨һ�����ڷ�ʽ1�������ǿת
        //���������ڶ�Ӧ����
        SettingsWindow win = (SettingsWindow)EditorWindow.GetWindow(typeof(SettingsWindow));
        //���ô��ڱ���
        win.titleContent = new GUIContent("ȫ������");
        //չʾ����
        win.Show();
    }
    /// <summary>
    /// ��������ļ��а�ť
    /// </summary>
    [MenuItem("Violet/AssetBundleCreate")]
    public static void AssetBundleCreate()
    {
        //�趨һ�����ڷ�ʽ2������ǿת
        //���������ڶ�Ӧ����
        AssetBundleWindow win = EditorWindow.GetWindow<AssetBundleWindow>();

        win.titleContent = new GUIContent("��Դ���");

        win.Show();
    }

    /// <summary>
    /// �����������ð�ť:û����
    /// </summary>
    [MenuItem("Violet/LanguageSetting")]
    public static void LanguageSetting()
    { 
        
    }

    /// <summary>
    /// ����ʼ��Դ������StreamingAssets 
    /// </summary>
    [MenuItem("Violet/CopyToStreamingAssets")]
    public static void AssetBundleCopyToStreamingAssets()
    {
        //Ҫ���õ�·��
        string toPath = Application.streamingAssetsPath + "/AssetBundles/";
        //���ļ����Ѵ��ڣ���ɾ�������·���
        if (Directory.Exists(toPath))
        {
            Directory.Delete(toPath, true);
        }
        Directory.CreateDirectory(toPath);

        //���ļ���������
        IOUtil.CopyDirectory(Application.persistentDataPath,toPath);
        //ˢ���ļ�
        AssetDatabase.Refresh();
        Debug.Log("�������");
    }

}
