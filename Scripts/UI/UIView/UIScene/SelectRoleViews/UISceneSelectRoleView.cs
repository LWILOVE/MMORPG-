using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择角色视图
/// </summary>
public class UISceneSelectRoleView : UISceneViewBase
{
    #region 变量
    /// <summary>
    /// 拖拽视图
    /// </summary>
    public UISelectRoleDragView UISelectRoleDragView;

    /// <summary>
    /// 职业项（当角色点击时转向到目标点）
    /// </summary>
    public UISelectRoleJobItemView[] jobItems;

    /// <summary>
    /// 当前职业描述
    /// </summary>
    public UISelectRoleJobDescView selectRoleJobDescView;

    /// <summary>
    /// 昵称
    /// </summary>
    public InputField txtNickName;

    /// <summary>
    /// 角色创建用UI
    /// </summary>
    [SerializeField]
    private Transform[] UICreateRole;

    /// <summary>
    /// 已有角色预设
    /// </summary>
    [SerializeField]
    private GameObject m_RoleItemPrefab;

    /// <summary>
    /// 已有角色列表的容器
    /// </summary>
    [SerializeField]
    private Transform m_RoleListContainer;

    /// <summary>
    /// 角色头像
    /// </summary>
    [SerializeField]
    private Sprite[] m_RoleHeadPic;

    /// <summary>
    /// 角色删除
    /// </summary>
    [SerializeField]
    private UISelectRoleDeleteRoleView m_DeleteRoleView;

    /// <summary>
    /// 角色相关UI链表
    /// </summary>
    private List<UISelectRoleRoleItemView> m_RoleItemViewList = new List<UISelectRoleRoleItemView>();

    /// <summary>
    /// 选择角色UI
    /// </summary>
    public Transform[] UISelectRole;

    #region 委托
    /// <summary>
    /// 开始游戏按钮点击委托
    /// </summary>
    public System.Action OnBtnBeginGameClick;

    /// <summary>
    /// 删除角色按钮点击委托
    /// </summary>
    public System.Action OnBtnDeleteRoleClick;

    /// <summary>
    /// 返回按钮点击委托
    /// </summary>
    public System.Action OnBtnReturnClick;

    /// <summary>
    /// 新建角色按钮点击
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
    /// 创建角色用的那些UI
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
    /// 随机名字
    /// </summary>
    public void RandomName()
    {
        txtNickName.text = NameRandomFactor(UnityEngine.Random.Range(1, 10));
    }

    /// <summary>
    /// 角色名随机器
    /// </summary>
    /// <param name="factor"></param>
    /// <returns></returns>
    public string NameRandomFactor(int factor)
    {
        string name = "随机1号";
        switch (factor)
        {
            case 1:
                name = "修你**的BUG";
                break;
            case 2:
                name = "全是BUG的教程";
                break;
            case 3:
                name = "别说我抽象";
                break;
            case 4:
                name = "什么教程";
                break;
            case 5:
                name = "悔不当初";
                break;
            case 6:
                name = "言出必行";
                break;
            case 7:
                name = "这教程是真的拉";
                break;
            case 8:
                name = "活该你不火";
                break;
            case 9:
                name = "说的就是你";
                break;
            case 10:
                name = "不想看教程";
                break;
            default:
                name = "无了";
                break;
        }
        return name;
    }

    /// <summary>
    /// 设置已有角色
    /// </summary>
    /// <param name="list"></param>
    public void SetRoleList(List<RoleOperation_LogOnGameServerReturnProto.RoleItem> list, Action<int> OnSelectRole)
    {
        //清空已有角色UI
        ClearRoleListUI();

        for (int i = 0; i < list.Count; i++)
        {
            //克隆UI
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
    /// 清空已有角色UI
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
    /// 选择角色用的那些UI
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
    /// 删除当前选中的角色
    /// </summary>
    /// <param name="nickName"></param>
    /// <param name="onBtnOkClick"></param>
    public void DeleteSelectRole(string nickName, Action onBtnOkClick)
    {
        m_DeleteRoleView.gameObject.SetActive(true);
        m_DeleteRoleView.Show(nickName, onBtnOkClick);
    }

    /// <summary>
    /// 关闭删除角色界面
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
