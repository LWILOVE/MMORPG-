using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 神奇教程的角色控制管理器
/// </summary>
public class RoleMgr : SingletonMiddle<RoleMgr>
{
    /// <summary>
    /// 主角是否初始化完成
    /// </summary>
    private bool m_IsMainPlayerInit = false;
    /// <summary>
    /// 主角初始化
    /// </summary>
    public void InitMainPlayer()
    {
        if (m_IsMainPlayerInit)
        {
            return;
        }

        if (GlobalInit.Instance.MainPlayerInfo != null)
        {
            Debug.Log("将要加载的职业编号是："+GlobalInit.Instance.MainPlayerInfo.JobId);
            //根据职业ID克隆主角
            GameObject mainPlayerObj = Object.Instantiate(GlobalInit.Instance.JobObjectDic[GlobalInit.Instance.MainPlayerInfo.JobId], new Vector3(50, 0, 50),Quaternion.identity);
            //主角将永存于各种加载之中
            Object.DontDestroyOnLoad(mainPlayerObj);
            GlobalInit.Instance.MainPlayerInfo.SetPhySkillId(JobDBModel.Instance.Get(GlobalInit.Instance.MainPlayerInfo.JobId).UsedPhyAttackIds);
            //获取角色身上的控制器,并给他赋初值
            GlobalInit.Instance.currentPlayer = mainPlayerObj.GetComponent<RoleCtrl>();
            //初始化角色信息
            GlobalInit.Instance.currentPlayer.Init(RoleType.MainPlayer, GlobalInit.Instance.MainPlayerInfo, new RoleMainPlayerCityAI(GlobalInit.Instance.currentPlayer));
        }
        m_IsMainPlayerInit = true;
    }

    /// <summary>
    /// 角色镜像字典
    /// </summary>
    private Dictionary<string, GameObject> m_Role = new Dictionary<string, GameObject>();

    /// <summary>
    /// 根据玩家职业编号加载角色模型
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    public GameObject LoadPlayerModel(int jobId)
    {
        GameObject obj = GlobalInit.Instance.JobObjectDic[jobId];
        return Object.Instantiate(obj);
    }

    ///// <summary>
    ///// 角色加载方法
    ///// </summary>
    ///// <param name="name">角色名</param>
    ///// <returns></returns>
    //public GameObject LoadRole(string name)
    //{
    //    GameObject obj = null;
    //    //如果角色在字典中，则直接等于，否则进行加载
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
    //        //角色加载
    //        obj = ResourceMgr.Instance.Load(ResourcesType.Role, string.Format(@"Biology/{0}", name), cache: true);
    //        //将角色名加入字典
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
