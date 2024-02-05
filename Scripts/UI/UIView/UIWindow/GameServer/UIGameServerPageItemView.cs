using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameServerPageItemView : MonoBehaviour
{
    /// <summary>
    /// 页码
    /// </summary>
    private int m_PageIndex;

    /// <summary>
    /// 名称
    /// </summary>
    [SerializeField]
    private Text m_GameServerPageName;

    /// <summary>
    /// 页签点击事件
    /// </summary>
    public Action<int> OnGameServerPageClick;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(gameServerPageClick);
    }

    private void gameServerPageClick()
    {
        if (OnGameServerPageClick != null)
        {
            OnGameServerPageClick(m_PageIndex);
        }
    }

    public void SetUI(RetGameServerPageEntity entity)
    {
        m_PageIndex = entity.PageIndex;
        m_GameServerPageName.text = entity.Name;
    }
}
