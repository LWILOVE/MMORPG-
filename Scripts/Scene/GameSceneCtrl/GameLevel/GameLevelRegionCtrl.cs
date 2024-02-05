using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡区域控制器
/// </summary>
public class GameLevelRegionCtrl : MonoBehaviour
{
    /// <summary>
    /// 区域编号
    /// </summary>
    public int RegionId;

    /// <summary>
    /// 角色出生点
    /// </summary>
    [SerializeField] 
    public Transform RoleBornPos;

    /// <summary>
    /// 怪的所有可能出生点
    /// </summary>
    [SerializeField]
    public Transform[]  MonsterBornPos;

    /// <summary>
    /// 本区域下所有的门
    /// </summary>
    [SerializeField]
    private GameLevelDoorCtrl[] AllDoor;

    /// <summary>
    /// 区域遮挡物体
    /// </summary>
    public GameObject RegionMask;

    private void Awake()
    {
        //隐去刷怪笼
        if(MonsterBornPos != null && MonsterBornPos.Length > 0)
        {
            for (int i = 0; i < MonsterBornPos.Length; i++)
            {
                Renderer render =  MonsterBornPos[i].GetComponent<Renderer>();
                if (render != null)
                {
                    render.enabled = false;
                }
            }
        }

        //隐去区域门
        if (AllDoor != null && AllDoor.Length > 0)
        {
            for (int i = 0; i < AllDoor.Length; i++)
            {
                Renderer render = AllDoor[i].GetComponent<Renderer>();
                if (render != null)
                {
                    render.enabled = false;
                }
                AllDoor[i].OwnerRegionId = RegionId; 
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 画像
    /// </summary>
    public void OnDrawGizmos()
    {
        //区域球
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position,3f);

        //出生球
        if (RoleBornPos != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(RoleBornPos.position, 3f);

            Gizmos.DrawLine(transform.position,RoleBornPos.position);
        }

        //怪物球
        if (MonsterBornPos != null && MonsterBornPos.Length > 0)
        {  
            Gizmos.color = Color.red;
            for (int i = 0; i < MonsterBornPos.Length; i++)
            {
                Gizmos.DrawSphere(MonsterBornPos[i].position,2f);
                Gizmos.DrawLine(transform.position, MonsterBornPos[i].position);
            }
        }

        //本区域所有门
        if (AllDoor != null && AllDoor.Length > 0)
        {
            Gizmos.color = Color.grey   ;
            for (int i = 0; i < AllDoor.Length; i++)
            {
                Gizmos.DrawSphere(AllDoor[i].transform.position, 3f);
                Gizmos.DrawLine(transform.position, AllDoor[i].transform.position);
            }
        }
    }
#endif

    /// <summary>
    /// 获取通往下一区域的门
    /// </summary>
    /// <param name="nextRegionId">下一区域的ID</param>
    /// <returns></returns>
    public GameLevelDoorCtrl GetNextRegionDoor(int nextRegionId)
    {
        if (AllDoor != null && AllDoor.Length > 0)
        {
            for (int i = 0; i < AllDoor.Length; i++)
            {
                if(AllDoor[i].ConnectToDoor != null)
                {
                    if (AllDoor[i].ConnectToDoor.OwnerRegionId == nextRegionId)
                    {
                        return AllDoor[i];
                    }
                }
            }
        }
            return null;
    }
}
