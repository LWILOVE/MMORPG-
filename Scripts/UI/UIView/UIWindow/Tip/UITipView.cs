using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PathologicalGames;

public class UITipView : UISubViewBase
{
    public static UITipView Instance;

    /// <summary>
    /// 击杀收获显示队列
    /// </summary>
    private Queue<TipEntity> m_TipQueue;

    /// <summary>
    /// 上次提示时间
    /// </summary>
    private float m_PreTipTime = 0;

    /// <summary>
    /// Tip池
    /// </summary>
    private SpawnPool m_TiPPool;

    private Transform m_TipItem;
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
        m_TipQueue = new Queue<TipEntity>();
    }
    protected override void OnStart()
    {
        base.OnStart();
        //m_TipItem = ResourceMgr.Instance.LoadOrCtreat(ResourcesType.Other, "UITipItem", cache: true,retClone:false).transform;
        //克隆提示UI
        AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/UIPrefab/UIWindow/UITipItem.assetbundle"), "UITipItem",
            (GameObject obj) =>
            {
                m_TipItem = Instantiate(obj).transform;
            }, type: 0);

        m_TiPPool = PoolManager.Pools.Create("TipPool");
        m_TiPPool.group.parent = transform;
        m_TiPPool.group.localPosition = Vector3.zero;

        PrefabPool prefabPool = new PrefabPool(m_TipItem);
        prefabPool.preloadAmount = 5;

        prefabPool.cullDespawned = true;
        prefabPool.cullAbove = 10;
        prefabPool.cullDelay = 2;
        prefabPool.cullMaxPerPass = 2;
        m_TiPPool.CreatePrefabPool(prefabPool);
    }
    
    private void Update()
    {
        //显示提示
        if (Time.time > m_PreTipTime+0.5f)
        {
            m_PreTipTime = Time.time;
            if(m_TipQueue.Count >0)
            {
                //出队
                TipEntity entity = m_TipQueue.Dequeue();

                Transform trans = m_TiPPool.Spawn(m_TipItem);

                trans.GetComponent<UITipItemView>().SetUI(entity.Type,entity.Text);
                trans.SetParent(transform);
                trans.localPosition = Vector3.zero;
                trans.localScale = Vector3.one;
                trans.DOLocalMoveY(10, 0.5f).OnComplete(() => 
                {
                    m_TiPPool.Despawn(trans);
                });
            }
        }
    }

    public void ShowTip(int type,string text)
    {
        //入队
        m_TipQueue.Enqueue(new TipEntity() { Type=type,Text = text});
    }
}
