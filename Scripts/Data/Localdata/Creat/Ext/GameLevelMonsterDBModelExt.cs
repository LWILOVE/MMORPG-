using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelMonsterDBModel
{

    /// <summary>
    ///  根据游戏关卡编号及其等级获取游戏关卡中怪的总数量
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
    /// 根据游戏关卡编号，难度等级和区域编号获取怪当前区域的怪的总数量
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
    /// 根据游戏关卡编号和难度获取怪物的种类
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
    /// 根据游戏关卡编号，难度等级与区域编号获取所有怪物的数量
    /// </summary>
    /// <param name="gameLevelId">当前关卡编号</param>
    /// <param name="grade">当前关卡难度</param>
    /// <param name="regionId">区域编号</param>
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
    /// 获取游戏关卡区域怪实体
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
