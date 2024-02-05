using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��ɫ������
/// </summary>
[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(FunnelModifier))]
public class RoleCtrl : MonoBehaviour
{
    #region ����
    #region ��ɫ��������
    /// <summary>
    /// �ƶ��ٶ�
    /// </summary>
    public float Speed = 5.0f;
    /// <summary>
    /// ��Ұ��Χ
    /// </summary>
    public float ViewRange = 10f;
    /// <summary>
    /// Ѳ�߷�Χ
    /// </summary>
    public float PatrolRange = 5f;
    /// <summary>
    /// ������Χ
    /// </summary>
    public float AttackRange = 1.5f;
    /// <summary>
    /// ����ֵ
    /// </summary>
    public float HP;
    /// <summary>
    /// ����
    /// </summary>
    public float MP;
    //��ɫö������
    public RoleType CurrRoleType = RoleType.None;
    /// <summary>
    /// ��ɫ�Ƿ��Ѿ�����
    /// </summary>
    [HideInInspector]
    public bool IsDied;
    /// <summary>
    /// ��ɫ������Ч��
    /// </summary>
    [SerializeField]
    private string DieAudioName;
    #endregion

    #region ��ɫ�Ҽ�
    /// <summary>
    /// ����
    /// </summary>
    public Animator Animator;
    /// <summary>
    /// ��ɫ�Ļ���
    /// </summary>
    public Canvas m_RoleCanvas;
    /// <summary>
    /// ��ɫ��
    /// </summary>
    public Text m_NickName;
    /// <summary>
    /// ���ͷ��UIλ��
    /// </summary>
    public Transform m_HeadBarPos;
    /// <summary>
    /// ���ͷ��UI
    /// </summary>
    [HideInInspector]
    public GameObject m_HeadBar;

    #endregion

    #region ��ɫս�����
    /// <summary>
    /// ��ɫ������
    /// </summary>
    public RoleAttack Attack;

    /// <summary>
    /// ��ɫ������
    /// </summary>
    private RoleHurt m_Hurt;

    /// <summary>
    /// ��ɫ�Ƿ��ڽ�ֱ״̬
    /// </summary>
    [HideInInspector]
    public bool IsRigidity;

    /// <summary>
    /// ��ɫ����ί��
    /// </summary>
    public Action<Transform> OnRoleDestroy;

    /// <summary>
    /// �ϴ�ս����ʱ��
    /// </summary>
    [HideInInspector]
    public float PrevFightTime = 0f;

    /// <summary>
    /// ���Ź�������
    /// </summary>
    /// <param name="skillId"></param>
    public void PlayAttack(int skillId)
    {
        Attack.PlayAttack(skillId);
    }
    #endregion

    #region ��ɫ�Զ���������/ί��
    /// <summary>
    /// �Ƿ���ɳ�ʼ��
    /// </summary>
    private bool m_IsInit = false;
    /// <summary>
    /// ��ǰ������Ϣ
    /// </summary>
    public RoleAttackInfo CurrAttackInfo;
    /// <summary>
    /// ��ǰ��ɫ��Ϣ
    /// </summary>
    public RoleInfoBase CurrentRoleInfo = null;
    /// <summary>
    /// ��ǰ��ɫAI
    /// </summary>
    public IRoleAI CurrRoleAI = null;
    /// <summary>
    /// ��ɫ������
    /// </summary>
    [HideInInspector]
    public CharacterController m_CharacterController;
    /// <summary>
    /// ��ǰ��ɫ����״̬��������
    /// </summary>
    public RoleFSMMgr CurrentRoleFSMMgr = null;
    /// <summary>
    /// ��ɫ�ƶ���Ŀ��λ��
    /// </summary>
    [HideInInspector]
    public Vector3 m_TargetPos = Vector3.zero;
    /// <summary>
    /// �����¼�---Ϊ��ʵ�־�ͷ����ǰ�����
    /// </summary>
    private float mouseScollWheel;
    private float screenX;
    private float screenY;
    private float limitScreenX;
    private float limitScreenY;
    private bool openMouseMove = false;
    /// <summary>
    /// С�ֳ�����
    /// </summary> 
    [HideInInspector]
    public Vector3 BornPoint;
    /// <summary>
    /// ��������
    /// </summary>
    [HideInInspector]
    public RoleCtrl LockEnemy;
    /// <summary>
    /// A*Ѱ·������·����
    /// </summary>
    private Seeker m_Seeker;
    public Seeker Seeker
    {
        get { return m_Seeker; }
    }
    /// <summary>
    /// A*Ѱ··���洢��
    /// </summary>
    [HideInInspector]
    public ABPath AStartPath;
    /// <summary>
    /// ��ǰ��Ҫǰ����·��������    
    /// </summary>
    public int AStartCurrWayPointIndex = 1;
    /// <summary>
    /// ��ɫ������������е��ƶ��ٶ�
    /// </summary>
    [HideInInspector]
    public float ModifySpeed = 0f;
    /// <summary>
    /// ���ֱ仯ί��ԭ��
    /// </summary>
    /// <param name="type"></param>
    public delegate void OnValueChangeHandler(ValueChangeType type);
    /// <summary>
    /// HP�仯ί��
    /// </summary>
    public OnValueChangeHandler OnHPChange;
    /// <summary>
    /// MP�仯ί��
    /// </summary>
    public OnValueChangeHandler OnMPChange;
    /// <summary>
    /// ��ɫ����ί��
    /// </summary>
    [HideInInspector]
    public Action OnRoleHurt;
    ///��ɫ����ί��:<>:��������(������)
    [HideInInspector]
    public Action<RoleCtrl> OnRoleDie;
    /// <summary>
    /// ��ɫѪ����  
    /// </summary>
    [HideInInspector]
    public RoleHeadBarView roleHeadBarView;
    #endregion

    #region ����������
    /// <summary>
    /// ����XYλ��
    /// </summary>
    private float mouseX;
    private float mouseY;
    #endregion

    #endregion   

    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();

        //��ȡѰ·���������·����
        m_Seeker = GetComponent<Seeker>();

        roleHeadBarView = GetComponentInChildren<RoleHeadBarView>();
        screenX = Screen.width / 2;
        screenY = Screen.height / 2;
        limitScreenX = screenX / 4;
        limitScreenY = screenY / 4;
        //�������ʼ�����������
        if (CurrRoleType == RoleType.MainPlayer)
        {
            if (CameraManager.Instance != null)
            {
                CameraManager.Instance.CameraInit();
            }
        }

        //��ɫ״̬��ʵ����
        CurrentRoleFSMMgr = new RoleFSMMgr(this, OnDieCallBack, OnDestroyCallBack);
        m_Hurt = new RoleHurt(CurrentRoleFSMMgr);
        m_Hurt.OnRoleHurt = OnRoleHurtCallBack;
        Attack.SetFSM(CurrentRoleFSMMgr);

        InitHeadBar();
    }

    /// <summary>
    /// ���ٻص�
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
            Debug.Log("��������ɫ��");
            InitHeadBar();
        }

        //��ɫAI:�ж���ɫ��AI�����û��AI�Ľ�ɫû��Ҫ�˷���˼
        if (CurrRoleAI == null)
        { return; }
        CurrRoleAI.DoAI();

        //�����ǰ��ɫ��ĳһ״̬������ÿ֡���½�ɫ��״̬
        if (CurrentRoleFSMMgr != null)
        {
            CurrentRoleFSMMgr.OnUpdate();
        }

        if (m_IsInit)
        {
            m_IsInit = false;
            //������ʱ�����ɫ�������ˣ��ǾͲ���Ҫ���ŵ��¶���
            if (CurrentRoleInfo.CurrHP <= 0)
            {
                ToDie(isDied: true);
            }
            else
            {

                //Ϊ�����ṩ��Ϣ����(ÿ�γ���ʱ����һ��״̬��ʼ��)
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
            //��ɫ�ƶ�
            characterMoveWithMouseOrTouch(Speed);
            //����ˢ�����÷���
            //characterTest();
            //С��ͼ���������
            CameraAutoFollow();
        }
    }

    private void OnDestroy()
    {
        OnDestroyCallBack();
    }

    #region ��ͼ����-С��ͼ�����
    /// <summary>
    /// С��ͼ����
    /// </summary>
    private void AutoSmallMap()
    {
        if (SmallMapHelper.Instance == null || UIMainCitySmallMapView.Instance == null)
        {
            return;
        }
        //�òο���λ����Զ�������λ��
        SmallMapHelper.Instance.transform.position = transform.position;
        //����С��ͼλ�ã�*512����Ϊ��ͼ��С����512��*-����Ϊ�������Ǹ���
        UIMainCitySmallMapView.Instance.SmallMap.transform.localPosition = new Vector3(SmallMapHelper.Instance.transform.position.x * GlobalInit.Instance.smallMapFollowRate, SmallMapHelper.Instance.transform.position.z * GlobalInit.Instance.smallMapFollowRate, 0);
        //��С��ͷ�ܹ�ʵʱ��Ӧ��ɫ����
        UIMainCitySmallMapView.Instance.SmallMapPointer.transform.localEulerAngles = new Vector3(0, 0, 360 - transform.eulerAngles.y);
    }

    /// <summary>
    /// ������Զ�����
    /// </summary>
    private void CameraAutoFollow()
    {

        //���򿪵�UI���ڴ���1ʱ��������
        if (UIViewUtil.Instance.OpenWindowCount > 0)
        {
            return;
        }

        if (CameraManager.Instance == null)
        { return; }
        //ʵʱͬ��������������λ��
        CameraManager.Instance.transform.position = gameObject.transform.position;
        CameraManager.Instance.AutoLookAt(gameObject.transform.position);


        ///ͨ����������12���м����ƶ���Ļ
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

        ///ͨ�������ƶ���Ļ
        ///��F1��ʹ�ÿ���ͨ����������Ļ�ƶ�
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
            //ʹ�������Ƶ���Ļ�ƶ�
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

    #region ��ɫ��ʼ������
    /// <summary>
    /// ��ɫ��ʼ��--��Ϣ��
    /// </summary>
    /// <param name="roleType">��ʼ�����󣺽�ɫ����</param>
    /// <param name="roleInfo">��ʼ�����󣺽�ɫ��Ϣ</param>
    /// <param name="ai">��ʼ�����ͣ�AI</param>
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
    /// ��ɫ�����趨
    /// </summary>
    /// <param name="born_Pos"></param>
    public void Born(Vector3 born_Pos)
    {
        BornPoint = born_Pos;
        transform.position = born_Pos;
        //InitHeadBar();
    }

    /// <summary>
    /// ��ʼ��ͷ����ǩ
    /// </summary>
    private void InitHeadBar()
    {
        if (RoleHeadBarRoot.Instance == null) return;
        if (CurrentRoleInfo == null) return;
        if (m_HeadBarPos == null) return;

        //m_HeadBar = ResourceMgr.Instance.Load(ResourcesType.Other, "RoleHeadBar");
        //���ؽ�ɫͷ��UI��
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
    /// ��ɫ��ʼ��--չʾ��
    /// </summary>
    public void InitShow(string nickName)
    {
        //�ܳ�ʼ����ǰ�������ǵ�ר�û���������
        if (!m_RoleCanvas.isActiveAndEnabled)
            return;
        //��ɫ��
        m_NickName.text = nickName;
    }
    #endregion

    #region ��ɫ�ƶ�
    /// <summary>
    /// ��ɫ����/����ƶ�����
    /// </summary>
    /// <param name="m_speed">�ƶ��ٶ�</param>
    private void characterMoveWithMouseOrTouch(float m_speed)
    {
        //���ǹһ������ƶ�
        if (UILoadingCtrl.Instance.CurrentSceneType == SceneType.ShanGu)
        {
            if (GlobalInit.Instance.currentPlayer.Attack.IsAutoFight)
            {
                return;
            }
        }
        //��ɫ��������----����OVER
        if (m_CharacterController == null)
        {
            Debug.Log("��ɫ�޿�����");
            return;
        }
        //�����Ļ
        //Input.GetMouseButtonDown(0)���������������ܶ����
        //Input.touchCount����ָ������Ļʱ������ָ����1��˫����2
        if ((Input.GetMouseButtonDown(0) || Input.touchCount == 1) && EventSystem.current != null)
        {
            //����UGUI���󴥼��

            if (EventSystem.current.IsPointerOverGameObject() == true)
            {
                Debug.Log("����UI��");
                return;
            }


            //��ҵ������ʱ��ȡ���Զ��ƶ�
            WorldMapCtrl.Instance.IsAutoMove = false;

            //������Ļ���
            //���߼��
            //ScreenPointToRay�����������һ������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //��ȡ�����Ľ�ɫ
            RaycastHit[] hitArr = Physics.RaycastAll(ray, Mathf.Infinity, 1 << LayerMask.NameToLayer("Player"));

            if (hitArr.Length > 0)
            {
                RoleCtrl hitRole = hitArr[0].collider.gameObject.GetComponent<RoleCtrl>();
                //����������ǹ֣������ǵĵ����ǹ�
                if (hitRole.CurrRoleType == RoleType.Monster)
                {
                    GlobalInit.Instance.currentPlayer.LockEnemy = hitRole;
                    return;
                }
                else if (hitRole.CurrRoleType == RoleType.OtherPlayer)
                {
                    //�ж���ҵ�ǰ���������Ƿ�������һ�Ź
                    GlobalInit.Instance.currentPlayer.LockEnemy = hitRole;
                    return;
                    
                }
            }

            RaycastHit hitInfo;
            //���������������巵�ظ�hitInfo
            if (Physics.Raycast(ray, out hitInfo, 1000f, 1 << LayerMask.NameToLayer("Ground")))
            {

                //���õ�������Ƿ��ǵ��棬���ƶ�
                if (GlobalInit.Instance.currentPlayer != null)
                {
                    GlobalInit.Instance.currentPlayer.LockEnemy = null;

                    GlobalInit.Instance.currentPlayer.MoveTo(hitInfo.point);
                }

            }


        }
        //��ɫ����������ɫ�Ƿ��ŵأ��ܺ��ã�
        if (!m_CharacterController.isGrounded)
        {
            m_CharacterController.Move((new Vector3(0, -0.001f, 0)));
        }
        //С��ͼ���������
        if (CurrRoleType == RoleType.MainPlayer)
        {
            CameraAutoFollow();
            AutoSmallMap();
        }
    }

    /// <summary>
    /// ��ɫ�ƶ�����---������---ToRun
    /// </summary>
    /// <param name="targetPos"></param>
    public void MoveTo(Vector3 targetPos)
    {
        //��ֱ������״̬�½�ɫ�ǲ����ƶ���
        if (IsRigidity || CurrentRoleFSMMgr.currRoleStateEnum == RoleState.Die)
        {
            return;
        }
        //���Ŀ�ĵؾ���ԭ�㣬����
        if (targetPos == Vector3.zero)
        {
            return;
        }

        m_TargetPos = targetPos;

        //1.ʹ��AStar����·������
        m_Seeker.StartPath(transform.position, targetPos, (Path p) =>
        {
            if (!p.error)
            {
                //ABPath:Path������֮һ
                AStartPath = (ABPath)p;
                //����Ѱ·����:��Ŀ��㲻�ɵ������
                if (Vector3.Distance(AStartPath.endPoint, new Vector3(AStartPath.originalEndPoint.x, AStartPath.endPoint.y, AStartPath.originalStartPoint.z)) < 0.5f)
                {
                    Debug.Log("Ŀ����޷�����");
                    AStartPath = null;
                }
                else
                {
                    //������Ϣ�����������÷�����ͬ�����λ��
                    if (CurrRoleType == RoleType.MainPlayer)
                    {
                        //PVP������Ϣ��������
                        SendPVPMove(targetPos, AStartPath.vectorPath);
                    }

                    AStartCurrWayPointIndex = 1;
                    CurrentRoleFSMMgr.ChangeState(RoleState.Run);
                }
            }
            else
            {
                Debug.Log("Ѱ·����");
                AStartPath = null;
            }
        });

    }
    #endregion

    #region PVP�ƶ�ͬ����
    /// <summary>
    /// �ͷ���˽��н�ɫ�ƶ�ͬ��
    /// </summary>
    /// <param name="targetPos">Ŀ���</param>
    /// <param name="path">A*�㷨���������·����</param>
    private void SendPVPMove(Vector3 targetPos, List<Vector3> path)
    {
        //��PVP�����£�����Ϣ�����������÷�����ͬ����ɫ�ƶ�
        if (UILoadingCtrl.Instance.CurrentPlayerType == PlayerType.PVP)
        {
            //����·�����ܳ���
            float pathLen = 0f;
            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                { continue; }
                float distance = Vector3.Distance(path[i], path[i + 1]);
                pathLen += distance;
            }
            //���������Ͻ�ɫ����Ŀ�������Ҫ��ʱ��
            float needTime = pathLen / Speed;

            WorldMap_CurrRoleMoveProto proto = new WorldMap_CurrRoleMoveProto();
            proto.TargetPosX = targetPos.x;
            proto.TargetPosY = targetPos.y;
            proto.TargetPosZ = targetPos.z;
            //��ȡ��ǰ�ķ�����ʱ��
            proto.ServerTime = GlobalInit.Instance.GetCurrentServerTime();
            proto.NeedTime = (int)(needTime * 1000);

            Debug.Log("��ɫ��PVP������Ҫ�ƶ����������ǣ�" + targetPos);
            Debug.Log("Ҫ�ƶ��ľ���Ϊ��" + pathLen);
            Debug.Log("Ҫ�ƶ�������ʱ���ǣ�" + needTime);
            NetWorkSocket.Instance.SendMessage(proto.ToArray());
        }
    }
    #endregion

    #region ��ɫ��������
    /// <summary>
    /// ����ģʽ
    /// </summary>
    /// <param name="state"></param>
    public void ToIdle(RoleIdleState state = RoleIdleState.IdleNormal)
    {
        CurrentRoleFSMMgr.ToIdleState = state;
        CurrentRoleFSMMgr.ChangeState(RoleState.Idle);
    }

    /// <summary>
    /// ���ݼ����������𹥻�
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void ToAttackByIndex(RoleAttackType type, int index)
    {
        Attack.ToAttackByIndex(type, index);
    }

    /// <summary>
    /// ���ݼ��ܱ�ŷ��𹥻�
    /// </summary>
    /// <param name="type"></param>
    /// <param name="skillId"></param>
    public bool ToAttack(RoleAttackType type, int skillId)
    {
        return Attack.ToAttack(type, skillId);
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="roleTransferAttackInfo"></param>
    public void ToHurt(RoleTransferAttackInfo roleTransferAttackInfo)
    {
        StartCoroutine(m_Hurt.ToHurt(roleTransferAttackInfo));
    }

    /// <summary>
    /// ��ɫ����
    /// </summary>
    /// <param name="isDied"></param>
    public void ToDie(bool isDied = false)
    {
#if !DEBUG_ROLESTATE
        IsDied = isDied;
        CurrentRoleInfo.CurrHP = 0;
#endif
        //���Ž�ɫ������ЧTODO
        //PlayAudio(DieAudioName, 0);
        CurrentRoleFSMMgr.ChangeState(RoleState.Die);
    }

    public void ToSelect()
    {
        CurrentRoleFSMMgr.ChangeState(RoleState.Select);
    }
    #endregion

    #region ��ɫ����-����-�������
    /// <summary>
    /// ��ɫ���˻ص�
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
    /// ��ɫ�����ص�
    /// </summary>
    private void OnDieCallBack()
    {
        //����ɫ������ʧ������ʱ��ǵü��
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
    /// ��ɫ����״̬�ָ�����
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

    #region ��ɫ��Ч
    /// <summary>
    /// ���Ž�ɫ��Ч
    /// </summary>
    /// <param name="audioName">��Ч��</param>
    /// <param name="delayTime">�ӳ�ʱ��</param>
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

