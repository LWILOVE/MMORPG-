using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 区服选择窗口  
/// </summary>
public class UIGameServerSelectView : UIWindowViewBase
{
    #region 页签
    /// <summary>
    /// 游戏页签预设
    /// </summary>
    [SerializeField]
    private GameObject GameServerPageItemPrefab;

    /// <summary>
    /// 页签列表
    /// </summary>
    [SerializeField]
    private GridLayoutGroup GameServerPageGrid;

    /// <summary>
    /// 页签点击事件
    /// </summary>
    public Action<int> OnPageClick;

    /// <summary>
    /// 游戏服点击事件
    /// </summary>
    public Action<RetGameServerEntity> OnGameServerClick;

    protected override void OnStart()
    {
        base.OnStart();
        //开始先克隆十个游戏服预设
        for (int i = 0; i < 10; i++)
        {
            GameObject obj = Instantiate(GameServerItemPrefab) as GameObject;
            obj.transform.parent = GameServerGrid.transform;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
            obj.SetActive(false);

            m_GameServerObjList.Add(obj);
        }
    }

    /// <summary>
    /// 设置页签属性（X-X服，有几个）
    /// </summary>
    /// <param name="list"></param>
    public void SetGameServerPageUI(List<RetGameServerPageEntity> list)
    {
        Debug.Log("页签数量：" + list.Count);  
        if (list == null || GameServerPageItemPrefab == null)
        {
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            //塑造页签
            GameObject obj = Instantiate(GameServerPageItemPrefab) as GameObject;
            obj.transform.parent = GameServerPageGrid.transform;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            UIGameServerPageItemView view = obj.GetComponent<UIGameServerPageItemView>();

            if (view != null)
            {
                //设置页签内容
                view.SetUI(list[i]);
                view.OnGameServerPageClick = OnGameServerPageClick;
            }
        }
    }

    private void OnGameServerPageClick(int obj)
    {
        if (OnPageClick != null)
        {
            OnPageClick(obj);
        }

    }


    #endregion

    #region 服务器列表
    /// <summary>
    /// 游戏服预设
    /// </summary>
    [SerializeField]
    private GameObject GameServerItemPrefab;

    /// <summary>
    /// 游戏服列表
    /// </summary>
    [SerializeField]
    private GridLayoutGroup GameServerGrid;

    /// <summary>
    /// 当前游戏服列表
    /// </summary>
    private List<GameObject> m_GameServerObjList = new List<GameObject>();

    /// <summary>
    /// 服务器列表属性
    /// </summary>
    /// <param name="list"></param>
    public void SetGameServerUI(List<RetGameServerEntity> list)
    {
        if(list == null || GameServerItemPrefab == null)
        {
            return;
        }

        for (int i = 0; i < m_GameServerObjList.Count; i++)
        {
            if (i > list.Count-1)
            {
                m_GameServerObjList[i].SetActive(false);
            }
        }

        Debug.Log("开始进行游戏服显示");
        for(int i = 0;i<list.Count;i++)
        {
            GameObject obj = m_GameServerObjList[i];

            if (!obj.activeSelf)
            {
                obj.SetActive(true);
            }

            UIGameServerItemView view = obj.GetComponent<UIGameServerItemView>();
            if (view != null)
            {
                view.SetUI(list[i]);
                view.OnGameServerClick = OnGameServerClickCallBack;
            }
        }


    }

    /// <summary>
    /// 服务器点击回调
    /// </summary>
    /// <param name="obj"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnGameServerClickCallBack(RetGameServerEntity obj)
    {
        if (OnGameServerClick != null)
        {
            OnGameServerClick(obj);
        }
    }
    #endregion

    #region 已选择服务器

    /// <summary>
    /// 已选择的游戏服
    /// </summary>
    [SerializeField]
    private UIGameServerItemView m_CurrentSelectGameServer;
    
    /// <summary>
    /// 设置已选择的游戏服
    /// </summary>
    /// <param name="entity">游戏服实体</param>
    public void SetSelectGameServerUI(RetGameServerEntity entity)
    {
        if (m_CurrentSelectGameServer == null)
        {
            Debug.Log("初次启动，未发现有选中的游戏服");
            return;
        }
        m_CurrentSelectGameServer.SetUI(entity);
    }
    #endregion
}
