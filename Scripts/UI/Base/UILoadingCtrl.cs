using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景加载控制器
/// </summary>
public class UILoadingCtrl : SingletonMiddle<UILoadingCtrl>
{
    #region 变量
    /// <summary>
    /// 当前场景UI
    /// </summary>
    public UISceneViewBase CurrentUIScene;

    /// <summary>
    /// 目标世界地图的传送点ID
    /// </summary>
    public int targetWorldMapTransWorld;

    /// <summary>
    /// 当前场景类型
    /// </summary>
    public SceneType CurrentSceneType
    {
        get;
        set;
    }

    /// <summary>
    /// 将要前往的世界地图场景
    /// </summary>
    public int m_WillToWorldMapId = 0;

    /// <summary>
    /// 玩家当前的状态
    /// </summary>
    public PlayerType CurrentPlayerType
    {
        get;
        set;
    }

    /// <summary>
    /// 场景是否允许进行战斗
    /// </summary>
    public bool isFightingScene
    {
        get;
        private set;
    }
    #endregion

    public UILoadingCtrl()
    {
        //服务器返回角色进入世界地图场景消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_RoleEnterReturn, OnWorldMapRoleEnterReturn);
    }

    /// <summary>
    /// 其他玩家加载方法
    /// </summary>
    /// <param name="roleId">角色ID</param>
    /// <param name="nickName">角色名</param>
    /// <param name="roleLevel">角色等级</param>
    /// <param name="jobId">角色职业</param>
    /// <param name="currHP">角色剩余生命值</param>
    /// <param name="maxHP">角色最大生命值</param>
    /// <param name="currMP">角色剩余蓝量</param>
    /// <param name="maxMP">角色最大蓝量</param>
    /// <returns></returns>
    public RoleCtrl LoadOtherRole(int roleId, string nickName, int roleLevel, int jobId, int currHP, int maxHP, int currMP, int maxMP,Vector3 pos)
    {
        GameObject roleObj = Object.Instantiate(GlobalInit.Instance.JobObjectDic[jobId],pos,Quaternion.identity);
        RoleCtrl roleCtrl = roleObj.GetComponent<RoleCtrl>();

        RoleInfoMainPlayer roleInfo = new RoleInfoMainPlayer();
        roleInfo.RoldId = roleId;
        roleInfo.RoleNickName = nickName;
        roleInfo.Level = roleLevel;
        roleInfo.JobId = (byte)roleId;
        roleInfo.MaxHP = maxHP;
        roleInfo.MaxMP = maxMP;
        roleInfo.CurrHP = currHP;
        roleInfo.CurrMP = currMP;

        roleCtrl.Init(RoleType.OtherPlayer, roleInfo, new OtherRoleAI(roleCtrl));

        UITipView.Instance.ShowTip(2, string.Format("{0}进场了",nickName));
        return roleCtrl;
    }

    /// <summary>
    /// 服务器返回角色进入世界场景消息
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMapRoleEnterReturn(byte[] buffer)
    {
        WorldMap_RoleEnterReturnProto proto = WorldMap_RoleEnterReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            m_CurrWorldMapId = CurrentWorldMapId;
            CurrentSceneType = SceneType.MainCity;
            CurrentPlayerType = PlayerType.PVP;
            WorldMapEntity entity = WorldMapDBModel.Instance.Get(m_CurrWorldMapId);
            if (entity != null)
            {
                isFightingScene = entity.IsCity == 0;
            }
            LoadToLoading();
        }
    }
    /// <summary>
    /// 加载登录窗口
    /// </summary>
    public void LoadToLogOn()
    {
        CurrentSceneType = SceneType.LogOn;
        LoadToLoading();
    }


    /// <summary>
    /// 当前所在世界地图编号
    /// </summary>
    private int m_CurrWorldMapId;
    public int CurrentWorldMapId { get { return m_CurrWorldMapId; } }

    /// <summary>
    /// 去世界地图场景（主城+野外场景）
    /// 主城是4
    /// </summary>
    /// <param name="worldMapId">世界编号</param>
    public void LoadToWorldMap(int worldMapId)
    {
        //因为主城是4，在数据表中是1，所以其他自然要+3
        Debug.Log("当前场景编号是："+ m_CurrWorldMapId+"  正在前往的世界编号是：" + worldMapId + " 世界名：" + ((SceneType)(worldMapId+3)).ToString());
        if (m_CurrWorldMapId == worldMapId)
        {
            MessageCtrl.Instance.Show("你已经在这个场景了，请不要乱传送哦！");
            return;
        }

        m_CurrWorldMapId = worldMapId;
        //+3是为了能够正确传送
        CurrentSceneType = (SceneType)(worldMapId+3);
        LoadToLoading();
    }

    /// <summary>
    /// 客户端发送进入世界地图消息
    /// </summary>
    /// <param name="worldMapId"></param>
    private void WorldMapRoleEnter(int worldMapId)
    {
        m_WillToWorldMapId = worldMapId;
        WorldMap_RoleEnterProto proto = new WorldMap_RoleEnterProto();
        proto.WorldMapSceneId = m_WillToWorldMapId;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }

    /// <summary>
    /// 进入选人场景
    /// </summary>
    public void LoadToSelectRole()
    {
        CurrentSceneType = SceneType.SelectRole;
        LoadToLoading();
    }


    public void LoadSceneUI(string path, XLuaCustomExport.OnCreate OnCreate)
    {
        LoadSceneUI(SceneUIType.None, null, OnCreate, path);
    }

    /// <summary>
    /// 加载场景UI_Root_X系列场景
    /// </summary>
    /// <param name="type">场景类别</param>
    /// <param name="OnLoadComplete">加载完成回调</param>
    /// <param name="OnCreate">XLUA创建方法</param>
    /// <param name="path">download短路径</param>
    public void LoadSceneUI(SceneUIType type, System.Action<GameObject> OnLoadComplete = null, XLuaCustomExport.OnCreate OnCreate = null, string path = null)
    {
        string strUIName = string.Empty;
        string newPath = string.Empty;
        if (type != SceneUIType.None)
        {
            switch (type)
            {
                case SceneUIType.LogOn:
                    strUIName = "UI_Root_LogOn";
                    break;
                case SceneUIType.SelectRole:
                    strUIName = "UI_Root_SelectRole";
                    break;
                case SceneUIType.MainCity:
                    strUIName = "UI_Root_MainCity";
                    break;
            }

            newPath = string.Format("download/prefab/uiprefab/uiroot/{0}.assetbundle", strUIName);
            
        }
        else
        {
            newPath = path;
        }
        Debug.Log("场景基本UI路径布置完成"+newPath);  

        AssetBundleMgr.Instance.LoadOrDownload(newPath, strUIName,
            (GameObject obj) =>
            {
                obj = Object.Instantiate(obj);
                CurrentUIScene = obj.GetComponent<UISceneViewBase>();
                if (OnLoadComplete != null)
                {
                    OnLoadComplete(obj);
                }
                if (OnCreate != null)
                {
                    //若是从lua中加载的
                    obj.GetOrCreatComponent<LuaViewBehaviour>();
                    OnCreate(obj);
                }
            });
    }

    /// <summary>
    /// 当前关卡ID
    /// </summary>
    private int m_CurrentGameLevelId;
    public int CurrentGameLevelId
    {
        get { return m_CurrentGameLevelId; }
    }

    /// <summary>
    /// 当前关卡难度
    /// </summary>
    private GameLevelGrade m_CurrentGameLevelGrade;
    public GameLevelGrade CurrentGameLevelGrade
    {
        get { return m_CurrentGameLevelGrade; }
    }

    /// <summary>
    /// 游戏关卡加载
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    public void LoadToGameLevel(int gameLevelId, GameLevelGrade grade)
    {
        m_CurrentGameLevelId = gameLevelId;
        m_CurrentGameLevelGrade = grade;
        //跳到指定场景
        CurrentSceneType = SceneType.ShanGu;
        m_CurrWorldMapId = (int)CurrentSceneType;
        LoadToLoading();
    }

    /// <summary>
    /// 根据精灵编号返回精灵预设镜像
    /// </summary>
    /// <param name="spriteId"></param>
    /// <returns></returns>
    public void LoadMonsterSprite(int spriteId, System.Action<GameObject> onComplete)
    {
        SpriteEntity entity = SpriteDBModel.Instance.Get(spriteId);
        if (entity == null)
        {
            return;
        }

        AssetBundleMgr.Instance.LoadOrDownload(string.Format("Download/Prefab/RolePrefab/Monster/{0}.assetbundle", entity.PrefabName), entity.PrefabName, onComplete);
    }

    /// <summary>
    /// 加载NPC
    /// </summary>
    /// <param name="prefabName">.../DownLoad/...NPC/xxx的xxx</param>
    /// <returns></returns>
    public void LoadNPC(string prefabName, System.Action<GameObject> onComplete)
    {
        Debug.Log("正在加载NPC："+prefabName);
        AssetBundleMgr.Instance.LoadOrDownload(string.Format("Download/Prefab/RolePrefab/NPC/{0}.assetbundle", prefabName), prefabName,
            (GameObject obj) =>
            {
                if (onComplete != null)
                {
                    onComplete(Object.Instantiate(obj));
                }
            });
    }

    [System.Obsolete]
    /// <summary>
    /// 加载技能图片
    /// </summary>
    /// <param name="skillPic"></param>
    /// <returns></returns>
    public Sprite LoadSkillPic(string skillPic)
    {
        Sprite sprite = null;
        AssetBundleMgr.Instance.LoadOrDownload(string.Format("Download/Source/UISource/Skill/{0}.assetbundle", skillPic), skillPic,
    (Sprite obj) =>
    {
         sprite= obj;
    },type:1);
        return sprite;
    }

    /// <summary>
    /// 加载到加载场景
    /// </summary>
    private void LoadToLoading()
    {
        SceneManager.LoadScene(1);
    }
}
