using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����ͼ������
/// </summary>
public class WorldMapCtrl : SystemCtrlBase<WorldMapCtrl>, ISystemCtrl
{
    /// <summary>
    /// �����ͼ����
    /// </summary>
    private UIWorldMapView m_UIWorldMapView;
    /// <summary>
    /// �����ͼʧ�ܴ���
    /// </summary>
    private UIWorldMapFailView m_UIWorldMapFailView;
    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.WorldMap:
                OpenWorldMapView();
                break;
            case WindowUIType.WorldMapFail:
                OpenWorldMapFailView();
                break;
        }
    }


    /// <summary>
    /// �������ͼ����
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void OpenWorldMapView()
    {
        //�ӱ��������ͼ���ȡ����
        List<WorldMapEntity> listWorldMap = WorldMapDBModel.Instance.GetList();
        if (listWorldMap == null || listWorldMap.Count == 0)
        { return; }
        UIViewUtil.Instance.LoadWindow(WindowUIType.WorldMap.ToString(),
            (GameObject obj) =>
            {
                m_UIWorldMapView = obj.GetComponent<UIWorldMapView>();
            });
        TransferData data = new TransferData();
        List<TransferData> list = new List<TransferData>();
        for (int i = 0; i < listWorldMap.Count; i++)
        {
            WorldMapEntity entity = listWorldMap[i];
            //�粻�ڵ�ͼ����ʾ��������
            if (entity.IsShowInMap == 0)
            { continue; }

            TransferData childData = new TransferData();
            childData.SetValue(ConstDefine.WorldMapId, entity.Id);
            childData.SetValue(ConstDefine.WorldMapName, entity.Name);
            childData.SetValue(ConstDefine.WorldMapIco, entity.IcoInMap);

            string[] arr = entity.PosInMap.Split("_");
            Vector2 pos = new Vector2();
            if (arr.Length == 2)
            {
                pos.x = float.Parse(arr[0]);
                pos.y = float.Parse(arr[1]);
            }
            childData.SetValue(ConstDefine.WorldMapPosition, pos);
            list.Add(childData);
        }
        data.SetValue(ConstDefine.WorldMapList, list);
        m_UIWorldMapView.SetUI(data, onWorldMapItemClick);
    }

    /// <summary>
    /// �����ͼ�������
    /// </summary>
    /// <param name="obj"></param>
    private void onWorldMapItemClick(int worldId)
    {
        ////ֱ�Ӵ���
        //UILoadingCtrl.Instance.LoadToWorldMap(worldId);
        //��·����
        CalculateTargetScenePath(UILoadingCtrl.Instance.CurrentWorldMapId, worldId);
    }

    /// <summary>
    /// �򿪵�ͼʧ�ܴ���
    /// </summary>
    public void OpenWorldMapFailView()  
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelFail.ToString(),
             (GameObject obj) =>
             {
                 //m_UIWorldMapFailView = obj.GetComponent<UIWorldMapFailView>();
                 m_UIWorldMapFailView = obj.AddComponent<UIWorldMapFailView>();
                 m_UIWorldMapFailView.ViewName = WindowUIType.GameLevelFail.ToString();
                 //m_UIWorldMapFailView.SetUI(EnemyNickName);
                 m_UIWorldMapFailView.OnResurgence = () =>
                 {
                     Debug.LogWarning("���ڳ��Ը���");
                     //��ҵ�����ť�󣬽���ҿ����������Ϣ���͸�������
                     WorldMap_CurrRoleResurgenceProto proto = new WorldMap_CurrRoleResurgenceProto();
                     proto.Type = 0;//��������
                     NetWorkSocket.Instance.SendMessage(proto.ToArray());
                     m_UIWorldMapFailView.Close();
                     //m_UIWorldMapFailView.gameObject.SetActive(false);
                 };
             });
    }

    /// <summary>
    /// ��ʼ����ID
    /// </summary>
    private int m_BeginSceneId;
    /// <summary>
    /// ��������ID
    /// </summary>
    private int m_TargetSceneId;
    /// <summary>
    /// ��ʱ�������ݴ洢���б�
    /// </summary>
    private List<int> m_WorldMapSceneList;
    /// <summary>
    /// Ŀ�곡��  
    /// </summary>
    private WorldMapSceneEntity m_TargetScene;

    /// <summary>
    /// ����ID����
    /// </summary>
    public Queue<int> SceneIdQueue;

    private Dictionary<int, WorldMapSceneEntity> m_WorldMapSceneDic;

    /// <summary>
    /// �������Ƿ����
    /// </summary>
    private bool m_IsFindOver = false;
    /// <summary>
    /// ��ǰ����ID
    /// </summary>
    public int CurrentSceneId;
    /// <summary>
    /// Ҫǰ���ĳ���ID
    /// </summary>
    public int ToSceneId;
    /// <summary>
    /// �Ƿ��Զ��ƶ�
    /// </summary>
    public bool IsAutoMove = false;
    //Ҫǰ���ĵص�
    public Vector3 ToScenePos = Vector3.zero;
    
    /// <summary>
    /// PVP��������
    /// </summary>
    public string EnemyNickName;

    /// <summary>
    /// ���㵽��Ŀ�곡��·��_�㷨��
    /// </summary>
    /// <param name="currentSceneId"></param>
    private void CalculateTargetScenePath(int currentSceneId)
    {
        if (!m_WorldMapSceneDic.ContainsKey(currentSceneId))
        { return; }
        //��ȡ��ǰ����ʵ��
        WorldMapSceneEntity entity = m_WorldMapSceneDic[currentSceneId];
        //��ֹ�������
        string[] arr = entity.NearScene.Split("_");
        for (int i = 0; i < arr.Length; i++)
        {
            if (m_IsFindOver)
            { continue; }
            //��ȡ��������ID
            int sceneId = int.Parse(arr[i]);
            if (sceneId == m_BeginSceneId)
            { continue; }
            //���ݹ�������ID���ʵ��,��ʵ���Ѿ����ʹ��ˣ��򷵻�
            WorldMapSceneEntity findScene = m_WorldMapSceneDic[sceneId];

            if (findScene.IsVisit)
            { continue; }
            findScene.IsVisit = true;
            //���ø����ʱ�� ������IsVisit֮��
            findScene.Parent = entity;
            //���ҵ�Ŀ�곡����������ѭ��
            if (findScene.Id == m_TargetSceneId)
            {
                m_IsFindOver = true;
                m_TargetScene = findScene;
                break;
            }
            else
            {
                CalculateTargetScenePath(findScene.Id);
            }
        }
    }

    /// <summary>
    /// ���㵽��Ŀ�곡��·��
    /// </summary>
    /// <param name="beginSceneId">���ID</param>
    /// <param name="endSceneId">�յ�ID</param>
    public void CalculateTargetScenePath(int beginSceneId, int endSceneId)
    {
        //��ȡ�������
        List<WorldMapEntity> listWorldMap = WorldMapDBModel.Instance.GetList();
        if (m_WorldMapSceneDic == null)
        {
            m_WorldMapSceneDic = new Dictionary<int, WorldMapSceneEntity>();
            for (int i = 0; i < listWorldMap.Count; i++)
            {
                m_WorldMapSceneDic[listWorldMap[i].Id] = new WorldMapSceneEntity()
                {
                    Id = listWorldMap[i].Id,
                    NearScene = listWorldMap[i].NearScene,
                    IsVisit = false
                };
            }
            SceneIdQueue = new Queue<int>();
            m_WorldMapSceneList = new List<int>();
        }

        m_BeginSceneId = beginSceneId;
        m_TargetSceneId = endSceneId;
        SceneIdQueue.Clear();
        m_IsFindOver = false;

        //�ֵ䳡������Ϊδ����
        for (int i = 0; i < listWorldMap.Count; i++)
        {
            m_WorldMapSceneDic[listWorldMap[i].Id].IsVisit = false;
            m_WorldMapSceneDic[listWorldMap[i].Id].Parent = null;
        }
        //��ʼѰ·�㷨����Ѱ·
        CalculateTargetScenePath(m_BeginSceneId);

        if (m_TargetScene != null)
        {
            m_WorldMapSceneList.Clear();
            GetParentScene(m_TargetScene);
        }

        for (int i = m_WorldMapSceneList.Count - 1; i >= 0; i--)
        {
            SceneIdQueue.Enqueue(m_WorldMapSceneList[i]);
        }
        //�Ѿ����������Ѱ·���
        if (SceneIdQueue.Count >= 2)
        {
            IsAutoMove = true;
            CurrentSceneId = SceneIdQueue.Dequeue();
            ToSceneId = SceneIdQueue.Dequeue();
            //�رյ�ǰ�ĵ�ͼ����
            m_UIWorldMapView.Close();
            if (WorldMapSceneCtrl.Instance != null)
            {
                WorldMapSceneCtrl.Instance.AutoMove();
            }
        }
    }

    /// <summary>
    /// ��ȡ��ǰ���ĸ����
    /// </summary>
    /// <param name="entity"></param>
    private void GetParentScene(WorldMapSceneEntity entity)
    {
        m_WorldMapSceneList.Add(entity.Id);
        if (entity.Parent != null)
        {
            GetParentScene(entity.Parent);
        }
    }
}



/// <summary>
/// ���������ͼѰ·ʵ��
/// </summary>
public class WorldMapSceneEntity
{
    /// <summary>
    /// ���
    /// </summary>
    public int Id;
    /// <summary>
    /// ���ڳ���
    /// </summary>
    public string NearScene;
    /// <summary>
    /// �Ƿ������ʹ�
    /// </summary>
    public bool IsVisit;
    /// <summary>
    /// �����
    /// </summary>
    public WorldMapSceneEntity Parent;
}
