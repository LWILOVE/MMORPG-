using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡地图连线点UI
/// </summary>
public class UIGameLevelMapPointView : UISubViewBase
{
    /// <summary>
    /// 通关图
    /// </summary>
    [SerializeField]
    private Image imgPass;

    /// <summary>
    /// 未通关图
    /// </summary>
    [SerializeField]
    private Image imgUnPass;

    public void SetUI(bool isPass)
    {
        //若通关，则显示通关图，否则显示未通关
        if (isPass)
        {
            imgPass.gameObject.SetActive(true);
        }
        else
        {
            imgUnPass.gameObject.SetActive(true);
        }
    }
}