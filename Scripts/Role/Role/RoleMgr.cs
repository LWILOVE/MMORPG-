using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����̵̳Ľ�ɫ���ƹ�����
/// </summary>
public class RoleMgr : SingletonMiddle<RoleMgr>
{
    /// <summary>
    /// �����Ƿ��ʼ�����
    /// </summary>
    private bool m_IsMainPlayerInit = false;
    /// <summary>
    /// ���ǳ�ʼ��
    /// </summary>
    public void InitMainPlayer()
    {
        if (m_IsMainPlayerInit)
        {
            return;
        }

        if (GlobalInit.Instance.MainPlayerInfo != null)
        {
            Debug.Log("��Ҫ���ص�ְҵ����ǣ�"+GlobalInit.Instance.MainPlayerInfo.JobId);
            //����ְҵID��¡����
            GameObject mainPlayerObj = Object.Instantiate(GlobalInit.Instance.JobObjectDic[GlobalInit.Instance.MainPlayerInfo.JobId], new Vector3(50, 0, 50),Quaternion.identity);
            //���ǽ������ڸ��ּ���֮��
            Object.DontDestroyOnLoad(mainPlayerObj);
            GlobalInit.Instance.MainPlayerInfo.SetPhySkillId(JobDBModel.Instance.Get(GlobalInit.Instance.MainPlayerInfo.JobId).UsedPhyAttackIds);
            //��ȡ��ɫ���ϵĿ�����,����������ֵ
            GlobalInit.Instance.currentPlayer = mainPlayerObj.GetComponent<RoleCtrl>();
            //��ʼ����ɫ��Ϣ
            GlobalInit.Instance.currentPlayer.Init(RoleType.MainPlayer, GlobalInit.Instance.MainPlayerInfo, new RoleMainPlayerCityAI(GlobalInit.Instance.currentPlayer));
        }
        m_IsMainPlayerInit = true;
    }

    /// <summary>
    /// ��ɫ�����ֵ�
    /// </summary>
    private Dictionary<string, GameObject> m_Role = new Dictionary<string, GameObject>();

    /// <summary>
    /// �������ְҵ��ż��ؽ�ɫģ��
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public GameObject LoadPlayerModel(int jobId)
    {
        GameObject obj = GlobalInit.Instance.JobObjectDic[jobId];
        return Object.Instantiate(obj);
    }

    ///// <summary>
    ///// ��ɫ���ط���
    ///// </summary>
    ///// <param name="name">��ɫ��</param>
    ///// <returns></returns>
    //public GameObject LoadRole(string name)
    //{
    //    GameObject obj = null;
    //    //�����ɫ���ֵ��У���ֱ�ӵ��ڣ�������м���
    //    if (m_Role.ContainsKey(name))
    //    {
    //        obj = m_Role[name];
    //        if (obj == null)
    //        {
    //            m_Role.Remove(name);
    //        }
    //        else
    //        {
    //            return GameObject.Instantiate(obj);
    //        }
    //    }
    //    else
    //    {
    //        //��ɫ����
    //        obj = ResourceMgr.Instance.Load(ResourcesType.Role, string.Format(@"Biology/{0}", name), cache: true);
    //        //����ɫ�������ֵ�
    //        m_Role.Add(name, obj);
    //    }
    //    return obj;
    //}
    public override void Dispose()
    {
        base.Dispose();
        m_Role.Clear();
    }




    
}
