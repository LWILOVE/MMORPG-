using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainCityRoleInfoView : UISubViewBase
{
    /// <summary>
    /// 角色头像
    /// </summary>
    [SerializeField]
    public Image imgHead;

    /// <summary>
    /// 角色名
    /// </summary>
    [SerializeField]
    private Text lblNickName;

    /// <summary>
    /// 角色等级
    /// </summary>
    [SerializeField]
    private Text lblLV;

    /// <summary>
    /// 角色钱币
    /// </summary>
    [SerializeField]
    private Text lblMoney;

    /// <summary>
    /// 角色金币
    /// </summary>
    [SerializeField]
    private Text lblGold;

    /// <summary>
    /// 角色当前金币量
    /// </summary>
    private int currentGold;

    /// <summary>
    /// 血条
    /// </summary>
    [SerializeField]
    private Slider sliderHP;

    /// <summary>
    /// 蓝条
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
    /// 设置角色UI
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
        //加载角色头像图片
        lblNickName.text = nickName;
        lblLV.text = string.Format("LV:{0}", level);
        lblMoney.text = money.ToString();
        lblGold.text = gold.ToString();
        currentGold = gold;
        sliderHP.value = (float)currHP / maxHP;
        sliderMP.value = (float)currMP / maxMP;

    }

    /// <summary>
    /// 设置血量
    /// </summary>
    /// <param name="currHP"></param>
    /// <param name="maxHP"></param>
    public void SetHP(int currHP,int maxHP)
    {
        sliderHP.SetSlider((float)currHP/maxHP);   
    }
    /// <summary>
    /// 设置蓝量
    /// </summary>
    /// <param name="currMP"></param>
    /// <param name="maxMP"></param>
    public void SetMP(int currMP, int maxMP)
    {
        sliderMP.SetSlider((float)currMP/maxMP);
    }
    
    /// <summary>
    /// 设置铜币
    /// </summary>
    /// <param name="changeNum">增加量</param>
    public void SetGold(int changeNum)
    {
        currentGold += changeNum;
        lblGold.text = currentGold.ToString();
    }
    /// <summary>
    /// 设置元宝
    /// </summary>
    /// <param name="money"></param>
    public void SetMoney(int money)
    {
        //播放元宝设置动画
        GameUtil.AutoNumberAnimation(lblMoney.gameObject,money);
        //lblMoney.SetText(money.ToString(), true, 2f, DG.Tweening.ScrambleMode.Numerals);
    }
}
