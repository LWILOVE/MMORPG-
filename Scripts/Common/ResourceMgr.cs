using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourceMgr : SingletonMiddle<ResourceMgr>
{
    /// <summary>
    /// 预设的哈希表
    /// </summary>
    private Hashtable m_PrefabTable;

    public ResourceMgr()
    {
        m_PrefabTable = new Hashtable();
    }


    [System.Obsolete]
    /// <summary>
    /// 加载资源(已弃用)
    /// </summary>
    /// <param name="type">资源类型</param>
    /// <param name="path">短路径</param>
    /// <param name="cache">是否存入缓存中</param>
    ///  <param name="retClone">是否返回克隆体</param>
    /// <returns>预设克隆体</returns>
    public GameObject Load(ResourcesType type, string path,bool cache=false,bool retClone = true)
    {
        StringBuilder sbr = new StringBuilder();
        switch (type)
        {
            case ResourcesType.UIScene:
                sbr.Append(@"Prefabs/UIScene/");
                break;
            case ResourcesType.UIWindow:
                sbr.Append(@"Prefabs/UIWindow/");
                break;
            case ResourcesType.Role:
                sbr.Append(@"Prefabs/Role/");
                break;
            case ResourcesType.Effect:
                sbr.Append(@"Prefabs/Effect/");
                break;
            case ResourcesType.Other:
                sbr.Append(@"Prefabs/Other/");
                break;
            case ResourcesType.UIWindowsChild:
                sbr.Append(@"Prefabs/UIWindowsChild/");
                break;
            case ResourcesType.Useless:
                sbr.Append(@"Prefabs/Useless/");
                break;
        }
        sbr.Append(path);
        GameObject obj = null;
        ///检验是否需要重新克隆与存入哈希表中
        if (m_PrefabTable.Contains(path))
        {

            obj = m_PrefabTable[path] as GameObject;
        }
        else
        {
            if (cache)
            {
                m_PrefabTable.Add(path,obj);
            }
        }
        obj = Resources.Load(sbr.ToString()) as GameObject;
        return retClone ? GameObject.Instantiate(obj):obj;
    }
    /// <summary>
    /// 释放资源   
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        m_PrefabTable.Clear();

        //释放未使用的资源
        Resources.UnloadUnusedAssets();
    }
}
