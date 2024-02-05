using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ�ؿ���UI
/// </summary>
public class UIGameLevelMapView : UIWindowViewBase
{
    /// <summary>
    /// �½���
    /// </summary>
    [SerializeField]
    private Text txtChapterName;
    /// <summary>
    /// ����ͼ
    /// </summary>
    [SerializeField]
    private RawImage imgMap;

    /// <summary>
    /// �½ڱ��
    /// </summary>
    [HideInInspector]
    private int m_ChapterId;

    /// <summary>
    /// 
    /// </summary>
    private List<Transform> m_GameLevelItems = new List<Transform>();

    /// <summary>
    /// ���ߵ�����
    /// </summary>
    [SerializeField]
    private Transform pointContainer;

    private List<TransferData> m_ListData;

    public Action<int> OnGameLevelItemClick;
    protected override void OnStart()
    {
        base.OnStart();
        StartCoroutine(CreateGameLevelItem());
    }


    /// <summary>
    /// ������Ϸ�ؿ�������
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateGameLevelItem()
    {
        if (m_ListData == null)
        {
            yield break;
        }
        if (m_ListData != null && m_ListData.Count > 0)
        {
            m_GameLevelItems.Clear();
            AssetBundleMgr.Instance.LoadOrDownload("Download/Prefab/UIPrefab/UIWindow/GameLevel/GameLevelMapItem/GameLevelMapItem.assetbundle", "GameLevelMapItem",
                (GameObject obj) =>
                {
                    StartCoroutine(LoadItem(obj));
                });
        }
    }

    /// <summary>
    /// ���عؿ���
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerator LoadItem(GameObject obj)
    {
        for (int i = 0; i < m_ListData.Count; i++)
        {
            //�ؿ���
            obj = Instantiate(obj);
            obj.SetParent(imgMap.transform);
            Vector2 pos = m_ListData[i].GetValue<Vector2>(ConstDefine.GameLevelPosition);
            obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
            UIGameLevelMapItemView view = obj.GetComponent<UIGameLevelMapItemView>();
            view.SetUI(m_ListData[i], OnGameLevelItemClick);
            m_GameLevelItems.Add(obj.transform);
            yield return null;
        }
        AssetBundleMgr.Instance.LoadOrDownload("Download/Prefab/UIPrefab/UIWindow/GameLevel/GameLevelMapItem/GameLevelMapPoint.assetbundle", "GameLevelMapPoint",
    (GameObject objPoint) =>
    {
        StartCoroutine(LoadPoint(objPoint));
    });
    }

    /// <summary>
    /// �������ߵ�
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private IEnumerator LoadPoint(GameObject obj)
    {
        //=========�������ߵ�=============
        for (int i = 0; i < m_GameLevelItems.Count; i++)
        {
            if (i == m_GameLevelItems.Count - 1)
            { break; }
            //�������
            Transform transBegin = m_GameLevelItems[i];
            //�����յ�
            Transform transEnd = m_GameLevelItems[i + 1];
            //�����������
            float distance = Vector2.Distance(transBegin.localPosition, transEnd.localPosition);
            //�������ߵ����ɵ�����
            int createCount = Mathf.FloorToInt(distance / 20f);
            //����ÿ���������λ��
            float xLen = transEnd.localPosition.x - transBegin.localPosition.x;
            float yLen = transEnd.localPosition.y - transBegin.localPosition.y;
            //XY����
            float stepX = xLen / createCount;
            float stepY = yLen / createCount;

            //������
            for (int j = 0; j < createCount; j++)
            {
                if (j < 1 || j > createCount - 1)
                { continue; }
                //��¡��
                obj = Instantiate(obj);
                obj.SetParent(pointContainer);
                obj.transform.localPosition = new Vector3(transBegin.transform.localPosition.x + (stepX * j), transBegin.transform.localPosition.y + (stepY * j), 0f);

                UIGameLevelMapPointView view = obj.GetComponent<UIGameLevelMapPointView>();
                if (view != null)
                {
                    //TODO:����������н���--
                    view.SetUI(false);
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// ���ùؿ�����UI�������Ϣ
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onGameLevelItemClick"></param>
    public void SetUI(TransferData data, Action<int> onGameLevelItemClick)
    {
        OnGameLevelItemClick = onGameLevelItemClick;
        m_ChapterId = data.GetValue<int>(ConstDefine.ChapterId);
        txtChapterName.SetText(data.GetValue<string>(ConstDefine.ChapterName));
        string picName = data.GetValue<string>(ConstDefine.ChapterBG);
        AssetBundleMgr.Instance.LoadOrDownload<Texture>(string.Format("Download/Source/UISource/GameLevel/GameLevelMap{0}.assetbundle", picName), "GameLevelMap"+picName,
    (Texture obj) =>
    {
        imgMap.texture = obj;
    }, type: 1);
        //��ȡ�ؿ�����
        m_ListData = data.GetValue<List<TransferData>>(ConstDefine.GameLevelList);
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        txtChapterName = null;
        imgMap = null;
    }


}
