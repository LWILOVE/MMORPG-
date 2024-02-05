using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC������
/// </summary>
public class NPCCtrl : MonoBehaviour
{
    /// <summary>
    /// �ǳƹҵ�
    /// </summary>
    [SerializeField]
    private Transform m_HeadBarPos;

    /// <summary>
    /// ͷ��UI��
    /// </summary>
    private GameObject m_HeadBar;

    /// <summary>
    /// ��ǰNPCʵ��
    /// </summary>
    private NPCEntity m_CurrNPCEntity;

    /// <summary>
    /// ��ǰNPCUI��
    /// </summary>
    private NPCHeaderBarView m_NPCHeaderBarView;

    /// <summary>
    /// NPC��һ��˵����ʱ��
    /// </summary>
    private float m_NextTalkTime = 0;

    /// <summary>
    /// NPCҪ˵�Ļ�
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
            //NPC10��BBһ��     
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
    /// ��ʼ��NPCͷ��UI��
    /// </summary>
    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (m_CurrNPCEntity == null) return;
        if (m_HeadBarPos == null) return;

        //m_HeadBar = ResourceMgr.Instance.Load(ResourcesType.Other, "NPCHeadBar");
        //����NPCͷ��UI��
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
