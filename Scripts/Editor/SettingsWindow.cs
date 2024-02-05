using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

public class SettingsWindow : EditorWindow
{
    /// <summary>
    /// ���б�
    /// </summary>
    private List<MacorItem> m_List = new List<MacorItem>();
    /// <summary>
    /// ��Ӧ���Ƿ���
    /// <����,��״̬>:���ڵ�ǰ����ϵͳ���ǿ���״̬ʱ����״̬Ϊ1������Ϊ-1
    /// </summary>
    private Dictionary<string, bool> m_Dic = new Dictionary<string, bool>();
    /// <summary>
    /// ��ǰϵͳ���趨��
    /// </summary>
    private string m_Macor = null;
    
    /// <summary>
    /// �����ڿ���OR����ʱ����
    /// </summary>
    private void OnEnable()
    {
        //��ȡָ��ƽ̨�ĺ��趨��
        //��������Ӧƽ̨
        //ע��ԭ�ȴ˴�ʹ�õ�PlayerSettings.GetScriptingDefineSymbolsForGroup�Ѿ�������������ʹ�õ���GetScriptingDefineSymbols���
        //���ߵ�ʹ�ò���������Ҳ������ͬ��ԭ�ȸù��ܿ��������⴦���ã����Ǵ�ʱ�ù��ܽ�����OnEnable�е���
        //NamedBuildTarget:Ŀ��ƽ̨������������ƽ̨�������Բ飬�����ռ��ǣ�UnityEditor.Build
        m_Macor = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);
        //Debug.Log("��ǰ����ϵͳ�ĺ��趨�����£�" + m_Macor);
        m_List.Clear();
        m_List.Add(new MacorItem() { Name = "DEBUG_MODEL", DisplayName = "����ģʽ", IsDebug = true, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "DEBUG_LOG", DisplayName = "��ӡ��־", IsDebug = true, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "START_TD", DisplayName = "����ͳ��", IsDebug = false, IsRelease = true });
        m_List.Add(new MacorItem() { Name = "DEBUG_ROLESTATE", DisplayName = "���Խ�ɫ״̬", IsDebug = false, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "DISABLE_ASSETBUNDLE", DisplayName = "���ô���ļ�", IsDebug = false, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "HOTFIX_ENABLE", DisplayName = "�����Ȳ���", IsDebug = false, IsRelease = true });

        //���ݵ�ǰƽ̨�ĺ����������֪��Ĺ�ѡ���
        for (int i = 0; i < m_List.Count; i++)
        {
            if (!string.IsNullOrEmpty(m_Macor) && m_Macor.IndexOf(m_List[i].Name) != -1)
            {
                m_Dic[m_List[i].Name] = true;
            }
            else
            {
                m_Dic[m_List[i].Name] = false;
            }
        }
    }

    /// <summary>
    /// OnGUI:Ҫ��ʾ�ڴ����ϵ�����
    /// </summary>
    private void OnGUI()
    {
        for (int i = 0; i < m_List.Count; i++)
        {
            //��GUI�����������µ��ж�д��GUI��ģʽĬ������ģʽ����ÿ����������ռ��һ�У��Զ����У���������ģʽ������������ռ�ݲ���1�У����ŵ�ͬһ�У�
            //����1�������ж�дģʽ��box����ʽģʽ�����ڱ����ж�д���������ݶ��ú��ӿ�����
            EditorGUILayout.BeginHorizontal("box");
            // GUILayout.Toggle���������ܣ�ѡ��ť��ÿ��Ĭ��ռ�ÿռ�1��
            //��Ĭ��״̬��������
            m_Dic[m_List[i].Name] = GUILayout.Toggle(m_Dic[m_List[i].Name], m_List[i].DisplayName);
            //�������ڵı����еĶ�д
            EditorGUILayout.EndHorizontal();
        }
        //�����ж�дģʽ��������ļ����趨ģʽ��һ��ȥ
        EditorGUILayout.BeginHorizontal("box");
        //GUILayout.Button���������ܣ���ť
        //�������ı���������ȣ�
        if (GUILayout.Button("���趨����", GUILayout.Width(100)))
        {
            SaveMacor();
        }
        if (GUILayout.Button("���趨����", GUILayout.Width(100)))
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_Dic[m_List[i].Name] = m_List[i].IsDebug;
            }
            SaveMacor();
        }
        if (GUILayout.Button("���趨����", GUILayout.Width(100)))
        {   
            for (int i = 0; i < m_List.Count; i++)
            {
                m_Dic[m_List[i].Name] = m_List[i].IsRelease;
            }
            SaveMacor();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void SaveMacor()
    {
        m_Macor = string.Empty;
        foreach (var item in m_Dic)
        {
            if (item.Value == true)
            {
                m_Macor += string.Format("{0};", item.Key);
            }
            if (item.Key.Equals("DISABLE_ASSETBUNDLE", System.StringComparison.CurrentCultureIgnoreCase))
            {           
                //������ô��������download�µĳ�����Ч
                //��ȡ�ټ��س�������ĳ������������Ǽ���
                EditorBuildSettingsScene[] arrScene = EditorBuildSettings.scenes;
                for (int i = 0; i < arrScene.Length; i++)
                {
                    if (arrScene[i].path.IndexOf("download", System.StringComparison.CurrentCultureIgnoreCase) > -1)
                    {
                        arrScene[i].enabled = item.Value;
                    }
                }
                EditorBuildSettings.scenes = arrScene;
            }
            
        }
        Debug.Log("�����趨�ĺ����£�" + m_Macor);
        //Ϊָ��ƽ̨��Ӻ��趨
        //(Ŀ��ƽ̨������Ӻ��ֶ�)����Ҫ��Ӷ���꣬����ֶ�Ϊ�ֶ�A;�ֶ�B;�ֶ�C;
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, m_Macor);
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, m_Macor);
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, m_Macor);
        Debug.Log("��Ӧ�óɹ�");
    }

    /// <summary>
    /// ����Ŀ
    /// </summary>
    public class MacorItem
    {
        /// <summary>
        /// ����
        /// </summary>
        public string Name;
        /// <summary>
        /// ��������
        /// </summary>
        public string DisplayName;
        /// <summary>
        /// �Ƿ������
        /// </summary>
        public bool IsDebug;
        /// <summary>
        /// �Ƿ񷢲���
        /// </summary>
        public bool IsRelease;
    }
}
