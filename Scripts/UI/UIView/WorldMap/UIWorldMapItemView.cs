using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �����ͼ����UI
/// </summary>
public class UIWorldMapItemView : UISubViewBase
{
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    private Text txtName;

    /// <summary>
    /// ��ͼͼƬ
    /// </summary>
    [SerializeField]
    private Image imgIco;

    /// <summary>
    /// �������
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
    /// ��ť���
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
    /// ���������ͼ����UI
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onGameLevelItemClick"></param>
    public void SetUI(TransferData data, Action<int> onGameLevelItemClick)
    {
        OnGameLevelItemClick = onGameLevelItemClick;
        txtName.SetText(data.GetValue<string>(ConstDefine.WorldMapName));//��ͼ��
        imgIco.overrideSprite = GameUtil.LoadSprite(SpriteSourceType.WorldMapIco,data.GetValue<string>(ConstDefine.WorldMapIco));//��ͼͼƬ
        m_ScenenId = data.GetValue<int>(ConstDefine.WorldMapId);//��ͼID
    }
}
