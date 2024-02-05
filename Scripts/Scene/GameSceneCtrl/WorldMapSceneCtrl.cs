using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 世界地图场景主控制器
/// </summary>
public class WorldMapSceneCtrl : GameSceneCtrlBase
{
    #region 单例
    /// <summary>
    /// 单例
    /// </summary>
    public static WorldMapSceneCtrl Instance;
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }
    #endregion

    #region 变量
    /// <summary>
    /// 主角出生点(测试用出生点)
    /// </summary>
    [SerializeField]
    private Transform m_PlayerBornPos;

    /// <summary>
    /// 当前世界地图实体
    /// </summary>
    private WorldMapEntity CurrWorldMapEntity;

    /// <summary>
    /// 传送点字典
    /// </summary>
    private Dictionary<int, WorldMapTransCtrl> m_TransPosDic;

    /// <summary>
    /// 世界地图地点协议
    /// </summary>
    private WorldMap_PosProto m_WorldMapPosProto;
    /// <summary>
    /// 下次同步角色位置的时间
    /// </summary>
    private float m_NextSendTime;
    /// <summary>
    /// 当前场景全玩家控制器字典《角色编号，角色控制器》
    /// </summary>
    private Dictionary<int, RoleCtrl> m_AllRoleDic;

    /// <summary>
    /// 客户端已经进入游戏协议
    /// </summary>
    private WorldMap_RoleAlreadyEnterProto m_RoleAlreadyEnterProto = new WorldMap_RoleAlreadyEnterProto();
    #endregion

    protected override void OnStart()
    {
        base.OnStart();
        // 获取场景中的所有事件系统
        EventSystem[] eventSystems = GameObject.FindObjectsOfType<EventSystem>();
        // 如果存在多于一个事件系统，删除多余的
        if (eventSystems.Length > 1)
        {
            // 删除多余的事件系统
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
        if (GlobalInit.Instance.currentPlayer != null)
        {
            GlobalInit.Instance.currentPlayer.Attack.IsAutoFight = false;
        }
        m_TransPosDic = new Dictionary<int, WorldMapTransCtrl>();//传送点字典
        m_AllRoleDic = new Dictionary<int, RoleCtrl>();//角色字典
        AddEventListener();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        //每五秒更新一次世界地图坐标     
        if (Time.time > m_NextSendTime)
        {
            m_NextSendTime += 3f;
            SendPlayerPos();
        }
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        RemoveEventListener();
    }

    #region 主城加载完成信息设置 OnLoadUIMainCityViewComplete InitTransPos InitNPC LoadNPC AutoMove
    /// <summary>
    /// 主城加载完成回调
    /// </summary>
    /// <param name="obj"></param>
    protected override void OnLoadUIMainCityViewComplete(GameObject obj)
    {
        base.OnLoadUIMainCityViewComplete(obj);
        UILoadingCtrl.Instance.CurrentPlayerType = PlayerType.PVP;
        if (GlobalInit.Instance == null)
        { return; }
        RoleMgr.Instance.InitMainPlayer();
        //将主角加入角色字典
        m_AllRoleDic.Add(GlobalInit.Instance.currentPlayer.CurrentRoleInfo.RoldId,GlobalInit.Instance.currentPlayer);
        if (GlobalInit.Instance.currentPlayer != null)
        {
            //设置主角的出生点：若是从传送点过来的，应该生在传送点，否则在默认位置
            CurrWorldMapEntity = WorldMapDBModel.Instance.Get(UILoadingCtrl.Instance.CurrentWorldMapId);
            //TODO
            //AudioBackGroundMgr.Instance.Play(CurrWorldMapEntity.Audio_BG);
            //初始化地图传送点
            InitTransPos();
            if (UILoadingCtrl.Instance.targetWorldMapTransWorld == 0)
            {
                if (!string.IsNullOrEmpty(PlayerCtrl.Instance.LastInWorldMapPos))
                {
                    string[] arr = PlayerCtrl.Instance.LastInWorldMapPos.Split("_");
                    Vector3 pos = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                    //设置主角的初始位置和面向方向
                    GlobalInit.Instance.currentPlayer.Born(pos);
                    GlobalInit.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0, float.Parse(arr[3]), 0);
                }
                else
                {
                    //设置主角的初始位置
                    if (CurrWorldMapEntity.RoleBirthPostion != Vector3.zero)
                    {
                        GlobalInit.Instance.currentPlayer.Born(CurrWorldMapEntity.RoleBirthPostion);
                        GlobalInit.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0, CurrWorldMapEntity.RoleBirthEulerAnglesY, 0);
                    }
                    else
                    {
                        GlobalInit.Instance.currentPlayer.Born(m_PlayerBornPos.position);
                    }
                }
            }
            else
            {
                //主角出生在传送点   
                if (m_TransPosDic.ContainsKey(UILoadingCtrl.Instance.targetWorldMapTransWorld))
                {
                    Vector3 newBornPos = m_TransPosDic[UILoadingCtrl.Instance.targetWorldMapTransWorld].transform.forward.normalized * 3 + m_TransPosDic[UILoadingCtrl.Instance.targetWorldMapTransWorld].transform.position;
                    Vector3 lookAtPos = m_TransPosDic[UILoadingCtrl.Instance.targetWorldMapTransWorld].transform.forward.normalized * 3.5f + m_TransPosDic[UILoadingCtrl.Instance.targetWorldMapTransWorld].transform.position;
                    GlobalInit.Instance.currentPlayer.Born(newBornPos);
                    GlobalInit.Instance.currentPlayer.transform.LookAt(lookAtPos);

                    UILoadingCtrl.Instance.targetWorldMapTransWorld = 0;
                }
            }
        }

        //PVP此时，玩家已经成功进入场景，则需要告诉服务器，让服务器同步玩家情况
        this.SendRoleAlreadyEnter(UILoadingCtrl.Instance.CurrentWorldMapId, GlobalInit.Instance.currentPlayer.transform.position, GlobalInit.Instance.currentPlayer.transform.eulerAngles.y);

        PlayerCtrl.Instance.SetMainCityRoleData();
        Debug.Log("主城UI加载完成");
        if (DelegateDefine.Instance.OnSceneLoadOk != null)
        {
            DelegateDefine.Instance.OnSceneLoadOk();
        }
        StartCoroutine(InitNPC());
        AutoMove();

    }

    /// <summary>
    /// 传送点初始化
    /// </summary>
    /// <returns></returns>
    private void InitTransPos()
    {
        //1.分解传送点具体信息
        string[] posInfoArr = CurrWorldMapEntity.TransPos.Split("|");
        for (int i = 0; i < posInfoArr.Length; i++)
        {
            string[] posInfo = posInfoArr[i].Split("_");
            if (posInfo.Length == 7)
            {
                Vector3 pos = new Vector3();
                //传送点坐标
                float transPos = 0;
                float.TryParse(posInfo[0], out transPos);
                pos.x = transPos;
                float.TryParse(posInfo[1], out transPos);
                pos.y = transPos;
                float.TryParse(posInfo[2], out transPos);
                pos.x = transPos;
                //获取该传送点的Y轴旋转
                float y = 0;
                float.TryParse(posInfo[3], out y);

                //传送点编号
                int currTransPosId = 0;
                int.TryParse(posInfo[4], out currTransPosId);
                //要传送的场景ID
                int targetTransSceneId = 0;
                //要传送的目标场景的传送点ID    
                int targetSceneTransId = 0;
                int.TryParse(posInfo[5], out targetTransSceneId);
                int.TryParse(posInfo[6], out targetSceneTransId);

                //克隆传送点
                AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/Effect/Common/Effect_Trans.assetbundle"), "Effect_Trans",
                    (GameObject obj) =>
                    {
                        Debug.Log("正在生成特效");
                        obj = Instantiate(obj);
                        if (Vector3.Distance(obj.transform.position,new Vector3(50,0,50))>50f)
                        {
                            obj.transform.position = new Vector3(UnityEngine.Random.Range(10f, 90f), 1, UnityEngine.Random.Range(10f, 90f));
                        }
                        else
                        {
                            obj.transform.position = new Vector3(pos.x+UnityEngine.Random.Range(0f,5f), 1, pos.z);
                        }
                        obj.transform.eulerAngles = new Vector3(0, y, 0);
                        WorldMapTransCtrl ctrl = obj.GetComponent<WorldMapTransCtrl>();
                        if (ctrl != null)
                        {
                            ctrl.SetParam(currTransPosId, targetTransSceneId, targetSceneTransId);
                        }
                        m_TransPosDic[currTransPosId] = ctrl;
                    }, type: 0);

            }
        }


    }

    /// <summary>
    /// 加载NPC,并进行初始化协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitNPC()
    {
        yield return null;
        if (CurrWorldMapEntity == null)
        {
            yield break;
        }
        LoadNPC(0);
    }
    /// <summary>
    /// 通过递归加载NPC
    /// </summary>
    /// <param name="index">下标</param>
    private void LoadNPC(int index)
    {
        if (CurrWorldMapEntity.NPCWorldMapList.Count == 0)
        { return; }
        NPCWorldMapData data = CurrWorldMapEntity.NPCWorldMapList[index];
        NPCEntity entity = NPCDBModel.Instance.Get(data.NPCId);
        UILoadingCtrl.Instance.LoadNPC(entity.PrefabName,
            (GameObject obj) =>
            {
                obj.transform.position = data.NPCPosition;
                obj.transform.eulerAngles = new Vector3(0, data.EulerAnglesY, 0);
                NPCCtrl ctrl = obj.GetComponent<NPCCtrl>();
                ctrl.Init(data);
                index++;
                if (index == CurrWorldMapEntity.NPCWorldMapList.Count)
                {
                    Debug.Log("NPC加载完成");
                }
                else
                {
                    LoadNPC(index);
                }
            });
    }

    /// <summary>
    /// 主角自动进行区域传送
    /// 根据SceneIdQueue：CurrentSceneId和ToSceneId让主角前往指定场景
    /// </summary>
    public void AutoMove()
    {
        if (!WorldMapCtrl.Instance.IsAutoMove)
        { return; }
        //若玩家已经到达目标场景，那么前往目标点
        if (UILoadingCtrl.Instance.CurrentWorldMapId == WorldMapCtrl.Instance.ToSceneId)
        {
            if (WorldMapCtrl.Instance.ToScenePos != Vector3.zero)
            {
                //角色到达目标场景
                GlobalInit.Instance.currentPlayer.MoveTo(WorldMapCtrl.Instance.ToScenePos);
            }
            WorldMapCtrl.Instance.IsAutoMove = false;
        }
        //检索目标传送点
        foreach (var item in m_TransPosDic)
        {
            if (item.Value.TargetTransSceneId == WorldMapCtrl.Instance.ToSceneId)
            {
                //前往通往目标区域的传送点
                GlobalInit.Instance.currentPlayer.MoveTo(item.Value.transform.position);
            }
        }
        WorldMapCtrl.Instance.CurrentSceneId = WorldMapCtrl.Instance.ToSceneId;
        if (WorldMapCtrl.Instance.SceneIdQueue.Count > 0)
        {
            WorldMapCtrl.Instance.ToSceneId = WorldMapCtrl.Instance.SceneIdQueue.Dequeue();
        }
    }
    #endregion

    #region 客户端进行信息发送  SendPlayerPos SendRoleAlreadyEnter
    /// <summary>
    /// 客户端发送玩家位置信息
    /// </summary>
    private void SendPlayerPos()
    {
        if (GlobalInit.Instance != null && GlobalInit.Instance.currentPlayer != null)
        {
            m_WorldMapPosProto.x = GlobalInit.Instance.currentPlayer.transform.position.x;
            m_WorldMapPosProto.y = GlobalInit.Instance.currentPlayer.transform.position.y;
            m_WorldMapPosProto.z = GlobalInit.Instance.currentPlayer.transform.position.z;
            m_WorldMapPosProto.yAngle = GlobalInit.Instance.currentPlayer.transform.eulerAngles.y;
            NetWorkSocket.Instance.SendMessage(m_WorldMapPosProto.ToArray());
        }
    }

    /// <summary>
    /// 客户端发送玩家进入游戏消息
    /// </summary>
    /// <param name="worldMapSceneId">世界地图场景ID</param>
    /// <param name="currRolePos">玩家位置</param>
    /// <param name="currRoleYAngle">玩家Y角度</param>
    private void SendRoleAlreadyEnter(int worldMapSceneId, Vector3 currRolePos, float currRoleYAngle)
    {
        m_RoleAlreadyEnterProto.TargetWorldMapSceneId = worldMapSceneId;
        m_RoleAlreadyEnterProto.RolePosX = currRolePos.x;
        m_RoleAlreadyEnterProto.RolePosY = currRolePos.y;
        m_RoleAlreadyEnterProto.RolePosZ = currRolePos.z;
        m_RoleAlreadyEnterProto.RoleYAngle = currRoleYAngle;

        NetWorkSocket.Instance.SendMessage(m_RoleAlreadyEnterProto.ToArray());
    }
    #endregion

    #region 服务器（主要是围绕其他玩家进行的一些信息管理以及主角的信息传输）
    /// <summary>
    /// 添加协议
    /// </summary>
    private void AddEventListener()
    {
        //服务器广播当前场景角色
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMapInitRole);
        //服务器广播其他角色进入场景消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMapOtherRoleEnter);
        //服务器广播其他角色离开场景消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMapOtherRoleLeave);
        //服务器广播其他角色移动消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMapOtherRoleMove);
        //服务器广播角色使用技能消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMapOtherRoleUseSkill);
        //服务器广播角色死亡消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMapOtherRoleDie);
        //服务器广播角色复活消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMapOtherRoleResurgence);
    }

    /// <summary>
    /// 移除协议
    /// </summary>
    private void RemoveEventListener()
    {
        //服务器广播当前场景角色
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMapInitRole);
        //服务器广播其他角色进入场景消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMapOtherRoleEnter);
        //服务器广播其他角色离开场景消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMapOtherRoleLeave);
        //服务器广播其他角色移动消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMapOtherRoleMove);
        //服务器广播角色使用技能消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMapOtherRoleUseSkill);
        //服务器广播角色死亡消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMapOtherRoleDie);
        //服务器广播角色复活消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMapOtherRoleResurgence);
    }

    #region 角色进场和离场
    /// <summary>
    /// 世界地图初始化-其他角色创建部分
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMapInitRole(byte[] buffer)
    {
        WorldMap_InitRoleProto proto = WorldMap_InitRoleProto.GetProto(buffer);
        int roleCount = proto.RoleCount;
        List<WorldMap_InitRoleProto.RoleItem> list = proto.ItemList;
        if (list == null)
        { return; }
        for (int i = 0; i < list.Count; i++)
        {
            int roleId = list[i].RoleId;
            string roleNickName = list[i].RoleNickName;
            int roleLevel = list[i].RoleLevel;
            int roleJobId = list[i].RoleJobId;
            Vector3 rolePos = new Vector3(list[i].RolePosX, list[i].RolePosY, list[i].RolePosZ);
            float roleYAngle = list[i].RoleYAngle;
            int currHP = list[i].RoleCurrHP;
            int maxHP = list[i].RoleMaxHP;
            int currMP = list[i].RoleCurrMP;
            int maxMP = list[i].RoleMaxMP;
            //创建其他玩家
            CreateOherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, currHP, maxHP, currMP, maxMP);
        }
       
    }

    /// <summary>
    /// 服务器广播其他玩家进入当前场景消息
    /// </summary>
    /// <param name="buffer"></param>
    private void OnWorldMapOtherRoleEnter(byte[] buffer)
    {
        WorldMap_OtherRoleEnterProto proto = WorldMap_OtherRoleEnterProto.GetProto(buffer);
        int roleId = proto.RoleId;
        string roleNickName = proto.RoleNickName;
        int roleLevel = proto.RoleLevel;
        int roleJobId = proto.RoleJobId;
        Vector3 rolePos = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
        float roleYAngle = proto.RoleYAngle;
        int currHP = proto.RoleCurrHP;
        int maxHP = proto.RoleMaxHP;
        int currMP = proto.RoleCurrMP;
        int maxMP = proto.RoleMaxMP;
        //创建其他玩家
        CreateOherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, currHP, maxHP, currMP, maxMP);
    }

    /// <summary>
    /// 服务器广播其他玩家离开场景消息
    /// </summary>
    /// <param name="p"></param>
    private void OnWorldMapOtherRoleLeave(byte[] buffer)
    {
        Debug.LogWarning("其他玩家离开场景");
        WorldMap_OtherRoleLeaveProto proto = WorldMap_OtherRoleLeaveProto.GetProto(buffer);
        int roleId = proto.RoleId;
        //当其他玩家离开当前场景时，销毁他们
        DestroyOtherRole(roleId);
    }

    #endregion

    /// <summary>
    /// 服务器广播其他角色移动消息
    /// 根据服务器反馈的其他角色的移动目标，在客户端上进行实现
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleMove(byte[] buffer)
    {
        Debug.LogWarning("其他玩家移动");
        WorldMap_OtherRoleMoveProto proto = WorldMap_OtherRoleMoveProto.GetProto(buffer);
        int roleId = proto.RoleId;
        Vector3 targetPos = new Vector3(proto.TargetPosX, proto.TargetPosY, proto.TargetPosZ);
        long serverTime = proto.ServerTime;
        int needTime = proto.NeedTime;

        //角色移动了
        if (m_AllRoleDic.ContainsKey(roleId))
        {
            ((OtherRoleAI)m_AllRoleDic[roleId].CurrRoleAI).MoveTo(targetPos, serverTime, needTime);
        }
    }

    /// <summary>
    /// 服务器广播其他角色使用技能消息
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleUseSkill(byte[] buffer)
    {
        Debug.LogWarning("其他玩家使用技能");
        WorldMap_OtherRoleUseSkillProto proto = WorldMap_OtherRoleUseSkillProto.GetProto(buffer);
        //攻击者模块
        //若攻击者仍然存在，则获取攻击者的角色控制器
        if (m_AllRoleDic.ContainsKey(proto.AttackRoleId))
        {
            RoleCtrl attackRole = m_AllRoleDic[proto.AttackRoleId];
            attackRole.transform.position = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
            attackRole.transform.eulerAngles = new Vector3(0, proto.RoleYAngle, 0);
            attackRole.PlayAttack(proto.SkillId);//播放攻击动画   
        }
        //受击者模块
        if (proto.ItemList != null && proto.ItemList.Count > 0)
        {
            for (int i = 0; i < proto.ItemList.Count; i++)
            {
                WorldMap_OtherRoleUseSkillProto.BeAttackItem item = proto.ItemList[i];
                if (m_AllRoleDic.ContainsKey(item.BeAttackRoleId))
                {
                    RoleCtrl beAttackRole = m_AllRoleDic[item.BeAttackRoleId];
                    RoleTransferAttackInfo attackInfo = new RoleTransferAttackInfo();
                    attackInfo.AttackRoleId = proto.AttackRoleId;
                    attackInfo.BeAttackRoleId = item.BeAttackRoleId;
                    attackInfo.SkillId = proto.SkillId;
                    attackInfo.SkillLevel = proto.SkillLevel;
                    attackInfo.IsCri = item.IsCri == 1;
                    attackInfo.HurtValue = item.ReduceHp;
                    beAttackRole.ToHurt(attackInfo);
                }
            }
        }
    }

    /// <summary>
    /// 服务器广播角色死亡消息
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleDie(byte[] buffer)
    {
        Debug.LogWarning("有玩家死了");
        WorldMap_OtherRoleDieProto proto = WorldMap_OtherRoleDieProto.GetProto(buffer);
        if (proto.RoleIdList != null && proto.RoleIdList.Count > 0)
        {
            for (int i = 0; i < proto.RoleIdList.Count; i++)
            {
                int dieRoleId = proto.RoleIdList[i];
                if (m_AllRoleDic.ContainsKey(dieRoleId))
                {
                    m_AllRoleDic[dieRoleId].ToDie();
                    if (m_AllRoleDic[dieRoleId].CurrRoleType == RoleType.MainPlayer)
                    {
                        //若玩家已经死亡，则弹出复活UI窗口
                        WorldMapCtrl.Instance.EnemyNickName = m_AllRoleDic[proto.AttackRoleId].CurrentRoleInfo.RoleNickName;
                        WorldMapCtrl.Instance.OpenView(WindowUIType.WorldMapFail);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 服务器广播角色复活消息
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleResurgence(byte[] buffer)
    {
        Debug.LogWarning("有玩家使用复活药了");
        WorldMap_OtherRoleResurgenceProto proto = WorldMap_OtherRoleResurgenceProto.GetProto(buffer);
        if (m_AllRoleDic.ContainsKey(proto.RoleId))
        {
            m_AllRoleDic[proto.RoleId].ToResurgence(RoleIdleState.IdleNormal);
        }
    }
    #endregion

    #region 其他玩家非服务器管理方法 CreateOherPlayer DestroyOtherRole
    /// <summary>
    /// 创建其他玩家
    /// </summary>
    /// <param name="roleId">玩家ID</param>
    /// <param name="roleNickName">玩家名</param>
    /// <param name="roleLevel">玩家等级</param>
    /// <param name="roleJobId">玩家职业ID</param>
    /// <param name="rolePos">玩家位置</param>
    /// <param name="roleYAngle">玩家朝向</param>
    /// <param name="currHP">角色剩余生命值</param>
    /// <param name="maxHP">角色最大生命值</param>
    /// <param name="currMP">角色剩余蓝量</param>
    /// <param name="maxMP">角色最大蓝量</param>
    /// <returns></returns>
    private void CreateOherPlayer(int roleId, string roleNickName, int roleLevel, int roleJobId, Vector3 rolePos, float roleYAngle, int currHP, int maxHP, int currMP, int maxMP)
    {
        RoleCtrl ctrl = UILoadingCtrl.Instance.LoadOtherRole(roleId, roleNickName, roleLevel, roleJobId, currHP, maxHP, currMP, maxMP,rolePos);
        ctrl.gameObject.transform.eulerAngles = new Vector3(0, roleYAngle, 0);
        m_AllRoleDic[roleId] = ctrl;
    }

    /// <summary>
    /// 销毁其他玩家
    /// </summary>
    /// <param name="roleId"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void DestroyOtherRole(int roleId)
    {
        if (m_AllRoleDic.ContainsKey(roleId))
        {
            //销毁角色的同时也要销毁角色的UI条
            Destroy(m_AllRoleDic[roleId].gameObject);
            m_AllRoleDic.Remove(roleId);
        }
    }
    #endregion
}
