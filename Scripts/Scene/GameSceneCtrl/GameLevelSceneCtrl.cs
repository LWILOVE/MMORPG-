using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�ؿ�����������
/// </summary>
public class GameLevelSceneCtrl : GameSceneCtrlBase
{
    #region ����
    /// <summary>
    /// ����
    /// </summary>
    public static GameLevelSceneCtrl Instance { get; private set; }
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }
    #endregion

    #region ����
    /// <summary>
    /// �ؿ�����
    /// </summary>
    [SerializeField]
    private GameLevelRegionCtrl[] AllRegion;

    /// <summary>
    /// ��ǰ���������������
    /// </summary>
    private int m_CurrentRegionIndex;

    /// <summary>
    /// ��ǰ��Ϸ�ؿ����
    /// </summary>
    private int m_CurrentGameLevelId;
    /// <summary>
    /// ����е������б�
    /// </summary>
    private List<GameLevelRegionEntity> m_RegionList;

    /// <summary>
    /// ��ǰ�ؿ��Ĺֵ�����
    /// </summary>
    private int m_AllMonsterCount;

    /// <summary>
    /// ��ǰ�ؿ��ֵ�����
    /// </summary>
    private int[] m_MonsterId;

    /// <summary>
    /// ��ǰ���򴴽��Ĺֵ�����
    /// </summary>
    private int m_CurrentRegionCreateMonsterCount;

    /// <summary>
    /// ��ǰ�����ɱ�Ĺֵ�����
    /// </summary>
    private int m_CurrentRegionkillMonsterCount;

    /// <summary>
    /// �����
    /// </summary>
    private SpawnPool m_MonsterPool;

    /// <summary>
    /// ��ǰ�ؿ��Ѷ�
    /// </summary>
    private GameLevelGrade m_CurrentGrade;

    /// <summary>
    /// ��ǰ����ֵ�����
    /// </summary>
    private int m_CurrentRegionMonsterCount;

    /// <summary>
    /// ��ǰ���������
    /// </summary>
    [SerializeField]
    private GameLevelRegionCtrl m_CurrentRegionCtrl;

    /// <summary>
    /// �´�ˢ��ʱ��
    /// </summary>
    private float m_NextCreateMonsterTime = 0;

    /// <summary>
    /// ��ǰ��������
    /// </summary>
    private int m_CurrentRegionId;

    /// <summary>
    /// ���ս��ʱ��
    /// </summary>
    private float m_FightTime = 0;

    /// <summary>
    /// �Ƿ���ս����
    /// </summary>
    private bool m_IsFighting;

    /// <summary>
    /// ��ǰ�����Ƿ��й�
    /// </summary>
    public bool CurrRegionHasMonster
    {
        get { return m_CurrentRegionkillMonsterCount < m_CurrentRegionMonsterCount; }
    }

    /// <summary>
    /// ��ǰ�����Ƿ������һ������
    /// </summary>
    public bool CurrRegionIsLast
    {
        get
        {
            return m_CurrentRegionIndex >= m_RegionList.Count ;
        }
    }

    /// <summary>
    /// ��ǰ����ֵ� (����,����)
    /// </summary>
    [SerializeField]
    private Dictionary<int, int> m_RegionMonsterDic;

    /// <summary>
    /// ��һ�������ǵĳ�����
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
            int regionId = 5-entity.RegionId;//���������Ƶ��ɳ���ǣ�����ֻ�ܵ�����,�������һ��������Ҫ+1
            Debug.Log("����" + regionId);
            return GetRegionCtrlByRegionId(regionId).transform.position;
        }
    }


    private int m_Index = 0;
    /// <summary>
    /// �������ʱ��ID������Ŀǰû�����ݿ⣬������ʱ�ã�
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
        //��ʼ��
        //��ȡ�ؿ���ź͹ؿ��Ѷȣ���������Ϣͬ��
        m_CurrentGameLevelId = UILoadingCtrl.Instance.CurrentGameLevelId;
        m_CurrentGrade = UILoadingCtrl.Instance.CurrentGameLevelGrade;
        GameLevelCtrl.Instance.CurrentGameLevelId = m_CurrentGameLevelId;
        GameLevelCtrl.Instance.CurrentGameLevelGrade = m_CurrentGrade;
        GameLevelEntity entity_temp = GameLevelDBModel.Instance.Get(m_CurrentGameLevelId);
        m_IsFighting = true;
        m_RegionMonsterDic = new Dictionary<int, int>();
        m_SpriteList = new List<int>();
        //�ؿ��ۼ���Ʒ�ؼ���
        GameLevelCtrl.Instance.currentGameLevelTotalExp = 0;
        GameLevelCtrl.Instance.currentGameLevelTotalGold = 0;
        GameLevelCtrl.Instance.currentGameLevelKillMonsterDic.Clear();
        GameLevelCtrl.Instance.currentGameLevelGetGoodsList.Clear();

        UILoadingCtrl.Instance.CurrentPlayerType = PlayerType.PVE;

    }

    /// <summary>
    /// ���Ǽ�����ɻص�
    /// </summary>
    /// <param name="obj"></param>
    protected override void OnLoadUIMainCityViewComplete(GameObject obj)
    {
        base.OnLoadUIMainCityViewComplete(obj);

        //�������Ͻ� ������Ϣ
        PlayerCtrl.Instance.SetMainCityRoleData();

        //������Ϸ�ؿ���ŷ�����������    
        m_RegionList = GameLevelRegionDBModel.Instance.GetListByGameLevelId(m_CurrentGameLevelId);

        //��ȡ�ؿ��¹ֵ�����
        m_AllMonsterCount = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterCount(m_CurrentGameLevelId, m_CurrentGrade);

        //��ȡ�ؿ��¹ֵ�����
        m_MonsterId = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterId(m_CurrentGameLevelId, m_CurrentGrade);

        //���������
        m_MonsterPool = PoolManager.Pools.Create("Monster");
        m_MonsterPool.group.parent = null;
        m_MonsterPool.group.localPosition = Vector3.zero;

        LoadMonster(0, OnLoadMonsterCallBack);

        ////����ÿ���ֵ�Ԥ���
        //for(int i = 0;i<m_MonsterId.Length;i++)
        //{
        //    PrefabPool prefabPool = new PrefabPool(UISceneCtrl.Instance.LoadSprite(m_MonsterId[i]).transform);
        //    prefabPool.preloadAmount = 5;//Ԥ��������
        //    prefabPool.cullDespawned = true;//�����Ƿ���������Զ�����ģʽ
        //    prefabPool.cullAbove = 5;//����ػᱣ���Ķ�����
        //    prefabPool.cullDelay = 2;//����������ʱ����
        //    prefabPool.cullMaxPerPass = 2;//�����ÿ�������Ԥ������
        //    m_MonsterPool.CreatePrefabPool(prefabPool);//����ش����ɹ�
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
            //����ˢ��
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

    #region ������� OnDrawGizmos EnterRegion GetGameLevelRegionEntityByIndex GetRegionCtrlByRegionId
#if UNITY_EDITOR 
    /// <summary>
    /// �ڱ༭���������UI
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
    /// ������뷽��
    /// </summary>
    /// <param name="regionIndex"></param>
    private void EnterRegion(int regionIndex)
    {
        //�������������ţ���ȡ����������ʵ��
        GameLevelRegionEntity entity = GetGameLevelRegionEntityByIndex(regionIndex);
        if (entity == null)
        {
            return;
        }

        //��ȡ����ID
        int regionId = entity.RegionId;
        //��ȡ���������
        m_CurrentRegionCtrl = GetRegionCtrlByRegionId(regionIndex);

        if (m_CurrentRegionCtrl == null)
        {
            Debug.Log("�����Ƿ����ָ����ŵ�����"  + regionIndex);
            return;
        }

        m_CurrentRegionCreateMonsterCount = 0;
        m_CurrentRegionkillMonsterCount = 0;

        //���ٱ�������ڵ���
        if (m_CurrentRegionCtrl.RegionMask != null)
        {
            Destroy(m_CurrentRegionCtrl.RegionMask);
        }

        //��ǰ����һ�������
        if (regionIndex != 0)
        {
            //ʵ���ϴε�������
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
        //����ǰλ����ʼ�����������������
        if (regionIndex == 0)
        {
            if (GlobalInit.Instance.currentPlayer != null)
            {
                //���ǳ���
                GlobalInit.Instance.currentPlayer.Born(m_CurrentRegionCtrl.RoleBornPos.position);
                for (int i = 0; i < 10000; i++)
                { }
                //�����ǽ���ս������ģʽ
                GlobalInit.Instance.currentPlayer.ToIdle(RoleIdleState.IdleFight);
                //������ɫ����ί��
                GlobalInit.Instance.currentPlayer.OnRoleDie = (RoleCtrl ctrl) =>
                {
                    StartCoroutine(ShowFailView());
                };
            }
        }
        //ˢ����
        //��ǰ����ֵ�����ע�������ɳ��̳��У������ǵ���ģ�����ĩλ��ʼ
        m_CurrentRegionMonsterCount = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterCount(m_CurrentGameLevelId, m_CurrentGrade, regionId);
        //��ǰ����ÿ�ֵֹ�����
        List<GameLevelMonsterEntity> list = GameLevelMonsterDBModel.Instance.GetGameLevelMonster(m_CurrentGameLevelId, m_CurrentGrade, regionId);
        Debug.Log("����" + regionId + " �֣�" + m_CurrentRegionMonsterCount + " �����" + list.Count);
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
    /// �������������Ż�ȡ����ʵ��
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
    /// ���������Ż�ȡ���������
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

    #region ����������� OnRoleDieCallBack OnRoleDestroyCallBack
    /// <summary>
    /// ��ɫ�����ص�
    /// </summary>
    /// <param name="roldId"></param>
    private void OnRoleDieCallBack(RoleCtrl ctrl)
    {
        m_CurrentRegionkillMonsterCount++;
        //����ֵ���ľ���ֵ�ͽ��
        RoleInfoMonster monsterInfo = (RoleInfoMonster)ctrl.CurrentRoleInfo;
        GameLevelMonsterEntity entity = GameLevelMonsterDBModel.Instance.GetGameLevelMonsterEntity(m_CurrentGameLevelId, m_CurrentGrade, m_CurrentRegionId, monsterInfo.SpriteEntity.Id);
        if (entity != null) 
        {
            if (entity.Exp > 0)
            {
                UITipView.Instance.ShowTip(0, string.Format("+{0}", entity.Exp));
                GameLevelCtrl.Instance.currentGameLevelTotalExp += entity.Exp;
                //���������
                GlobalInit.Instance.currentPlayer.CurrentRoleInfo.CurrExp +=entity.Exp;
            }
            
            if (entity.Gold > 0)
            {
                UITipView.Instance.ShowTip(1, string.Format("+{0}", entity.Gold));
                GameLevelCtrl.Instance.currentGameLevelTotalGold += entity.Gold;
                //���������
                UIMainCityRoleInfoView.Instance.SetGold(entity.Gold);
                GlobalInit.Instance.currentPlayer.CurrentRoleInfo.Gold += entity.Gold;

            }
        }

        //ͳ��ˢ������
        if (GameLevelCtrl.Instance.currentGameLevelKillMonsterDic.ContainsKey(monsterInfo.SpriteEntity.Id))
        {
            GameLevelCtrl.Instance.currentGameLevelKillMonsterDic[monsterInfo.SpriteEntity.Id] += 1;
        }
        else
        {
            GameLevelCtrl.Instance.currentGameLevelKillMonsterDic[monsterInfo.SpriteEntity.Id] = 1;
        }

        //����ɱ�Ĺֵ��������ڵ��ڵ�ǰ����ֵ�����������һ����OR�س�
        if (m_CurrentRegionkillMonsterCount >= m_CurrentRegionMonsterCount)
        {
            m_CurrentRegionIndex++;
            if (CurrRegionIsLast)
            {
                m_IsFighting = false;
                GameLevelCtrl.Instance.CurrentGameLevelPassTime = m_FightTime;
                //����ʤ��TODO
                TimeMgr.Instance.ChangeTimeScale(0.5f, 2f);
                StartCoroutine(ShowVictory());
                return;
            }
            //������һ��
            EnterRegion(m_CurrentRegionIndex);
        }
    }

    /// <summary>
    /// ��ɫ���ٻص�
    /// </summary>
    /// <param name="obj"></param>
    private void OnRoleDestroyCallBack(Transform obj)
    {
        //��ɫ����
        //���лس�
        m_MonsterPool.Despawn(obj);
    }
    #endregion

    #region ������� LoadMonster OnLoadMonsterCallBack CreateMonster
    /// <summary>
    /// �ݹ���ع���
    /// </summary>
    /// <param name="index">�±�</param>
    /// <param name="onComplete">���ί��</param>
    private void LoadMonster(int index, System.Action onComplete)
    {
        int monsterId = m_MonsterId[index];
        UILoadingCtrl.Instance.LoadMonsterSprite(monsterId,
            (GameObject obj) =>
            {
                PrefabPool prefabPool = new PrefabPool(obj.transform);
                prefabPool.preloadAmount = 5;//Ԥ��������
                m_MonsterPool.CreatePrefabPool(prefabPool);//����ش����ɹ�
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
    /// ����������ί��
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    private void OnLoadMonsterCallBack()
    {
        //���������ɺ�����Ͷ�ŵ���һ����
        m_CurrentRegionIndex = 0;
        EnterRegion(m_CurrentRegionIndex);
    }

    /// <summary>
    /// ˢ����
    /// </summary>
    private void CreateMonster()
    {
        m_SpriteList.Clear();
        foreach (var item in m_RegionMonsterDic)
        {
            m_SpriteList.Add(item.Key);
        }
        //�����ȡһ�ֹ�����п�¡
        m_Index = Random.Range(0, m_RegionMonsterDic.Count);
        int monsterId = m_SpriteList[m_Index];

        //�Ӷ������ȡ��������ҪԤ��
        //�ӻ������ȡ��
        Transform transMonster = m_MonsterPool.Spawn(SpriteDBModel.Instance.Get(monsterId).PrefabName);
        //���ַ������һ�������㴦
        Transform monsterBornPos = m_CurrentRegionCtrl.MonsterBornPos[Random.Range(0, m_CurrentRegionCtrl.MonsterBornPos.Length)];
        //��ȡ����Ŀ�����
        RoleCtrl roleMonsterCtrl = transMonster.GetComponent<RoleCtrl>();
        //���ùֵĻ�����Ϣ
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

        //��ʼ������
        roleMonsterCtrl.Init(RoleType.Monster, monsterInfo, new GameLevel_RoleMonsterAI(roleMonsterCtrl, monsterInfo));
        roleMonsterCtrl.OnRoleDestroy = OnRoleDestroyCallBack;
        roleMonsterCtrl.OnRoleDie = OnRoleDieCallBack;

        //�ֳ���
        roleMonsterCtrl.Born(monsterBornPos.position);

        //��ͬʱ����һ��Ҫ���ɵĸ����͵Ĺ�
        m_RegionMonsterDic[monsterId]--;
        if (m_RegionMonsterDic[monsterId] <= 0)
        {
            m_RegionMonsterDic.Remove(monsterId);
        }
        m_CurrentRegionCreateMonsterCount++;
    }


    #endregion

    /// <summary>
    /// ��ɫʤ��Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowVictory()
    {
        yield return new WaitForSeconds(3f);
        GameLevelCtrl.Instance.OpenView(WindowUIType.GameLevelVictory);
    }

    /// <summary>
    /// ��ɫʧ��Э��
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowFailView()
    {
        yield return new WaitForSeconds(3f);
        GameLevelCtrl.Instance.OpenView(WindowUIType.GameLevelFail);
    }

}
