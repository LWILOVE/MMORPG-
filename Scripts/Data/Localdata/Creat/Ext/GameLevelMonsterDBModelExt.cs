using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelMonsterDBModel
{

    /// <summary>
    ///  ������Ϸ�ؿ���ż���ȼ���ȡ��Ϸ�ؿ��йֵ�������
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public int GetGameLevelMonsterCount(int gameLevelId,GameLevelGrade grade)
    {
        int count = 0;
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade)
            {
                count += m_List[i].SpriteCount;
            }
        }
        return count;
    }


    /// <summary>
    /// ������Ϸ�ؿ���ţ��Ѷȵȼ��������Ż�ȡ�ֵ�ǰ����Ĺֵ�������
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <param name="regionId"></param>
    /// <returns></returns>
    public int GetGameLevelMonsterCount(int gameLevelId, GameLevelGrade grade, int regionId)
    {
        int count = 0;
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade && m_List[i].RegionId == regionId)
            {
                count += m_List[i].SpriteCount;
            }
        }
        return count;
    }

    /// <summary>
    /// ������Ϸ�ؿ���ź��ѶȻ�ȡ���������
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public int[] GetGameLevelMonsterId(int gameLevelId, GameLevelGrade grade)
    {
        List<int> list = new List<int>();
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade)
            {
                if (!list.Contains(m_List[i].SpriteId))
                {
                    list.Add(m_List[i].SpriteId);
                }
            }
        }
        return list.ToArray();
    }

    private List<GameLevelMonsterEntity> retList = new List<GameLevelMonsterEntity>();

    /// <summary>
    /// ������Ϸ�ؿ���ţ��Ѷȵȼ��������Ż�ȡ���й��������
    /// </summary>
    /// <param name="gameLevelId">��ǰ�ؿ����</param>
    /// <param name="grade">��ǰ�ؿ��Ѷ�</param>
    /// <param name="regionId">������</param>
    /// <returns></returns>
    public List<GameLevelMonsterEntity> GetGameLevelMonster(int gameLevelId, GameLevelGrade grade, int regionId)
    {
        retList.Clear();
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade && m_List[i].RegionId == regionId)
            {
                retList.Add(m_List[i]);
            }
        }
        return retList;
    }

    /// <summary>
    /// ��ȡ��Ϸ�ؿ������ʵ��
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <param name="regionId"></param>
    /// <param name="monsterId"></param>
    /// <returns></returns>
    public GameLevelMonsterEntity GetGameLevelMonsterEntity(int gameLevelId, GameLevelGrade grade, int regionId,int monsterId)
    {
        for (int i = 0; i < m_List.Count; i++)
        {

            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade 
                && m_List[i].RegionId == regionId && m_List[i].SpriteId == monsterId)
            {
                return m_List[i];
            }
        }
        return null;
    }

}
