using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoleInfoDetailView:UISubViewBase
{
    /// <summary>
    /// Ôª±¦
    /// </summary>
    [SerializeField]
    private Text lblMoney;

    /// <summary>
    /// ½ð±Ò
    /// </summary>
    [SerializeField]
    private Text lblGold;

    /// <summary>
    /// HP
    /// </summary>
    [SerializeField]
    private Slider sliderHP;
    [SerializeField]
    private Text lblHP;

    /// <summary>
    /// MP
    /// </summary>
    [SerializeField]
    private Slider sliderMP;
    [SerializeField]
    private Text lblMP;

    /// <summary>
    /// Exp
    /// </summary>
    [SerializeField]
    private Slider sliderExp;
    [SerializeField]
    private Text lblExp;

    /// <summary>
    /// Attack
    /// </summary>
    [SerializeField]
    private Text lblAttack;

    /// <summary>
    /// Denfense
    /// </summary>
    [SerializeField]
    private Text lblDenfense;

    /// <summary>
    /// Dodge
    /// </summary>
    [SerializeField]
    private Text lblDodge;

    /// <summary>
    /// Hit
    /// </summary>
    [SerializeField]
    private Text lblHit;

    /// <summary>
    /// Cri
    /// </summary>
    [SerializeField]
    private Text lblCri;

    /// <summary>
    /// Res
    /// </summary>
    [SerializeField]
    private Text lblRes;

    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
        lblMoney = null;
        lblGold = null;
        sliderHP = null;
        lblHP = null;
        sliderMP = null;
        lblMP = null;
        sliderExp = null;
        lblExp = null;
        lblAttack = null;
        lblDenfense = null;
        lblDodge = null;
        lblHit = null;
        lblCri = null;
        lblRes = null;
    }

    public void SetUI(TransferData data)
    {
        lblMoney.SetText(data.GetValue<int>(ConstDefine.Money).ToString());
        lblGold.SetText(data.GetValue<int>(ConstDefine.Gold).ToString());

        sliderHP.SetSlider((float)data.GetValue<int>(ConstDefine.CurrHP)/data.GetValue<int>(ConstDefine.MaxHP));
        lblHP.SetText(string.Format("{0}/{1}",data.GetValue<int>(ConstDefine.CurrHP),data.GetValue<int>(ConstDefine.MaxHP)));

        sliderMP.SetSlider((float)data.GetValue<int>(ConstDefine.CurrMP) / data.GetValue<int>(ConstDefine.MaxMP));
        lblMP.SetText(string.Format("{0}/{1}", data.GetValue<int>(ConstDefine.CurrMP), data.GetValue<int>(ConstDefine.MaxMP)));

        sliderExp.SetSlider((float)data.GetValue<int>(ConstDefine.CurrExp) / data.GetValue<int>(ConstDefine.MaxExp));
        lblExp.SetText(string.Format("{0}/{1}", data.GetValue<int>(ConstDefine.CurrExp), data.GetValue<int>(ConstDefine.MaxExp)));

        lblAttack.SetText(string.Format("¹¥»÷£º{0}",data.GetValue<int>(ConstDefine.Attack).ToString()));
        lblDenfense.SetText(string.Format("·ÀÓù£º{0}", data.GetValue<int>(ConstDefine.Defense).ToString()));
        lblDodge.SetText(string.Format("ÉÁ±Ü£º{0}",data.GetValue<int>(ConstDefine.Dodge).ToString()));
        lblHit.SetText(string.Format("ÃüÖÐ£º{0}",data.GetValue<int>(ConstDefine.Hit).ToString()));
        lblCri.SetText(string.Format("±©»÷£º{0}",data.GetValue<int>(ConstDefine.Cri).ToString()));
        lblRes.SetText(string.Format("¿¹ÐÔ£º{0}",data.GetValue<int>(ConstDefine.Res).ToString()));
    }
    
}
