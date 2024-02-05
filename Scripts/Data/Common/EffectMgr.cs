using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectMgr : SingletonMiddle<EffectMgr>
{
    /// <summary>
    /// ��Ч��
    /// </summary>
    private SpawnPool m_EffectPool;

    private MonoBehaviour m_Mono;

    /// <summary>
    /// ��Ч���ֵ�
    /// </summary>
    private Dictionary<string, Transform> m_EffectDic = new Dictionary<string, Transform>();

    /// <summary>
    /// ��Ч�س�ʼ��
    /// </summary>
    public void Init(MonoBehaviour mono)
    {
        m_Mono = mono;
        m_EffectPool = PoolManager.Pools.Create("Effect");
    }

    /// <summary>
    /// ��Ч����
    /// </summary>
    /// <param name="effectPath">���Զ�·������Download����·����</param>
    /// <param name="effectName">��Ч��</param>
    /// <param name="onComplete">��Ч������ɻص�</param>
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
                        //����Чû���Ź�
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
    /// ������Ч
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
    /// ��Ч�����
    /// </summary>
    public void Clear()
    {
        m_EffectDic.Clear();
        m_EffectPool = null;
    }

}
