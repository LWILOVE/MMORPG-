using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡场景控制器
/// </summary>
public class GameLevelSceneCtrl : GameSceneCtrlBase
{
    #region 单例
    /// <summary>
    /// 单例
    /// </summary>
    public static GameLevelSceneCtrl Instance { get; private set; }
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }
    #endregion

    #region 变量
    /// <summary>
    /// 关卡区域
    /// </summary>
    [SerializeField]
    private GameLevelRegionCtrl[] AllRegion;

    /// <summary>
    /// 当前进入的区域索引号
    /// </summary>
    private int m_CurrentRegionIndex;

    /// <summary>
    /// 当前游戏关卡编号
    /// </summary>
    private int m_CurrentGameLevelId;
    /// <summary>
    /// 表格中的区域列表
    /// </summary>
    private List<GameLevelRegionEntity> m_RegionList;

    /// <summary>
    /// 当前关卡的怪的总数
    /// </summary>
    private int m_AllMonsterCount;

    /// <summary>
    /// 当前关卡怪的类型
    /// </summary>
    private int[] m_MonsterId;

    /// <summary>
    /// 当前区域创建的怪的数量
    /// </summary>
    private int m_CurrentRegionCreateMonsterCount;

    /// <summary>
    /// 当前区域击杀的怪的数量
    /// </summary>
    private int m_CurrentRegionkillMonsterCount;

    /// <summary>
    /// 怪物池
    /// </summary>
    private SpawnPool m_MonsterPool;

    /// <summary>
    /// 当前关卡难度
    /// </summary>
    private GameLevelGrade m_CurrentGrade;

    /// <summary>
    /// 当前区域怪的数量
    /// </summary>
    private int m_CurrentRegionMonsterCount;

    /// <summary>
    /// 当前区域控制器
    /// </summary>
    [SerializeField]
    private GameLevelRegionCtrl m_CurrentRegionCtrl;

    /// <summary>
    /// 下次刷怪时间
    /// </summary>
    private float m_NextCreateMonsterTime = 0;

    /// <summary>
    /// 当前的区域编号
    /// </summary>
    private int m_CurrentRegionId;

    /// <summary>
    /// 玩家战斗时间
    /// </summary>
    private float m_FightTime = 0;

    /// <summary>
    /// 是否处于战斗中
    /// </summary>
    private bool m_IsFighting;

    /// <summary>
    /// 当前区域是否有怪
    /// </summary>
    public bool CurrRegionHasMonster
    {
        get { return m_CurrentRegionkillMonsterCount < m_CurrentRegionMonsterCount; }
    }

    /// <summary>
    /// 当前区域是否是最后一个区域
    /// </summary>
    public bool CurrRegionIsLast
    {
        get
        {
            return m_CurrentRegionIndex >= m_RegionList.Count ;
        }
    }

    /// <summary>
    /// 当前区域怪的 (类型,数量)
    /// </summary>
    [SerializeField]
    private Dictionary<int, int> m_RegionMonsterDic;

    /// <summary>
    /// 下一区域主角的出生点
    /// </summary>
    public Vector3 NextRegionPlayerBornPos
    {
        get
        {
            GameLevelRegionEntity entity = GetGameLevelRegionEntityByIndex(m_CurrentRegionIndex);
            if (entity == null)
            {
                return Vector3.zero;
            }
            int regionId = 5-entity.RegionId;//鉴于这个视频的沙雕标记，所以只能倒着来,外加是下一区域所以要+1
            Debug.Log("区域：" + regionId);
            return GetRegionCtrlByRegionId(regionId).transform.position;
        }
    }


    private int m_Index = 0;
    /// <summary>
    /// 怪物的临时用ID（由于目前没接数据库，所以暂时用）
    /// </summary>
    private int m_MonsterTempId = 0;

    /// <summary>
    /// 
    /// </summary>
    private List<int> m_SpriteList;
    #endregion

    protected override void OnStart()
    {
        base.OnStart();
        //初始化
        //获取关卡编号和关卡难度，并进行信息同步
        m_CurrentGameLevelId = UILoadingCtrl.Instance.CurrentGameLevelId;
        m_CurrentGrade = UILoadingCtrl.Instance.CurrentGameLevelGrade;
        GameLevelCtrl.Instance.CurrentGameLevelId = m_CurrentGameLevelId;
        GameLevelCtrl.Instance.CurrentGameLevelGrade = m_CurrentGrade;
        GameLevelEntity entity_temp = GameLevelDBModel.Instance.Get(m_CurrentGameLevelId);
        m_IsFighting = true;
        m_RegionMonsterDic = new Dictionary<int, int>();
        m_SpriteList = new List<int>();
        //关卡累计物品重计算
        GameLevelCtrl.Instance.currentGameLevelTotalExp = 0;
        GameLevelCtrl.Instance.currentGameLevelTotalGold = 0;
        GameLevelCtrl.Instance.currentGameLevelKillMonsterDic.Clear();
        GameLevelCtrl.Instance.currentGameLevelGetGoodsList.Clear();

        UILoadingCtrl.Instance.CurrentPlayerType = PlayerType.PVE;

    }

    /// <summary>
    /// 主城加载完成回调
    /// </summary>
    /// <param name="obj"></param>
    protected override void OnLoadUIMainCityViewComplete(GameObject obj)
    {
        base.OnLoadUIMainCityViewComplete(obj);

        //设置左上角 基本信息
        PlayerCtrl.Instance.SetMainCityRoleData();

        //根据游戏关卡编号返回所有区域    
        m_RegionList = GameLevelRegionDBModel.Instance.GetListByGameLevelId(m_CurrentGameLevelId);

        //获取关卡下怪的总量
        m_AllMonsterCount = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterCount(m_CurrentGameLevelId, m_CurrentGrade);

        //获取关卡下怪的种类
        m_MonsterId = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterId(m_CurrentGameLevelId, m_CurrentGrade);

        //创建怪物池
        m_MonsterPool = PoolManager.Pools.Create("Monster");
        m_MonsterPool.group.parent = null;
        m_MonsterPool.group.localPosition = Vector3.zero;

        LoadMonster(0, OnLoadMonsterCallBack);

        ////创建每个怪的预设池
        //for(int i = 0;i<m_MonsterId.Length;i++)
        //{
        //    PrefabPool prefabPool = new PrefabPool(UISceneCtrl.Instance.LoadSprite(m_MonsterId[i]).transform);
        //    prefabPool.preloadAmount = 5;//预加载数量
        //    prefabPool.cullDespawned = true;//设置是否开启缓存池自动清理模式
        //    prefabPool.cullAbove = 5;//缓存池会保留的对象数
        //    prefabPool.cullDelay = 2;//缓存池清理的时间间隔
        //    prefabPool.cullMaxPerPass = 2;//缓存池每次清理的预设数量
        //    m_MonsterPool.CreatePrefabPool(prefabPool);//对象池创建成功
        //}

        //m_CurrentRegionIndex = 0;
        //EnterRegion(m_CurrentRegionIndex);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (m_IsFighting)
        {
            m_FightTime += Time.deltaTime;
            //进行刷怪
            if (m_CurrentRegionCreateMonsterCount < m_CurrentRegionMonsterCount)
            {
                if (Time.time > m_NextCreateMonsterTime)
                {
                    m_NextCreateMonsterTime = Time.time + 1f;
                    CreateMonster();
                }
            }
        }
    }

    #region 区域相关 OnDrawGizmos EnterRegion GetGameLevelRegionEntityByIndex GetRegionCtrlByRegionId
#if UNITY_EDITOR 
    /// <summary>
    /// 在编辑器界面绘制UI
    /// </summary>
    public void OnDrawGizmos()
    {
        if (AllRegion != null && AllRegion.Length > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(this.transform.position, 5f);

            Gizmos.color = Color.cyan;
            for (int i = 0; i < AllRegion.Length; i++)
            {
                Gizmos.DrawLine(this.transform.position, AllRegion[i].gameObject.transform.position);
            }
        }
    }
#endif

    /// <summary>
    /// 区域进入方法
    /// </summary>
    /// <param name="regionIndex"></param>
    private void EnterRegion(int regionIndex)
    {
        //根据区域索引号，获取进入的区域的实体
        GameLevelRegionEntity entity = GetGameLevelRegionEntityByIndex(regionIndex);
        if (entity == null)
        {
            return;
        }

        //获取区域ID
        int regionId = entity.RegionId;
        //获取区域控制器
        m_CurrentRegionCtrl = GetRegionCtrlByRegionId(regionIndex);

        if (m_CurrentRegionCtrl == null)
        {
            Debug.Log("请检查是否存在指定编号的区域"  + regionIndex);
            return;
        }

        m_CurrentRegionCreateMonsterCount = 0;
        m_CurrentRegionkillMonsterCount = 0;

        //销毁本区域的遮挡物
        if (m_CurrentRegionCtrl.RegionMask != null)
        {
            Destroy(m_CurrentRegionCtrl.RegionMask);
        }

        //打开前往下一区域的门
        if (regionIndex != 0)
        {
            //实验上次的区域编号
            GameLevelDoorCtrl toNextRegionDoor = m_CurrentRegionCtrl.GetNextRegionDoor(regionIndex);
            if (toNextRegionDoor != null)
            {
                toNextRegionDoor.gameObject.SetActive(false);
                if (toNextRegionDoor.ConnectToDoor != null)
                {
                    toNextRegionDoor.ConnectToDoor.gameObject.SetActive(false);
                }
            }
        }

        m_CurrentRegionId = regionId;
        //若当前位于起始区域则设置其出生点
        if (regionIndex == 0)
        {
            if (GlobalInit.Instance.currentPlayer != null)
            {
                //主角出生
                GlobalInit.Instance.currentPlayer.Born(m_CurrentRegionCtrl.RoleBornPos.position);
                for (int i = 0; i < 10000; i++)
                { }
                //让主角进入战斗待机模式
                GlobalInit.Instance.currentPlayer.ToIdle(RoleIdleState.IdleFight);
                //监听角色死亡委托
                GlobalInit.Instance.currentPlayer.OnRoleDie = (RoleCtrl ctrl) =>
                {
                    StartCoroutine(ShowFailView());
                };
            }
        }
        //刷怪笼
        //当前区域怪的总数注：在这个沙雕教程中，区域是倒序的，即从末位开始
        m_CurrentRegionMonsterCount = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterCount(m_CurrentGameLevelId, m_CurrentGrade, regionId);
        //当前区域每种怪的数量
        List<GameLevelMonsterEntity> list = GameLevelMonsterDBModel.Instance.GetGameLevelMonster(m_CurrentGameLevelId, m_CurrentGrade, regionId);
        Debug.Log("区域：" + regionId + " 怪：" + m_CurrentRegionMonsterCount + " 怪类别：" + list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            if (m_RegionMonsterDic.ContainsKey(list[i].SpriteId))
            {
                m_RegionMonsterDic[list[i].SpriteId] += list[i].SpriteCount;
            }
            else
            {
                m_RegionMonsterDic[list[i].SpriteId] = list[i].SpriteCount;
            }
        }
    }

    /// <summary>
    /// 根据区域索引号获取区域实体
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private GameLevelRegionEntity GetGameLevelRegionEntityByIndex(int index)
    {
        for (int i = 0; i < m_RegionList.Count; i++)
        {
            if (i == index)
            {
                return m_RegionList[i];
            }
        }
        return null;
    }

    /// <summary>
    /// 根据区域编号获取区域控制器
    /// </summary>
    /// <param name="regionId"></param>
    /// <returns></returns>
    private GameLevelRegionCtrl GetRegionCtrlByRegionId(int regionId)
    {
        for (int i = 0; i < AllRegion.Length; i++)
        {
            if (AllRegion[i].RegionId == regionId)
            {
                return AllRegion[i];
            }
        }
        return null;
    }
    #endregion

    #region 怪物死亡相关 OnRoleDieCallBack OnRoleDestroyCallBack
    /// <summary>
    /// 角色死亡回调
    /// </summary>
    /// <param name="roldId"></param>
    private void OnRoleDieCallBack(RoleCtrl ctrl)
    {
        m_CurrentRegionkillMonsterCount++;
        //处理怪掉落的经验值和金币
        RoleInfoMonster monsterInfo = (RoleInfoMonster)ctrl.CurrentRoleInfo;
        GameLevelMonsterEntity entity = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterEntity(m_CurrentGameLevelId, m_CurrentGrade, m_CurrentRegionId, monsterInfo.SpriteEntity.Id);
        if (entity != null) 
        {
            if (entity.Exp > 0)
            {
                UITipView.Instance.ShowTip(0, string.Format("+{0}", entity.Exp));
                GameLevelCtrl.Instance.currentGameLevelTotalExp += entity.Exp;
                //共享到面板上
                GlobalInit.Instance.currentPlayer.CurrentRoleInfo.CurrExp +=entity.Exp;
            }
            
            if (entity.Gold > 0)
            {
                UITipView.Instance.ShowTip(1, string.Format("+{0}", entity.Gold));
                GameLevelCtrl.Instance.currentGameLevelTotalGold += entity.Gold;
                //共享到面板上
                UIMainCityRoleInfoView.Instance.SetGold(entity.Gold);
                GlobalInit.Instance.currentPlayer.CurrentRoleInfo.Gold += entity.Gold;

            }
        }

        //统计刷怪详情
        if (GameLevelCtrl.Instance.currentGameLevelKillMonsterDic.ContainsKey(monsterInfo.SpriteEntity.Id))
        {
            GameLevelCtrl.Instance.currentGameLevelKillMonsterDic[monsterInfo.SpriteEntity.Id] += 1;
        }
        else
        {
            GameLevelCtrl.Instance.currentGameLevelKillMonsterDic[monsterInfo.SpriteEntity.Id] = 1;
        }

        //若击杀的怪的数量大于等于当前区域怪的总量则开启下一区域OR回城
        if (m_CurrentRegionkillMonsterCount >= m_CurrentRegionMonsterCount)
        {
            m_CurrentRegionIndex++;
            if (CurrRegionIsLast)
            {
                m_IsFighting = false;
                GameLevelCtrl.Instance.CurrentGameLevelPassTime = m_FightTime;
                //宣判胜利TODO
                TimeMgr.Instance.ChangeTimeScale(0.5f, 2f);
                StartCoroutine(ShowVictory());
                return;
            }
            //开启下一关
            EnterRegion(m_CurrentRegionIndex);
        }
    }

    /// <summary>
    /// 角色销毁回调
    /// </summary>
    /// <param name="obj"></param>
    private void OnRoleDestroyCallBack(Transform obj)
    {
        //角色销毁
        //进行回池
        m_MonsterPool.Despawn(obj);
    }
    #endregion

    #region 怪物相关 LoadMonster OnLoadMonsterCallBack CreateMonster
    /// <summary>
    /// 递归加载怪物
    /// </summary>
    /// <param name="index">下标</param>
    /// <param name="onComplete">完成委托</param>
    private void LoadMonster(int index, System.Action onComplete)
    {
        int monsterId = m_MonsterId[index];
        UILoadingCtrl.Instance.LoadMonsterSprite(monsterId,
            (GameObject obj) =>
            {
                PrefabPool prefabPool = new PrefabPool(obj.transform);
                prefabPool.preloadAmount = 5;//预加载数量
                m_MonsterPool.CreatePrefabPool(prefabPool);//对象池创建成功
                index++;
                if (index == m_MonsterId.Length)
                {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                }
                else
                {
                    LoadMonster(index, onComplete);
                }
            });
    }

    /// <summary>
    /// 怪物加载完成委托
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnLoadMonsterCallBack()
    {
        //怪物加载完成后将他们投放到第一区域
        m_CurrentRegionIndex = 0;
        EnterRegion(m_CurrentRegionIndex);
    }

    /// <summary>
    /// 刷怪笼
    /// </summary>
    private void CreateMonster()
    {
        m_SpriteList.Clear();
        foreach (var item in m_RegionMonsterDic)
        {
            m_SpriteList.Add(item.Key);
        }
        //随机获取一种怪物进行克隆
        m_Index = Random.Range(0, m_RegionMonsterDic.Count);
        int monsterId = m_SpriteList[m_Index];

        //从对象池内取出我们需要预设
        //从缓存池中取怪
        Transform transMonster = m_MonsterPool.Spawn(SpriteDBModel.Instance.Get(monsterId).PrefabName);
        //将怪放在随机一个出生点处
        Transform monsterBornPos = m_CurrentRegionCtrl.MonsterBornPos[Random.Range(0, m_CurrentRegionCtrl.MonsterBornPos.Length)];
        //获取怪物的控制器
        RoleCtrl roleMonsterCtrl = transMonster.GetComponent<RoleCtrl>();
        //设置怪的基本信息
        RoleInfoMonster monsterInfo = new RoleInfoMonster();
        SpriteEntity entity = SpriteDBModel.Instance.Get(monsterId);
        if (entity != null)
        {
            monsterInfo.RoldId = ++m_MonsterTempId;
            monsterInfo.RoleNickName = entity.Name;
            monsterInfo.Level = entity.Level;
            monsterInfo.MaxHP = entity.HP;
            monsterInfo.CurrHP = monsterInfo.MaxHP;
            monsterInfo.MaxMP = entity.MP;
            monsterInfo.CurrMP = monsterInfo.MaxMP;
            monsterInfo.Attack = entity.Attack;
            monsterInfo.Defense = entity.Defense;
            monsterInfo.Hit = entity.Hit;
            monsterInfo.Dodge = entity.Dodge;
            monsterInfo.Cri = entity.Cri;
            monsterInfo.Res = entity.Res;
            monsterInfo.Fighting = entity.Fighting;

            monsterInfo.RoleNickName = entity.Name;
            monsterInfo.SpriteEntity = entity;
            roleMonsterCtrl.ViewRange = entity.Range_View;
            roleMonsterCtrl.Speed = entity.MoveSpeed;
        }

        //初始化怪物
        roleMonsterCtrl.Init(RoleType.Monster, monsterInfo, new GameLevel_RoleMonsterAI(roleMonsterCtrl, monsterInfo));
        roleMonsterCtrl.OnRoleDestroy = OnRoleDestroyCallBack;
        roleMonsterCtrl.OnRoleDie = OnRoleDieCallBack;

        //怪出生
        roleMonsterCtrl.Born(monsterBornPos.position);

        //并同时减少一个要生成的该类型的怪
        m_RegionMonsterDic[monsterId]--;
        if (m_RegionMonsterDic[monsterId] <= 0)
        {
            m_RegionMonsterDic.Remove(monsterId);
        }
        m_CurrentRegionCreateMonsterCount++;
    }


    #endregion

    /// <summary>
    /// 角色胜利协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowVictory()
    {
        yield return new WaitForSeconds(3f);
        GameLevelCtrl.Instance.OpenView(WindowUIType.GameLevelVictory);
    }

    /// <summary>
    /// 角色失败协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowFailView()
    {
        yield return new WaitForSeconds(3f);
        GameLevelCtrl.Instance.OpenView(WindowUIType.GameLevelFail);
    }

}
