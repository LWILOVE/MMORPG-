using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����ѡ�񴰿�  
/// </summary>
public class UIGameServerSelectView : UIWindowViewBase
{
    #region ҳǩ
    /// <summary>
    /// ��ϷҳǩԤ��
    /// </summary>
    [SerializeField]
    private GameObject GameServerPageItemPrefab;

    /// <summary>
    /// ҳǩ�б�
    /// </summary>
    [SerializeField]
    private GridLayoutGroup GameServerPageGrid;

    /// <summary>
    /// ҳǩ����¼�
    /// </summary>
    public Action<int> OnPageClick;

    /// <summary>
    /// ��Ϸ������¼�
    /// </summary>
    public Action<RetGameServerEntity> OnGameServerClick;

    protected override void OnStart()
    {
        base.OnStart();
        //��ʼ�ȿ�¡ʮ����Ϸ��Ԥ��
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
    /// ����ҳǩ���ԣ�X-X�����м�����
    /// </summary>
    /// <param name="list"></param>
    public void SetGameServerPageUI(List<RetGameServerPageEntity> list)
    {
        Debug.Log("ҳǩ������" + list.Count);  
        if (list == null || GameServerPageItemPrefab == null)
        {
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            //����ҳǩ
            GameObject obj = Instantiate(GameServerPageItemPrefab) as GameObject;
            obj.transform.parent = GameServerPageGrid.transform;
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;

            UIGameServerPageItemView view = obj.GetComponent<UIGameServerPageItemView>();

            if (view != null)
            {
                //����ҳǩ����
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

    #region �������б�
    /// <summary>
    /// ��Ϸ��Ԥ��
    /// </summary>
    [SerializeField]
    private GameObject GameServerItemPrefab;

    /// <summary>
    /// ��Ϸ���б�
    /// </summary>
    [SerializeField]
    private GridLayoutGroup GameServerGrid;

    /// <summary>
    /// ��ǰ��Ϸ���б�
    /// </summary>
    private List<GameObject> m_GameServerObjList = new List<GameObject>();

    /// <summary>
    /// �������б�����
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

        Debug.Log("��ʼ������Ϸ����ʾ");
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
    /// ����������ص�
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

    #region ��ѡ�������

    /// <summary>
    /// ��ѡ�����Ϸ��
    /// </summary>
    [SerializeField]
    private UIGameServerItemView m_CurrentSelectGameServer;
    
    /// <summary>
    /// ������ѡ�����Ϸ��
    /// </summary>
    /// <param name="entity">��Ϸ��ʵ��</param>
    public void SetSelectGameServerUI(RetGameServerEntity entity)
    {
        if (m_CurrentSelectGameServer == null)
        {
            Debug.Log("����������δ������ѡ�е���Ϸ��");
            return;
        }
        m_CurrentSelectGameServer.SetUI(entity);
    }
    #endregion
}
