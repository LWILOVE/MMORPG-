using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地图传送点控制器 
/// </summary>
public class WorldMapTransCtrl : MonoBehaviour
{
    /// <summary>
    /// 当前地图传送点编号
    /// </summary>
    [SerializeField]
    private int m_TransPosId;
    /// <summary>
    /// 要传送的目标场景ID
    /// </summary>
    [SerializeField]
    private int m_TargetTransSceneId;
    public int TargetTransSceneId
    { get { return m_TargetTransSceneId; } }
    /// <summary>
    /// 目标场景传送点ID
    /// </summary>
    private int m_TargetSceneTransId;

    /// <summary>
    /// 传送需要的时间
    /// </summary>
    private float TransTime;

    /// <summary>
    /// 设置传送点参数
    /// </summary>
    /// <param name="transPosId">当前场景ID</param>
    /// <param name="targetTranSceneId">目标场景</param>
    /// <param name="targetSceneTransId">目标场景ID</param>
    public void SetParam(int transPosId, int targetTranSceneId, int targetSceneTransId)
    {
        m_TransPosId = transPosId;
        m_TargetTransSceneId = targetTranSceneId;
        m_TargetSceneTransId = targetSceneTransId;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            if (TransTime < 1f)
            {
                TransTime += Time.deltaTime;
            }
            else
            {
                TransTime = 0f;
                Debug.Log("正在准备进行传送：" + m_TargetTransSceneId);
                RoleCtrl ctrl = collider.gameObject.GetComponent<RoleCtrl>();
                //只有本客户端的玩家才有触发传送点的资格，其他玩家有他们自己的传送点
                if (ctrl != null && ctrl.CurrRoleType == RoleType.MainPlayer)
                {
                    //设置目标世界地图传送点的ID
                    UILoadingCtrl.Instance.targetWorldMapTransWorld = m_TargetTransSceneId;
                    UILoadingCtrl.Instance.LoadToWorldMap(m_TargetTransSceneId);
                }
            }
        }
    }

    private void Start()
    {
        
    }


}
