using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelRegionDBModel
{
    private List<GameLevelRegionEntity> list = new List<GameLevelRegionEntity>();

    /// <summary>
    /// 根据游戏关卡编号返回集合
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <returns></returns>
    public List<GameLevelRegionEntity> GetListByGameLevelId(int gameLevelId)
    {
        list.Clear();

        for(int i = 0;i<m_List.Count;i++)
        {
            if (m_List[i].GameLevelId == gameLevelId)
            {
                list.Add(m_List[i]);
            }
        }

        return list;
    }
}
