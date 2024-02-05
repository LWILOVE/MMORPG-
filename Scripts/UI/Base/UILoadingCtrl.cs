using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �������ؿ�����
/// </summary>
public class UILoadingCtrl : SingletonMiddle<UILoadingCtrl>
{
    #region ����
    /// <summary>
    /// ��ǰ����UI
    /// </summary>
    public UISceneViewBase CurrentUIScene;

    /// <summary>
    /// Ŀ�������ͼ�Ĵ��͵�ID
    /// </summary>
    public int targetWorldMapTransWorld;

    /// <summary>
    /// ��ǰ��������
    /// </summary>
    public SceneType CurrentSceneType
    {
        get;
        set;
    }

    /// <summary>
    /// ��Ҫǰ���������ͼ����
    /// </summary>
    public int m_WillToWorldMapId = 0;

    /// <summary>
    /// ��ҵ�ǰ��״̬
    /// </summary>
    public PlayerType CurrentPlayerType
    {
        get;
        set;
    }

    /// <summary>
    /// �����Ƿ��������ս��
    /// </summary>
    public bool isFightingScene
    {
        get;
        private set;
    }
    #endregion

    public UILoadingCtrl()
    {
        //���������ؽ�ɫ���������ͼ������Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_RoleEnterReturn, OnWorldMapRoleEnterReturn);
    }

    /// <summary>
    /// ������Ҽ��ط���
    /// </summary>
    /// <param name="roleId">��ɫID</param>
    /// <param name="nickName">��ɫ��</param>
    /// <param name="roleLevel">��ɫ�ȼ�</param>
    /// <param name="jobId">��ɫְҵ</param>
    /// <param name="currHP">��ɫʣ������ֵ</param>
    /// <param name="maxHP">��ɫ�������ֵ</param>
    /// <param name="currMP">��ɫʣ������</param>
    /// <param name="maxMP">��ɫ�������</param>
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

        UITipView.Instance.ShowTip(2, string.Format("{0}������",nickName));
        return roleCtrl;
    }

    /// <summary>
    /// ���������ؽ�ɫ�������糡����Ϣ
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
    /// ���ص�¼����
    /// </summary>
    public void LoadToLogOn()
    {
        CurrentSceneType = SceneType.LogOn;
        LoadToLoading();
    }


    /// <summary>
    /// ��ǰ���������ͼ���
    /// </summary>
    private int m_CurrWorldMapId;
    public int CurrentWorldMapId { get { return m_CurrWorldMapId; } }

    /// <summary>
    /// ȥ�����ͼ����������+Ұ�ⳡ����
    /// ������4
    /// </summary>
    /// <param name="worldMapId">������</param>
    public void LoadToWorldMap(int worldMapId)
    {
        //��Ϊ������4�������ݱ�����1������������ȻҪ+3
        Debug.Log("��ǰ��������ǣ�"+ m_CurrWorldMapId+"  ����ǰ�����������ǣ�" + worldMapId + " ��������" + ((SceneType)(worldMapId+3)).ToString());
        if (m_CurrWorldMapId == worldMapId)
        {
            MessageCtrl.Instance.Show("���Ѿ�����������ˣ��벻Ҫ�Ҵ���Ŷ��");
            return;
        }

        m_CurrWorldMapId = worldMapId;
        //+3��Ϊ���ܹ���ȷ����
        CurrentSceneType = (SceneType)(worldMapId+3);
        LoadToLoading();
    }

    /// <summary>
    /// �ͻ��˷��ͽ��������ͼ��Ϣ
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
    /// ����ѡ�˳���
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
    /// ���س���UI_Root_Xϵ�г���
    /// </summary>
    /// <param name="type">�������</param>
    /// <param name="OnLoadComplete">������ɻص�</param>
    /// <param name="OnCreate">XLUA��������</param>
    /// <param name="path">download��·��</param>
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
        Debug.Log("��������UI·���������"+newPath);  

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
                    //���Ǵ�lua�м��ص�
                    obj.GetOrCreatComponent<LuaViewBehaviour>();
                    OnCreate(obj);
                }
            });
    }

    /// <summary>
    /// ��ǰ�ؿ�ID
    /// </summary>
    private int m_CurrentGameLevelId;
    public int CurrentGameLevelId
    {
        get { return m_CurrentGameLevelId; }
    }

    /// <summary>
    /// ��ǰ�ؿ��Ѷ�
    /// </summary>
    private GameLevelGrade m_CurrentGameLevelGrade;
    public GameLevelGrade CurrentGameLevelGrade
    {
        get { return m_CurrentGameLevelGrade; }
    }

    /// <summary>
    /// ��Ϸ�ؿ�����
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    public void LoadToGameLevel(int gameLevelId, GameLevelGrade grade)
    {
        m_CurrentGameLevelId = gameLevelId;
        m_CurrentGameLevelGrade = grade;
        //����ָ������
        CurrentSceneType = SceneType.ShanGu;
        m_CurrWorldMapId = (int)CurrentSceneType;
        LoadToLoading();
    }

    /// <summary>
    /// ���ݾ����ŷ��ؾ���Ԥ�辵��
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
    /// ����NPC
    /// </summary>
    /// <param name="prefabName">.../DownLoad/...NPC/xxx��xxx</param>
    /// <returns></returns>
    public void LoadNPC(string prefabName, System.Action<GameObject> onComplete)
    {
        Debug.Log("���ڼ���NPC��"+prefabName);
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
    /// ���ؼ���ͼƬ
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
    /// ���ص����س���
    /// </summary>
    private void LoadToLoading()
    {
        SceneManager.LoadScene(1);
    }
}
