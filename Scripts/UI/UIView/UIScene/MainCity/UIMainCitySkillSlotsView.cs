using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���ܲ���ͼ�����
/// </summary>
public class UIMainCitySkillSlotsView : UISubViewBase
{
    [Header("���޸�����")]
    /// <summary>
    /// ���ܲ۱��
    /// </summary>
    public int SlotsNo;
    /// <summary>
    /// ��ȴ����
    /// </summary>
    [SerializeField]
    private Image CDImage;
    /// <summary>
    /// ����ͼ��
    /// </summary>
    [SerializeField]
    private Image SkillImg;

    private Action<int> OnSkillClick;

    [Header("���ݹ۲�")]
    /// <summary>
    /// ����ID
    /// </summary>
    [SerializeField]
    public int SkillId;
    [SerializeField]
    /// <summary>
    /// ��ȴʱ��
    /// </summary>
    private float m_CDTime = 0f;

    [SerializeField]
    /// <summary>
    /// �����Ƿ�����ȴʱ��
    /// </summary>
    private bool m_IsCD = false;
    /// <summary>
    /// ���ܿ�ʼ��ȴ��ʱ��
    /// </summary>
    private float m_BeginCDTime = 0f;
    /// <summary>
    /// ��ת�İٷֱ�
    /// </summary>
    private float m_CurrFillAmount = 0f;
    protected override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("���ܣ�����");
        SkillImg.gameObject.SetActive(false);
        CDImage.transform.parent.gameObject.SetActive(false);
        m_IsCD = false;
    }

    protected override void OnStart()
    {
        base.OnStart();
        //����
        EventTriggerListener.Get(gameObject).onClick += OnClick;
    }
    /// <summary>
    /// ���ܲ۰�ť���
    /// </summary>
    /// <param name="obj"></param>
    private void OnClick(GameObject obj)
    {
        Debug.Log("���ܱ������");
        //�����ܲ���δװ�似�ܣ��򷵻�
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
    /// ���ü���UI
    /// </summary>
    /// <param name="skillId">����ID</param>
    /// <param name="skillLevel">���ܵȼ�</param>
    /// <param name="skillPic">����ͼƬ</param>
    /// <param name="cdTime">����CD</param>
    /// <param name="onSkillClick">���ܵ���ص�</param>
    public void SetUI(int skillId, int skillLevel, string skillPic, float cdTime, Action<int> onSkillClick)
    {
        if (skillId == 0)
        { return; }
        SkillId = skillId;
        m_CDTime = cdTime;//��ȴʱ��
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
    /// ��ʼ��ȴ
    /// </summary>
    public void BeginCD()
    {
        Debug.Log("���ܿ�ʼ��ȴ");
        CDImage.transform.parent.gameObject.SetActive(true);
        m_IsCD = true;
        m_BeginCDTime = Time.time;
        m_CurrFillAmount = 1f;

    }

    private void Update()
    {
        if (m_IsCD)
        {
            //��ȡ�ٷֱ�
            m_CurrFillAmount = Mathf.Lerp(1, 0, (Time.time - m_BeginCDTime) / m_CDTime);
            CDImage.fillAmount = m_CurrFillAmount;
            if (Time.time > m_BeginCDTime + m_CDTime)
            {
                m_IsCD = false;
            }
        }
    }
}
