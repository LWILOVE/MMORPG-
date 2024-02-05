using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ؿ�����UI��ͼ
/// </summary>
public class UIGameLevelRewardView : UISubViewBase
{
    /// <summary>
    /// ����ͼ��
    /// </summary>
    [SerializeField]
    private Image imgIco;

    /// <summary>
    /// ������
    /// </summary>
    [SerializeField]
    private Text lblName;

    /// <summary>
    /// ������Ϸ�ؿ�����UI
    /// </summary>
    /// <param name="name">������</param>
    /// <param name="goodsId">����ID</param>
    /// <param name="type">�������</param>
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
