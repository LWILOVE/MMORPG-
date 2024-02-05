using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC控制器
/// </summary>
public class NPCCtrl : MonoBehaviour
{
    /// <summary>
    /// 昵称挂点
    /// </summary>
    [SerializeField]
    private Transform m_HeadBarPos;

    /// <summary>
    /// 头顶UI条
    /// </summary>
    private GameObject m_HeadBar;

    /// <summary>
    /// 当前NPC实体
    /// </summary>
    private NPCEntity m_CurrNPCEntity;

    /// <summary>
    /// 当前NPCUI条
    /// </summary>
    private NPCHeaderBarView m_NPCHeaderBarView;

    /// <summary>
    /// NPC下一轮说话的时间
    /// </summary>
    private float m_NextTalkTime = 0;

    /// <summary>
    /// NPC要说的话
    /// </summary>
    private string[] m_NPCTalk;
    // Start is called before the first frame update
    void Start()
    {
        InitHeadBar();
    }

    private void Update()
    {
        if (Time.time > m_NextTalkTime)
        {
            //NPC10秒BB一次     
            m_NextTalkTime = Time.time + 10f;

            if (m_NPCHeaderBarView != null &&   m_NPCTalk.Length > 0)
            {
                m_NPCHeaderBarView.Talk(m_NPCTalk[Random.Range(0,m_NPCTalk.Length)], 5f);
            }
        }

    }

    public void Init(NPCWorldMapData npcData)
    {
        m_CurrNPCEntity = NPCDBModel.Instance.Get(npcData.NPCId);

        m_NPCTalk = m_CurrNPCEntity.Talk.Split('|');
    }

    /// <summary>
    /// 初始化NPC头顶UI条
    /// </summary>
    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (m_CurrNPCEntity == null) return;
        if (m_HeadBarPos == null) return;

        //m_HeadBar = ResourceMgr.Instance.Load(ResourcesType.Other, "NPCHeadBar");
        //加载NPC头顶UI条
        AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/RolePrefab/NPC/NPCHeadBar.assetbundle"), "NPCHeadBar",
            (GameObject obj) =>
            {
                m_HeadBar = Instantiate(obj);
                m_HeadBar.transform.SetParent(RoleHeadBarRoot.Instance.gameObject.transform);
                m_HeadBar.transform.localScale = Vector3.one;

                m_NPCHeaderBarView = m_HeadBar.GetComponent<NPCHeaderBarView>();

                m_NPCHeaderBarView.Init(m_HeadBarPos, m_CurrNPCEntity.Name);
            }, type: 0);
    }
}
