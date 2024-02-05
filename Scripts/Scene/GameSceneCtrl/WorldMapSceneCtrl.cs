using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// �����ͼ������������
/// </summary>
public class WorldMapSceneCtrl : GameSceneCtrlBase
{
    #region ����
    /// <summary>
    /// ����
    /// </summary>
    public static WorldMapSceneCtrl Instance;
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }
    #endregion

    #region ����
    /// <summary>
    /// ���ǳ�����(�����ó�����)
    /// </summary>
    [SerializeField]
    private Transform m_PlayerBornPos;

    /// <summary>
    /// ��ǰ�����ͼʵ��
    /// </summary>
    private WorldMapEntity CurrWorldMapEntity;

    /// <summary>
    /// ���͵��ֵ�
    /// </summary>
    private Dictionary<int, WorldMapTransCtrl> m_TransPosDic;

    /// <summary>
    /// �����ͼ�ص�Э��
    /// </summary>
    private WorldMap_PosProto m_WorldMapPosProto;
    /// <summary>
    /// �´�ͬ����ɫλ�õ�ʱ��
    /// </summary>
    private float m_NextSendTime;
    /// <summary>
    /// ��ǰ����ȫ��ҿ������ֵ䡶��ɫ��ţ���ɫ��������
    /// </summary>
    private Dictionary<int, RoleCtrl> m_AllRoleDic;

    /// <summary>
    /// �ͻ����Ѿ�������ϷЭ��
    /// </summary>
    private WorldMap_RoleAlreadyEnterProto m_RoleAlreadyEnterProto = new WorldMap_RoleAlreadyEnterProto();
    #endregion

    protected override void OnStart()
    {
        base.OnStart();
        // ��ȡ�����е������¼�ϵͳ
        EventSystem[] eventSystems = GameObject.FindObjectsOfType<EventSystem>();
        // ������ڶ���һ���¼�ϵͳ��ɾ�������
        if (eventSystems.Length > 1)
        {
            // ɾ��������¼�ϵͳ
            for (int i = 1; i < eventSystems.Length; i++)
            {
                Destroy(eventSystems[i].gameObject);
            }
        }
        if (GlobalInit.Instance.currentPlayer != null)
        {
            GlobalInit.Instance.currentPlayer.Attack.IsAutoFight = false;
        }
        m_TransPosDic = new Dictionary<int, WorldMapTransCtrl>();//���͵��ֵ�
        m_AllRoleDic = new Dictionary<int, RoleCtrl>();//��ɫ�ֵ�
        AddEventListener();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        //ÿ�������һ�������ͼ����     
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

    #region ���Ǽ��������Ϣ���� OnLoadUIMainCityViewComplete InitTransPos InitNPC LoadNPC AutoMove
    /// <summary>
    /// ���Ǽ�����ɻص�
    /// </summary>
    /// <param name="obj"></param>
    protected override void OnLoadUIMainCityViewComplete(GameObject obj)
    {
        base.OnLoadUIMainCityViewComplete(obj);
        UILoadingCtrl.Instance.CurrentPlayerType = PlayerType.PVP;
        if (GlobalInit.Instance == null)
        { return; }
        RoleMgr.Instance.InitMainPlayer();
        //�����Ǽ����ɫ�ֵ�
        m_AllRoleDic.Add(GlobalInit.Instance.currentPlayer.CurrentRoleInfo.RoldId,GlobalInit.Instance.currentPlayer);
        if (GlobalInit.Instance.currentPlayer != null)
        {
            //�������ǵĳ����㣺���ǴӴ��͵�����ģ�Ӧ�����ڴ��͵㣬������Ĭ��λ��
            CurrWorldMapEntity = WorldMapDBModel.Instance.Get(UILoadingCtrl.Instance.CurrentWorldMapId);
            //TODO
            //AudioBackGroundMgr.Instance.Play(CurrWorldMapEntity.Audio_BG);
            //��ʼ����ͼ���͵�
            InitTransPos();
            if (UILoadingCtrl.Instance.targetWorldMapTransWorld == 0)
            {
                if (!string.IsNullOrEmpty(PlayerCtrl.Instance.LastInWorldMapPos))
                {
                    string[] arr = PlayerCtrl.Instance.LastInWorldMapPos.Split("_");
                    Vector3 pos = new Vector3(float.Parse(arr[0]), float.Parse(arr[1]), float.Parse(arr[2]));
                    //�������ǵĳ�ʼλ�ú�������
                    GlobalInit.Instance.currentPlayer.Born(pos);
                    GlobalInit.Instance.currentPlayer.gameObject.transform.eulerAngles = new Vector3(0, float.Parse(arr[3]), 0);
                }
                else
                {
                    //�������ǵĳ�ʼλ��
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
                //���ǳ����ڴ��͵�   
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

        //PVP��ʱ������Ѿ��ɹ����볡��������Ҫ���߷��������÷�����ͬ��������
        this.SendRoleAlreadyEnter(UILoadingCtrl.Instance.CurrentWorldMapId, GlobalInit.Instance.currentPlayer.transform.position, GlobalInit.Instance.currentPlayer.transform.eulerAngles.y);

        PlayerCtrl.Instance.SetMainCityRoleData();
        Debug.Log("����UI�������");
        if (DelegateDefine.Instance.OnSceneLoadOk != null)
        {
            DelegateDefine.Instance.OnSceneLoadOk();
        }
        StartCoroutine(InitNPC());
        AutoMove();

    }

    /// <summary>
    /// ���͵��ʼ��
    /// </summary>
    /// <returns></returns>
    private void InitTransPos()
    {
        //1.�ֽ⴫�͵������Ϣ
        string[] posInfoArr = CurrWorldMapEntity.TransPos.Split("|");
        for (int i = 0; i < posInfoArr.Length; i++)
        {
            string[] posInfo = posInfoArr[i].Split("_");
            if (posInfo.Length == 7)
            {
                Vector3 pos = new Vector3();
                //���͵�����
                float transPos = 0;
                float.TryParse(posInfo[0], out transPos);
                pos.x = transPos;
                float.TryParse(posInfo[1], out transPos);
                pos.y = transPos;
                float.TryParse(posInfo[2], out transPos);
                pos.x = transPos;
                //��ȡ�ô��͵��Y����ת
                float y = 0;
                float.TryParse(posInfo[3], out y);

                //���͵���
                int currTransPosId = 0;
                int.TryParse(posInfo[4], out currTransPosId);
                //Ҫ���͵ĳ���ID
                int targetTransSceneId = 0;
                //Ҫ���͵�Ŀ�곡���Ĵ��͵�ID    
                int targetSceneTransId = 0;
                int.TryParse(posInfo[5], out targetTransSceneId);
                int.TryParse(posInfo[6], out targetSceneTransId);

                //��¡���͵�
                AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/Effect/Common/Effect_Trans.assetbundle"), "Effect_Trans",
                    (GameObject obj) =>
                    {
                        Debug.Log("����������Ч");
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
    /// ����NPC,�����г�ʼ��Э��
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
    /// ͨ���ݹ����NPC
    /// </summary>
    /// <param name="index">�±�</param>
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
                    Debug.Log("NPC�������");
                }
                else
                {
                    LoadNPC(index);
                }
            });
    }

    /// <summary>
    /// �����Զ�����������
    /// ����SceneIdQueue��CurrentSceneId��ToSceneId������ǰ��ָ������
    /// </summary>
    public void AutoMove()
    {
        if (!WorldMapCtrl.Instance.IsAutoMove)
        { return; }
        //������Ѿ�����Ŀ�곡������ôǰ��Ŀ���
        if (UILoadingCtrl.Instance.CurrentWorldMapId == WorldMapCtrl.Instance.ToSceneId)
        {
            if (WorldMapCtrl.Instance.ToScenePos != Vector3.zero)
            {
                //��ɫ����Ŀ�곡��
                GlobalInit.Instance.currentPlayer.MoveTo(WorldMapCtrl.Instance.ToScenePos);
            }
            WorldMapCtrl.Instance.IsAutoMove = false;
        }
        //����Ŀ�괫�͵�
        foreach (var item in m_TransPosDic)
        {
            if (item.Value.TargetTransSceneId == WorldMapCtrl.Instance.ToSceneId)
            {
                //ǰ��ͨ��Ŀ������Ĵ��͵�
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

    #region �ͻ��˽�����Ϣ����  SendPlayerPos SendRoleAlreadyEnter
    /// <summary>
    /// �ͻ��˷������λ����Ϣ
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
    /// �ͻ��˷�����ҽ�����Ϸ��Ϣ
    /// </summary>
    /// <param name="worldMapSceneId">�����ͼ����ID</param>
    /// <param name="currRolePos">���λ��</param>
    /// <param name="currRoleYAngle">���Y�Ƕ�</param>
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

    #region ����������Ҫ��Χ��������ҽ��е�һЩ��Ϣ�����Լ����ǵ���Ϣ���䣩
    /// <summary>
    /// ���Э��
    /// </summary>
    private void AddEventListener()
    {
        //�������㲥��ǰ������ɫ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMapInitRole);
        //�������㲥������ɫ���볡����Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMapOtherRoleEnter);
        //�������㲥������ɫ�뿪������Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMapOtherRoleLeave);
        //�������㲥������ɫ�ƶ���Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMapOtherRoleMove);
        //�������㲥��ɫʹ�ü�����Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMapOtherRoleUseSkill);
        //�������㲥��ɫ������Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMapOtherRoleDie);
        //�������㲥��ɫ������Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMapOtherRoleResurgence);
    }

    /// <summary>
    /// �Ƴ�Э��
    /// </summary>
    private void RemoveEventListener()
    {
        //�������㲥��ǰ������ɫ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_InitRole, OnWorldMapInitRole);
        //�������㲥������ɫ���볡����Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleEnter, OnWorldMapOtherRoleEnter);
        //�������㲥������ɫ�뿪������Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleLeave, OnWorldMapOtherRoleLeave);
        //�������㲥������ɫ�ƶ���Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleMove, OnWorldMapOtherRoleMove);
        //�������㲥��ɫʹ�ü�����Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleUseSkill, OnWorldMapOtherRoleUseSkill);
        //�������㲥��ɫ������Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleDie, OnWorldMapOtherRoleDie);
        //�������㲥��ɫ������Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.WorldMap_OtherRoleResurgence, OnWorldMapOtherRoleResurgence);
    }

    #region ��ɫ�������볡
    /// <summary>
    /// �����ͼ��ʼ��-������ɫ��������
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
            //�����������
            CreateOherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, currHP, maxHP, currMP, maxMP);
        }
       
    }

    /// <summary>
    /// �������㲥������ҽ��뵱ǰ������Ϣ
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
        //�����������
        CreateOherPlayer(roleId, roleNickName, roleLevel, roleJobId, rolePos, roleYAngle, currHP, maxHP, currMP, maxMP);
    }

    /// <summary>
    /// �������㲥��������뿪������Ϣ
    /// </summary>
    /// <param name="p"></param>
    private void OnWorldMapOtherRoleLeave(byte[] buffer)
    {
        Debug.LogWarning("��������뿪����");
        WorldMap_OtherRoleLeaveProto proto = WorldMap_OtherRoleLeaveProto.GetProto(buffer);
        int roleId = proto.RoleId;
        //����������뿪��ǰ����ʱ����������
        DestroyOtherRole(roleId);
    }

    #endregion

    /// <summary>
    /// �������㲥������ɫ�ƶ���Ϣ
    /// ���ݷ�����������������ɫ���ƶ�Ŀ�꣬�ڿͻ����Ͻ���ʵ��
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleMove(byte[] buffer)
    {
        Debug.LogWarning("��������ƶ�");
        WorldMap_OtherRoleMoveProto proto = WorldMap_OtherRoleMoveProto.GetProto(buffer);
        int roleId = proto.RoleId;
        Vector3 targetPos = new Vector3(proto.TargetPosX, proto.TargetPosY, proto.TargetPosZ);
        long serverTime = proto.ServerTime;
        int needTime = proto.NeedTime;

        //��ɫ�ƶ���
        if (m_AllRoleDic.ContainsKey(roleId))
        {
            ((OtherRoleAI)m_AllRoleDic[roleId].CurrRoleAI).MoveTo(targetPos, serverTime, needTime);
        }
    }

    /// <summary>
    /// �������㲥������ɫʹ�ü�����Ϣ
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleUseSkill(byte[] buffer)
    {
        Debug.LogWarning("�������ʹ�ü���");
        WorldMap_OtherRoleUseSkillProto proto = WorldMap_OtherRoleUseSkillProto.GetProto(buffer);
        //������ģ��
        //����������Ȼ���ڣ����ȡ�����ߵĽ�ɫ������
        if (m_AllRoleDic.ContainsKey(proto.AttackRoleId))
        {
            RoleCtrl attackRole = m_AllRoleDic[proto.AttackRoleId];
            attackRole.transform.position = new Vector3(proto.RolePosX, proto.RolePosY, proto.RolePosZ);
            attackRole.transform.eulerAngles = new Vector3(0, proto.RoleYAngle, 0);
            attackRole.PlayAttack(proto.SkillId);//���Ź�������   
        }
        //�ܻ���ģ��
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
    /// �������㲥��ɫ������Ϣ
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleDie(byte[] buffer)
    {
        Debug.LogWarning("���������");
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
                        //������Ѿ��������򵯳�����UI����
                        WorldMapCtrl.Instance.EnemyNickName = m_AllRoleDic[proto.AttackRoleId].CurrentRoleInfo.RoleNickName;
                        WorldMapCtrl.Instance.OpenView(WindowUIType.WorldMapFail);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �������㲥��ɫ������Ϣ
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnWorldMapOtherRoleResurgence(byte[] buffer)
    {
        Debug.LogWarning("�����ʹ�ø���ҩ��");
        WorldMap_OtherRoleResurgenceProto proto = WorldMap_OtherRoleResurgenceProto.GetProto(buffer);
        if (m_AllRoleDic.ContainsKey(proto.RoleId))
        {
            m_AllRoleDic[proto.RoleId].ToResurgence(RoleIdleState.IdleNormal);
        }
    }
    #endregion

    #region ������ҷǷ����������� CreateOherPlayer DestroyOtherRole
    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="roleId">���ID</param>
    /// <param name="roleNickName">�����</param>
    /// <param name="roleLevel">��ҵȼ�</param>
    /// <param name="roleJobId">���ְҵID</param>
    /// <param name="rolePos">���λ��</param>
    /// <param name="roleYAngle">��ҳ���</param>
    /// <param name="currHP">��ɫʣ������ֵ</param>
    /// <param name="maxHP">��ɫ�������ֵ</param>
    /// <param name="currMP">��ɫʣ������</param>
    /// <param name="maxMP">��ɫ�������</param>
    /// <returns></returns>
    private void CreateOherPlayer(int roleId, string roleNickName, int roleLevel, int roleJobId, Vector3 rolePos, float roleYAngle, int currHP, int maxHP, int currMP, int maxMP)
    {
        RoleCtrl ctrl = UILoadingCtrl.Instance.LoadOtherRole(roleId, roleNickName, roleLevel, roleJobId, currHP, maxHP, currMP, maxMP,rolePos);
        ctrl.gameObject.transform.eulerAngles = new Vector3(0, roleYAngle, 0);
        m_AllRoleDic[roleId] = ctrl;
    }

    /// <summary>
    /// �����������
    /// </summary>
    /// <param name="roleId"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void DestroyOtherRole(int roleId)
    {
        if (m_AllRoleDic.ContainsKey(roleId))
        {
            //���ٽ�ɫ��ͬʱҲҪ���ٽ�ɫ��UI��
            Destroy(m_AllRoleDic[roleId].gameObject);
            m_AllRoleDic.Remove(roleId);
        }
    }
    #endregion
}
