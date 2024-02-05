using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 选角场景控制器
/// </summary>
public class SelectRoleSceneCtrl : MonoBehaviour
{
    #region 变量
    /// <summary>
    /// 职业列表
    /// </summary>
    private List<JobEntity> m_JobList;

    /// <summary>
    /// 角色容器    
    /// </summary>
    public Transform[] CreateRoleContainers;

    /// <summary>
    /// 相机旋转角度
    /// </summary>
    public float[] CameraRotate;
    
    /// <summary>
    /// 职业控制器
    /// </summary>
    private Dictionary<int, RoleCtrl> m_JobRoleCtrl = new Dictionary<int, RoleCtrl>();

    /// <summary>
    /// 场景视图
    /// </summary>
    private UISceneSelectRoleView m_UISceneSelectRoleView;

    /// <summary>
    /// 拖拽目标
    /// </summary>
    public Transform DragTarget;

    /// <summary>
    /// 每次拖拽后旋转角度
    /// </summary>
    private float m_RotateAngle = 90;

    /// <summary>
    /// 旋转目标角度
    /// </summary>
    private float m_TargetAngle = 0;

    /// <summary>
    /// 是否正处于旋转状态
    /// </summary>
    private bool m_IsRotating;

    /// <summary>
    /// 旋转速度
    /// </summary>
    private float m_RotateSpeed = 100f;

    /// <summary>
    /// 当前选择的职业ID
    /// </summary>
    private int m_CurrSelectJobId;

    /// <summary>
    /// 新建角色所需的场景模型：实际上无
    /// </summary>
    public Transform[] CreateRoleSceneModel;

    /// <summary>
    /// 是否处于新建角色阶段
    /// </summary>
    private bool m_IsCreateRole;
    
    /// <summary>
    /// 克隆角色链表
    /// </summary>
    private List<GameObject> m_CloneCreateRoleList = new List<GameObject>();

    #region 已有角色设置变量
    /// <summary>
    /// 已有角色列表
    /// </summary>
    private List<RoleOperation_LogOnGameServerReturnProto.RoleItem> m_RoleList;

    /// <summary>
    /// 当前选择的角色模型
    /// </summary>
    private GameObject m_CurrSelectRoleModel;

    /// <summary>
    /// 当前选择的角色编号
    /// </summary>
    private int m_CurrSelectRoleId;
    #endregion

    /// <summary>
    /// 最后进入的世界地图ID
    /// </summary>
    private int m_LastInWorldMapId;
    #endregion

    // Start is called before the first frame update
    void Start() 
    {
        if (DelegateDefine.Instance.OnSceneLoadOk != null)
        {
            DelegateDefine.Instance.OnSceneLoadOk();
        }
        //加载选择角色场景health
        UILoadingCtrl.Instance.LoadSceneUI(SceneUIType.SelectRole,
    (GameObject obj) =>
    {
        m_UISceneSelectRoleView = obj.GetComponent<UISceneSelectRoleView>();
        if (m_UISceneSelectRoleView != null)
        {
            //添加场景视图监听
            m_UISceneSelectRoleView.UISelectRoleDragView.OnSelectRoleDrag = OnSelectRoleDrag;

            //设置每个角色的镜头
            if (m_UISceneSelectRoleView.jobItems != null && m_UISceneSelectRoleView.jobItems.Length > 0)
            {
                for (int i = 0; i < m_UISceneSelectRoleView.jobItems.Length; i++)
                {
                    m_UISceneSelectRoleView.jobItems[i].OnSelectJob = OnSelectJobCallBack;
                }
            }
        }
        #region 协议与UI事件
        //进行协议监听
        //服务器返回登录信息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_LogOnGameServerReturn, OnLogOnGameServerReturn);
        //服务器返回创建角色消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_CreateRoleReturn, OnCreateRoleReturn);
        //服务器返回进入游戏消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_EnterGameReturn, OnEnterGameReturn);
        //服务器返回删除角色消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_DeleteRoleReturn, OnDeleteRoleReturn);
        //服务器返回角色信息消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfoReturn, OnSelectRoleInfoReturn);
        //服务器返回角色学会的技能消息
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleData_SkillReturn, OnSkillReturn);

        //开始游戏按钮点击
        m_UISceneSelectRoleView.OnBtnBeginGameClick = OnBtnBeginGameClick;
        //删除角色按钮点击
        m_UISceneSelectRoleView.OnBtnDeleteRoleClick = OnBtnDeleteRoleClick;
        //返回按钮点击
        m_UISceneSelectRoleView.OnBtnReturnClick = OnBtnReturnClick;
        //新建角色按钮点击
        m_UISceneSelectRoleView.OnBtnCreateRoleClick = OnCreateRoleClick;
        #endregion
        //初始化时的职业ID为1
        m_CurrSelectJobId = 1;
        //设置当前选择的角色
        SetSelectJob();
        //加载角色模型
        LoadJobObject(OnLoadJobObjectComplete);
        Debug.Log("完成选角场景加载");
    });
    }

    private void OnDestroy()
    {
        //服务器返回登录信息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_LogOnGameServerReturn, OnLogOnGameServerReturn);
        //服务器返回创建角色消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_CreateRoleReturn, OnCreateRoleReturn);
        //服务器返回进入游戏消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_EnterGameReturn, OnEnterGameReturn);
        //服务器返回角色信息消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfoReturn, OnSelectRoleInfoReturn);
        //服务器返回角色学会的技能消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleData_SkillReturn, OnSkillReturn);
        //服务器返回删除角色消息
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_DeleteRoleReturn, OnDeleteRoleReturn);
    }

    #region 协议
    /// <summary>
    /// 服务器返回登录消息
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLogOnGameServerReturn(byte[] p)
    {
        RoleOperation_LogOnGameServerReturnProto proto = RoleOperation_LogOnGameServerReturnProto.GetProto(p);
        int roleCount = proto.RoleCount;
        Debug.Log("已有角色数：" + roleCount);
        if (roleCount == 0)
        {
            m_IsCreateRole = true;
            SetCreateRoleSceneModelShow(true);//当前缺乏资源，待寻
            m_UISceneSelectRoleView.SetUICreateRoleShow(true);//显示创建角色用的那些UI
            m_UISceneSelectRoleView.SetUISelectRoleActive(false);//隐藏选择角色用的那些UI
            //若无角色，则进入新建界面
            CloneCreateRole();
            //初始化时的职业ID为1
            m_CurrSelectJobId = 1;
            SetSelectJob();
            m_UISceneSelectRoleView.RandomName();
        }
        else
        {
            m_IsCreateRole = false;
            //隐藏其他不该出现的UI
            SetCreateRoleSceneModelShow(false);
            m_UISceneSelectRoleView.SetUICreateRoleShow(false);
            //选择角色页面状态激活
            m_UISceneSelectRoleView.SetUISelectRoleActive(true);
            //否则获取已有角色，让玩家进行选择角色
            if (proto.RoleList != null)
            {
                m_RoleList = proto.RoleList;
                //设置已有角色UI信息
                m_UISceneSelectRoleView.SetRoleList(m_RoleList, SelectRoleCallBack);

                SetSelectRole(m_RoleList[0].RoleId);
            }
        }
        Debug.Log("登录界面已完成");
    }

    /// <summary>
    /// 新建角色服务器返回新建角色消息
    /// </summary>
    /// <param name="p"></param>
    private void OnCreateRoleReturn(byte[] p)
    {
        RoleOperation_CreateRoleReturnProto proto = RoleOperation_CreateRoleReturnProto.GetProto(p);
        if (proto.IsSuccess)
        {
            Debug.Log("角色创建成功,正在完善主角马甲");
        }
        else
        {
            MessageCtrl.Instance.Show("角色创建失败");
        }
    }

    /// <summary>
    /// 已有角色 服务器返回进入游戏消息
    /// </summary>
    /// <param name="p"></param>
    private void OnEnterGameReturn(byte[] p)
    {
        RoleOperation_EnterGameReturnProto proto = RoleOperation_EnterGameReturnProto.GetProto(p);

        if (proto.IsSuccess)
        {
            Debug.Log("进入游戏成功，正在完善主角马甲");

        }
        else
        {
            MessageCtrl.Instance.Show("进入游戏失败");
        }
    }

    /// <summary>
    /// 服务器返回角色信息
    /// </summary>
    /// <param name="p"></param>
    private void OnSelectRoleInfoReturn(byte[] p)
    {
        RoleOperation_SelectRoleInfoReturnProto proto = RoleOperation_SelectRoleInfoReturnProto.GetProto(p);
        //若角色创建成功则切换场景到主城
        if (proto.IsSuccess)
        {
            //设置主角信息
            GlobalInit.Instance.MainPlayerInfo = new RoleInfoMainPlayer(proto);
            m_LastInWorldMapId = proto.LastInWorldMapId;
            PlayerCtrl.Instance.LastInWorldMapId = m_LastInWorldMapId;
            PlayerCtrl.Instance.LastInWorldMapPos = proto.LastInWorldMapPos;
            Debug.Log("角色信息：");
            Debug.Log("最后进入的世界：" + PlayerCtrl.Instance.LastInWorldMapId +" 角色名："+GlobalInit.Instance.MainPlayerInfo.RoleNickName );
        }
    }
    
    /// <summary>
    /// 服务器返回角色学会的技能信息
    /// </summary>
    /// <param name="p"></param>
    private void OnSkillReturn(byte[] p)
    {
        RoleData_SkillReturnProto proto = RoleData_SkillReturnProto.GetProto(p);
        Debug.Log("角色学会的技能数："+proto.CurrSkillDataList.Count);
        GlobalInit.Instance.MainPlayerInfo.LoadSkill(proto);
        //如果玩家只是创建角色而没有去过任何场景，那么默认设置为1，即新手村
        if (PlayerCtrl.Instance.LastInWorldMapId == 0)
        {
            m_LastInWorldMapId = 1;
        }
        PlayerCtrl.Instance.LastInWorldMapId = m_LastInWorldMapId;
        //切换场景
        Debug.Log("测试时默认跳转场景是主城");
        UILoadingCtrl.Instance.LoadToWorldMap(PlayerCtrl.Instance.LastInWorldMapId);
    }

    /// <summary>
    /// 服务器返回删除角色消息
    /// </summary>
    /// <param name="p"></param>
    private void OnDeleteRoleReturn(byte[] p)
    {
        RoleOperation_DeleteRoleReturnProto proto = RoleOperation_DeleteRoleReturnProto.GetProto(p);

        if (proto.IsSuccess)
        {
            Debug.Log("删除角色成功");
            m_UISceneSelectRoleView.CloseDeleteRoleView();
            DeleteRole(m_CurrSelectRoleId);

        }
        else
        {
            MessageCtrl.Instance.Show("删除角色失败");
        }
    }
    #endregion

    #region 按钮回调
    /// <summary>
    /// 开始游戏按钮点击
    /// </summary>
    private void OnBtnBeginGameClick()
    {

        if (m_IsCreateRole)
        {
            //新建角色逻辑
            RoleOperation_CreateRoleProto proto = new RoleOperation_CreateRoleProto();
            proto.JobId = (byte)m_CurrSelectJobId;
            proto.RoleNickName = m_UISceneSelectRoleView.txtNickName.text;

            if (string.IsNullOrEmpty(proto.RoleNickName))
            {
                MessageCtrl.Instance.Show("请输入昵称");
                return;
            }
            Debug.Log("即将进入游戏世界");
            NetWorkSocket.Instance.SendMessage(proto.ToArray());
        }
        else
        {
            //选择已有角色逻辑
            //客户端进入游戏发送消息协议
            Debug.Log("即将进入游戏世界");
            RoleOperation_EnterGameProto proto = new RoleOperation_EnterGameProto();
            proto.RoleId = m_CurrSelectRoleId;
            NetWorkSocket.Instance.SendMessage(proto.ToArray());
        }
    }

    /// <summary>
    /// 删除角色点击
    /// </summary>
    private void OnBtnDeleteRoleClick()
    {
        m_UISceneSelectRoleView.DeleteSelectRole(GetRoleItem(m_CurrSelectRoleId).RoleNickName, OnDeleteRoleClickCallBack);
    }

    /// <summary>
    /// 确定删除点击回调
    /// </summary>
    private void OnDeleteRoleClickCallBack()
    {
        //发送删除角色消息
        RoleOperation_DeleteRoleProto proto = new RoleOperation_DeleteRoleProto();
        proto.RoleId = m_CurrSelectRoleId;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }

    /// <summary>
    /// 新建角色按钮点击
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnCreateRoleClick()
    {
        ToCreateRoleUI();
    }

    /// <summary>
    /// 新建角色方法
    /// </summary>
    private void ToCreateRoleUI()
    {
        m_UISceneSelectRoleView.ClearRoleListUI();
        //删除角色模型
        if (m_CurrSelectRoleModel != null)
        {
            Destroy(m_CurrSelectRoleModel);
        }
        //新建角色
        m_IsCreateRole = true;
        SetCreateRoleSceneModelShow(true);
        m_UISceneSelectRoleView.SetUICreateRoleShow(true);
        m_UISceneSelectRoleView.SetUISelectRoleActive(false);

        CloneCreateRole();

        //初始化时，职业默认是初始职业
        m_CurrSelectJobId = 1;
        SetSelectJob();
        m_UISceneSelectRoleView.RandomName();
    }


    /// <summary>
    /// 返回按钮点击
    /// </summary>
    private void OnBtnReturnClick()
    {
        //如果是新建角色界面 且 当前没有角色 则返回选区场景,并与服务器断开连接
        //若是新建角色界面 则返回已有角色界面
        //若是已有角色界面，则返回选区场景
        if (m_IsCreateRole)
        {
            if (m_RoleList == null || m_RoleList.Count == 0)
            {
                NetWorkSocket.Instance.DisConnect();
                UILoadingCtrl.Instance.LoadToLogOn();
            }
            else
            {
                //清除新建角色UI与模型
                ClearCloneCreateRole();
                //固定相机
                m_IsRotating = false;
                m_CurrSelectRoleId = 0;
                DragTarget.eulerAngles = Vector3.zero;

                //返回已有角色界面
                m_IsCreateRole = false;
                SetCreateRoleSceneModelShow(false);
                m_UISceneSelectRoleView.SetUICreateRoleShow(false);
                m_UISceneSelectRoleView.SetUISelectRoleActive(true);

                m_UISceneSelectRoleView.SetRoleList(m_RoleList, SelectRoleCallBack);
                SetSelectRole(m_RoleList[0].RoleId);
            }
        }
        else
        {
            NetWorkSocket.Instance.DisConnect();
            UILoadingCtrl.Instance.LoadToLogOn();
        }
    }

    /// <summary>
    /// 清除克隆的角色
    /// </summary>
    private void ClearCloneCreateRole()
    {
        if (m_CloneCreateRoleList != null && m_CloneCreateRoleList.Count > 0)
        {
            for (int i = 0; i < m_CloneCreateRoleList.Count; i++)
            {
                Destroy(m_CloneCreateRoleList[i]);
            }
            m_CloneCreateRoleList.Clear();
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        ///若选人镜头正在旋转
        if (m_IsRotating)
        {
            float toAngle = Mathf.MoveTowardsAngle(DragTarget.eulerAngles.y, CameraRotate[m_CurrSelectJobId-1], Time.deltaTime * m_RotateSpeed);
            DragTarget.eulerAngles = Vector3.up * toAngle;
            if (Mathf.RoundToInt(CameraRotate[m_CurrSelectJobId-1]) == Mathf.RoundToInt(toAngle) || Mathf.RoundToInt(CameraRotate[m_CurrSelectJobId-1] + 360) == Mathf.RoundToInt(toAngle))
            {
                m_IsRotating = false;
                DragTarget.eulerAngles = Vector3.up * CameraRotate[m_CurrSelectJobId-1];
                Debug.Log("旋转已完成,现在的职业是：" + m_CurrSelectJobId);
            }

        }
    }

    #region 场景资源功能初始化
    /// <summary>
    /// UI上点击职业项后的回调-设置相机要移动的幅度
    /// </summary>
    /// <param name="jobId">职业ID</param>
    /// <param name="rotateAngle">旋转角度</param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnSelectJobCallBack(int jobId, int rotateAngle)
    {
        Debug.Log("jobId" + jobId);
        Debug.Log("rotateAngle" + rotateAngle);

        if (m_IsRotating)
        {
            return;
        }

        m_CurrSelectJobId = jobId;
        SetSelectJob();

        m_IsRotating = true;
        m_TargetAngle = rotateAngle;

    }

    /// <summary>
    /// 角色选择拖拽委托 0左1右
    /// </summary>
    /// <param name="obj"></param>
    private void OnSelectRoleDrag(int obj)
    {
        if (m_IsRotating)
        {
            return;
        }

        m_RotateAngle = Mathf.Abs(m_RotateAngle) * (obj == 0 ? -1 : 1);
        m_IsRotating = true;
        m_TargetAngle = DragTarget.eulerAngles.y + m_RotateAngle;

        //处理当前选中的职业ID
        if (obj == 1)
        {
            m_CurrSelectJobId--;
            if (m_CurrSelectJobId <= 0)
            {
                m_CurrSelectJobId = 2;
            }
        }
        else
        {
            m_CurrSelectJobId++;
            if (m_CurrSelectJobId > 2)
            {
                m_CurrSelectJobId = 1;
            }
        }
        SetSelectJob();
    }

    /// <summary> 
    /// 加载角色镜像
    /// </summary>
    private void LoadJobObject(Action onComplete)
    {
        Debug.Log("职业数：" + m_JobList.Count);
        LoadJob(0, onComplete);
    }

    /// <summary>
    /// 使用递归的手段加载职业信息
    /// </summary>
    /// <param name="index">职业下标</param>
    /// <param name="onComplete">完成委托</param>
    private void LoadJob(int index, Action onComplete)
    {
        JobEntity entity = m_JobList[index];

        AssetBundleMgr.Instance.LoadOrDownload(String.Format("Download/Prefab/RolePrefab/Player/{0}.assetbundle", entity.PrefabName), entity.PrefabName,
            (GameObject obj) =>
            {
                if (obj != null)
                {
                    GlobalInit.Instance.JobObjectDic[entity.Id] = obj;
                    index++;
                    if (index == m_JobList.Count)
                    {
                        //加载完成，执行加载委托
                        if (onComplete != null)
                        { onComplete(); }
                    }
                    else
                    { LoadJob(index, onComplete); }
                }
            });
    }

    /// <summary>
    /// 职业物品加载完成回调
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLoadJobObjectComplete()
    {
        LogOnGameServer();
    }

    /// <summary>
    /// 登录区服消息
    /// </summary>
    private void LogOnGameServer()
    {
        RoleOperation_LogOnGameServerProto proto = new RoleOperation_LogOnGameServerProto();
        proto.AccountId = GlobalInit.Instance.CurrAccount.Id;
        //向服务器发送
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }
    #endregion

    /// <summary>
    /// 设置选择的职业
    /// </summary>
    private void SetSelectJob()
    {
        //获取所有职业列表集合:
        m_JobList = JobDBModel.Instance.GetList();
        //更新选择的职业信息
        for (int i = 0; i < m_JobList.Count; i++)
        {
            if (m_JobList[i].Id == m_CurrSelectJobId)
            {
                m_UISceneSelectRoleView.selectRoleJobDescView.SetUI(m_JobList[i].Name, m_JobList[i].Desc);
                break;
            }
        }
        for (int i = 0; i < m_UISceneSelectRoleView.jobItems.Length; i++)
        {
            m_UISceneSelectRoleView.jobItems[i].SetSelected(m_CurrSelectJobId);
        }
    }

    /// <summary>
    /// 选择已有角色回调
    /// </summary>
    /// <param name="roleId"></param>
    private void SelectRoleCallBack(int roleId)
    {
        SetSelectRole(roleId);
    }

    /// <summary>
    /// 设置选择的角色
    /// </summary>
    /// <param name="roleId"></param>
    private void SetSelectRole(int roleId)
    {
        //如果反复点击同一角色则无反应
        if (m_CurrSelectRoleId == roleId)
        {
            return;
        }
        m_CurrSelectRoleId = roleId;

        //如果选择界面上已经有角色了，销毁
        if (m_CurrSelectRoleModel != null)
        {
            Destroy(m_CurrSelectRoleModel);
        }

        RoleOperation_LogOnGameServerReturnProto.RoleItem item = GetRoleItem(roleId);
        //根据角色的职业ID克隆角色
        if (CreateRoleContainers == null || CreateRoleContainers.Length < 4)
        {
            return;
        }

        m_CurrSelectRoleModel = Instantiate(GlobalInit.Instance.JobObjectDic[item.RoleJob]);
        m_CurrSelectRoleModel.transform.parent = CreateRoleContainers[0];

        m_CurrSelectRoleModel.transform.localScale = Vector3.one;
        m_CurrSelectRoleModel.transform.localRotation = Quaternion.Euler(Vector3.zero);
        m_CurrSelectRoleModel.transform.localPosition = Vector3.zero;

        RoleCtrl roleCtrl = m_CurrSelectRoleModel.GetComponent<RoleCtrl>();
    }


    #region 新建角色相关代码
    /// <summary>
    /// 设置新建角色场景模型是否显示
    /// </summary>
    /// <param name="isShow"></param>
    public void SetCreateRoleSceneModelShow(bool isShow)
    {
        if (CreateRoleSceneModel != null && CreateRoleSceneModel.Length > 0)
        {
            for (int i = 0; i < CreateRoleSceneModel.Length; i++)
            {
                CreateRoleSceneModel[i].gameObject.SetActive(isShow);
            }
        }
    }

    /// <summary>
    /// 克隆新建角色
    /// </summary>
    private void CloneCreatRole()
    {
        for (int i = 0; i < m_JobList.Count; i++)
        {
            GameObject obj = Instantiate(GlobalInit.Instance.JobObjectDic[m_JobList[i].Id]);
        }
    }

    /// <summary>
    /// 克隆所有职业模型
    /// </summary>
    private void CloneCreateRole()
    {
        if (CreateRoleContainers == null || CreateRoleContainers.Length < 4)
        {
            return;
        }
        ClearCloneCreateRole();
        for (int i = 0; i < m_JobList.Count; i++)
        {
            GameObject objRole = Instantiate(GlobalInit.Instance.JobObjectDic[m_JobList[i].Id]);
            objRole.transform.parent = CreateRoleContainers[i];

            objRole.transform.localScale = Vector3.one;
            objRole.transform.localPosition = Vector3.zero;
            objRole.transform.localRotation = Quaternion.Euler(Vector3.zero);

            m_CloneCreateRoleList.Add(objRole);

            RoleCtrl roleCtrl = objRole.GetComponent<RoleCtrl>();
            if (roleCtrl != null)
            {
                m_JobRoleCtrl[m_JobList[i].Id] = roleCtrl;
            }
        }

    }
    #endregion

    #region 已有角色相关
    /// <summary>
    /// 根据角色编号获取已有角色项
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    private RoleOperation_LogOnGameServerReturnProto.RoleItem GetRoleItem(int roleId)
    {
        if (m_RoleList != null)
        {
            for (int i = 0; i < m_RoleList.Count; i++)
            {
                if (m_RoleList[i].RoleId == roleId)
                {
                    return m_RoleList[i];
                }
            }
        }
        return default;
    }




    #endregion

    /// <summary>
    /// 从本地列表删除角色
    /// </summary>
    /// <param name="roleId"></param>
    private void DeleteRole(int roleId)
    {
        for (int i = m_RoleList.Count - 1; i >= 0; i--)
        {
            if (m_RoleList[i].RoleId == roleId)
            {
                m_RoleList.RemoveAt(i);
            }
        }
        if (m_RoleList.Count == 0)
        {
            ToCreateRoleUI();
        }
        else
        {
            m_UISceneSelectRoleView.SetRoleList(m_RoleList, SelectRoleCallBack);
            SetSelectRole(m_RoleList[0].RoleId);
        }
    }

}
