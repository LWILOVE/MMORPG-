using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevelDoorCtrl : MonoBehaviour
{
    /// <summary>
    /// 关联的门
    /// </summary>
    [SerializeField]
    public GameLevelDoorCtrl ConnectToDoor;

    /// <summary>
    /// 当前门的所属区域ID
    /// </summary>
    public int OwnerRegionId;

#if UNITY_EDITOR
    /// <summary>
    /// 画像
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
