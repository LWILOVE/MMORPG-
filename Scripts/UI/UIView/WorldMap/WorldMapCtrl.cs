using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 世界地图控制器
/// </summary>
public class WorldMapCtrl : SystemCtrlBase<WorldMapCtrl>, ISystemCtrl
{
    /// <summary>
    /// 世界地图窗口
    /// </summary>
    private UIWorldMapView m_UIWorldMapView;
    /// <summary>
    /// 世界地图失败窗口
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
    /// 打开世界地图窗口
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void OpenWorldMapView()
    {
        //从本地世界地图表获取数据
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
            //如不在地图上显示，则跳过
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
    /// 世界地图场景点击
    /// </summary>
    /// <param name="obj"></param>
    private void onWorldMapItemClick(int worldId)
    {
        ////直接传送
        //UILoadingCtrl.Instance.LoadToWorldMap(worldId);
        //按路传送
        CalculateTargetScenePath(UILoadingCtrl.Instance.CurrentWorldMapId, worldId);
    }

    /// <summary>
    /// 打开地图失败窗口
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
                     Debug.LogWarning("正在尝试复活");
                     //玩家点击复活按钮后，将玩家渴望复活的消息发送给服务器
                     WorldMap_CurrRoleResurgenceProto proto = new WorldMap_CurrRoleResurgenceProto();
                     proto.Type = 0;//复活种类
                     NetWorkSocket.Instance.SendMessage(proto.ToArray());
                     m_UIWorldMapFailView.Close();
                     //m_UIWorldMapFailView.gameObject.SetActive(false);
                 };
             });
    }

    /// <summary>
    /// 起始场景ID
    /// </summary>
    private int m_BeginSceneId;
    /// <summary>
    /// 结束场景ID
    /// </summary>
    private int m_TargetSceneId;
    /// <summary>
    /// 临时进行数据存储的列表
    /// </summary>
    private List<int> m_WorldMapSceneList;
    /// <summary>
    /// 目标场景  
    /// </summary>
    private WorldMapSceneEntity m_TargetScene;

    /// <summary>
    /// 场景ID队列
    /// </summary>
    public Queue<int> SceneIdQueue;

    private Dictionary<int, WorldMapSceneEntity> m_WorldMapSceneDic;

    /// <summary>
    /// 树查找是否结束
    /// </summary>
    private bool m_IsFindOver = false;
    /// <summary>
    /// 当前场景ID
    /// </summary>
    public int CurrentSceneId;
    /// <summary>
    /// 要前往的场景ID
    /// </summary>
    public int ToSceneId;
    /// <summary>
    /// 是否自动移动
    /// </summary>
    public bool IsAutoMove = false;
    //要前往的地点
    public Vector3 ToScenePos = Vector3.zero;
    
    /// <summary>
    /// PVP敌人名字
    /// </summary>
    public string EnemyNickName;

    /// <summary>
    /// 计算到达目标场景路径_算法层
    /// </summary>
    /// <param name="currentSceneId"></param>
    private void CalculateTargetScenePath(int currentSceneId)
    {
        if (!m_WorldMapSceneDic.ContainsKey(currentSceneId))
        { return; }
        //获取当前场景实体
        WorldMapSceneEntity entity = m_WorldMapSceneDic[currentSceneId];
        //拆分关联场景
        string[] arr = entity.NearScene.Split("_");
        for (int i = 0; i < arr.Length; i++)
        {
            if (m_IsFindOver)
            { continue; }
            //获取关联场景ID
            int sceneId = int.Parse(arr[i]);
            if (sceneId == m_BeginSceneId)
            { continue; }
            //根据关联场景ID获得实体,若实体已经访问过了，则返回
            WorldMapSceneEntity findScene = m_WorldMapSceneDic[sceneId];

            if (findScene.IsVisit)
            { continue; }
            findScene.IsVisit = true;
            //设置父结点时候 必须在IsVisit之后
            findScene.Parent = entity;
            //若找到目标场景，则跳出循环
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
    /// 计算到达目标场景路径
    /// </summary>
    /// <param name="beginSceneId">起点ID</param>
    /// <param name="endSceneId">终点ID</param>
    public void CalculateTargetScenePath(int beginSceneId, int endSceneId)
    {
        //获取表格数据
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

        //字典场景重置为未访问
        for (int i = 0; i < listWorldMap.Count; i++)
        {
            m_WorldMapSceneDic[listWorldMap[i].Id].IsVisit = false;
            m_WorldMapSceneDic[listWorldMap[i].Id].Parent = null;
        }
        //开始寻路算法进行寻路
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
        //已经计算出来了寻路结果
        if (SceneIdQueue.Count >= 2)
        {
            IsAutoMove = true;
            CurrentSceneId = SceneIdQueue.Dequeue();
            ToSceneId = SceneIdQueue.Dequeue();
            //关闭当前的地图窗口
            m_UIWorldMapView.Close();
            if (WorldMapSceneCtrl.Instance != null)
            {
                WorldMapSceneCtrl.Instance.AutoMove();
            }
        }
    }

    /// <summary>
    /// 获取当前结点的父结点
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
/// 进行世界读图寻路实体
/// </summary>
public class WorldMapSceneEntity
{
    /// <summary>
    /// 编号
    /// </summary>
    public int Id;
    /// <summary>
    /// 相邻场景
    /// </summary>
    public string NearScene;
    /// <summary>
    /// 是否曾访问过
    /// </summary>
    public bool IsVisit;
    /// <summary>
    /// 父结点
    /// </summary>
    public WorldMapSceneEntity Parent;
}
