using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipItemView : UISubViewBase
{
    /// <summary>
    /// �ı�
    /// </summary>
    [SerializeField]
    private Text lblText;
    /// <summary>
    /// ����
    /// </summary>
    [SerializeField]
    private Image ImgIco;
    /// <summary>
    /// ��Դ��������
    /// </summary>
    [SerializeField]
    private Sprite[] sprType;

    protected override void OnStart()
    {
        base.OnStart();
        lblText.horizontalOverflow = HorizontalWrapMode.Overflow;
    }

    public void SetUI(int type, string text)
    {
        if (type >= 0 && type < sprType.Length)
        {
            ImgIco.overrideSprite = sprType[type];
            ImgIco.SetNativeSize();
        }
        lblText.SetText(text);
    }
}
