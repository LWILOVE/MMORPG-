using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

[Hotfix]
public class UIRoleInfoEquipView : UISubViewBase
{
    /// <summary>
    /// 角色模型容器
    /// </summary>
    [SerializeField]
    public Transform RoleModelContainer;

    /// <summary>
    /// 职业编号
    /// </summary>
    public int m_JobId;

    /// <summary>
    /// 昵称
    /// </summary>
    [SerializeField]
    public Text lblNickName;

    /// <summary>
    /// 角色等级
    /// </summary>
    [SerializeField]
    public Text lblLevel;

    /// <summary>
    /// 综合战力
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
    /// 设置UI
    /// </summary>
    /// <param name="data"></param>
    public void SetUI(TransferData data)
    {
        m_JobId = data.GetValue<byte>(ConstDefine.JobId);
        lblNickName.text = data.GetValue<string>(ConstDefine.NickName); ;
        lblLevel.text = string.Format("Lv:{0}", data.GetValue<int>(ConstDefine.Level));
        lblFighting.text = string.Format("综合战斗力：{0}", data.GetValue<int>(ConstDefine.Fighting));
    }

    /// <summary>
    /// 克隆角色模型
    /// </summary>
    public void CloneRoleModel()
    {
        GameObject obj = RoleMgr.Instance.LoadPlayerModel(m_JobId);

        obj.SetParent(RoleModelContainer);

        obj.SetLayer("UI");
    }
    
}
