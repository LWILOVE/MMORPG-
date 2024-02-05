using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 世界地图子项UI
/// </summary>
public class UIWorldMapItemView : UISubViewBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [SerializeField]
    private Text txtName;

    /// <summary>
    /// 地图图片
    /// </summary>
    [SerializeField]
    private Image imgIco;

    /// <summary>
    /// 场景编号
    /// </summary>
    private int m_ScenenId;

    public Action<int> OnGameLevelItemClick;
    protected override void OnAwake()
    {
        base.OnAwake();
    }

    protected override void OnStart()
    {
        base.OnStart();
        GetComponent<Button>().onClick.AddListener(BtnClick);
    }

    /// <summary>
    /// 按钮点击
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void BtnClick()
    {
        if (OnGameLevelItemClick != null)
        {
            OnGameLevelItemClick(m_ScenenId);
        }
    }

    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
    }

    /// <summary>
    /// 设置世界地图子项UI
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onGameLevelItemClick"></param>
    public void SetUI(TransferData data, Action<int> onGameLevelItemClick)
    {
        OnGameLevelItemClick = onGameLevelItemClick;
        txtName.SetText(data.GetValue<string>(ConstDefine.WorldMapName));//地图名
        imgIco.overrideSprite = GameUtil.LoadSprite(SpriteSourceType.WorldMapIco,data.GetValue<string>(ConstDefine.WorldMapIco));//地图图片
        m_ScenenId = data.GetValue<int>(ConstDefine.WorldMapId);//地图ID
    }
}
