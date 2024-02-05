using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCityRoleInfoView : UISubViewBase
{
    /// <summary>
    /// ��ɫͷ��
    /// </summary>
    [SerializeField]
    public Image imgHead;

    /// <summary>
    /// ��ɫ��
    /// </summary>
    [SerializeField]
    private Text lblNickName;

    /// <summary>
    /// ��ɫ�ȼ�
    /// </summary>
    [SerializeField]
    private Text lblLV;

    /// <summary>
    /// ��ɫǮ��
    /// </summary>
    [SerializeField]
    private Text lblMoney;

    /// <summary>
    /// ��ɫ���
    /// </summary>
    [SerializeField]
    private Text lblGold;

    /// <summary>
    /// ��ɫ��ǰ�����
    /// </summary>
    private int currentGold;

    /// <summary>
    /// Ѫ��
    /// </summary>
    [SerializeField]
    private Slider sliderHP;

    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    private Slider sliderMP;

    public static UIMainCityRoleInfoView Instance;

    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;    
    }

    private void Start()
    {
         
    }

    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
        imgHead = null;
        lblNickName = null;
        lblLV = null;
        lblMoney = null;
        lblGold = null;
        sliderHP = null;
        sliderMP = null;
        Instance = null;
    }

    /// <summary>
    /// ���ý�ɫUI
    /// </summary>
    /// <param name="headPic"></param>
    /// <param name="nickName"></param>
    /// <param name="level"></param>
    /// <param name="money"></param>
    /// <param name="gold"></param>
    /// <param name="currHP"></param>
    /// <param name="maxHP"></param>
    /// <param name="currMP"></param>
    /// <param name="maxMP"></param>
    public void SetUI(string headPic,string nickName,int level,
        int money,int gold,int currHP,int maxHP,int currMP,int maxMP)
    {//skillpic_cike5
        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("Download/Source/UISource/HeadImg/{0}.assetbundle", headPic), headPic,
        (Texture2D obj) =>
            {
                var iconRect = new Rect(0, 0, obj.width, obj.height);
                var iconSprite = Sprite.Create(obj, iconRect, new Vector2(0.5f, 0.5f));
                imgHead.overrideSprite = iconSprite;
            }, type: 1);
        //���ؽ�ɫͷ��ͼƬ
        lblNickName.text = nickName;
        lblLV.text = string.Format("LV:{0}", level);
        lblMoney.text = money.ToString();
        lblGold.text = gold.ToString();
        currentGold = gold;
        sliderHP.value = (float)currHP / maxHP;
        sliderMP.value = (float)currMP / maxMP;

    }

    /// <summary>
    /// ����Ѫ��
    /// </summary>
    /// <param name="currHP"></param>
    /// <param name="maxHP"></param>
    public void SetHP(int currHP,int maxHP)
    {
        sliderHP.SetSlider((float)currHP/maxHP);   
    }
    /// <summary>
    /// ��������
    /// </summary>
    /// <param name="currMP"></param>
    /// <param name="maxMP"></param>
    public void SetMP(int currMP, int maxMP)
    {
        sliderMP.SetSlider((float)currMP/maxMP);
    }
    
    /// <summary>
    /// ����ͭ��
    /// </summary>
    /// <param name="changeNum">������</param>
    public void SetGold(int changeNum)
    {
        currentGold += changeNum;
        lblGold.text = currentGold.ToString();
    }
    /// <summary>
    /// ����Ԫ��
    /// </summary>
    /// <param name="money"></param>
    public void SetMoney(int money)
    {
        //����Ԫ�����ö���
        GameUtil.AutoNumberAnimation(lblMoney.gameObject,money);
        //lblMoney.SetText(money.ToString(), true, 2f, DG.Tweening.ScrambleMode.Numerals);
    }
}
