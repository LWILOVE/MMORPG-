using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelGradeDBModel
{
    /// <summary>
    /// 根据关卡编号及其难度返回实体
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <returns></returns>
    public GameLevelGradeEntity GetEntityByGameLevelIdAndGrade(int gameLevelId, GameLevelGrade grade)
    {
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].GameLevelId == gameLevelId && m_List[i].Grade == (int)grade)
            {
                return m_List[i];
            }
        }
        return null;
    }
}
