using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 剧情关卡地图上子项视图
/// </summary>
public class UIGameLevelMapItemView : UISubViewBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [SerializeField]
    private Text txtName;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Image imgIco;

    /// <summary>
    /// 章节编号
    /// </summary>
    private int m_ChapterId;

    /// <summary>
    /// 关卡编号
    /// </summary>
    private int m_GameLevelId;

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
        //TODO
        //AudioEffectMgr.Instance.PlayUIAudioEffect(UIAudioEffectType.ButtonClick);
        if (OnGameLevelItemClick != null)
        {
            OnGameLevelItemClick(m_GameLevelId);
        }
    }
    
    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
    }

    /// <summary>
    /// 设置关卡子项UI
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onGameLevelItemClick"></param>
    public void SetUI(TransferData data, Action<int> onGameLevelItemClick)
    {
        OnGameLevelItemClick = onGameLevelItemClick;
        txtName.SetText(data.GetValue<string>(ConstDefine.GameLevelName));
        m_GameLevelId = data.GetValue<int>(ConstDefine.GameLevelId);
        string picName = data.GetValue<string>(ConstDefine.GameLevelIco);
        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("Download/Source/UISource/GameLevel/GameLevelIco/{0}.assetbundle", picName), picName,
    (Texture2D obj) =>
    {
        if (obj == null)
        { return; }
        var iconRect = new Rect(0, 0, obj.width, obj.height);
        var iconSprite = Sprite.Create(obj, iconRect, new Vector2(0.5f, 0.5f));

        imgIco.overrideSprite = iconSprite;
    }, type: 1);
    }


}
