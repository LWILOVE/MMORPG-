using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡奖励UI视图
/// </summary>
public class UIGameLevelRewardView : UISubViewBase
{
    /// <summary>
    /// 奖励图标
    /// </summary>
    [SerializeField]
    private Image imgIco;

    /// <summary>
    /// 奖励名
    /// </summary>
    [SerializeField]
    private Text lblName;

    /// <summary>
    /// 设置游戏关卡奖励UI
    /// </summary>
    /// <param name="name">奖励名</param>
    /// <param name="goodsId">奖励ID</param>
    /// <param name="type">奖励类别</param>
    public void SetUI(string name, int goodsId,GoodsType type)
    {
        imgIco.overrideSprite = GameUtil.LoadGoodsImg(goodsId,type);
        lblName.SetText(name);
    }

    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
        imgIco = null;
        lblName = null;
    }
}
