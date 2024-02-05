using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class UIRoleInfoEquipView : UISubViewBase
{
    /// <summary>
    /// ��ɫģ������
    /// </summary>
    [SerializeField]
    public Transform RoleModelContainer;

    /// <summary>
    /// ְҵ���
    /// </summary>
    public int m_JobId;

    /// <summary>
    /// �ǳ�
    /// </summary>
    [SerializeField]
    public Text lblNickName;

    /// <summary>
    /// ��ɫ�ȼ�
    /// </summary>
    [SerializeField]
    public Text lblLevel;

    /// <summary>
    /// �ۺ�ս��
    /// </summary>
    public Text lblFighting;

    // Start is called before the first frame update


    protected override void OnStart()
    {
        base.OnStart();
        CloneRoleModel();
    }
    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
        lblNickName = null;
        lblLevel = null;
        lblFighting = null;
    }


    
    /// <summary>
    /// ����UI
    /// </summary>
    /// <param name="data"></param>
    public void SetUI(TransferData data)
    {
        m_JobId = data.GetValue<byte>(ConstDefine.JobId);
        lblNickName.text = data.GetValue<string>(ConstDefine.NickName); ;
        lblLevel.text = string.Format("Lv:{0}", data.GetValue<int>(ConstDefine.Level));
        lblFighting.text = string.Format("�ۺ�ս������{0}", data.GetValue<int>(ConstDefine.Fighting));
    }

    /// <summary>
    /// ��¡��ɫģ��
    /// </summary>
    public void CloneRoleModel()
    {
        GameObject obj = RoleMgr.Instance.LoadPlayerModel(m_JobId);

        obj.SetParent(RoleModelContainer);

        obj.SetLayer("UI");
    }
    
}
