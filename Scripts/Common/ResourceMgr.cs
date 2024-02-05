using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ResourceMgr : SingletonMiddle<ResourceMgr>
{
    /// <summary>
    /// Ԥ��Ĺ�ϣ��
    /// </summary>
    private Hashtable m_PrefabTable;

    public ResourceMgr()
    {
        m_PrefabTable = new Hashtable();
    }


    [System.Obsolete]
    /// <summary>
    /// ������Դ(������)
    /// </summary>
    /// <param name="type">��Դ����</param>
    /// <param name="path">��·��</param>
    /// <param name="cache">�Ƿ���뻺����</param>
    ///  <param name="retClone">�Ƿ񷵻ؿ�¡��</param>
    /// <returns>Ԥ���¡��</returns>
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
        ///�����Ƿ���Ҫ���¿�¡������ϣ����
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
    /// �ͷ���Դ   
    /// </summary>
    public override void Dispose()
    {
        base.Dispose();
        m_PrefabTable.Clear();

        //�ͷ�δʹ�õ���Դ
        Resources.UnloadUnusedAssets();
    }
}
