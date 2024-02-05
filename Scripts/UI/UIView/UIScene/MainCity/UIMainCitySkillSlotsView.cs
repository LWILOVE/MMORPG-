using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能槽视图设计类
/// </summary>
public class UIMainCitySkillSlotsView : UISubViewBase
{
    [Header("可修改内容")]
    /// <summary>
    /// 技能槽编号
    /// </summary>
    public int SlotsNo;
    /// <summary>
    /// 冷却遮罩
    /// </summary>
    [SerializeField]
    private Image CDImage;
    /// <summary>
    /// 技能图标
    /// </summary>
    [SerializeField]
    private Image SkillImg;

    private Action<int> OnSkillClick;

    [Header("数据观察")]
    /// <summary>
    /// 技能ID
    /// </summary>
    [SerializeField]
    public int SkillId;
    [SerializeField]
    /// <summary>
    /// 冷却时间
    /// </summary>
    private float m_CDTime = 0f;

    [SerializeField]
    /// <summary>
    /// 技能是否处于冷却时间
    /// </summary>
    private bool m_IsCD = false;
    /// <summary>
    /// 技能开始冷却的时间
    /// </summary>
    private float m_BeginCDTime = 0f;
    /// <summary>
    /// 旋转的百分比
    /// </summary>
    private float m_CurrFillAmount = 0f;
    protected override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("技能：启动");
        SkillImg.gameObject.SetActive(false);
        CDImage.transform.parent.gameObject.SetActive(false);
        m_IsCD = false;
    }

    protected override void OnStart()
    {
        base.OnStart();
        //外类
        EventTriggerListener.Get(gameObject).onClick += OnClick;
    }
    /// <summary>
    /// 技能槽按钮点击
    /// </summary>
    /// <param name="obj"></param>
    private void OnClick(GameObject obj)
    {
        Debug.Log("技能被点击了");
        //若技能槽上未装配技能，则返回
        if (SkillId < 1)
        { return; }
        if (m_IsCD)
        { return; }
        if (OnSkillClick != null)
        {
            OnSkillClick(SkillId);
        }
    }

    /// <summary>
    /// 设置技能UI
    /// </summary>
    /// <param name="skillId">技能ID</param>
    /// <param name="skillLevel">技能等级</param>
    /// <param name="skillPic">技能图片</param>
    /// <param name="cdTime">技能CD</param>
    /// <param name="onSkillClick">技能点击回调</param>
    public void SetUI(int skillId, int skillLevel, string skillPic, float cdTime, Action<int> onSkillClick)
    {
        if (skillId == 0)
        { return; }
        SkillId = skillId;
        m_CDTime = cdTime;//冷却时间
        SkillImg.gameObject.SetActive(true);
        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("Download/Source/UISource/Skill/{0}.assetbundle", skillPic), skillPic,
    (Texture2D obj) =>
    {
        var iconRect = new Rect(0, 0, obj.width, obj.height);
        var iconSprite = Sprite.Create(obj, iconRect, new Vector2(0.5f, 0.5f));
        SkillImg.overrideSprite = iconSprite;
    }, type: 1);
        OnSkillClick = onSkillClick;
    }



    /// <summary>
    /// 开始冷却
    /// </summary>
    public void BeginCD()
    {
        Debug.Log("技能开始冷却");
        CDImage.transform.parent.gameObject.SetActive(true);
        m_IsCD = true;
        m_BeginCDTime = Time.time;
        m_CurrFillAmount = 1f;

    }

    private void Update()
    {
        if (m_IsCD)
        {
            //获取百分比
            m_CurrFillAmount = Mathf.Lerp(1, 0, (Time.time - m_BeginCDTime) / m_CDTime);
            CDImage.fillAmount = m_CurrFillAmount;
            if (Time.time > m_BeginCDTime + m_CDTime)
            {
                m_IsCD = false;
            }
        }
    }
}
