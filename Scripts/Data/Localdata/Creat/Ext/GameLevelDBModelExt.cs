using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DBModel����չ����
/// </summary>
public partial class GameLevelDBModel
{
    private List<GameLevelEntity> m_RetList = new List<GameLevelEntity>();

    /// <summary>
    /// �����½ڱ�Ż�ȡ�ؿ�����
    /// </summary>
    /// <returns></returns>
    public List<GameLevelEntity> GetListByChapterId(int chapterId)
    {
        if (m_List == null || m_List.Count == 0)
        {
            return null;
        }
        m_RetList.Clear();

        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].ChapterID == chapterId)
            {
                m_RetList.Add(m_List[i]);
            }
        }

        return m_RetList;
    }
}
