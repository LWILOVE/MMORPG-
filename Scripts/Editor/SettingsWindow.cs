using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;

public class SettingsWindow : EditorWindow
{
    /// <summary>
    /// 宏列表
    /// </summary>
    private List<MacorItem> m_List = new List<MacorItem>();
    /// <summary>
    /// 对应宏是否开启
    /// <宏名,宏状态>:当在当前操作系统宏是开启状态时，宏状态为1，否则为-1
    /// </summary>
    private Dictionary<string, bool> m_Dic = new Dictionary<string, bool>();
    /// <summary>
    /// 当前系统宏设定表
    /// </summary>
    private string m_Macor = null;
    
    /// <summary>
    /// 当窗口开启OR重载时调用
    /// </summary>
    private void OnEnable()
    {
        //获取指定平台的宏设定表
        //参数：对应平台
        //注：原先此处使用的PlayerSettings.GetScriptingDefineSymbolsForGroup已经被废弃，现在使用的是GetScriptingDefineSymbols这个
        //两者的使用参数，方法也有所不同，原先该功能可以在任意处调用，但是此时该功能仅可在OnEnable中调用
        //NamedBuildTarget:目标平台名，包含多种平台，用请自查，命名空间是：UnityEditor.Build
        m_Macor = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android);
        //Debug.Log("当前操作系统的宏设定表如下：" + m_Macor);
        m_List.Clear();
        m_List.Add(new MacorItem() { Name = "DEBUG_MODEL", DisplayName = "调试模式", IsDebug = true, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "DEBUG_LOG", DisplayName = "打印日志", IsDebug = true, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "START_TD", DisplayName = "开启统计", IsDebug = false, IsRelease = true });
        m_List.Add(new MacorItem() { Name = "DEBUG_ROLESTATE", DisplayName = "调试角色状态", IsDebug = false, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "DISABLE_ASSETBUNDLE", DisplayName = "禁用打包文件", IsDebug = false, IsRelease = false });
        m_List.Add(new MacorItem() { Name = "HOTFIX_ENABLE", DisplayName = "开启热补丁", IsDebug = false, IsRelease = true });

        //根据当前平台的宏情况决定已知宏的勾选情况
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
    /// OnGUI:要显示在窗口上的内容
    /// </summary>
    private void OnGUI()
    {
        for (int i = 0; i < m_List.Count; i++)
        {
            //在GUI窗口中启动新的行读写（GUI的模式默认是竖模式，即每个操作功能占用一行（自动换行），开启该模式后，若操作功能占据不满1行，则会放到同一行）
            //参数1：本次行读写模式：box：盒式模式，即在本次行读写的所有内容都用盒子框起来
            EditorGUILayout.BeginHorizontal("box");
            // GUILayout.Toggle：操作功能：选择按钮：每个默认占用空间1行
            //（默认状态，项名）
            m_Dic[m_List[i].Name] = GUILayout.Toggle(m_Dic[m_List[i].Name], m_List[i].DisplayName);
            //结束窗口的本次行的读写
            EditorGUILayout.EndHorizontal();
        }
        //启用行读写模式，让下面的几个设定模式到一块去
        EditorGUILayout.BeginHorizontal("box");
        //GUILayout.Button：操作功能：按钮
        //（按键文本，按键宽度）
        if (GUILayout.Button("宏设定保存", GUILayout.Width(100)))
        {
            SaveMacor();
        }
        if (GUILayout.Button("宏设定调试", GUILayout.Width(100)))
        {
            for (int i = 0; i < m_List.Count; i++)
            {
                m_Dic[m_List[i].Name] = m_List[i].IsDebug;
            }
            SaveMacor();
        }
        if (GUILayout.Button("宏设定发布", GUILayout.Width(100)))
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
                //如果禁用打包。则让download下的场景生效
                //获取再加载场景界面的场景，并将他们激活
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
        Debug.Log("本次设定的宏如下：" + m_Macor);
        //为指定平台添加宏设定
        //(目标平台，待添加宏字段)：若要添加多个宏，则宏字段为字段A;字段B;字段C;
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Android, m_Macor);
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, m_Macor);
        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, m_Macor);
        Debug.Log("宏应用成功");
    }

    /// <summary>
    /// 宏项目
    /// </summary>
    public class MacorItem
    {
        /// <summary>
        /// 宏名
        /// </summary>
        public string Name;
        /// <summary>
        /// 宏中文名
        /// </summary>
        public string DisplayName;
        /// <summary>
        /// 是否调试项
        /// </summary>
        public bool IsDebug;
        /// <summary>
        /// 是否发布项
        /// </summary>
        public bool IsRelease;
    }
}
