using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMgr : SingletonMiddle<EffectMgr>
{
    /// <summary>
    /// 特效池
    /// </summary>
    private SpawnPool m_EffectPool;

    private MonoBehaviour m_Mono;

    /// <summary>
    /// 特效池字典
    /// </summary>
    private Dictionary<string, Transform> m_EffectDic = new Dictionary<string, Transform>();

    /// <summary>
    /// 特效池初始化
    /// </summary>
    public void Init(MonoBehaviour mono)
    {
        m_Mono = mono;
        m_EffectPool = PoolManager.Pools.Create("Effect");
    }

    /// <summary>
    /// 特效播放
    /// </summary>
    /// <param name="effectPath">特性短路径（即Download后续路径）</param>
    /// <param name="effectName">特效名</param>
    /// <param name="onComplete">特效播放完成回调</param>
    public void PlayEffect(string effectPath, string effectName,System.Action<Transform> onComplete)
    {
        if (m_EffectPool == null)
        { return; }
        if (!m_EffectDic.ContainsKey(effectName))
        {
            AssetBundleMgr.Instance.LoadOrDownload(string.Format(effectPath + "{0}.assetbundle",effectName), effectName,
                (GameObject obj) =>
                {
                    if (!m_EffectDic.ContainsKey(effectName))
                    {
                        //obj = GameObject.Instantiate(obj);
                        //若特效没播放过
                        m_EffectDic[effectName] = obj.transform;
                        PrefabPool prefabPool = new PrefabPool(m_EffectDic[effectName]);
                        prefabPool.preloadAmount = 0;
                        prefabPool.cullDespawned = true;
                        prefabPool.cullAbove = 5;
                        prefabPool.cullDelay = 2;
                        prefabPool.cullMaxPerPass = 2;
                        m_EffectPool.CreatePrefabPool(prefabPool);
                        if (onComplete != null)
                        {
                            onComplete(m_EffectPool.Spawn(m_EffectDic[effectName]));
                        }
                    }
                    else
                    {
                        if (onComplete != null)
                        {
                            onComplete(m_EffectPool.Spawn(m_EffectDic[effectName]));
                        }
                    }
                });
        }
        else
        {
            if (onComplete != null)
            {
                onComplete(m_EffectPool.Spawn(m_EffectDic[effectName]));
            }
        }
    }
    
    /// <summary>
    /// 销毁特效
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="delay"></param>
   public void DestroyEffect(Transform effect,float delay)
    {
        m_Mono.StartCoroutine(DestroyEffectCoroutine(effect,delay));
    }

    private IEnumerator DestroyEffectCoroutine(Transform effect,float delay)
    {
        yield return new WaitForSeconds(delay);
        m_EffectPool.Despawn(effect);
    }

    /// <summary>
    /// 特效池清空
    /// </summary>
    public void Clear()
    {
        m_EffectDic.Clear();
        m_EffectPool = null;
    }

}
