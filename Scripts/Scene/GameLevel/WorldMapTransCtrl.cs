using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ͼ���͵������ 
/// </summary>
public class WorldMapTransCtrl : MonoBehaviour
{
    /// <summary>
    /// ��ǰ��ͼ���͵���
    /// </summary>
    [SerializeField]
    private int m_TransPosId;
    /// <summary>
    /// Ҫ���͵�Ŀ�곡��ID
    /// </summary>
    [SerializeField]
    private int m_TargetTransSceneId;
    public int TargetTransSceneId
    { get { return m_TargetTransSceneId; } }
    /// <summary>
    /// Ŀ�곡�����͵�ID
    /// </summary>
    private int m_TargetSceneTransId;

    /// <summary>
    /// ������Ҫ��ʱ��
    /// </summary>
    private float TransTime;

    /// <summary>
    /// ���ô��͵����
    /// </summary>
    /// <param name="transPosId">��ǰ����ID</param>
    /// <param name="targetTranSceneId">Ŀ�곡��</param>
    /// <param name="targetSceneTransId">Ŀ�곡��ID</param>
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
                Debug.Log("����׼�����д��ͣ�" + m_TargetTransSceneId);
                RoleCtrl ctrl = collider.gameObject.GetComponent<RoleCtrl>();
                //ֻ�б��ͻ��˵���Ҳ��д������͵���ʸ���������������Լ��Ĵ��͵�
                if (ctrl != null && ctrl.CurrRoleType == RoleType.MainPlayer)
                {
                    //����Ŀ�������ͼ���͵��ID
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
