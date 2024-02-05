using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ѡ�ǳ���������
/// </summary>
public class SelectRoleSceneCtrl : MonoBehaviour
{
    #region ����
    /// <summary>
    /// ְҵ�б�
    /// </summary>
    private List<JobEntity> m_JobList;

    /// <summary>
    /// ��ɫ����    
    /// </summary>
    public Transform[] CreateRoleContainers;

    /// <summary>
    /// �����ת�Ƕ�
    /// </summary>
    public float[] CameraRotate;
    
    /// <summary>
    /// ְҵ������
    /// </summary>
    private Dictionary<int, RoleCtrl> m_JobRoleCtrl = new Dictionary<int, RoleCtrl>();

    /// <summary>
    /// ������ͼ
    /// </summary>
    private UISceneSelectRoleView m_UISceneSelectRoleView;

    /// <summary>
    /// ��קĿ��
    /// </summary>
    public Transform DragTarget;

    /// <summary>
    /// ÿ����ק����ת�Ƕ�
    /// </summary>
    private float m_RotateAngle = 90;

    /// <summary>
    /// ��תĿ��Ƕ�
    /// </summary>
    private float m_TargetAngle = 0;

    /// <summary>
    /// �Ƿ���������ת״̬
    /// </summary>
    private bool m_IsRotating;

    /// <summary>
    /// ��ת�ٶ�
    /// </summary>
    private float m_RotateSpeed = 100f;

    /// <summary>
    /// ��ǰѡ���ְҵID
    /// </summary>
    private int m_CurrSelectJobId;

    /// <summary>
    /// �½���ɫ����ĳ���ģ�ͣ�ʵ������
    /// </summary>
    public Transform[] CreateRoleSceneModel;

    /// <summary>
    /// �Ƿ����½���ɫ�׶�
    /// </summary>
    private bool m_IsCreateRole;
    
    /// <summary>
    /// ��¡��ɫ����
    /// </summary>
    private List<GameObject> m_CloneCreateRoleList = new List<GameObject>();

    #region ���н�ɫ���ñ���
    /// <summary>
    /// ���н�ɫ�б�
    /// </summary>
    private List<RoleOperation_LogOnGameServerReturnProto.RoleItem> m_RoleList;

    /// <summary>
    /// ��ǰѡ��Ľ�ɫģ��
    /// </summary>
    private GameObject m_CurrSelectRoleModel;

    /// <summary>
    /// ��ǰѡ��Ľ�ɫ���
    /// </summary>
    private int m_CurrSelectRoleId;
    #endregion

    /// <summary>
    /// ������������ͼID
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
        //����ѡ���ɫ����health
        UILoadingCtrl.Instance.LoadSceneUI(SceneUIType.SelectRole,
    (GameObject obj) =>
    {
        m_UISceneSelectRoleView = obj.GetComponent<UISceneSelectRoleView>();
        if (m_UISceneSelectRoleView != null)
        {
            //��ӳ�����ͼ����
            m_UISceneSelectRoleView.UISelectRoleDragView.OnSelectRoleDrag = OnSelectRoleDrag;

            //����ÿ����ɫ�ľ�ͷ
            if (m_UISceneSelectRoleView.jobItems != null && m_UISceneSelectRoleView.jobItems.Length > 0)
            {
                for (int i = 0; i < m_UISceneSelectRoleView.jobItems.Length; i++)
                {
                    m_UISceneSelectRoleView.jobItems[i].OnSelectJob = OnSelectJobCallBack;
                }
            }
        }
        #region Э����UI�¼�
        //����Э�����
        //���������ص�¼��Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_LogOnGameServerReturn, OnLogOnGameServerReturn);
        //���������ش�����ɫ��Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_CreateRoleReturn, OnCreateRoleReturn);
        //���������ؽ�����Ϸ��Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_EnterGameReturn, OnEnterGameReturn);
        //����������ɾ����ɫ��Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_DeleteRoleReturn, OnDeleteRoleReturn);
        //���������ؽ�ɫ��Ϣ��Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfoReturn, OnSelectRoleInfoReturn);
        //���������ؽ�ɫѧ��ļ�����Ϣ
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.RoleData_SkillReturn, OnSkillReturn);

        //��ʼ��Ϸ��ť���
        m_UISceneSelectRoleView.OnBtnBeginGameClick = OnBtnBeginGameClick;
        //ɾ����ɫ��ť���
        m_UISceneSelectRoleView.OnBtnDeleteRoleClick = OnBtnDeleteRoleClick;
        //���ذ�ť���
        m_UISceneSelectRoleView.OnBtnReturnClick = OnBtnReturnClick;
        //�½���ɫ��ť���
        m_UISceneSelectRoleView.OnBtnCreateRoleClick = OnCreateRoleClick;
        #endregion
        //��ʼ��ʱ��ְҵIDΪ1
        m_CurrSelectJobId = 1;
        //���õ�ǰѡ��Ľ�ɫ
        SetSelectJob();
        //���ؽ�ɫģ��
        LoadJobObject(OnLoadJobObjectComplete);
        Debug.Log("���ѡ�ǳ�������");
    });
    }

    private void OnDestroy()
    {
        //���������ص�¼��Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_LogOnGameServerReturn, OnLogOnGameServerReturn);
        //���������ش�����ɫ��Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_CreateRoleReturn, OnCreateRoleReturn);
        //���������ؽ�����Ϸ��Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_EnterGameReturn, OnEnterGameReturn);
        //���������ؽ�ɫ��Ϣ��Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_SelectRoleInfoReturn, OnSelectRoleInfoReturn);
        //���������ؽ�ɫѧ��ļ�����Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleData_SkillReturn, OnSkillReturn);
        //����������ɾ����ɫ��Ϣ
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.RoleOperation_DeleteRoleReturn, OnDeleteRoleReturn);
    }

    #region Э��
    /// <summary>
    /// ���������ص�¼��Ϣ
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLogOnGameServerReturn(byte[] p)
    {
        RoleOperation_LogOnGameServerReturnProto proto = RoleOperation_LogOnGameServerReturnProto.GetProto(p);
        int roleCount = proto.RoleCount;
        Debug.Log("���н�ɫ����" + roleCount);
        if (roleCount == 0)
        {
            m_IsCreateRole = true;
            SetCreateRoleSceneModelShow(true);//��ǰȱ����Դ����Ѱ
            m_UISceneSelectRoleView.SetUICreateRoleShow(true);//��ʾ������ɫ�õ���ЩUI
            m_UISceneSelectRoleView.SetUISelectRoleActive(false);//����ѡ���ɫ�õ���ЩUI
            //���޽�ɫ��������½�����
            CloneCreateRole();
            //��ʼ��ʱ��ְҵIDΪ1
            m_CurrSelectJobId = 1;
            SetSelectJob();
            m_UISceneSelectRoleView.RandomName();
        }
        else
        {
            m_IsCreateRole = false;
            //�����������ó��ֵ�UI
            SetCreateRoleSceneModelShow(false);
            m_UISceneSelectRoleView.SetUICreateRoleShow(false);
            //ѡ���ɫҳ��״̬����
            m_UISceneSelectRoleView.SetUISelectRoleActive(true);
            //�����ȡ���н�ɫ������ҽ���ѡ���ɫ
            if (proto.RoleList != null)
            {
                m_RoleList = proto.RoleList;
                //�������н�ɫUI��Ϣ
                m_UISceneSelectRoleView.SetRoleList(m_RoleList, SelectRoleCallBack);

                SetSelectRole(m_RoleList[0].RoleId);
            }
        }
        Debug.Log("��¼���������");
    }

    /// <summary>
    /// �½���ɫ�����������½���ɫ��Ϣ
    /// </summary>
    /// <param name="p"></param>
    private void OnCreateRoleReturn(byte[] p)
    {
        RoleOperation_CreateRoleReturnProto proto = RoleOperation_CreateRoleReturnProto.GetProto(p);
        if (proto.IsSuccess)
        {
            Debug.Log("��ɫ�����ɹ�,���������������");
        }
        else
        {
            MessageCtrl.Instance.Show("��ɫ����ʧ��");
        }
    }

    /// <summary>
    /// ���н�ɫ ���������ؽ�����Ϸ��Ϣ
    /// </summary>
    /// <param name="p"></param>
    private void OnEnterGameReturn(byte[] p)
    {
        RoleOperation_EnterGameReturnProto proto = RoleOperation_EnterGameReturnProto.GetProto(p);

        if (proto.IsSuccess)
        {
            Debug.Log("������Ϸ�ɹ������������������");

        }
        else
        {
            MessageCtrl.Instance.Show("������Ϸʧ��");
        }
    }

    /// <summary>
    /// ���������ؽ�ɫ��Ϣ
    /// </summary>
    /// <param name="p"></param>
    private void OnSelectRoleInfoReturn(byte[] p)
    {
        RoleOperation_SelectRoleInfoReturnProto proto = RoleOperation_SelectRoleInfoReturnProto.GetProto(p);
        //����ɫ�����ɹ����л�����������
        if (proto.IsSuccess)
        {
            //����������Ϣ
            GlobalInit.Instance.MainPlayerInfo = new RoleInfoMainPlayer(proto);
            m_LastInWorldMapId = proto.LastInWorldMapId;
            PlayerCtrl.Instance.LastInWorldMapId = m_LastInWorldMapId;
            PlayerCtrl.Instance.LastInWorldMapPos = proto.LastInWorldMapPos;
            Debug.Log("��ɫ��Ϣ��");
            Debug.Log("����������磺" + PlayerCtrl.Instance.LastInWorldMapId +" ��ɫ����"+GlobalInit.Instance.MainPlayerInfo.RoleNickName );
        }
    }
    
    /// <summary>
    /// ���������ؽ�ɫѧ��ļ�����Ϣ
    /// </summary>
    /// <param name="p"></param>
    private void OnSkillReturn(byte[] p)
    {
        RoleData_SkillReturnProto proto = RoleData_SkillReturnProto.GetProto(p);
        Debug.Log("��ɫѧ��ļ�������"+proto.CurrSkillDataList.Count);
        GlobalInit.Instance.MainPlayerInfo.LoadSkill(proto);
        //������ֻ�Ǵ�����ɫ��û��ȥ���κγ�������ôĬ������Ϊ1�������ִ�
        if (PlayerCtrl.Instance.LastInWorldMapId == 0)
        {
            m_LastInWorldMapId = 1;
        }
        PlayerCtrl.Instance.LastInWorldMapId = m_LastInWorldMapId;
        //�л�����
        Debug.Log("����ʱĬ����ת����������");
        UILoadingCtrl.Instance.LoadToWorldMap(PlayerCtrl.Instance.LastInWorldMapId);
    }

    /// <summary>
    /// ����������ɾ����ɫ��Ϣ
    /// </summary>
    /// <param name="p"></param>
    private void OnDeleteRoleReturn(byte[] p)
    {
        RoleOperation_DeleteRoleReturnProto proto = RoleOperation_DeleteRoleReturnProto.GetProto(p);

        if (proto.IsSuccess)
        {
            Debug.Log("ɾ����ɫ�ɹ�");
            m_UISceneSelectRoleView.CloseDeleteRoleView();
            DeleteRole(m_CurrSelectRoleId);

        }
        else
        {
            MessageCtrl.Instance.Show("ɾ����ɫʧ��");
        }
    }
    #endregion

    #region ��ť�ص�
    /// <summary>
    /// ��ʼ��Ϸ��ť���
    /// </summary>
    private void OnBtnBeginGameClick()
    {

        if (m_IsCreateRole)
        {
            //�½���ɫ�߼�
            RoleOperation_CreateRoleProto proto = new RoleOperation_CreateRoleProto();
            proto.JobId = (byte)m_CurrSelectJobId;
            proto.RoleNickName = m_UISceneSelectRoleView.txtNickName.text;

            if (string.IsNullOrEmpty(proto.RoleNickName))
            {
                MessageCtrl.Instance.Show("�������ǳ�");
                return;
            }
            Debug.Log("����������Ϸ����");
            NetWorkSocket.Instance.SendMessage(proto.ToArray());
        }
        else
        {
            //ѡ�����н�ɫ�߼�
            //�ͻ��˽�����Ϸ������ϢЭ��
            Debug.Log("����������Ϸ����");
            RoleOperation_EnterGameProto proto = new RoleOperation_EnterGameProto();
            proto.RoleId = m_CurrSelectRoleId;
            NetWorkSocket.Instance.SendMessage(proto.ToArray());
        }
    }

    /// <summary>
    /// ɾ����ɫ���
    /// </summary>
    private void OnBtnDeleteRoleClick()
    {
        m_UISceneSelectRoleView.DeleteSelectRole(GetRoleItem(m_CurrSelectRoleId).RoleNickName, OnDeleteRoleClickCallBack);
    }

    /// <summary>
    /// ȷ��ɾ������ص�
    /// </summary>
    private void OnDeleteRoleClickCallBack()
    {
        //����ɾ����ɫ��Ϣ
        RoleOperation_DeleteRoleProto proto = new RoleOperation_DeleteRoleProto();
        proto.RoleId = m_CurrSelectRoleId;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }

    /// <summary>
    /// �½���ɫ��ť���
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnCreateRoleClick()
    {
        ToCreateRoleUI();
    }

    /// <summary>
    /// �½���ɫ����
    /// </summary>
    private void ToCreateRoleUI()
    {
        m_UISceneSelectRoleView.ClearRoleListUI();
        //ɾ����ɫģ��
        if (m_CurrSelectRoleModel != null)
        {
            Destroy(m_CurrSelectRoleModel);
        }
        //�½���ɫ
        m_IsCreateRole = true;
        SetCreateRoleSceneModelShow(true);
        m_UISceneSelectRoleView.SetUICreateRoleShow(true);
        m_UISceneSelectRoleView.SetUISelectRoleActive(false);

        CloneCreateRole();

        //��ʼ��ʱ��ְҵĬ���ǳ�ʼְҵ
        m_CurrSelectJobId = 1;
        SetSelectJob();
        m_UISceneSelectRoleView.RandomName();
    }


    /// <summary>
    /// ���ذ�ť���
    /// </summary>
    private void OnBtnReturnClick()
    {
        //������½���ɫ���� �� ��ǰû�н�ɫ �򷵻�ѡ������,����������Ͽ�����
        //�����½���ɫ���� �򷵻����н�ɫ����
        //�������н�ɫ���棬�򷵻�ѡ������
        if (m_IsCreateRole)
        {
            if (m_RoleList == null || m_RoleList.Count == 0)
            {
                NetWorkSocket.Instance.DisConnect();
                UILoadingCtrl.Instance.LoadToLogOn();
            }
            else
            {
                //����½���ɫUI��ģ��
                ClearCloneCreateRole();
                //�̶����
                m_IsRotating = false;
                m_CurrSelectRoleId = 0;
                DragTarget.eulerAngles = Vector3.zero;

                //�������н�ɫ����
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
    /// �����¡�Ľ�ɫ
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
        ///��ѡ�˾�ͷ������ת
        if (m_IsRotating)
        {
            float toAngle = Mathf.MoveTowardsAngle(DragTarget.eulerAngles.y, CameraRotate[m_CurrSelectJobId-1], Time.deltaTime * m_RotateSpeed);
            DragTarget.eulerAngles = Vector3.up * toAngle;
            if (Mathf.RoundToInt(CameraRotate[m_CurrSelectJobId-1]) == Mathf.RoundToInt(toAngle) || Mathf.RoundToInt(CameraRotate[m_CurrSelectJobId-1] + 360) == Mathf.RoundToInt(toAngle))
            {
                m_IsRotating = false;
                DragTarget.eulerAngles = Vector3.up * CameraRotate[m_CurrSelectJobId-1];
                Debug.Log("��ת�����,���ڵ�ְҵ�ǣ�" + m_CurrSelectJobId);
            }

        }
    }

    #region ������Դ���ܳ�ʼ��
    /// <summary>
    /// UI�ϵ��ְҵ���Ļص�-�������Ҫ�ƶ��ķ���
    /// </summary>
    /// <param name="jobId">ְҵID</param>
    /// <param name="rotateAngle">��ת�Ƕ�</param>
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
    /// ��ɫѡ����קί�� 0��1��
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

        //����ǰѡ�е�ְҵID
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
    /// ���ؽ�ɫ����
    /// </summary>
    private void LoadJobObject(Action onComplete)
    {
        Debug.Log("ְҵ����" + m_JobList.Count);
        LoadJob(0, onComplete);
    }

    /// <summary>
    /// ʹ�õݹ���ֶμ���ְҵ��Ϣ
    /// </summary>
    /// <param name="index">ְҵ�±�</param>
    /// <param name="onComplete">���ί��</param>
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
                        //������ɣ�ִ�м���ί��
                        if (onComplete != null)
                        { onComplete(); }
                    }
                    else
                    { LoadJob(index, onComplete); }
                }
            });
    }

    /// <summary>
    /// ְҵ��Ʒ������ɻص�
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnLoadJobObjectComplete()
    {
        LogOnGameServer();
    }

    /// <summary>
    /// ��¼������Ϣ
    /// </summary>
    private void LogOnGameServer()
    {
        RoleOperation_LogOnGameServerProto proto = new RoleOperation_LogOnGameServerProto();
        proto.AccountId = GlobalInit.Instance.CurrAccount.Id;
        //�����������
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }
    #endregion

    /// <summary>
    /// ����ѡ���ְҵ
    /// </summary>
    private void SetSelectJob()
    {
        //��ȡ����ְҵ�б���:
        m_JobList = JobDBModel.Instance.GetList();
        //����ѡ���ְҵ��Ϣ
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
    /// ѡ�����н�ɫ�ص�
    /// </summary>
    /// <param name="roleId"></param>
    private void SelectRoleCallBack(int roleId)
    {
        SetSelectRole(roleId);
    }

    /// <summary>
    /// ����ѡ��Ľ�ɫ
    /// </summary>
    /// <param name="roleId"></param>
    private void SetSelectRole(int roleId)
    {
        //����������ͬһ��ɫ���޷�Ӧ
        if (m_CurrSelectRoleId == roleId)
        {
            return;
        }
        m_CurrSelectRoleId = roleId;

        //���ѡ��������Ѿ��н�ɫ�ˣ�����
        if (m_CurrSelectRoleModel != null)
        {
            Destroy(m_CurrSelectRoleModel);
        }

        RoleOperation_LogOnGameServerReturnProto.RoleItem item = GetRoleItem(roleId);
        //���ݽ�ɫ��ְҵID��¡��ɫ
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


    #region �½���ɫ��ش���
    /// <summary>
    /// �����½���ɫ����ģ���Ƿ���ʾ
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
    /// ��¡�½���ɫ
    /// </summary>
    private void CloneCreatRole()
    {
        for (int i = 0; i < m_JobList.Count; i++)
        {
            GameObject obj = Instantiate(GlobalInit.Instance.JobObjectDic[m_JobList[i].Id]);
        }
    }

    /// <summary>
    /// ��¡����ְҵģ��
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

    #region ���н�ɫ���
    /// <summary>
    /// ���ݽ�ɫ��Ż�ȡ���н�ɫ��
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
    /// �ӱ����б�ɾ����ɫ
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
