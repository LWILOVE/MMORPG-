using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����ؿ���ͼ��������ͼ
/// </summary>
public class UIGameLevelMapItemView : UISubViewBase
{
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    private Text txtName;

    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private Image imgIco;

    /// <summary>
    /// �½ڱ��
    /// </summary>
    private int m_ChapterId;

    /// <summary>
    /// �ؿ����
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
    /// ��ť���
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
    /// ���ùؿ�����UI
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
