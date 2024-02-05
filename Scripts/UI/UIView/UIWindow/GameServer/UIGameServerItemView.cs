using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ��������
/// </summary>
public class UIGameServerItemView : MonoBehaviour
{
    /// <summary>
    /// ��Ϸ��״̬
    /// </summary>
    [SerializeField]
    private Sprite[] m_GameSetverStatus;

    /// <summary>
    /// ��ǰ��Ϸ��״̬
    /// </summary>
    [SerializeField]
    private Image m_CurrentGameServerStatus;

    /// <summary>
    /// ��Ϸ������
    /// </summary>
    [SerializeField]
    private Text m_GameServerName;

    /// <summary>
    /// ��ǰ��Ϸ��ʵ��
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
    /// ������Ϸ����Ϣ
    /// </summary>
    /// <param name="entity"></param>
    public void SetUI(RetGameServerEntity entity)
    {
        m_CurrentGameServerData = entity;
        m_CurrentGameServerStatus.overrideSprite = m_GameSetverStatus[entity.RunStatus];
        m_GameServerName.text = entity.Name;
        
    }
}
