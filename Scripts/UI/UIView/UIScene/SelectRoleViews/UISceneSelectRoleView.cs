using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ѡ���ɫ��ͼ
/// </summary>
public class UISceneSelectRoleView : UISceneViewBase
{
    #region ����
    /// <summary>
    /// ��ק��ͼ
    /// </summary>
    public UISelectRoleDragView UISelectRoleDragView;

    /// <summary>
    /// ְҵ�����ɫ���ʱת��Ŀ��㣩
    /// </summary>
    public UISelectRoleJobItemView[] jobItems;

    /// <summary>
    /// ��ǰְҵ����
    /// </summary>
    public UISelectRoleJobDescView selectRoleJobDescView;

    /// <summary>
    /// �ǳ�
    /// </summary>
    public InputField txtNickName;

    /// <summary>
    /// ��ɫ������UI
    /// </summary>
    [SerializeField]
    private Transform[] UICreateRole;

    /// <summary>
    /// ���н�ɫԤ��
    /// </summary>
    [SerializeField]
    private GameObject m_RoleItemPrefab;

    /// <summary>
    /// ���н�ɫ�б������
    /// </summary>
    [SerializeField]
    private Transform m_RoleListContainer;

    /// <summary>
    /// ��ɫͷ��
    /// </summary>
    [SerializeField]
    private Sprite[] m_RoleHeadPic;

    /// <summary>
    /// ��ɫɾ��
    /// </summary>
    [SerializeField]
    private UISelectRoleDeleteRoleView m_DeleteRoleView;

    /// <summary>
    /// ��ɫ���UI����
    /// </summary>
    private List<UISelectRoleRoleItemView> m_RoleItemViewList = new List<UISelectRoleRoleItemView>();

    /// <summary>
    /// ѡ���ɫUI
    /// </summary>
    public Transform[] UISelectRole;

    #region ί��
    /// <summary>
    /// ��ʼ��Ϸ��ť���ί��
    /// </summary>
    public System.Action OnBtnBeginGameClick;

    /// <summary>
    /// ɾ����ɫ��ť���ί��
    /// </summary>
    public System.Action OnBtnDeleteRoleClick;

    /// <summary>
    /// ���ذ�ť���ί��
    /// </summary>
    public System.Action OnBtnReturnClick;

    /// <summary>
    /// �½���ɫ��ť���
    /// </summary>
    public System.Action OnBtnCreateRoleClick;

    #endregion
    #endregion


    protected override void OnStart()
    {
        base.OnStart();
        m_DeleteRoleView.gameObject.SetActive(false);
    }

    /// <summary>
    /// ������ɫ�õ���ЩUI
    /// </summary>
    /// <param name="isShow"></param>
    public void SetUICreateRoleShow(bool isShow)
    {
        if (UICreateRole != null && UICreateRole.Length > 0)
        {
            for (int i = 0; i < UICreateRole.Length; i++)
            {
                UICreateRole[i].gameObject.SetActive(isShow);
            }
        }
    }

    protected override void OnBtnClick(GameObject go)
    {
        switch (go.name)
        {
            case "Btn_RandomName":
                RandomName();
                break;
            case "Btn_BeginGame":
                if (OnBtnBeginGameClick != null)
                {
                    OnBtnBeginGameClick();
                }
                break;
            case "Btn_DeleteRole":
                if (OnBtnDeleteRoleClick != null)
                {
                    OnBtnDeleteRoleClick();
                }
                break;
            case "Btn_Return":
                if (OnBtnReturnClick != null)
                {
                    OnBtnReturnClick();
                }
                break;
            case "Btn_CreateRole":
                if (OnBtnCreateRoleClick != null)
                {
                    OnBtnCreateRoleClick();
                }
                break;

        }
    }

    /// <summary>
    /// �������
    /// </summary>
    public void RandomName()
    {
        txtNickName.text = NameRandomFactor(UnityEngine.Random.Range(1, 10));
    }

    /// <summary>
    /// ��ɫ�������
    /// </summary>
    /// <param name="factor"></param>
    /// <returns></returns>
    public string NameRandomFactor(int factor)
    {
        string name = "���1��";
        switch (factor)
        {
            case 1:
                name = "����**��BUG";
                break;
            case 2:
                name = "ȫ��BUG�Ľ̳�";
                break;
            case 3:
                name = "��˵�ҳ���";
                break;
            case 4:
                name = "ʲô�̳�";
                break;
            case 5:
                name = "�ڲ�����";
                break;
            case 6:
                name = "�Գ�����";
                break;
            case 7:
                name = "��̳��������";
                break;
            case 8:
                name = "����㲻��";
                break;
            case 9:
                name = "˵�ľ�����";
                break;
            case 10:
                name = "���뿴�̳�";
                break;
            default:
                name = "����";
                break;
        }
        return name;
    }

    /// <summary>
    /// �������н�ɫ
    /// </summary>
    /// <param name="list"></param>
    public void SetRoleList(List<RoleOperation_LogOnGameServerReturnProto.RoleItem> list, Action<int> OnSelectRole)
    {
        //������н�ɫUI
        ClearRoleListUI();

        for (int i = 0; i < list.Count; i++)
        {
            //��¡UI
            GameObject obj = Instantiate(m_RoleItemPrefab);
            UISelectRoleRoleItemView view = obj.GetComponent<UISelectRoleRoleItemView>();
            m_RoleItemViewList.Add(view);
            if (view != null)
            {
                view.SetUI(list[i].RoleId, list[i].RoleNickName, list[i].RoleLevel, list[i].RoleJob, m_RoleHeadPic[list[i].RoleJob - 1], OnSelectRole);
            }
            obj.transform.SetParent(m_RoleListContainer);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = new Vector3(0, -100 * i, 0);
        }
    }

    /// <summary>
    /// ������н�ɫUI
    /// </summary>
    public void ClearRoleListUI()
    {
        if (m_RoleItemViewList.Count > 0)
        {
            for (int i = 0; i < m_RoleItemViewList.Count; i++)
            {
                Destroy(m_RoleItemViewList[i].gameObject);
            }
            m_RoleItemViewList.Clear();
        }
    }

    /// <summary>
    /// ѡ���ɫ�õ���ЩUI
    /// </summary>
    /// <param name="v"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void SetUISelectRoleActive(bool isActive)
    {
        if (UISelectRole != null && UISelectRole.Length > 0)
        {
            for (int i = 0; i < UISelectRole.Length; i++)
            {
                UISelectRole[i].gameObject.SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// ɾ����ǰѡ�еĽ�ɫ
    /// </summary>
    /// <param name="nickName"></param>
    /// <param name="onBtnOkClick"></param>
    public void DeleteSelectRole(string nickName, Action onBtnOkClick)
    {
        m_DeleteRoleView.gameObject.SetActive(true);
        m_DeleteRoleView.Show(nickName, onBtnOkClick);
    }

    /// <summary>
    /// �ر�ɾ����ɫ����
    /// </summary>
    public void CloseDeleteRoleView()
    {
        m_DeleteRoleView.Close();
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        selectRoleJobDescView = null;
        jobItems.SetNull();
        selectRoleJobDescView = null;
        txtNickName = null;
        OnBtnBeginGameClick = null;
        OnBtnDeleteRoleClick = null;
        OnBtnReturnClick = null;
        OnBtnCreateRoleClick = null;
        UICreateRole.SetNull();
        UISelectRole.SetNull();
        m_RoleHeadPic.SetNull();
        m_RoleItemPrefab = null;
        m_RoleListContainer = null;
        m_DeleteRoleView = null;
        m_RoleItemViewList.ToArray().SetNull();


    }
}
