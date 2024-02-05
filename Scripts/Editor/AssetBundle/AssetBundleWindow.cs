    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;

/// <summary>
/// AssetBundle������
/// </summary>
public class AssetBundleWindow : EditorWindow
{
    #region ����
    /// <summary>
    /// ��������λ��
    /// </summary>
    private Vector2 pos;
    /// <summary>
    /// xml·������
    /// </summary>
    private AssetBundleDAL dal;
    /// <summary>
    /// xml�����б�
    /// </summary>
    private List<AssetBundleEntity> m_List;
    /// <summary>
    /// ��������ֵ�
    /// <�������,�Ƿ���>
    /// </summary>
    private Dictionary<string, bool> m_Dic;
    /// <summary>
    /// ���ݱ������
    /// </summary>
    private string[] arrTag = { "All", "Scene", "Role", "Effect", "Audio","UI" ,"None" };
    /// <summary>
    /// ��ǰѡ�е��������
    /// </summary>
    private int tagIndex = 0;
    /// <summary>
    /// ѡ�еı�ǵ���������������ֻҪ�����б����һ��ѡ��Ҫ������ļ��Ĺ��ܣ�
    /// </summary>
    private int selectTagIndex = -1;
    /// <summary>
    /// ���ƽ̨����
    /// </summary>
    private string[] arrBuildTarget = { "Windows", "Android", "iOS" };
    /// <summary>
    /// ѡ�еĴ��ƽ̨����
    /// </summary>
    private int selectBuildTargetIndex = -1;
#if UNITY_STANDALONE_WIN
    /// <summary>
    /// �����Ŀ��ƽ̨
    /// </summary>
    private BuildTarget target = BuildTarget.StandaloneWindows;
    /// <summary>
    /// ���Ŀ��ƽ̨���±�
    /// </summary>
    private int buildTargetIndex = 0;
#elif UNITY_ANDROID
    /// <summary>
    /// �����Ŀ��ƽ̨
    /// </summary>
    private BuildTarget target = BuildTarget.Android;
    /// <summary>
    /// ���Ŀ��ƽ̨���±�
    /// </summary>
    private int buildTargetIndex = 1;
#elif UNITY_IPHONE
    /// <summary>
    /// �����Ŀ��ƽ̨
    /// </summary>
    private BuildTarget target = BuildTarget.iOS;
    /// <summary>
    /// ���Ŀ��ƽ̨���±�
    /// </summary>
    private int buildTargetIndex = 2;
#endif
    #endregion
    private void OnEnable()
    {
        ///////////////////////���ô�������ֵ�
        //��ȡXML�ļ�
        string xmlPath = Application.dataPath + @"\Scripts\Editor\AssetBundle\AssetBundleConfig.xml";
        dal = new AssetBundleDAL(xmlPath);
        //��ȡXML����
        m_List = dal.GetList();
        m_Dic = new Dictionary<string, bool>();
        //��Ҫ������������ͽ��ֵ�  
        for (int i = 0; i < m_List.Count; i++)
        {
            m_Dic[m_List[i].Key] = true;
        }
        ///////////////////////���ô�������ֵ�
    }

    /// <summary>
    /// ���ƴ���
    /// </summary>
    private void OnGUI()
    {
        //���û����Ҫ����Ķ�������ô�Ͳ���Ҫ��ɻ�
        if (m_List == null)
        {return;}
        #region GUI���ܰ�ť
        GUILayout.BeginHorizontal("box");
        //EditorGUILayout.Popup:�������ܣ������б�ÿ��ռ��һС��ռ�
        //(Ĭ��ѡ����,�����б�����,�б���)
        selectTagIndex = EditorGUILayout.Popup(tagIndex, arrTag, GUILayout.Width(98.5f));
        //����ֻҪѡ��������ѡ��ʱ�ͻ��Զ���ѡ����ָ�������µ���Ʒ�Ĺ���
        if (selectTagIndex != tagIndex)
        {
            tagIndex = selectTagIndex;
            EditorApplication.delayCall = OnSelectTagCallBack;
        }
        ////һ�������Ĺ��ܣ�����ѡ�е�ǰTagָ����������Ʒ
        //if (GUILayout.Button("ѡ��Tag", GUILayout.Width(98.5f)))
        //{
        //    EditorApplication.delayCall = OnSelectTagCallBack;
        //}
        //��ȡѡ�еĴ��ƽ̨
        selectBuildTargetIndex = EditorGUILayout.Popup(buildTargetIndex, arrBuildTarget, GUILayout.Width(98.5f));
        //���´��ƽ̨����
        if (selectBuildTargetIndex != buildTargetIndex)
        {
            buildTargetIndex = selectBuildTargetIndex;
            EditorApplication.delayCall = OnSelectTargetCallBack;
        }
        //if (GUILayout.Button("ѡ��Target", GUILayout.Width(98.5f)))
        //{
        //    EditorApplication.delayCall = OnSelectTargetCallBack;
        //}
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal("box");
        if (GUILayout.Button("��������", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnSaveAssetBundleCallBack;
        }
        if (GUILayout.Button("��AssetBundle��", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnAssetBundleCallBack;
        }
        if (GUILayout.Button("���AssetBundle��", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnClearAssetBundleCallBack;
        }
        if (GUILayout.Button("�������ݱ�", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnCopyDataTableCallBack;
        }
        if (GUILayout.Button("���ɰ汾�ļ�", GUILayout.Width(200)))
        {
            EditorApplication.delayCall = OnCreateVersionFileCallBack;
        }
        ////��������δ���õĲ��ֲ��Ͽ�
        //EditorGUILayout.Space();
        ////�趨ռ�ݿռ�Ŀ�
        ///(ռ�ݿռ��С)
        //GUILayout.Space(10);
        GUILayout.EndHorizontal();
        #endregion

        #region GUI�ļ���Ϣ��ʾ����
        GUILayout.BeginHorizontal("box");
        //GUILayout.Label:�������ܣ�����
        //(�����������)
        GUILayout.Space(25);
        GUILayout.Label("����"); GUILayout.Label("���",GUILayout.Width(75));   GUILayout.Label("�ļ���", GUILayout.Width(75)); GUILayout.Label("��ʼ��Դ", GUILayout.Width(75)); GUILayout.EndHorizontal();
        //��GUI�����������µ�����д
        GUILayout.BeginVertical();
        //��GUI�����п���������ͼ���о�Ч�������ԣ�
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
                GUILayout.Label("ȫ·����" + path);
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
        #endregion
    }

    #region ѡ����ǩ OnSelectTagCallBack
    /// <summary>
    /// ��ǩѡ���ص�
    /// (ѡ��������Ҫ������ļ�)
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnSelectTagCallBack()
    {
        //����ѡ���趨����
        switch (tagIndex)
        {
            case 0://ȫѡ
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = true;
                }
                break;
            case 1://����
                foreach (AssetBundleEntity entity in m_List)
                {
                    //entity.Tag.Equals("Scene",StringComparison.CurrentCultureIgnoreCase)���ж���ֻ�е�XXX������Ϊ������1��ʱ�ŷ���true
                    m_Dic[entity.Key] = entity.Tag.Equals("Scene", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 2://��ɫ
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = entity.Tag.Equals("Role", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 3://��Ч
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = entity.Tag.Equals("Effect", StringComparison.CurrentCultureIgnoreCase);
                }
                break;
            case 4://����
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
            case 6://��
                foreach (AssetBundleEntity entity in m_List)
                {
                    m_Dic[entity.Key] = false;
                }
                break;

        }
        Debug.LogFormat("��ǰѡ�е�TagΪ��{0}", arrTag[tagIndex]);
    }
    #endregion

    #region ѡ��ƽ̨ OnSelectTargetCallBack
    /// <summary>
    /// Ŀ��ƽ̨ѡ���ص�
    /// ѡ��Ҫ�������Ŀ��ƽ̨
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
        Debug.LogFormat("��ǰѡ�еĴ��ƽ̨Ϊ��{0}", arrBuildTarget[buildTargetIndex]);
    }
    #endregion

    #region �������� OnSaveAssetBundleCallBack SaveFolderSettings SaveFileSetting
    /// <summary>
    /// �������ûص�
    /// ���ð��ļ��ı���·��
    /// </summary>
    private void OnSaveAssetBundleCallBack()
    {
        //��Ҫ����Ķ�������������DAL����õ����������һ�ָ�ʽ���ѣ�
        List<AssetBundleEntity> listNeedBuild = new List<AssetBundleEntity>();
        //����Ҫ��������ݼ����б�
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
        //�����ļ��������ļ��е���İ���
        for (int i = 0; i < listNeedBuild.Count; i++)
        {
            //��ȡһ���ڵ�
            AssetBundleEntity entity = listNeedBuild[i];
            if (entity.IsFolder)
            {
                //���ýڵ����ļ��У���Ӧ�ñ����������ݣ������������
                //��������·����ȫ·����
                string[] folderArr = new string[entity.PathList.Count];
                for (int j = 0; j < entity.PathList.Count; j++)
                {
                    folderArr[j] = Application.dataPath + "/" + entity.PathList[j];
                }
                SaveFolderSettings(folderArr, !entity.IsChecked);
            }
            else
            {
                //���ý������ͨ���壬�Ǿ�ֻ��Ҫ���ø���������Լ���
                //��������·����ȫ·����
                string[] folderArr = new string[entity.PathList.Count];
                for (int j = 0; j < entity.PathList.Count; j++)
                {
                    folderArr[j] = Application.dataPath + "/" + entity.PathList[j];
                    SaveFileSetting(folderArr[j], !entity.IsChecked);
                }
            }
        }
        Debug.Log("·���������");
    }

    /// <summary>
    /// ����Ŀ���ļ����������ļ���Unity���·��
    /// </summary>
    /// <param name="folderArr">�ļ��еľ���·��</param>
    /// <param name="isSetNull">�Ƿ�����Ϊ��</param>
    private void SaveFolderSettings(string[] folderArr, bool isSetNull)
    {
        foreach (string folderPath in folderArr)
        {
            Debug.Log(folderPath);
            //��ȡ�ļ����µ��ļ�
            string[] arrFile = Directory.GetFiles(folderPath);
            //���ļ���������
            foreach (string filePath in arrFile)
            {
                //��������
                SaveFileSetting(filePath, isSetNull);
            }
            //�����ļ����µ����ļ���
            string[] arrFolder = Directory.GetDirectories(folderPath);
            SaveFolderSettings(arrFolder, isSetNull);
        }
    }

    /// <summary>
    /// ����Ŀ���ļ���Unity���·��
    /// </summary>
    /// <param name="filePath">Ŀ���ļ��ľ���·����ȫ·����</param>
    /// <param name="isSetNull">�Ƿ�Ϊ��</param>
    private void SaveFileSetting(string filePath, bool isSetNull)
    {
        //��ȡ�ļ���Ϣ
        FileInfo file = new FileInfo(filePath);
        //�����ļ���׺���ж�������Դ��Ϣ�洢�ļ�
        if (!file.Extension.Equals(".meta", StringComparison.CurrentCultureIgnoreCase))
        {
            //��ȡ�ļ��Ķ�·����xml��ȥ����׺��·����
            int index = filePath.IndexOf("Assets/", StringComparison.CurrentCultureIgnoreCase);
            string newPath = filePath.Substring(index);
            //ȥ���ļ��Ĳ���Ҫ��׺��ǰ������ȡ���趨��
            string fileName = newPath.Replace("Assets/", "").Replace(file.Extension, "");

            //�����ļ���׺��������׺����Ϊunity3d,����Ϊassetbundle��
            string variant = file.Extension.Equals(".unity", StringComparison.CurrentCultureIgnoreCase) ?
                "unity3d" : "assetbundle";
            //��Unity�����ô��·��
            //(Ҫ���ô���ļ����ļ�·��)
            AssetImporter import = AssetImporter.GetAtPath(newPath);
            //�������������չ��
            import.SetAssetBundleNameAndVariant(fileName, variant);
            //����Ҫ����Ϊ��
            if (isSetNull)
            {
                import.SetAssetBundleNameAndVariant(null, null);
            }
            //�����úõ���Ϣ���б���
            import.SaveAndReimport();
        }
    }
    #endregion

    #region ���д�� OnAssetBundleCallBack
    /// <summary>
    /// ����ص�(ͨ�������ļ����д��)
    /// ��������Ŀ��·�������д����
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnAssetBundleCallBack()
    {
        //������Ŀ¼
        string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        Debug.Log("��Ŀ¼");
        if (!Directory.Exists(toPath))
        {
            Directory.CreateDirectory(toPath);
        }

        //���ô������
        BuildPipeline.BuildAssetBundles(toPath, BuildAssetBundleOptions.None, target);
        Debug.Log("������");
    }
    #endregion

    #region ��մ�� OnClearAssetBundleCallBack
    /// <summary>
    /// ���AssetBundle���ص�
    /// ��ɾ��ָ��ƽ̨���ļ��У�
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnClearAssetBundleCallBack()
    {
        //�����·��
        string path = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        //�����ڣ���ɾ��
        if (Directory.Exists(path))
        {
            //�ļ���ɾ��
            //(·�����Ƿ�ɾ����Ŀ¼)
            Directory.Delete(path,true);
        }
        Debug.Log("�����");
    }
    #endregion

    #region ���ݸ��� OnCopyDataTableCallBack
    /// <summary>
    /// �������ݱ�ص�
    /// ������Download�ļ��е�.data�ļ����Ƶ�Ŀ����ƽ̨����
    /// </summary>
    private void OnCopyDataTableCallBack()
    {
        //�������ݻ�ȡ·��
        string fromPath = Application.dataPath + "/Download/DataTable";
        //�������ݷ���·��
        string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex] + "/Download/DataTable";
        //���ÿ�������
        IOUtil.CopyDirectory(fromPath, toPath);
        Debug.Log("�������,Ŀ��·���ǣ�" + toPath);
    }
    #endregion

    #region ���ɰ汾�ļ��ص� OnCreateVersionFileCallBack
    /// <summary>
    /// ���ɰ汾�ļ��ص�
    /// ��ʽ��
    /// ��һ�ڶ��У�ƽ̨��ƽ̨����֤�ֶ�
    /// ������
    /// ��һ�Σ���Դ���ڵı༭��λ�� 
    /// �ڶ��Σ���Դ��MD5������̬
    /// �����Σ���Դ��С
    /// ���ĶΣ���Դ�Ƿ��ǳ�ʼ����
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnCreateVersionFileCallBack()
    {
        //ָ��ƽ̨·��
        string path = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex];
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        //�汾�ļ�·��
        string strVersionFilePath = path + "/VersionFile.txt";
        //����ǰ�汾�ļ��Ѵ��ڣ���ɾ��
        IOUtil.DeleteFile(strVersionFilePath);
        //�ļ�����
        StringBuilder sbContent = new StringBuilder();
        DirectoryInfo directory = new DirectoryInfo(path);
        //��ȡ�ļ����µ������ļ�
        FileInfo[] arrFiles = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < arrFiles.Length; i++)
        {
            FileInfo file = arrFiles[i];
            //��ȡ�ļ����������ƣ�·��+��׺��
            string fullName = file.FullName;
            //��ȡ�ļ���ƽ̨��·������windows.XXX��Ҫ����XXX
            string name = fullName.Substring(fullName.IndexOf(arrBuildTarget[buildTargetIndex]) + arrBuildTarget[buildTargetIndex].Length + 1);
            //��ȡ�ļ���MD5������̬
            string md5 = EncryptUtil.GetFileMD5(fullName);
            if (md5 == null)
            { continue; }
            //��ȡ�ļ��Ĵ�С��k��
            string size = Math.Ceiling(file.Length / 1024f).ToString();
            //�Ƿ��ǳ�ʼ����
            bool isFirstData = false;
            //�Ƿ���Ҫ����
            bool isBreak = false;
            //��xml�л�ȡ�����б�������ݺ˶ԣ��������Ƿ��ǰ汾�ĳ�ʼ��Ϣ���ļ����е���Ϣ��
            for (int j = 0; j < m_List.Count; j++)
            {
                foreach (string xmlPath in m_List[j].PathList)
                {
                    string tempPath = xmlPath;
                    if (xmlPath.IndexOf(".") != -1)
                    {
                        //��ȡ�ļ���Ϣ��.֮ǰ����׺ǰ��
                        tempPath = xmlPath.Substring(0, xmlPath.IndexOf("."));
                    }
                    //����ļ�����xml�ļ���ȫ���ϣ���˵����ǰ�汾δ�ı��ϵ���ļ�
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
            //�ж��ǲ��Ǳ�����ݣ���������ǳ�ʼ����
            if (name.IndexOf("DataTable") != -1)
            {
                isFirstData = true;
            }
            //��������д�빤��
            string strLine = string.Format("{0} {1} {2} {3}", name, md5, size, isFirstData ? 1 : 0);
            sbContent.AppendLine(strLine);
        }
        //����TXT�汾�ļ�
        IOUtil.CreateTextFile(strVersionFilePath, sbContent.ToString());
        Debug.Log("�汾�ļ������ɹ������·���ǣ�" + strVersionFilePath);
    }
    #endregion

    /// <summary>
    /// �������
    /// </summary>
    /// <param name="entity"></param>
    private void BuildAssetBundle(AssetBundleEntity entity)
    {
        ////AssetBundleBuild:UnityEditor����
        //AssetBundleBuild[] arrBuild = new AssetBundleBuild[1];
        //AssetBundleBuild build = new AssetBundleBuild();
        ////����
        //build.assetBundleName = string.Format("{0}.{1}",entity.Name, (entity.Tag.Equals("Scene", StringComparison.CurrentCultureIgnoreCase) ? "unity3d" : "assetbundle"));
        ////��Դ·��
        //build.assetNames = entity.PathList.ToArray();
        //arrBuild[0] = build;
        ////�洢·��
        //string toPath = Application.dataPath + "/../AssetBundles/" + arrBuildTarget[buildTargetIndex] + entity.ToPath;
        ////���ļ��в����ڣ����½��ļ���
        //if (!System.IO.Directory.Exists(toPath))
        //{
        //    Directory.CreateDirectory(toPath);
        //}
        ////��ʼ���
        ////(Ŀ��·������Դ·��������ֶΣ�û����������Ŀ��ƽ̨)
        //BuildPipeline.BuildAssetBundles(toPath,arrBuild,BuildAssetBundleOptions.None,target);
    }
}
