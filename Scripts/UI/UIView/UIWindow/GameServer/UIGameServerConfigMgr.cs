using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameServerConfigMgr : SingletonMiddle<UIGameServerConfigMgr>
{

    /// <summary>
    /// 根据配置编码获取配置信息
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public GameServerConfigEntity Get(ConfigCode code)
    { 
        GameServerConfigEntity entity = null;
        if (m_List == null || m_List.Count == 0)
        {
            entity = new GameServerConfigEntity() { ConfigCode = code.ToString()};
            m_List.Add(entity);
        }
        for (int i = 0; i < m_List.Count; i++)
        {
            if (m_List[i].ConfigCode.Equals(code.ToString(), System.StringComparison.CurrentCultureIgnoreCase))
            {
                entity = m_List[i];
                break;
            }
        }
        if (entity == null)
        {
            entity = new GameServerConfigEntity() { ConfigCode = code.ToString() };
            m_List.Add(entity);
        }
        return entity;
    }

    public void AddConfig(string configCode, bool isOpen, string param)
    {
        m_List.Add(new GameServerConfigEntity()
        { ConfigCode = configCode, IsOpen = isOpen, Param = param });
    }

    private List<GameServerConfigEntity> m_List = new List<GameServerConfigEntity>();
    public class GameServerConfigEntity
    {
        /// <summary>
        /// 配置编码
        /// </summary>
        public string ConfigCode;
        /// <summary>
        /// 是否开启
        /// </summary>        
        public bool IsOpen;
        /// <summary>
        /// 配置参数
        /// </summary>
        public string Param;
    }
}
