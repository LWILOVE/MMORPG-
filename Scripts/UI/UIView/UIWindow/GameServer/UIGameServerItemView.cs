using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏服操作类
/// </summary>
public class UIGameServerItemView : MonoBehaviour
{
    /// <summary>
    /// 游戏服状态
    /// </summary>
    [SerializeField]
    private Sprite[] m_GameSetverStatus;

    /// <summary>
    /// 当前游戏服状态
    /// </summary>
    [SerializeField]
    private Image m_CurrentGameServerStatus;

    /// <summary>
    /// 游戏服名称
    /// </summary>
    [SerializeField]
    private Text m_GameServerName;

    /// <summary>
    /// 当前游戏服实体
    /// </summary>
    private RetGameServerEntity m_CurrentGameServerData;

    public Action<RetGameServerEntity> OnGameServerClick;

    // Start is called before the first frame update
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(GameServerClick);
        }
    }

    private void GameServerClick()
    {
        if (OnGameServerClick != null)
        {
            OnGameServerClick(m_CurrentGameServerData);
        }
    }

    /// <summary>
    /// 设置游戏服信息
    /// </summary>
    /// <param name="entity"></param>
    public void SetUI(RetGameServerEntity entity)
    {
        m_CurrentGameServerData = entity;
        m_CurrentGameServerStatus.overrideSprite = m_GameSetverStatus[entity.RunStatus];
        m_GameServerName.text = entity.Name;
        
    }
}
