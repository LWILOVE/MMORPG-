using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectRoleRoleItemView : MonoBehaviour
{
    /// <summary>
    /// 角色编号
    /// </summary>
    private int m_RoleId;

    /// <summary>
    /// 昵称
    /// </summary>
    [SerializeField]
    private Text m_lblNickName;

    /// <summary>
    /// 等级
    /// </summary>
    [SerializeField]
    private Text m_lblLevel;

    /// <summary>
    /// 职业名称
    /// </summary>
    [SerializeField]
    private Text m_lblJobName;

    /// <summary>
    /// 职业头像
    /// </summary>
    [SerializeField]
    private Image m_ImgRoleHeadPic;

    /// <summary>
    /// 选择已有角色委托
    /// </summary>
    private Action<int> OnSelectRole;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(RoleItemClick);
    }

    /// <summary>
    /// 角色按钮点击
    /// </summary>
    private void RoleItemClick()
    { 
        if(OnSelectRole != null)
        {
            OnSelectRole(m_RoleId);
        }
    }

    public void SetUI(int roleId,string nickName,int level,int jobId,Sprite headPic,Action<int> onSelectRole)
    {
        m_RoleId = roleId;
        m_lblNickName.text = nickName;
        m_lblLevel.text = string.Format("Lv{0}",level);
        m_lblJobName.text = JobDBModel.Instance.Get(jobId).Name;
        m_ImgRoleHeadPic.overrideSprite = headPic;
        OnSelectRole = onSelectRole;
    }

    private void OnDestroy()
    {
        m_lblJobName = null;
        m_lblLevel = null;
        m_lblNickName = null;
        m_ImgRoleHeadPic = null;
        OnSelectRole = null;
    }
}
