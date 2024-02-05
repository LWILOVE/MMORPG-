using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��չ�����ͼʵ��
/// </summary>
public partial class WorldMapEntity
{
    private Vector3 m_RoleBirthPosition = Vector3.zero;

    /// <summary>
    /// �������ǵĳ���������
    /// </summary>
    public Vector3 RoleBirthPostion
    {
        get 
        {
            if (m_RoleBirthPosition == Vector3.zero)
            {
                ///��ȡ���Ա�����Ϣ
                string[] arr = RoleBirthPos.Split('_');
                //���û�е�ͷ���
                if (arr.Length < 3)
                {
                    return Vector3.zero;
                }

                float x = 0, y = 0, z = 0;
                //��ȡxyz��������
                float.TryParse(arr[0], out x);
                float.TryParse(arr[1], out y);
                float.TryParse(arr[2], out z);
                m_RoleBirthPosition = new Vector3(x,y,z);
            }

            return m_RoleBirthPosition;
        }
    }

    private float m_RoleBirthEulerAngleY = -1;

    /// <summary>
    /// ����Y�����ת�Ƕ�
    /// </summary>
    public float RoleBirthEulerAnglesY
    {
        get
        {
            if (m_RoleBirthEulerAngleY == -1)
            {
                ///��ȡ���Ա�����Ϣ
                string[] arr = RoleBirthPos.Split('_');
                //���û�е�ͷ���
                if (arr.Length < 4)
                {
                    return 0;
                }
                float y = 0;
                float.TryParse(arr[0], out y);
                m_RoleBirthEulerAngleY = y;
            }
            return m_RoleBirthEulerAngleY;
        }
    }

    /// <summary>
    /// NPC��ʼ����Ϣ�б�
    /// </summary>
    private List<NPCWorldMapData> m_NPCWorldMapList;

    /// <summary>
    /// �����ͼ������NPC�б�
    /// </summary>
    public List<NPCWorldMapData> NPCWorldMapList
    {
        get 
        {
            if (m_NPCWorldMapList == null)
            {
                m_NPCWorldMapList = new List<NPCWorldMapData>();
                //NPC����|�ֿ�
                string[] arr1 = NPCList.Split('|');
                //NPC������_�ֿ�
                for (int i = 0; i < arr1.Length; i++)
                {
                    string[] arr2 = arr1[i].Split("_");
                    if(arr2.Length<6)
                    {
                        break;
                    }
                    int npcId = 0;
                    int.TryParse(arr2[0],out npcId);
                    float x = 0, y = 0, z = 0,anglesY=0;
                    float.TryParse(arr2[1],out x);
                    float.TryParse(arr2[2],out y);
                    float.TryParse(arr2[3],out z);
                    float.TryParse(arr2[4],out anglesY);

                    string prologue = arr2[5];

                    NPCWorldMapData entity = new NPCWorldMapData();
                    entity.NPCId = npcId;
                    entity.NPCPosition = new Vector3(x,y,z);
                    entity.Prologue = prologue;
                    m_NPCWorldMapList.Add(entity);
                }
            }
            return m_NPCWorldMapList;
        }
    }
}
