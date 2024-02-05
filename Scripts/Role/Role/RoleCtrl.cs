using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 角色控制器
/// </summary>
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(FunnelModifier))]
public class RoleCtrl : MonoBehaviour
{
    #region 变量
    #region 角色基本属性
    /// <summary>
    /// 移动速度
    /// </summary>
    public float Speed = 5.0f;
    /// <summary>
    /// 视野范围
    /// </summary>
    public float ViewRange = 10f;
    /// <summary>
    /// 巡逻范围
    /// </summary>
    public float PatrolRange = 5f;
    /// <summary>
    /// 攻击范围
    /// </summary>
    public float AttackRange = 1.5f;
    /// <summary>
    /// 生命值
    /// </summary>
    public float HP;
    /// <summary>
    /// 蓝量
    /// </summary>
    public float MP;
    //角色枚举类型
    public RoleType CurrRoleType = RoleType.None;
    /// <summary>
    /// 角色是否已经死亡
    /// </summary>
    [HideInInspector]
    public bool IsDied;
    /// <summary>
    /// 角色死亡音效名
    /// </summary>
    [SerializeField]
    private string DieAudioName;
    #endregion

    #region 角色挂件
    /// <summary>
    /// 动画
    /// </summary>
    public Animator Animator;
    /// <summary>
    /// 角色的画布
    /// </summary>
    public Canvas m_RoleCanvas;
    /// <summary>
    /// 角色名
    /// </summary>
    public Text m_NickName;
    /// <summary>
    /// 玩家头顶UI位置
    /// </summary>
    public Transform m_HeadBarPos;
    /// <summary>
    /// 玩家头顶UI
    /// </summary>
    [HideInInspector]
    public GameObject m_HeadBar;

    #endregion

    #region 角色战斗相关
    /// <summary>
    /// 角色攻击类
    /// </summary>
    public RoleAttack Attack;

    /// <summary>
    /// 角色受伤类
    /// </summary>
    private RoleHurt m_Hurt;

    /// <summary>
    /// 角色是否处于僵直状态
    /// </summary>
    [HideInInspector]
    public bool IsRigidity;

    /// <summary>
    /// 角色销毁委托
    /// </summary>
    public Action<Transform> OnRoleDestroy;

    /// <summary>
    /// 上次战斗的时间
    /// </summary>
    [HideInInspector]
    public float PrevFightTime = 0f;

    /// <summary>
    /// 播放攻击动画
    /// </summary>
    /// <param name="skillId"></param>
    public void PlayAttack(int skillId)
    {
        Attack.PlayAttack(skillId);
    }
    #endregion

    #region 角色自动分配属性/委托
    /// <summary>
    /// 是否完成初始化
    /// </summary>
    private bool m_IsInit = false;
    /// <summary>
    /// 当前攻击信息
    /// </summary>
    public RoleAttackInfo CurrAttackInfo;
    /// <summary>
    /// 当前角色信息
    /// </summary>
    public RoleInfoBase CurrentRoleInfo = null;
    /// <summary>
    /// 当前角色AI
    /// </summary>
    public IRoleAI CurrRoleAI = null;
    /// <summary>
    /// 角色控制器
    /// </summary>
    [HideInInspector]
    public CharacterController m_CharacterController;
    /// <summary>
    /// 当前角色有限状态机管理器
    /// </summary>
    public RoleFSMMgr CurrentRoleFSMMgr = null;
    /// <summary>
    /// 角色移动的目标位置
    /// </summary>
    [HideInInspector]
    public Vector3 m_TargetPos = Vector3.zero;
    /// <summary>
    /// 滚轮事件---为了实现镜头的向前，向后
    /// </summary>
    private float mouseScollWheel;
    private float screenX;
    private float screenY;
    private float limitScreenX;
    private float limitScreenY;
    private bool openMouseMove = false;
    /// <summary>
    /// 小怪出生点
    /// </summary> 
    [HideInInspector]
    public Vector3 BornPoint;
    /// <summary>
    /// 锁定敌人
    /// </summary>
    [HideInInspector]
    public RoleCtrl LockEnemy;
    /// <summary>
    /// A*寻路：计算路径类
    /// </summary>
    private Seeker m_Seeker;
    public Seeker Seeker
    {
        get { return m_Seeker; }
    }
    /// <summary>
    /// A*寻路路径存储类
    /// </summary>
    [HideInInspector]
    public ABPath AStartPath;
    /// <summary>
    /// 当前想要前往的路径点索引    
    /// </summary>
    public int AStartCurrWayPointIndex = 1;
    /// <summary>
    /// 角色在其他玩家眼中的移动速度
    /// </summary>
    [HideInInspector]
    public float ModifySpeed = 0f;
    /// <summary>
    /// 数字变化委托原型
    /// </summary>
    /// <param name="type"></param>
    public delegate void OnValueChangeHandler(ValueChangeType type);
    /// <summary>
    /// HP变化委托
    /// </summary>
    public OnValueChangeHandler OnHPChange;
    /// <summary>
    /// MP变化委托
    /// </summary>
    public OnValueChangeHandler OnMPChange;
    /// <summary>
    /// 角色受伤委托
    /// </summary>
    [HideInInspector]
    public Action OnRoleHurt;
    ///角色死亡委托:<>:监听对象(即参数)
    [HideInInspector]
    public Action<RoleCtrl> OnRoleDie;
    /// <summary>
    /// 角色血条类  
    /// </summary>
    [HideInInspector]
    public RoleHeadBarView roleHeadBarView;
    #endregion

    #region 测试用属性
    /// <summary>
    /// 鼠标的XY位置
    /// </summary>
    private float mouseX;
    private float mouseY;
    #endregion

    #endregion   

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();

        //获取寻路组件：计算路径类
        m_Seeker = GetComponent<Seeker>();

        roleHeadBarView = GetComponentInChildren<RoleHeadBarView>();
        screenX = Screen.width / 2;
        screenY = Screen.height / 2;
        limitScreenX = screenX / 4;
        limitScreenY = screenY / 4;
        //摄像机初始化：仅限玩家
        if (CurrRoleType == RoleType.MainPlayer)
        {
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.CameraInit();
            }
        }

        //角色状态机实例化
        CurrentRoleFSMMgr = new RoleFSMMgr(this, OnDieCallBack, OnDestroyCallBack);
        m_Hurt = new RoleHurt(CurrentRoleFSMMgr);
        m_Hurt.OnRoleHurt = OnRoleHurtCallBack;
        Attack.SetFSM(CurrentRoleFSMMgr);

        InitHeadBar();
    }

    /// <summary>
    /// 销毁回调
    /// </summary>
    private void OnDestroyCallBack()
    {
        if (OnRoleDestroy != null)
        {
            OnRoleDestroy(transform);
        }
        if (roleHeadBarView != null)
        {
            Destroy(roleHeadBarView.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_HeadBar == null && CurrRoleType == RoleType.MainPlayer && UILoadingCtrl.Instance.CurrentSceneType != SceneType.SelectRole)
        {
            Debug.Log("设置主角色名");
            InitHeadBar();
        }

        //角色AI:判定角色的AI情况，没有AI的角色没必要浪费心思
        if (CurrRoleAI == null)
        { return; }
        CurrRoleAI.DoAI();

        //如果当前角色在某一状态，必须每帧更新角色的状态
        if (CurrentRoleFSMMgr != null)
        {
            CurrentRoleFSMMgr.OnUpdate();
        }

        if (m_IsInit)
        {
            m_IsInit = false;
            //当启动时如果角色就是死人，那就不需要播放倒下动画
            if (CurrentRoleInfo.CurrHP <= 0)
            {
                ToDie(isDied: true);
            }
            else
            {

                //为怪物提供信息动作(每次出生时进行一次状态初始化)
                if (this.CurrRoleType == RoleType.Monster)
                {
                    ToIdle(RoleIdleState.IdleFight);
                }
                else
                {
                    ToIdle();
                }
            }
        }
        if (CurrRoleType == RoleType.MainPlayer)
        {
            //角色移动
            characterMoveWithMouseOrTouch(Speed);
            //测试刷怪笼用方法
            //characterTest();
            //小地图和相机调整
            CameraAutoFollow();
        }
    }

    private void OnDestroy()
    {
        OnDestroyCallBack();
    }

    #region 地图管理-小地图和相机
    /// <summary>
    /// 小地图功能
    /// </summary>
    private void AutoSmallMap()
    {
        if (SmallMapHelper.Instance == null || UIMainCitySmallMapView.Instance == null)
        {
            return;
        }
        //让参考点位置永远等于玩家位置
        SmallMapHelper.Instance.transform.position = transform.position;
        //设置小地图位置，*512是因为地图大小就是512，*-是因为向左走是负的
        UIMainCitySmallMapView.Instance.SmallMap.transform.localPosition = new Vector3(SmallMapHelper.Instance.transform.position.x * GlobalInit.Instance.smallMapFollowRate, SmallMapHelper.Instance.transform.position.z * GlobalInit.Instance.smallMapFollowRate, 0);
        //让小箭头能够实时反应角色方向
        UIMainCitySmallMapView.Instance.SmallMapPointer.transform.localEulerAngles = new Vector3(0, 0, 360 - transform.eulerAngles.y);
    }

    /// <summary>
    /// 摄像机自动跟随
    /// </summary>
    private void CameraAutoFollow()
    {

        //当打开的UI窗口大于1时，不跟随
        if (UIViewUtil.Instance.OpenWindowCount > 0)
        {
            return;
        }

        if (CameraManager.Instance == null)
        { return; }
        //实时同步摄像机和人物的位置
        CameraManager.Instance.transform.position = gameObject.transform.position;
        CameraManager.Instance.AutoLookAt(gameObject.transform.position);


        ///通过上下左右12进行键盘移动屏幕
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            CameraManager.Instance.SetCameraRotate(0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            CameraManager.Instance.SetCameraRotate(1);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            CameraManager.Instance.SetCameraUpAndDown(0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            CameraManager.Instance.SetCameraUpAndDown(1);
        }
        else if (Input.GetKey(KeyCode.Keypad1))
        {
            CameraManager.Instance.SetCameraZoom(1);
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            CameraManager.Instance.SetCameraZoom(0);
        }

        ///通过键鼠移动屏幕
        ///按F1键使得可以通过鼠标控制屏幕移动
        mouseScollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mouseScollWheel != 0)
        {
            CameraManager.Instance.SetCameraZoom(mouseScollWheel * 10);
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            openMouseMove = !openMouseMove;
        }
        if (openMouseMove)
        {
            //使用鼠标控制的屏幕移动
            mouseX = Input.mousePosition.x - screenX;
            mouseY = Input.mousePosition.y - screenY;

            if (Mathf.Abs(mouseX) > limitScreenX)
            {
                CameraManager.Instance.SetCameraRotate(mouseX);
            }
            if (Mathf.Abs(mouseY) > limitScreenY)
            {
                CameraManager.Instance.SetCameraUpAndDown(mouseY);
            }
        }
        else
        {
            return;
        }
    }
    #endregion

    #region 角色初始化部分
    /// <summary>
    /// 角色初始化--信息类
    /// </summary>
    /// <param name="roleType">初始化需求：角色类型</param>
    /// <param name="roleInfo">初始化需求：角色信息</param>
    /// <param name="ai">初始化类型：AI</param>
    public void Init(RoleType roleType, RoleInfoBase roleInfo, IRoleAI ai)
    {
        CurrRoleType = roleType;
        CurrentRoleInfo = roleInfo;
        CurrRoleAI = ai;
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = true;
        }
        m_IsInit = true;
        HP = roleInfo.MaxHP;
        MP = roleInfo.MaxMP;
    }

    /// <summary>
    /// 角色出生设定
    /// </summary>
    /// <param name="born_Pos"></param>
    public void Born(Vector3 born_Pos)
    {
        BornPoint = born_Pos;
        transform.position = born_Pos;
        //InitHeadBar();
    }

    /// <summary>
    /// 初始化头顶标签
    /// </summary>
    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (CurrentRoleInfo == null) return;
        if (m_HeadBarPos == null) return;

        //m_HeadBar = ResourceMgr.Instance.Load(ResourcesType.Other, "RoleHeadBar");
        //加载角色头顶UI条
        AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/RolePrefab/Player/RoleHeadBar.assetbundle"), "RoleHeadBar",
            (GameObject obj) =>
            {
                m_HeadBar = Instantiate(obj);

                m_HeadBar.transform.SetParent(RoleHeadBarRoot.Instance.gameObject.transform);
                m_HeadBar.transform.localScale = Vector3.one;

                roleHeadBarView = m_HeadBar.GetComponent<RoleHeadBarView>();

                roleHeadBarView.Init(m_HeadBarPos, CurrentRoleInfo.RoleNickName, isShowHPBar: (CurrRoleType == RoleType.MainPlayer ? false : true),
                    sliderValue: (float)this.CurrentRoleInfo.CurrHP / this.CurrentRoleInfo.MaxHP);
            }, type: 0);
    }

    /// <summary>
    /// 角色初始化--展示类
    /// </summary>
    public void InitShow(string nickName)
    {
        //能初始化的前提是主角的专用画布开启了
        if (!m_RoleCanvas.isActiveAndEnabled)
            return;
        //角色名
        m_NickName.text = nickName;
    }
    #endregion

    #region 角色移动
    /// <summary>
    /// 角色触屏/鼠标移动方法
    /// </summary>
    /// <param name="m_speed">移动速度</param>
    private void characterMoveWithMouseOrTouch(float m_speed)
    {
        //主角挂机不能移动
        if (UILoadingCtrl.Instance.CurrentSceneType == SceneType.ShanGu)
        {
            if (GlobalInit.Instance.currentPlayer.Attack.IsAutoFight)
            {
                return;
            }
        }
        //角色控制器空----程序OVER
        if (m_CharacterController == null)
        {
            Debug.Log("角色无控制器");
            return;
        }
        //点击屏幕
        //Input.GetMouseButtonDown(0)：鼠标左键，见过很多次了
        //Input.touchCount：手指触碰屏幕时，单手指返回1，双返回2
        if ((Input.GetMouseButtonDown(0) || Input.touchCount == 1) && EventSystem.current != null)
        {
            //进行UGUI防误触检测

            if (EventSystem.current.IsPointerOverGameObject() == true)
            {
                Debug.Log("穿过UI了");
                return;
            }


            //玩家点击地面时，取消自动移动
            WorldMapCtrl.Instance.IsAutoMove = false;

            //进行屏幕检测
            //射线检测
            //ScreenPointToRay：从相机发射一条射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //获取碰到的角色
            RaycastHit[] hitArr = Physics.RaycastAll(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));

            if (hitArr.Length > 0)
            {
                RoleCtrl hitRole = hitArr[0].collider.gameObject.GetComponent<RoleCtrl>();
                //若点击到的是怪，则主角的敌人是怪
                if (hitRole.CurrRoleType == RoleType.Monster)
                {
                    GlobalInit.Instance.currentPlayer.LockEnemy = hitRole;
                    return;
                }
                else if (hitRole.CurrRoleType == RoleType.OtherPlayer)
                {
                    //判定玩家当前所处场景是否允许玩家互殴
                    GlobalInit.Instance.currentPlayer.LockEnemy = hitRole;
                    return;
                    
                }
            }

            RaycastHit hitInfo;
            //将射线碰到的物体返回给hitInfo
            if (Physics.Raycast(ray, out hitInfo, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {

                //检测该点的名字是否是地面，并移动
                if (GlobalInit.Instance.currentPlayer != null)
                {
                    GlobalInit.Instance.currentPlayer.LockEnemy = null;

                    GlobalInit.Instance.currentPlayer.MoveTo(hitInfo.point);
                }

            }


        }
        //角色控制器检测角色是否着地（很好用）
        if (!m_CharacterController.isGrounded)
        {
            m_CharacterController.Move((new Vector3(0, -0.001f, 0)));
        }
        //小地图和相机调整
        if (CurrRoleType == RoleType.MainPlayer)
        {
            CameraAutoFollow();
            AutoSmallMap();
        }
    }

    /// <summary>
    /// 角色移动方法---动画版---ToRun
    /// </summary>
    /// <param name="targetPos"></param>
    public void MoveTo(Vector3 targetPos)
    {
        //僵直和死亡状态下角色是不该移动的
        if (IsRigidity || CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die)
        {
            return;
        }
        //如果目的地就是原点，返回
        if (targetPos == Vector3.zero)
        {
            return;
        }

        m_TargetPos = targetPos;

        //1.使用AStar进行路径计算
        m_Seeker.StartPath(transform.position, targetPos, (Path p) =>
        {
            if (!p.error)
            {
                //ABPath:Path的子类之一
                AStartPath = (ABPath)p;
                //计算寻路距离:若目标点不可到达。则处理，
                if (Vector3.Distance(AStartPath.endPoint, new Vector3(AStartPath.originalEndPoint.x, AStartPath.endPoint.y, AStartPath.originalStartPoint.z)) < 0.5f)
                {
                    Debug.Log("目标点无法到达");
                    AStartPath = null;
                }
                else
                {
                    //发送消息给服务器，让服务器同步玩家位置
                    if (CurrRoleType == RoleType.MainPlayer)
                    {
                        //PVP发送消息给服务器
                        SendPVPMove(targetPos, AStartPath.vectorPath);
                    }

                    AStartCurrWayPointIndex = 1;
                    CurrentRoleFSMMgr.ChangeState(RoleState.Run);
                }
            }
            else
            {
                Debug.Log("寻路报错");
                AStartPath = null;
            }
        });

    }
    #endregion

    #region PVP移动同步化
    /// <summary>
    /// 和服务端进行角色移动同步
    /// </summary>
    /// <param name="targetPos">目标点</param>
    /// <param name="path">A*算法计算出来的路径表</param>
    private void SendPVPMove(Vector3 targetPos, List<Vector3> path)
    {
        //在PVP环境下，发消息给服务器，让服务器同步角色移动
        if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVP)
        {
            //计算路径的总长度
            float pathLen = 0f;
            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                { continue; }
                float distance = Vector3.Distance(path[i], path[i + 1]);
                pathLen += distance;
            }
            //计算理论上角色到达目标点所需要的时间
            float needTime = pathLen / Speed;

            WorldMap_CurrRoleMoveProto proto = new WorldMap_CurrRoleMoveProto();
            proto.TargetPosX = targetPos.x;
            proto.TargetPosY = targetPos.y;
            proto.TargetPosZ = targetPos.z;
            //获取当前的服务器时间
            proto.ServerTime = GlobalInit.Instance.GetCurrentServerTime();
            proto.NeedTime = (int)(needTime * 1000);

            Debug.Log("角色在PVP场景中要移动到的坐标是：" + targetPos);
            Debug.Log("要移动的距离为：" + pathLen);
            Debug.Log("要移动的理论时间是：" + needTime);
            NetWorkSocket.Instance.SendMessage(proto.ToArray());
        }
    }
    #endregion

    #region 角色动画控制
    /// <summary>
    /// 休闲模式
    /// </summary>
    /// <param name="state"></param>
    public void ToIdle(RoleIdleState state = RoleIdleState.IdleNormal)
    {
        CurrentRoleFSMMgr.ToIdleState = state;
        CurrentRoleFSMMgr.ChangeState(RoleState.Idle);
    }

    /// <summary>
    /// 根据技能索引发起攻击
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void ToAttackByIndex(RoleAttackType type, int index)
    {
        Attack.ToAttackByIndex(type, index);
    }

    /// <summary>
    /// 根据技能编号发起攻击
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    public bool ToAttack(RoleAttackType type, int skillId)
    {
        return Attack.ToAttack(type, skillId);
    }

    /// <summary>
    /// 受伤
    /// </summary>
    /// <param name="roleTransferAttackInfo"></param>
    public void ToHurt(RoleTransferAttackInfo roleTransferAttackInfo)
    {
        StartCoroutine(m_Hurt.ToHurt(roleTransferAttackInfo));
    }

    /// <summary>
    /// 角色死亡
    /// </summary>
    /// <param name="isDied"></param>
    public void ToDie(bool isDied = false)
    {
#if !DEBUG_ROLESTATE
        IsDied = isDied;
        CurrentRoleInfo.CurrHP = 0;
#endif
        //播放角色死亡音效TODO
        //PlayAudio(DieAudioName, 0);
        CurrentRoleFSMMgr.ChangeState(RoleState.Die);
    }

    public void ToSelect()
    {
        CurrentRoleFSMMgr.ChangeState(RoleState.Select);
    }
    #endregion

    #region 角色受伤-死亡-复活相关
    /// <summary>
    /// 角色受伤回调
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnRoleHurtCallBack()
    {
        if (roleHeadBarView != null)
        {
            roleHeadBarView.SetSliderHP((float)CurrentRoleInfo.CurrHP / CurrentRoleInfo.MaxHP); ;
        }
        if (OnHPChange != null)
        {
            OnHPChange(ValueChangeType.Subtract);
        }
    }

    /// <summary>
    /// 角色死亡回调
    /// </summary>
    private void OnDieCallBack()
    {
        //将角色控制器失活（复活的时候记得激活）
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = false;
        }
        if (OnRoleDie != null && CurrentRoleInfo != null)
        {
            OnRoleDie(this);
        }
    }

    /// <summary>
    /// 角色复活状态恢复方法
    /// </summary>
    public void ToResurgence(RoleIdleState state = RoleIdleState.IdleFight)
    {
        if (m_CharacterController != null)
        {
            m_CharacterController.enabled = true;
        }
        PrevFightTime = 0;
        CurrentRoleInfo.CurrHP = CurrentRoleInfo.MaxHP;
        CurrentRoleInfo.CurrMP = CurrentRoleInfo.MaxMP;
        LockEnemy = null;
        if (OnHPChange != null)
        {
            OnHPChange(ValueChangeType.Add);
        }
        if (OnMPChange != null)
        {
            OnMPChange(ValueChangeType.Add);
        }
        this.roleHeadBarView.SetSliderHP(1);
        ToIdle(state);
    }


    #endregion

    #region 角色音效
    /// <summary>
    /// 播放角色音效
    /// </summary>
    /// <param name="audioName">音效名</param>
    /// <param name="delayTime">延迟时间</param>
    public void PlayAudio(string audioName, float delayTime)
    {
        StartCoroutine(PlayAudioCoroutine(audioName, delayTime));
    }

    private IEnumerator PlayAudioCoroutine(string audioName, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        AudioEffectMgr.Instance.Play(string.Format("Download/Audio/Fight/{0}", audioName), transform.position, is3D: true);
    }
    #endregion
}

