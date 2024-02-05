using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�ؿ����������
/// </summary>
public class GameLevelRegionCtrl : MonoBehaviour
{
    /// <summary>
    /// ������
    /// </summary>
    public int RegionId;

    /// <summary>
    /// ��ɫ������
    /// </summary>
    [SerializeField] 
    public Transform RoleBornPos;

    /// <summary>
    /// �ֵ����п��ܳ�����
    /// </summary>
    [SerializeField]
    public Transform[]  MonsterBornPos;

    /// <summary>
    /// �����������е���
    /// </summary>
    [SerializeField]
    private GameLevelDoorCtrl[] AllDoor;

    /// <summary>
    /// �����ڵ�����
    /// </summary>
    public GameObject RegionMask;

    private void Awake()
    {
        //��ȥˢ����
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

        //��ȥ������
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
    /// ����
    /// </summary>
    public void OnDrawGizmos()
    {
        //������
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(this.transform.position,3f);

        //������
        if (RoleBornPos != null)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(RoleBornPos.position, 3f);

            Gizmos.DrawLine(transform.position,RoleBornPos.position);
        }

        //������
        if (MonsterBornPos != null && MonsterBornPos.Length > 0)
        {  
            Gizmos.color = Color.red;
            for (int i = 0; i < MonsterBornPos.Length; i++)
            {
                Gizmos.DrawSphere(MonsterBornPos[i].position,2f);
                Gizmos.DrawLine(transform.position, MonsterBornPos[i].position);
            }
        }

        //������������
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
    /// ��ȡͨ����һ�������
    /// </summary>
    /// <param name="nextRegionId">��һ�����ID</param>
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
