using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelDoorCtrl : MonoBehaviour
{
    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField]
    public GameLevelDoorCtrl ConnectToDoor;

    /// <summary>
    /// ��ǰ�ŵ���������ID
    /// </summary>
    public int OwnerRegionId;

#if UNITY_EDITOR
    /// <summary>
    /// ����
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.transform.position, 1f);
        if (ConnectToDoor != null)
        {
            Gizmos.DrawLine(this.transform.position,ConnectToDoor.gameObject.transform.position );
        }
    }
#endif
}
