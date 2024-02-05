using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectRoleRoleItemView : MonoBehaviour
{
    /// <summary>
    /// ��ɫ���
    /// </summary>
    private int m_RoleId;

    /// <summary>
    /// �ǳ�
    /// </summary>
    [SerializeField]
    private Text m_lblNickName;

    /// <summary>
    /// �ȼ�
    /// </summary>
    [SerializeField]
    private Text m_lblLevel;

    /// <summary>
    /// ְҵ����
    /// </summary>
    [SerializeField]
    private Text m_lblJobName;

    /// <summary>
    /// ְҵͷ��
    /// </summary>
    [SerializeField]
    private Image m_ImgRoleHeadPic;

    /// <summary>
    /// ѡ�����н�ɫί��
    /// </summary>
    private Action<int> OnSelectRole;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(RoleItemClick);
    }

    /// <summary>
    /// ��ɫ��ť���
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
